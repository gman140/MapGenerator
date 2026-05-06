using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class StructureService
{
    private readonly IStructureDefinitionProvider _structureProvider;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly IPlayerRepository _playerRepo;
    private readonly IMapRepository _mapRepo;
    private readonly MapGeneratorService _mapCache;

    public StructureService(
        IStructureDefinitionProvider structureProvider,
        IResourceDefinitionProvider resourceProvider,
        IPlayerRepository playerRepo,
        IMapRepository mapRepo,
        MapGeneratorService mapCache)
    {
        _structureProvider = structureProvider;
        _resourceProvider  = resourceProvider;
        _playerRepo        = playerRepo;
        _mapRepo           = mapRepo;
        _mapCache          = mapCache;
    }

    public async Task<(bool success, string? error)> TryBuildAsync(Player player, StructureType type, HexTile tile)
    {
        var def = _structureProvider.GetByType(type);
        if (def == null) return (false, "Unknown structure.");

        if (tile.Structure != null)
            return (false, $"A {_structureProvider.GetByType(tile.Structure.Type)?.Name ?? tile.Structure.Type.ToString()} already stands here.");

        if (def.AllowedBiomes != null && !def.AllowedBiomes.Contains(tile.Biome))
            return (false, $"A {def.Name} cannot be built on {tile.Biome}.");

        foreach (var ingredient in def.Ingredients)
        {
            player.Inventory.TryGetValue(ingredient.ResourceId, out int have);
            if (have >= ingredient.Quantity) continue;
            var name = _resourceProvider.GetById(ingredient.ResourceId)?.Name ?? ingredient.ResourceId;
            return (false, $"Not enough {name}. Need {ingredient.Quantity}, have {have}.");
        }

        foreach (var ingredient in def.Ingredients)
        {
            player.Inventory[ingredient.ResourceId] -= ingredient.Quantity;
            if (player.Inventory[ingredient.ResourceId] <= 0)
                player.Inventory.Remove(ingredient.ResourceId);
        }

        var structure = new TileStructure
        {
            Type        = type,
            BuilderId   = player.Id,
            BuilderName = player.Username,
            BuiltAt     = DateTime.UtcNow,
        };

        tile.Structure  = structure;
        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);
        await _mapRepo.SetStructureAsync(tile.Q, tile.R, structure);
        _mapCache.UpdateCachedStructure(tile.Q, tile.R, structure);

        return (true, null);
    }

    public async Task DestroyAsync(HexTile tile)
    {
        tile.Structure = null;
        await _mapRepo.RemoveStructureAsync(tile.Q, tile.R);
        _mapCache.UpdateCachedStructure(tile.Q, tile.R, null);
    }
}
