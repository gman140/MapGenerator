using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryResourceDefinitionProvider : IResourceDefinitionProvider
{
    private static readonly ResourceDefinition[] _definitions =
    [
        // ── Gatherable materials (variable quantity) ──────────────────────────
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

        // ── Raw food ingredients (also ConsumableDefinition entries) ──────────
        new() { Id = "Berries",     Name = "Berries",       Description = "Small, tart, and numerous. Edible now. Better cooked into something.",           MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "Mushroom",    Name = "Mushroom",      Description = "A forest mushroom. Edible raw in a pinch, considerably better cooked.",          MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "Grain",       Name = "Grain",         Description = "Dry seed-heads gathered from open ground. Not edible raw, but useful.",          MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "WildCarrot",  Name = "Wild Carrot",   Description = "Thin, pale, and slightly bitter. Better than nothing. Slightly.",                MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "Acorn",       Name = "Acorn",         Description = "Bitter and starchy. Technically edible. The squirrels are doing something right.", MinQuantity = 2, MaxQuantity = 5 },
        new() { Id = "WildGarlic",  Name = "Wild Garlic",   Description = "Pungent in the way that announces itself well in advance.",                      MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "BaobabFruit", Name = "Baobab Fruit",  Description = "Dry and chalky and vaguely citrus. From a tree that looks older than everything.", MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "Yam",         Name = "Yam",           Description = "A heavy, starchy root. Raw it is unpleasant. Cooked it is something else entirely.", MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "HeartOfPalm",Name = "Heart of Palm",  Description = "Pale and fibrous, cut from deep inside a palm. Mild to the point of apology.",   MinQuantity = 1, MaxQuantity = 1 },
        new() { Id = "CattailRoot", Name = "Cattail Root",  Description = "Starchy and wet and pulled from the mud. It has a kind of dignity about it.",    MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "RockLichen",  Name = "Rock Lichen",   Description = "Scraped off a boulder. Grey, dry, and profoundly humble. Survivable.",            MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "PineNut",     Name = "Pine Nut",      Description = "Small, oily, and worth the trouble of finding them.",                             MinQuantity = 1, MaxQuantity = 3 },

        // ── Uncommon materials ────────────────────────────────────────────────
        new() { Id = "Herbs",     Name = "Herbs",     Description = "Fragrant leaves of ambiguous medicinal value. Edible, and sometimes medicinal." },
        new() { Id = "Feathers",  Name = "Feathers",  Description = "Shed or left behind. The bird is not here to explain." },
        new() { Id = "Salt",      Name = "Salt",      Description = "Crystallized from water, wind, or time." },
        new() { Id = "Clay",      Name = "Clay",      Description = "Dense, grey, workable. Remembers the shape of your hands." },
        new() { Id = "Coal",      Name = "Coal",      Description = "Compressed darkness. Burns long and hot and without comment." },
        new() { Id = "Sulfur",    Name = "Sulfur",    Description = "Yellow and pungent. You know what this is for. Everyone does." },
        new() { Id = "Amber",     Name = "Amber",     Description = "Tree resin, ancient, perfectly preserving something small inside." },
        new() { Id = "Quartz",    Name = "Quartz",    Description = "A clear crystal. Precise and self-important about it." },
        new() { Id = "Driftwood", Name = "Driftwood", Description = "Wood that has been somewhere and come back changed." },
        new() { Id = "Moss",      Name = "Moss",      Description = "Green and soft and quietly covering everything." },

        // ── Rare / Esoteric ───────────────────────────────────────────────────
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

        // ── Dungeon keys ──────────────────────────────────────────────────────
        new() { Id = "IronKey",    Name = "Iron Key",    Description = "Pitted and cold. It fits a lock somewhere underground." },
        new() { Id = "FrostKey",   Name = "Frost Key",   Description = "The metal is always cold. Something below is waiting to be opened." },
        new() { Id = "AncientKey", Name = "Ancient Key", Description = "Older than the tomb it opens. Handle with the respect that implies." },
        new() { Id = "WoodKey",    Name = "Wood Key",    Description = "Carved from something that grew in the dark. Still smells of it." },

        // ── Dungeon-exclusive resources ───────────────────────────────────────
        new() { Id = "CaveCrystal",    Name = "Cave Crystal",    Description = "It grew in the dark over a very long time. It does not know what sunlight is." },
        new() { Id = "GlowingMoss",    Name = "Glowing Moss",    Description = "Bioluminescent and cold to the touch. It illuminates nothing useful." },
        new() { Id = "AncientShard",   Name = "Ancient Shard",   Description = "Ceramic, probably. From something that mattered to someone, once." },
        new() { Id = "DeepMushroom",   Name = "Deep Mushroom",   Description = "Pale and enormous and growing where it has no right to grow." },
        new() { Id = "FrozenRelic",    Name = "Frozen Relic",    Description = "Encased in ice so old it has gone blue. Something waits inside." },
        new() { Id = "BoneRune",       Name = "Bone Rune",       Description = "Carved into something that used to be alive. The carving is precise and deliberate." },
        new() { Id = "MossGem",        Name = "Moss Gem",        Description = "A gem colonized by moss so thoroughly that the two have become one thing." },
        new() { Id = "DeepOre",        Name = "Deep Ore",        Description = "Denser and darker than surface ore. It came from somewhere that pressure is a way of life." },
        new() { Id = "IceShard",       Name = "Ice Shard",       Description = "Brittle and clouded and very cold. It has been ice for a long time." },
        new() { Id = "TangledRoot",    Name = "Tangled Root",    Description = "Root matter from the labyrinth above. It found its way down here. So did you." },
        new() { Id = "TarnishedRelic", Name = "Tarnished Relic", Description = "Something ceremonial, its purpose lost. The tarnish is its memory now." },

        // ── Enemy loot drops ──────────────────────────────────────────────────
        new() { Id = "SlimeGel",      Name = "Slime Gel",      Description = "A viscous, quivering substance left behind by the slime. Warm.",                      IsLoot = true },
        new() { Id = "GooShard",      Name = "Goo Shard",      Description = "A crystallized piece of slime. It shouldn't be solid, and yet.",                      IsLoot = true },
        new() { Id = "EggFragment",   Name = "Egg Fragment",   Description = "A piece of something that was an egg until recently. Still warm.",                     IsLoot = true },
        new() { Id = "GoldenYolk",    Name = "Golden Yolk",    Description = "The yolk of a sentient egg. It is deeply unsettling how golden it is.",                IsLoot = true },
        new() { Id = "WolfPelt",      Name = "Wolf Pelt",      Description = "Thick and coarse. The wolf won't need it anymore.",                                    IsLoot = true },
        new() { Id = "Fang",          Name = "Fang",           Description = "Long and curved. Sharper than it has any business being.",                             IsLoot = true },
        new() { Id = "BearHide",      Name = "Bear Hide",      Description = "Dense, thick fur and leather together. Heavy with former intent.",                     IsLoot = true },
        new() { Id = "Claw",          Name = "Claw",           Description = "A bear's claw, curved like a question you'd rather not answer.",                       IsLoot = true },
        new() { Id = "TrollHide",     Name = "Troll Hide",     Description = "Rough and grey and very thick. It absorbed a great deal before this.",                 IsLoot = true },
        new() { Id = "CrushedRock",   Name = "Crushed Rock",   Description = "Stone ground to coarse powder by something with more strength than patience.",         IsLoot = true },
        new() { Id = "HeronQuill",    Name = "Heron Quill",    Description = "A feather from a bird that has been dead for longer than it looks. Still sharp.",      IsLoot = true },
        new() { Id = "DampParchment", Name = "Damp Parchment", Description = "Covered in very small handwriting. Some of the entries are about you.",                IsLoot = true },
        new() { Id = "RatCrown",      Name = "Rat Crown",      Description = "Assembled from things rats considered valuable. The workmanship is sincere.",           IsLoot = true },
    ];

    private static readonly Dictionary<string, ResourceDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<ResourceDefinition> All => _definitions;

    public ResourceDefinition? GetById(string? id) =>
        id != null && _byId.TryGetValue(id, out var def) ? def : null;
}
