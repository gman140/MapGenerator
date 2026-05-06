using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class GatherService
{
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly IBiomeDefinitionProvider _biomeProvider;
    private readonly IFeatureDefinitionProvider _featureProvider;
    private readonly IPlayerRepository _playerRepo;
    private readonly MapGeneratorService _mapCache;

    private const long CooldownMs = 10_000;

    public GatherService(
        IResourceDefinitionProvider resourceProvider,
        IBiomeDefinitionProvider biomeProvider,
        IFeatureDefinitionProvider featureProvider,
        IPlayerRepository playerRepo,
        MapGeneratorService mapCache)
    {
        _resourceProvider = resourceProvider;
        _biomeProvider    = biomeProvider;
        _featureProvider  = featureProvider;
        _playerRepo       = playerRepo;
        _mapCache         = mapCache;
    }

    public async Task<GatherResult> TryGatherAsync(Player player, IReadOnlySet<Permission> permissions)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (!permissions.Contains(Permission.IgnoreCooldowns) && now < player.GatherCooldownUntil)
        {
            double secs = (player.GatherCooldownUntil - now) / 1000.0;
            return Fail($"You must wait {secs:F1}s before gathering again.");
        }

        var tile = _mapCache.GetCachedTile(player.Q, player.R);
        if (tile == null) return Fail("Cannot gather here.");

        var pool = BuildYieldPool(tile);
        var rng  = new Random();
        var gathered = new List<GatheredItem>();

        foreach (var yield in pool)
        {
            if (rng.NextDouble() >= yield.Probability) continue;

            var def = _resourceProvider.GetById(yield.ResourceId);
            if (def == null) continue;

            int qty = def.MinQuantity == def.MaxQuantity
                ? def.MinQuantity
                : rng.Next(def.MinQuantity, def.MaxQuantity + 1);

            gathered.Add(new GatheredItem { ResourceId = def.Id, Name = def.Name, Quantity = qty });
            player.Inventory.TryGetValue(def.Id, out int existing);
            player.Inventory[def.Id] = existing + qty;
        }

        long cooldown = permissions.Contains(Permission.IgnoreCooldowns) ? 0 : CooldownMs;
        player.GatherCooldownUntil = cooldown > 0 ? now + cooldown : 0;
        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        return new GatherResult { Success = true, Gathered = gathered, CooldownUntil = player.GatherCooldownUntil };
    }

    public bool TileHasResources(HexTile tile)
    {
        var biomeDef   = _biomeProvider.GetByType(tile.Biome);
        var featureDef = _featureProvider.GetById(tile.FeatureId);
        return (biomeDef?.ResourceYields.Length > 0) || (featureDef?.ResourceYields.Length > 0);
    }

    private List<ResourceYield> BuildYieldPool(HexTile tile)
    {
        var pool = new List<ResourceYield>();
        var biomeDef = _biomeProvider.GetByType(tile.Biome);
        if (biomeDef != null) pool.AddRange(biomeDef.ResourceYields);
        var featureDef = _featureProvider.GetById(tile.FeatureId);
        if (featureDef != null) pool.AddRange(featureDef.ResourceYields);
        return pool;
    }

    private static GatherResult Fail(string msg) => new() { Success = false, ErrorMessage = msg };
}
