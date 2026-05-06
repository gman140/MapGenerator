using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryStructureDefinitionProvider : IStructureDefinitionProvider
{
    private static readonly StructureDefinition[] _definitions =
    [
        new()
        {
            Type        = StructureType.Beacon,
            Name        = "Beacon",
            Description = "A fire set high enough to be seen from far away. It marks this place on every traveler's map.",
            Ingredients =
            [
                new() { ResourceId = "Wood",   Quantity = 3 },
                new() { ResourceId = "Coal",   Quantity = 2 },
                new() { ResourceId = "Quartz", Quantity = 1 },
            ],
        },
        new()
        {
            Type        = StructureType.Garden,
            Name        = "Garden",
            Description = "Carefully tended plots and cleared ground. The land gives back more when it has been listened to. Doubles all resource yields here.",
            Ingredients =
            [
                new() { ResourceId = "Herbs", Quantity = 4 },
                new() { ResourceId = "Fiber", Quantity = 3 },
                new() { ResourceId = "Peat",  Quantity = 2 },
                new() { ResourceId = "Moss",  Quantity = 2 },
            ],
            AllowedBiomes =
            [
                BiomeType.Grassland, BiomeType.Plains, BiomeType.Forest,
                BiomeType.Jungle, BiomeType.Swamp, BiomeType.Marsh, BiomeType.Savanna,
            ],
        },
        new()
        {
            Type        = StructureType.MineShaft,
            Name        = "Mine Shaft",
            Description = "Shored timbers and a rope ladder descending into the dark. What was buried comes up more readily now. Doubles all resource yields here.",
            Ingredients =
            [
                new() { ResourceId = "Stone", Quantity = 6 },
                new() { ResourceId = "Flint", Quantity = 4 },
                new() { ResourceId = "Wood",  Quantity = 3 },
            ],
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Volcano],
        },
        new()
        {
            Type        = StructureType.Workshop,
            Name        = "Workshop",
            Description = "A bench, some tools, and a place to work without interruption. Crafting here costs half the usual materials.",
            Ingredients =
            [
                new() { ResourceId = "Wood",  Quantity = 4 },
                new() { ResourceId = "Stone", Quantity = 3 },
                new() { ResourceId = "Flint", Quantity = 2 },
            ],
        },
        new()
        {
            Type        = StructureType.Dock,
            Name        = "Dock",
            Description = "Planks over the water, a place to step off from. Those without a vessel can board the water from here.",
            Ingredients =
            [
                new() { ResourceId = "Wood",      Quantity = 8 },
                new() { ResourceId = "Driftwood", Quantity = 4 },
                new() { ResourceId = "Fiber",     Quantity = 5 },
                new() { ResourceId = "Reed",      Quantity = 3 },
            ],
            AllowedBiomes = [BiomeType.Beach, BiomeType.Shallows, BiomeType.River, BiomeType.Marsh],
        },
    ];

    private static readonly Dictionary<StructureType, StructureDefinition> _byType =
        _definitions.ToDictionary(d => d.Type);

    public IReadOnlyList<StructureDefinition> All => _definitions;

    public StructureDefinition? GetByType(StructureType type) =>
        _byType.TryGetValue(type, out var def) ? def : null;
}
