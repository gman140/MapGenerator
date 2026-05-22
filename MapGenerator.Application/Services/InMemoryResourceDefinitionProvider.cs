using MapGenerator.Domain.Enums;
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

        // ── Food — raw gatherable (variable quantity) ─────────────────────────
        new() { Id = "Berries",      Name = "Berries",       Description = "Small, tart, and numerous. Edible now. Better cooked into something.",       MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "Mushroom",     Name = "Mushroom",      Description = "A forest mushroom. Edible raw in a pinch, considerably better cooked.",      MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "Grain",        Name = "Grain",         Description = "Dry seed-heads gathered from open ground. Not edible raw, but useful.",      MinQuantity = 1, MaxQuantity = 3 },
        new() { Id = "WildCarrot",   Name = "Wild Carrot",   Description = "Thin, pale, and slightly bitter. Better than nothing. Slightly.",            MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "Acorn",        Name = "Acorn",         Description = "Bitter and starchy. Technically edible. The squirrels are doing something right.", MinQuantity = 2, MaxQuantity = 5 },
        new() { Id = "WildGarlic",   Name = "Wild Garlic",   Description = "Pungent in the way that announces itself well in advance.",                  MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "BaobabFruit",  Name = "Baobab Fruit",  Description = "Dry and chalky and vaguely citrus. From a tree that looks older than everything.", MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "Yam",          Name = "Yam",           Description = "A heavy, starchy root. Raw it is unpleasant. Cooked it is something else entirely.", MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "HeartOfPalm",  Name = "Heart of Palm", Description = "Pale and fibrous, cut from deep inside a palm. Mild to the point of apology.", MinQuantity = 1, MaxQuantity = 1 },
        new() { Id = "CattailRoot",  Name = "Cattail Root",  Description = "Starchy and wet and pulled from the mud. It has a kind of dignity about it.", MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "RockLichen",   Name = "Rock Lichen",   Description = "Scraped off a boulder. Grey, dry, and profoundly humble. Survivable.", MinQuantity = 1, MaxQuantity = 2 },
        new() { Id = "PineNut",      Name = "Pine Nut",      Description = "Small, oily, and worth the trouble of finding them.",                         MinQuantity = 1, MaxQuantity = 3 },

        // ── Food — cooked (quantity 1) ────────────────────────────────────────
        new() { Id = "CookedFish",     Name = "Cooked Fish",     Description = "Fish charred over open flame. Filling and good." },
        new() { Id = "CookedMushroom", Name = "Cooked Mushroom", Description = "Heat transformed them into something much better." },
        new() { Id = "MushroomSoup",   Name = "Mushroom Soup",   Description = "Salt, grain, and mushrooms, simmered until the broth is deep." },
        new() { Id = "BerryPie",       Name = "Berry Pie",       Description = "Imperfect crust, too-tart berries, genuinely good." },
        new() { Id = "HerbTea",        Name = "Herb Tea",        Description = "Bitter, fragrant, and better for you than it tastes." },
        new() { Id = "GarlicFlatbread",Name = "Garlic Flatbread",Description = "Wild garlic pressed into grain dough and cooked flat over the fire. Aggressively aromatic." },
        new() { Id = "YamStew",        Name = "Yam Stew",        Description = "Yam, grain, and salt, cooked low and slow. Dense and sustaining." },
        new() { Id = "AcornPorridge",  Name = "Acorn Porridge",  Description = "A patient preparation that makes acorns tolerable, even pleasant." },
        new() { Id = "BaobabBrew",     Name = "Baobab Brew",     Description = "The fruit dissolved in hot water. Tart, frothy, and inexplicably refreshing." },
        new() { Id = "CattailCakes",   Name = "Cattail Cakes",   Description = "Dried cattail root ground with grain and pressed into small cakes. Earthy and filling." },

        // ── Uncommon (quantity 1) ─────────────────────────────────────────────
        new() { Id = "Herbs", Name = "Herbs", Description = "Fragrant leaves of ambiguous medicinal value. Edible, and sometimes medicinal.",
                Traits = ItemTrait.Resource | ItemTrait.CombatConsumable,
                CombatHpRestore = 15, CombatBuffLabel = "Heal +15" },
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
        new() { Id = "BoneFragment", Name = "Bone Fragment", Description = "Old. You prefer not to speculate further.",
                Traits = ItemTrait.Resource | ItemTrait.CombatConsumable,
                CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 4f, CombatBuffTurns = 2,
                CombatBuffLabel = "Def +4" },
        new() { Id = "FrozenFlower",  Name = "Frozen Flower",  Description = "Perfectly preserved. Has been waiting to be found." },
        new() { Id = "Ash",           Name = "Ash",            Description = "Grey powder from something that burned completely." },
        new() { Id = "HollowStone",   Name = "Hollow Stone",   Description = "A stone with a void inside. Something used to live here." },
        new() { Id = "TidalCoin",     Name = "Tidal Coin",     Description = "Old currency, revealed by water. The kingdom it represents is unrecognizable." },

        // ── Dungeon keys (quantity 1) ─────────────────────────────────────────
        new() { Id = "IronKey",    Name = "Iron Key",    Description = "Pitted and cold. It fits a lock somewhere underground." },
        new() { Id = "FrostKey",   Name = "Frost Key",   Description = "The metal is always cold. Something below is waiting to be opened." },
        new() { Id = "AncientKey", Name = "Ancient Key", Description = "Older than the tomb it opens. Handle with the respect that implies." },
        new() { Id = "WoodKey",    Name = "Wood Key",    Description = "Carved from something that grew in the dark. Still smells of it." },

        // ── Dungeon-exclusive resources (quantity 1) ──────────────────────────
        new() { Id = "CaveCrystal",   Name = "Cave Crystal",   Description = "It grew in the dark over a very long time. It does not know what sunlight is." },
        new() { Id = "GlowingMoss",   Name = "Glowing Moss",   Description = "Bioluminescent and cold to the touch. It illuminates nothing useful." },
        new() { Id = "AncientShard",  Name = "Ancient Shard",  Description = "Ceramic, probably. From something that mattered to someone, once." },
        new() { Id = "DeepMushroom",  Name = "Deep Mushroom",  Description = "Pale and enormous and growing where it has no right to grow." },
        new() { Id = "FrozenRelic",   Name = "Frozen Relic",   Description = "Encased in ice so old it has gone blue. Something waits inside." },
        new() { Id = "BoneRune",      Name = "Bone Rune",      Description = "Carved into something that used to be alive. The carving is precise and deliberate." },
        new() { Id = "MossGem",       Name = "Moss Gem",       Description = "A gem colonized by moss so thoroughly that the two have become one thing." },
        new() { Id = "DeepOre",       Name = "Deep Ore",       Description = "Denser and darker than surface ore. It came from somewhere that pressure is a way of life." },
        new() { Id = "IceShard",      Name = "Ice Shard",      Description = "Brittle and clouded and very cold. It has been ice for a long time." },
        new() { Id = "TangledRoot",   Name = "Tangled Root",   Description = "Root matter from the labyrinth above. It found its way down here. So did you." },
        new() { Id = "TarnishedRelic",Name = "Tarnished Relic",Description = "Something ceremonial, its purpose lost. The tarnish is its memory now." },

        // ── Equipment — Weapons (physical) ───────────────────────────────────
        new() { Id = "FlintKnife", Name = "Flint Knife",  Description = "Flint knapped to a point and wrapped in fiber. Sharp enough to matter.", EquipmentSlot = "Weapon", AttackBonus = 1, WeaponDamageType = DamageType.Piercing },
        new() { Id = "WoodClub",   Name = "Wood Club",    Description = "Heavy and unbalanced. Effective in the manner that blunt things are.", EquipmentSlot = "Weapon", AttackBonus = 2, WeaponDamageType = DamageType.Bludgeoning },
        new() { Id = "IronDagger", Name = "Iron Dagger",  Description = "Short, direct, and faster than it looks.", EquipmentSlot = "Weapon", AttackBonus = 2, WeaponDamageType = DamageType.Piercing },
        new() { Id = "IronSword",  Name = "Iron Sword",   Description = "A dull but reliable iron blade. It has seen better years but none worse.", EquipmentSlot = "Weapon", AttackBonus = 3, WeaponDamageType = DamageType.Slashing },
        new() { Id = "IronMace",   Name = "Iron Mace",    Description = "A flanged iron head on a wrapped grip. It does not negotiate.", EquipmentSlot = "Weapon", AttackBonus = 3, WeaponDamageType = DamageType.Bludgeoning },

        // ── Equipment — Weapons (staves) ─────────────────────────────────────
        new() { Id = "EmberStaff", Name = "Ember Staff",  Description = "Char-blackened wood with a coal core. Warm to the touch. Your spells carry more heat.", EquipmentSlot = "Weapon", AttackBonus = 1, MagicBonus = 1, WeaponDamageType = DamageType.Fire },
        new() { Id = "FrostStaff", Name = "Frost Staff",  Description = "A branch that never thawed. The air around it is still. Your spells cut colder.", EquipmentSlot = "Weapon", AttackBonus = 1, MagicBonus = 1, WeaponDamageType = DamageType.Frost },
        new() { Id = "StormStaff", Name = "Storm Staff",  Description = "Quartz-tipped and faintly humming. It remembers the lightning that made it.", EquipmentSlot = "Weapon", AttackBonus = 1, MagicBonus = 1, WeaponDamageType = DamageType.Storm },
        new() { Id = "VineStaff",  Name = "Vine Staff",   Description = "Still growing, faintly. The bark is warm and the wood is alive. Your spells carry that.", EquipmentSlot = "Weapon", AttackBonus = 1, MagicBonus = 1, WeaponDamageType = DamageType.Nature },
        new() { Id = "ShadowStaff",Name = "Shadow Staff", Description = "Wood that absorbed something it shouldn't have. Dark to the core. Your spells follow.", EquipmentSlot = "Weapon", AttackBonus = 1, MagicBonus = 1, WeaponDamageType = DamageType.Dark },

        // ── Equipment — Armor ─────────────────────────────────────────────────
        new() { Id = "LeatherArmor",  Name = "Leather Armor",   Description = "Stitched from scraps. Better than nothing, which it slightly exceeds.", EquipmentSlot = "Armor", DefenseBonus = 6 },
        new() { Id = "BearHideCloak", Name = "Bear Hide Cloak", Description = "Heavy and warm and smells of the bear it used to be. Excellent protection.", EquipmentSlot = "Armor", DefenseBonus = 10 },
        new() { Id = "EchoMantle",    Name = "Echo Mantle",     Description = "Woven with quartz dust and something older. Physical blows still land. Other things don't.", EquipmentSlot = "Armor", ResistanceBonus = 6 },

        // ── Equipment — Hats ──────────────────────────────────────────────────
        new() { Id = "TrailCap",     Name = "Trail Cap",      Description = "Light and close-fitting. Worn by people who intend to hit things many times.", EquipmentSlot = "Hat", StaminaRegenBonus = 2 },
        new() { Id = "MeditationCowl", Name = "Meditation Cowl", Description = "Deep-hooded and very quiet inside. The wearer finds their focus faster.", EquipmentSlot = "Hat", ManaRegenBonus = 2 },
        new() { Id = "MossHood",     Name = "Moss Hood",      Description = "Still damp. Still growing. Something about wearing living things helps the body remember what it's doing.", EquipmentSlot = "Hat", HpRegenBonus = 1 },
        new() { Id = "HealersWrap",  Name = "Healer's Wrap",  Description = "Herb-soaked linen wound tight around the head. Every remedy you apply works better than it should.", EquipmentSlot = "Hat", HealBonus = 0.20f },

        // ── Enemy loot drops ──────────────────────────────────────────────────
        new() { Id = "SlimeGel", Name = "Slime Gel", Description = "A viscous, quivering substance left behind by the slime. Warm.",
                Traits = ItemTrait.Loot | ItemTrait.CombatConsumable,
                CombatHpRestore = 10, CombatBuffLabel = "Heal +10" },
        new() { Id = "GooShard",     Name = "Goo Shard",     Description = "A crystallized piece of slime. It shouldn't be solid, and yet." },
        new() { Id = "EggFragment",  Name = "Egg Fragment",  Description = "A piece of something that was an egg until recently. Still warm." },
        new() { Id = "GoldenYolk", Name = "Golden Yolk", Description = "The yolk of a sentient egg. It is deeply unsettling how golden it is.",
                Traits = ItemTrait.Loot | ItemTrait.CombatConsumable,
                CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 6f, CombatBuffTurns = 2,
                CombatBuffLabel = "Atk +6" },
        new() { Id = "WolfPelt",     Name = "Wolf Pelt",     Description = "Thick and coarse. The wolf won't need it anymore." },
        new() { Id = "Fang",         Name = "Fang",          Description = "Long and curved. Sharper than it has any business being." },
        new() { Id = "BearHide",     Name = "Bear Hide",     Description = "Dense, thick fur and leather together. Heavy with former intent." },
        new() { Id = "Claw",         Name = "Claw",          Description = "A bear's claw, curved like a question you'd rather not answer." },
        new() { Id = "TrollHide",   Name = "Troll Hide",   Description = "Rough and grey and very thick. It absorbed a great deal before this.", Traits = ItemTrait.Loot },
        new() { Id = "CrushedRock", Name = "Crushed Rock", Description = "Stone ground to coarse powder by something with more strength than patience.", Traits = ItemTrait.Loot },
        new() { Id = "HeronQuill",    Name = "Heron Quill",    Description = "A feather from a bird that has been dead for longer than it looks. Still sharp.", Traits = ItemTrait.Loot },
        new() { Id = "DampParchment", Name = "Damp Parchment", Description = "Covered in very small handwriting. Some of the entries are about you.", Traits = ItemTrait.Loot },
        new() { Id = "RatCrown",      Name = "Rat Crown",      Description = "Assembled from things rats considered valuable. The workmanship is sincere.", Traits = ItemTrait.Loot },

        // ── Combat consumables ────────────────────────────────────────────────
        new() { Id = "Poultice", Name = "Poultice", Description = "Herbs, moss, and reed, pressed together with intent. Smells of the ground. Works better than it smells.",
                Traits = ItemTrait.CombatConsumable,
                CombatHpRestore = 40, CombatBuffLabel = "Heal +40" },
        new() { Id = "StaminaDraught", Name = "Stamina Draught", Description = "A bitter, reedy brew. Tastes like effort. Restores your will to keep moving.",
                Traits = ItemTrait.CombatConsumable,
                CombatStaminaRestore = 5, CombatBuffLabel = "Stamina +5" },
        new() { Id = "WarPaint", Name = "War Paint", Description = "Charred bone and coal, smeared with purpose. It changes something behind the eyes.",
                Traits = ItemTrait.CombatConsumable,
                CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 10f, CombatBuffTurns = 3,
                CombatBuffLabel = "Atk +10" },
        new() { Id = "Antidote", Name = "Antidote", Description = "Herbs and amber steeped with pale mushroom. Bitter and immediate. The affliction recedes.",
                Traits = ItemTrait.CombatConsumable,
                CombatBuffLabel = "Cure Disease/Venom",
                ClearsStatuses = [ModifierStat.Disease, ModifierStat.Venom] },
        new() { Id = "SoothingSalve", Name = "Soothing Salve", Description = "Clay, moss, and herbs worked into a cooling paste. Applied quickly, it draws the harm out.",
                Traits = ItemTrait.CombatConsumable,
                CombatBuffLabel = "Cure Burn/Curse",
                ClearsStatuses = [ModifierStat.Burn, ModifierStat.StaminaDrain] },
    ];

    private static readonly Dictionary<string, ResourceDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<ResourceDefinition> All => _definitions;

    public ResourceDefinition? GetById(string? id) =>
        id != null && _byId.TryGetValue(id, out var def) ? def : null;
}
