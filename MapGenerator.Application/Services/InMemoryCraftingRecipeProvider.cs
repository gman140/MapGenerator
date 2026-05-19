using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryCraftingRecipeProvider : ICraftingRecipeProvider
{
    private static readonly CraftingRecipe[] _recipes =
    [
        new()
        {
            Id          = "Boat",
            Name        = "Boat",
            Description = "Eight planks, three pieces of driftwood, rope made of fiber, sealed with reed. It floats. This is the main thing.",
            Ingredients =
            [
                new() { ResourceId = "Wood",      Quantity = 8 },
                new() { ResourceId = "Driftwood", Quantity = 3 },
                new() { ResourceId = "Fiber",     Quantity = 4 },
                new() { ResourceId = "Reed",      Quantity = 3 },
            ],
            Effects = [ItemEffect.AllowOceanTraversal, ItemEffect.AllowLakeTraversal],
        },
        new()
        {
            Id          = "FishingRod",
            Name        = "Fishing Rod",
            Description = "A bent stick, some line, a feather at the tip to watch. Old technology. Still works.",
            Ingredients =
            [
                new() { ResourceId = "Wood",     Quantity = 2 },
                new() { ResourceId = "Fiber",    Quantity = 3 },
                new() { ResourceId = "Feathers", Quantity = 1 },
            ],
            Effects = [ItemEffect.ImproveAquaticGather],
        },
        new()
        {
            Id          = "WarmCloak",
            Name        = "Warm Cloak",
            Description = "Fiber and feathers and moss, layered and stitched. Something between wearing a blanket and wearing a small animal. Warm, regardless.",
            Ingredients =
            [
                new() { ResourceId = "Fiber",    Quantity = 4 },
                new() { ResourceId = "Feathers", Quantity = 3 },
                new() { ResourceId = "Moss",     Quantity = 2 },
            ],
            Effects = [ItemEffect.ReduceColdBiomeCooldown],
        },
        new()
        {
            Id          = "Pickaxe",
            Name        = "Pickaxe",
            Description = "Flint on wood, bound with fiber. The flint is sharp. The binding is tight. You've made worse tools.",
            Ingredients =
            [
                new() { ResourceId = "Flint", Quantity = 3 },
                new() { ResourceId = "Wood",  Quantity = 2 },
                new() { ResourceId = "Fiber", Quantity = 2 },
            ],
            Effects = [ItemEffect.ImproveMineralGather],
        },
        new()
        {
            Id          = "Lantern",
            Name        = "Lantern",
            Description = "Quartz holds light differently than glass. Clay holds the frame. Coal provides the reason. The result is useful in the dark.",
            Ingredients =
            [
                new() { ResourceId = "Quartz", Quantity = 1 },
                new() { ResourceId = "Clay",   Quantity = 2 },
                new() { ResourceId = "Coal",   Quantity = 2 },
            ],
            Effects = [ItemEffect.ImproveUndergroundGather],
        },
        new()
        {
            Id           = "Poultice",
            Name         = "Poultice",
            Description  = "Herbs, moss, and reed, pressed together with intent. Smells of the ground. Works better than it smells.",
            IsConsumable = true,
            Ingredients  =
            [
                new() { ResourceId = "Herbs", Quantity = 3 },
                new() { ResourceId = "Moss",  Quantity = 2 },
                new() { ResourceId = "Reed",  Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id           = "Rope",
            Name         = "Rope",
            Description  = "Five lengths of fiber twisted into one longer and stronger one. The math is unfavorable but the result is reliable.",
            IsConsumable = true,
            Ingredients  =
            [
                new() { ResourceId = "Fiber", Quantity = 5 },
                new() { ResourceId = "Reed",  Quantity = 3 },
            ],
            Effects = [ItemEffect.AllowCliffTraversal],
        },
        new()
        {
            Id          = "Compass",
            Name        = "Compass",
            Description = "Two old coins, a smooth river pebble, and something that was clearly an orb once. It points. You choose to trust it.",
            Ingredients =
            [
                new() { ResourceId = "TidalCoin", Quantity = 2 },
                new() { ResourceId = "RiverGlass",Quantity = 1 },
                new() { ResourceId = "CrackedOrb",Quantity = 1 },
            ],
            Effects = [ItemEffect.ReduceMovementCooldown],
        },
        new()
        {
            Id          = "BoneFlute",
            Name        = "Bone Flute",
            Description = "Two bone fragments, two reeds. The holes were placed by someone who understood something you are still working out. It sounds correct.",
            Ingredients =
            [
                new() { ResourceId = "BoneFragment", Quantity = 2 },
                new() { ResourceId = "Reed",         Quantity = 2 },
            ],
            Effects = [ItemEffect.CharmNearbyCreatures],
        },
        new()
        {
            Id          = "AmberVial",
            Name        = "Amber Vial",
            Description = "Two pieces of amber, one hollow stone, two herbs. Whatever is inside has been there longer than you. The seal has not been broken. You are not sure you should be the one to break it.",
            Ingredients =
            [
                new() { ResourceId = "Amber",      Quantity = 2 },
                new() { ResourceId = "HollowStone",Quantity = 1 },
                new() { ResourceId = "Herbs",      Quantity = 2 },
            ],
            Effects = [ItemEffect.PreserveRareFinds],
        },

        // ── Campfire-exclusive (cooking) ──────────────────────────────────────────
        new()
        {
            Id               = "CookedFish",
            Name             = "Cooked Fish",
            Description      = "One fish, held over the fire until it's done. The smoke gets into it just right.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      = [ new() { ResourceId = "Fish", Quantity = 1 } ],
            Effects          = [],
        },
        new()
        {
            Id               = "CookedMushroom",
            Name             = "Cooked Mushroom",
            Description      = "Heat changes them into something the raw version only suggested.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      = [ new() { ResourceId = "Mushroom", Quantity = 1 } ],
            Effects          = [],
        },
        new()
        {
            Id               = "MushroomSoup",
            Name             = "Mushroom Soup",
            Description      = "Two mushrooms, a measure of grain, a pinch of salt. Simmered until the broth is deep and the cold is gone. Reduces hunger drain by 40% for 24 actions.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      =
            [
                new() { ResourceId = "Mushroom", Quantity = 2 },
                new() { ResourceId = "Grain",    Quantity = 1 },
                new() { ResourceId = "Salt",     Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "BerryPie",
            Name             = "Berry Pie",
            Description      = "Three handfuls of berries, two measures of grain, pressed into something that qualifies as a pie. It is better than it looks. Improves gather yield by 50% for 16 gathers.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      =
            [
                new() { ResourceId = "Berries", Quantity = 3 },
                new() { ResourceId = "Grain",   Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "HerbTea",
            Name             = "Herb Tea",
            Description      = "Two bundles of herbs steeped in water over the fire. Bitter, fragrant, and genuinely useful. Reduces action cooldowns by 50% for 20 actions.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      = [ new() { ResourceId = "Herbs", Quantity = 2 } ],
            Effects          = [],
        },
        new()
        {
            Id               = "GarlicFlatbread",
            Name             = "Garlic Flatbread",
            Description      = "Wild garlic pounded into grain dough, pressed flat, cooked until blistered. Pungent and good. Improves gather yield by 50% for 12 gathers.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      =
            [
                new() { ResourceId = "WildGarlic", Quantity = 2 },
                new() { ResourceId = "Grain",      Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "YamStew",
            Name             = "Yam Stew",
            Description      = "Yam and grain simmered low with salt until it thickens. Dense, slow, sustaining. Reduces hunger drain by 40% for 24 actions.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      =
            [
                new() { ResourceId = "Yam",   Quantity = 1 },
                new() { ResourceId = "Grain", Quantity = 1 },
                new() { ResourceId = "Salt",  Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "AcornPorridge",
            Name             = "Acorn Porridge",
            Description      = "Three acorns leached and ground with grain, cooked until almost palatable. Reduces action cooldowns by 35% for 12 actions.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      =
            [
                new() { ResourceId = "Acorn", Quantity = 3 },
                new() { ResourceId = "Grain", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "BaobabBrew",
            Name             = "Baobab Brew",
            Description      = "Baobab fruit dissolved in hot water. Tart, slightly fizzy, deeply strange. Reduces action cooldowns by 40% for 32 actions.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      = [ new() { ResourceId = "BaobabFruit", Quantity = 2 } ],
            Effects          = [],
        },
        new()
        {
            Id               = "CattailCakes",
            Name             = "Cattail Cakes",
            Description      = "Cattail root dried and ground with grain, pressed into small flat cakes. Earthy, filling, unremarkable in the best way.",
            RequiresCampfire = true,
            IsConsumable     = true,
            Ingredients      =
            [
                new() { ResourceId = "CattailRoot", Quantity = 2 },
                new() { ResourceId = "Grain",       Quantity = 1 },
            ],
            Effects = [],
        },

        // ── Workshop-exclusive ────────────────────────────────────────────────────
        new()
        {
            Id               = "Spyglass",
            Name             = "Spyglass",
            Description      = "Quartz ground to a lens, set in a wood-and-amber casing. You can see further than you should be able to. That feels like a gift you should use carefully.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",   Quantity = 2 },
                new() { ResourceId = "Quartz", Quantity = 2 },
                new() { ResourceId = "Amber",  Quantity = 1 },
            ],
            Effects = [ItemEffect.ExtendRevealRadius],
        },
        new()
        {
            Id               = "SteelIngot",
            Name             = "Steel Ingot",
            Description      = "Four ore, three coal, one flint to strike the heat right. The forge does most of the work. You just had to know to ask it.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Ore",   Quantity = 4 },
                new() { ResourceId = "Coal",  Quantity = 3 },
                new() { ResourceId = "Flint", Quantity = 1 },
            ],
            Effects = [ItemEffect.ImproveMineralGather],
        },
        new()
        {
            Id               = "ApothecaryKit",
            Name             = "Apothecary Kit",
            Description      = "Herbs, amber, a hollow stone, and quartz ground fine as dust. The kit does not tell you what to do. It assumes you already know.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Herbs",      Quantity = 3 },
                new() { ResourceId = "Amber",      Quantity = 2 },
                new() { ResourceId = "HollowStone",Quantity = 1 },
                new() { ResourceId = "Quartz",     Quantity = 1 },
            ],
            Effects = [ItemEffect.ImproveAllGather],
        },

        // ── Equipment — Weapons ───────────────────────────────────────────────
        new()
        {
            Id               = "WoodClub",
            Name             = "Wood Club",
            Description      = "Three pieces of wood and two shards of flint, and something to be afraid of. Heavy and graceless. Gets the job done.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",  Quantity = 3 },
                new() { ResourceId = "Flint", Quantity = 2 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "IronSword",
            Name             = "Iron Sword",
            Description      = "Three ore smelted with coal, shaped on a wood haft. Dull in sunlight. Reliable in the dark. You made it yourself, which means something.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Ore",   Quantity = 3 },
                new() { ResourceId = "Coal",  Quantity = 2 },
                new() { ResourceId = "Wood",  Quantity = 1 },
            ],
            Effects = [],
        },

        // ── Equipment — Armor ─────────────────────────────────────────────────
        new()
        {
            Id               = "LeatherArmor",
            Name             = "Leather Armor",
            Description      = "Four lengths of fiber and two feathers, pressed and stitched into something that stops things. Smells of sweat and intent.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Fiber",    Quantity = 4 },
                new() { ResourceId = "Feathers", Quantity = 3 },
                new() { ResourceId = "Peat",     Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "BearHideCloak",
            Name             = "Bear Hide Cloak",
            Description      = "Two bear hides and three lengths of fiber, heavy and warm and made from something that was formidable. Some of that transfers over.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "BearHide", Quantity = 2 },
                new() { ResourceId = "Fiber",    Quantity = 3 },
            ],
            Effects = [],
        },

        // ── Equipment — Hats ──────────────────────────────────────────────────
        new()
        {
            Id               = "TravelHat",
            Name             = "Travel Hat",
            Description      = "Three fiber, two feathers, shaped into something that moves when you do. A well-worn hat learns your habits.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Fiber",    Quantity = 3 },
                new() { ResourceId = "Feathers", Quantity = 2 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "HoodedCowl",
            Name             = "Hooded Cowl",
            Description      = "Four fiber and one wolf pelt, stitched into something that keeps the darkness where it belongs — outside.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Fiber",    Quantity = 4 },
                new() { ResourceId = "WolfPelt", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id          = "StaminaDraught",
            Name        = "Stamina Draught",
            Description = "Reed and peat, steeped until it becomes something worth drinking. Only barely.",
            IsConsumable = true,
            Ingredients =
            [
                new() { ResourceId = "Reed", Quantity = 2 },
                new() { ResourceId = "Peat", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id          = "WarPaint",
            Name        = "War Paint",
            Description = "Crushed coal and bone fragment, mixed until it looks like something that means business.",
            IsConsumable = true,
            Ingredients =
            [
                new() { ResourceId = "Coal",         Quantity = 1 },
                new() { ResourceId = "BoneFragment", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id           = "Antidote",
            Name         = "Antidote",
            Description  = "Herbs and amber steeped with pale mushroom. Bitter and immediate. Cures Disease and Venom.",
            IsConsumable = true,
            Ingredients  =
            [
                new() { ResourceId = "Herbs",       Quantity = 2 },
                new() { ResourceId = "PaleMushroom",Quantity = 1 },
                new() { ResourceId = "Amber",       Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id           = "SoothingSalve",
            Name         = "Soothing Salve",
            Description  = "Clay, moss, and herbs worked into a cooling paste. Cures Burn and stamina drain.",
            IsConsumable = true,
            Ingredients  =
            [
                new() { ResourceId = "Herbs", Quantity = 2 },
                new() { ResourceId = "Moss",  Quantity = 2 },
                new() { ResourceId = "Clay",  Quantity = 1 },
            ],
            Effects = [],
        },
    ];

    private static readonly Dictionary<string, CraftingRecipe> _byId =
        _recipes.ToDictionary(r => r.Id);

    public IReadOnlyList<CraftingRecipe> All => _recipes;

    public CraftingRecipe? GetById(string? id) =>
        id != null && _byId.TryGetValue(id, out var recipe) ? recipe : null;

    public bool PlayerHasEffect(Player player, ItemEffect effect) =>
        _recipes.Any(r =>
            r.Effects.Contains(effect) &&
            player.Inventory.TryGetValue(r.Id, out int qty) && qty > 0);
}
