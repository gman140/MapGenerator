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


    public MovementService(
        IMapRepository mapRepo,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        MapGeneratorService mapCache)
    {
        _mapRepo = mapRepo;
        _playerRepo = playerRepo;
        _visitRepo = visitRepo;
        _mapCache = mapCache;
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

        if (tile.Biome == BiomeType.Lake)
            return Fail("The lake is too deep to cross without a boat.");

        if (tile.Biome == BiomeType.Volcano)
            return Fail("The volcanic terrain is impassable.");

        if (tile.Biome == BiomeType.Ocean)
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
            : BiomeProperties.CooldownMs(tile.Biome);

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
