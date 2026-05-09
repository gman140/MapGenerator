using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class MovementService
{
    private readonly IMapRepository _mapRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly MapGeneratorService _mapCache;
    private readonly IBiomeDefinitionProvider _biomeProvider;
    private readonly ICraftingRecipeProvider _recipeProvider;
    private readonly SettlementCacheService _settlementCache;

    public MovementService(
        IMapRepository mapRepo,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        MapGeneratorService mapCache,
        IBiomeDefinitionProvider biomeProvider,
        ICraftingRecipeProvider recipeProvider,
        SettlementCacheService settlementCache)
    {
        _mapRepo         = mapRepo;
        _playerRepo      = playerRepo;
        _visitRepo       = visitRepo;
        _mapCache        = mapCache;
        _biomeProvider   = biomeProvider;
        _recipeProvider  = recipeProvider;
        _settlementCache = settlementCache;
    }

    public static bool AreAdjacent(int q1, int r1, int q2, int r2)
    {
        int dq = q2 - q1, dr = r2 - r1;
        return MapGeneratorService.HexNeighborOffsets().Any(o => o.dq == dq && o.dr == dr);
    }

    public async Task<MovementResult> TryMoveAsync(
        Player player,
        IReadOnlySet<Permission> permissions,
        int targetQ, int targetR,
        bool oceanConfirmed = false)
    {
        if (!permissions.Contains(Permission.MoveToAnyTile) &&
            !AreAdjacent(player.Q, player.R, targetQ, targetR))
            return Fail("You can only move to adjacent tiles.");

        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (!permissions.Contains(Permission.IgnoreCooldowns) &&
            now < player.MovementCooldownUntil)
        {
            double secs = (player.MovementCooldownUntil - now) / 1000.0;
            return Fail($"You must wait {secs:F1}s before moving again.");
        }

        var tile = _mapCache.GetCachedTile(targetQ, targetR)
                   ?? await _mapRepo.GetTileAsync(targetQ, targetR);

        if (tile == null)
            return Fail("That tile does not exist.");

        var originTile = _mapCache.GetCachedTile(player.Q, player.R);
        bool hasDock = originTile?.Structure?.Type == StructureType.Dock;

        if (tile.Biome == BiomeType.Lake &&
            !_recipeProvider.PlayerHasEffect(player, ItemEffect.AllowLakeTraversal) && !hasDock)
            return Fail("The lake is too deep to cross without a boat.");

        if (tile.Biome == BiomeType.Volcano && !_recipeProvider.PlayerHasEffect(player, ItemEffect.AllowCliffTraversal))
            return Fail("The volcanic terrain is impassable.");

        if (tile.Biome == BiomeType.Ocean &&
            !_recipeProvider.PlayerHasEffect(player, ItemEffect.AllowOceanTraversal) && !hasDock)
        {
            if (!oceanConfirmed)
                return new MovementResult { Success = false, RequiresOceanConfirmation = true };

            // Drown — caller is responsible for clearing session
            await _visitRepo.DeleteAllForPlayerAsync(player.Id);
            await _playerRepo.DeleteAsync(player.Id);
            return new MovementResult { Success = true, PlayerDrowned = true };
        }

        long cooldown = permissions.Contains(Permission.IgnoreCooldowns)
            ? 0
            : (_biomeProvider.GetByType(tile.Biome)?.CooldownMs ?? 400);

        if (cooldown > 0 && _settlementCache.IsRoadTile(targetQ, targetR))
            cooldown = 0;

        if (cooldown > 0)
        {
            if (_recipeProvider.PlayerHasEffect(player, ItemEffect.ReduceMovementCooldown))
                cooldown = (long)(cooldown * 0.70);

            bool isColdBiome = tile.Biome is BiomeType.Tundra or BiomeType.Snow or BiomeType.Glacier;
            if (isColdBiome && _recipeProvider.PlayerHasEffect(player, ItemEffect.ReduceColdBiomeCooldown))
                cooldown = (long)(cooldown * 0.60);
        }

        await _visitRepo.RecordDepartureAsync(player.Id, player.Q, player.R);

        player.Q = targetQ;
        player.R = targetR;
        player.MovementCooldownUntil = cooldown > 0 ? now + cooldown : 0;
        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        await _visitRepo.RecordArrivalAsync(player.Id, targetQ, targetR);

        return new MovementResult
        {
            Success = true,
            NewQ = targetQ,
            NewR = targetR,
            CooldownUntil = player.MovementCooldownUntil
        };
    }

    private static MovementResult Fail(string msg) => new() { Success = false, ErrorMessage = msg };
}
