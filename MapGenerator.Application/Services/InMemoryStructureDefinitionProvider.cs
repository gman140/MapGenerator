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
            MapIcon = "<line x1='0' y1='5' x2='0' y2='-3' stroke='#c8a040' stroke-width='1.5' stroke-linecap='round'/><circle cx='0' cy='-7' r='3.5' fill='#e06820' opacity='0.9'/><circle cx='0' cy='-7' r='1.8' fill='#f8e060'/>",
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
            MapIcon = "<circle cx='0' cy='-5' r='3.5' fill='#4a8830' opacity='0.95'/><circle cx='-4' cy='-1' r='3' fill='#5a9840' opacity='0.9'/><circle cx='4' cy='-1' r='3' fill='#5a9840' opacity='0.9'/><line x1='0' y1='1' x2='0' y2='5' stroke='#7a5030' stroke-width='1.3' stroke-linecap='round'/>",
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
            MapIcon = "<rect x='-6' y='-1' width='12' height='7' fill='#3a2818' rx='1'/><rect x='-3' y='-5' width='6' height='5' fill='#2a1808' rx='1'/><ellipse cx='0' cy='3' rx='3' ry='3.5' fill='#0e0e12'/>",
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
            MapIcon = "<rect x='-5' y='2' width='10' height='4' fill='#506080' rx='1'/><rect x='-4' y='-2' width='8' height='5' fill='#607090' rx='1'/><rect x='-2' y='-5' width='4' height='4' fill='#4a5870'/>",
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
            MapIcon = "<line x1='-6' y1='-1' x2='6' y2='-1' stroke='#8b5a30' stroke-width='2.5' stroke-linecap='round'/><line x1='-4' y1='-1' x2='-4' y2='6' stroke='#8b5a30' stroke-width='1.5' stroke-linecap='round'/><line x1='0' y1='-1' x2='0' y2='6' stroke='#8b5a30' stroke-width='1.5' stroke-linecap='round'/><line x1='4' y1='-1' x2='4' y2='6' stroke='#8b5a30' stroke-width='1.5' stroke-linecap='round'/>",
        },
    ];

    private static readonly Dictionary<StructureType, StructureDefinition> _byType =
        _definitions.ToDictionary(d => d.Type);

    public IReadOnlyList<StructureDefinition> All => _definitions;

    public StructureDefinition? GetByType(StructureType type) =>
        _byType.TryGetValue(type, out var def) ? def : null;
}
