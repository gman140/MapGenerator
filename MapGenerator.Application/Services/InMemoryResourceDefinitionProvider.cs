using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryResourceDefinitionProvider : IResourceDefinitionProvider
{
    private static readonly ResourceDefinition[] _definitions =
    [
        // ── Common (variable quantity) ────────────────────────────────────────
        new() { Id = "Wood",  Name = "Wood",  Description = "Rough timber stripped from nearby growth.",                                      MinQuantity = 1, MaxQuantity = 4 },
        new() { Id = "Stone", Name = "Stone", Description = "Loose rock, worn smooth or broken sharp by circumstance.",                       MinQuantity = 1, MaxQuantity = 4 },
        new() { Id = "Fiber", Name = "Fiber", Description = "Coarse plant material suitable for binding or weaving.",                          MinQuantity = 1, MaxQuantity = 5 },
        new() { Id = "Ore",   Name = "Ore",   Description = "A dense chunk of mineralized rock. Something in it is valuable.",                 MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "Fish",  Name = "Fish",  Description = "A fish. It is dead now but was recently otherwise.",                              MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "Sand",  Name = "Sand",  Description = "Fine particulate that has worked its way into everything you own.",               MinQuantity = 1, MaxQuantity = 4 },
        new() { Id = "Ice",   Name = "Ice",   Description = "A block of it. Cold. Currently.",                                                 MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "Reed",  Name = "Reed",  Description = "Hollow grass stems from the waterline. Useful. Wet.",                            MinQuantity = 1, MaxQuantity = 4 },
        new() { Id = "Peat",  Name = "Peat",  Description = "Dense compressed plant matter, from something that died slowly.",                 MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "Flint", Name = "Flint", Description = "A sharp-edged stone that makes other things sharp.",                             MinQuantity = 1, MaxQuantity = 3 },

        // ── Uncommon (quantity 1) ─────────────────────────────────────────────
        new() { Id = "Herbs",     Name = "Herbs",     Description = "Fragrant leaves of ambiguous medicinal value." },
        new() { Id = "Feathers",  Name = "Feathers",  Description = "Shed or left behind. The bird is not here to explain." },
        new() { Id = "Salt",      Name = "Salt",      Description = "Crystallized from water, wind, or time." },
        new() { Id = "Clay",      Name = "Clay",      Description = "Dense, grey, workable. Remembers the shape of your hands." },
        new() { Id = "Coal",      Name = "Coal",      Description = "Compressed darkness. Burns long and hot and without comment." },
        new() { Id = "Sulfur",    Name = "Sulfur",    Description = "Yellow and pungent. You know what this is for. Everyone does." },
        new() { Id = "Amber",     Name = "Amber",     Description = "Tree resin, ancient, perfectly preserving something small inside." },
        new() { Id = "Quartz",    Name = "Quartz",    Description = "A clear crystal. Precise and self-important about it." },
        new() { Id = "Driftwood", Name = "Driftwood", Description = "Wood that has been somewhere and come back changed." },
        new() { Id = "Moss",      Name = "Moss",      Description = "Green and soft and quietly covering everything." },

        // ── Rare / Esoteric (quantity 1) ──────────────────────────────────────
        new() { Id = "CrackedOrb",    Name = "Cracked Orb",    Description = "It was clearly meant to do something. It no longer does it." },
        new() { Id = "TarnishedRing", Name = "Tarnished Ring", Description = "Silver, probably. Sized for a finger that is not here." },
        new() { Id = "RottenSilks",   Name = "Rotten Silks",   Description = "What remains of something that was once very fine." },
        new() { Id = "DeadGrass",     Name = "Dead Grass",     Description = "A dry bundle. Gathered more out of habit than intent." },
        new() { Id = "CrowFeather",   Name = "Crow Feather",   Description = "Black and glossy. Left with intention, you suspect." },
        new() { Id = "RiverGlass",    Name = "River Glass",    Description = "A pebble worn so smooth by water it has become something else entirely." },
        new() { Id = "PaleMushroom",  Name = "Pale Mushroom",  Description = "White and silent and growing where it probably shouldn't." },
        new() { Id = "BoneFragment",  Name = "Bone Fragment",  Description = "Old. You prefer not to speculate further." },
        new() { Id = "FrozenFlower",  Name = "Frozen Flower",  Description = "Perfectly preserved. Has been waiting to be found." },
        new() { Id = "Ash",           Name = "Ash",            Description = "Grey powder from something that burned completely." },
        new() { Id = "HollowStone",   Name = "Hollow Stone",   Description = "A stone with a void inside. Something used to live here." },
        new() { Id = "TidalCoin",     Name = "Tidal Coin",     Description = "Old currency, revealed by water. The kingdom it represents is unrecognizable." },
    ];

    private static readonly Dictionary<string, ResourceDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<ResourceDefinition> All => _definitions;

    public ResourceDefinition? GetById(string? id) =>
        id != null && _byId.TryGetValue(id, out var def) ? def : null;
}
