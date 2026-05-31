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
            Id               = "ReinforcedRod",
            Name             = "Reinforced Rod",
            Description      = "A fishing rod rebuilt with iridescent river scale. The line is stronger, the hook-set window wider. Unlocks mountain and reef fishing.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "FishingRod",  Quantity = 1 },
                new() { ResourceId = "RiverScale",  Quantity = 1 },
                new() { ResourceId = "Stone",        Quantity = 2 },
            ],
            Effects = [ItemEffect.ImproveAquaticGather],
        },
        new()
        {
            Id               = "LightLure",
            Name             = "Light Lure",
            Description      = "A rod fitted with a pale blue lure carved from a deepwater pearl. Fish bite faster. Unlocks kelp forest and hot spring fishing.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "ReinforcedRod",   Quantity = 1 },
                new() { ResourceId = "DeepwaterPearl",  Quantity = 1 },
                new() { ResourceId = "Feathers",         Quantity = 2 },
            ],
            Effects = [ItemEffect.ImproveAquaticGather],
        },
        new()
        {
            Id               = "LuckyRod",
            Name             = "Lucky Rod",
            Description      = "A light lure enhanced with a coral chip that hums faintly. Rare fish appear more often. Unlocks bioluminescent deep fishing.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "LightLure",   Quantity = 1 },
                new() { ResourceId = "CoralChip",   Quantity = 1 },
                new() { ResourceId = "RiverGlass",  Quantity = 2 },
            ],
            Effects = [ItemEffect.ImproveAquaticGather],
        },
        new()
        {
            Id               = "IceAuger",
            Name             = "Ice Auger",
            Description      = "A lucky rod fitted with an ancient lure that cuts through ice and draws creatures from frozen depths. Unlocks glacier and frozen shrine fishing.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "LuckyRod",    Quantity = 1 },
                new() { ResourceId = "AncientLure", Quantity = 1 },
                new() { ResourceId = "Ice",          Quantity = 3 },
            ],
            Effects = [ItemEffect.ImproveAquaticGather],
        },
        new()
        {
            Id          = "FreshwaterJig",
            Name        = "Freshwater Jig",
            Description = "Wound from river fiber and a river scale sliver. Draws freshwater fish to the surface.",
            Ingredients =
            [
                new() { ResourceId = "FishScale", Quantity = 3 },
                new() { ResourceId = "Fiber",     Quantity = 2 },
            ],
        },
        new()
        {
            Id          = "PearlSpinner",
            Name        = "Pearl Spinner",
            Description = "A sliver of deepwater pearl set on a wire frame. Catches light the way rare fish notice.",
            Ingredients =
            [
                new() { ResourceId = "FishScale", Quantity = 2 },
                new() { ResourceId = "FishFin",   Quantity = 2 },
            ],
        },
        new()
        {
            Id          = "ReefLure",
            Name        = "Reef Lure",
            Description = "Carved from salt crystal and threaded with brine. Saltwater fish take it for something familiar.",
            Ingredients =
            [
                new() { ResourceId = "FishScale",    Quantity = 2 },
                new() { ResourceId = "BrineCrystal", Quantity = 2 },
            ],
        },
        new()
        {
            Id               = "AbyssalHook",
            Name             = "Abyssal Hook",
            Description      = "An old hook set with a pearl fragment that glows faintly in darkness. Deep things come to look.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "FishFin",        Quantity = 3 },
                new() { ResourceId = "DeepwaterPearl", Quantity = 1 },
            ],
        },
        new()
        {
            Id               = "GlacialBarb",
            Name             = "Glacial Barb",
            Description      = "A shard of glacier ice bound in brine-cured cord. Cold-water fish mistake it for home.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "BrineCrystal", Quantity = 2 },
                new() { ResourceId = "GlacierShard", Quantity = 1 },
            ],
        },
        // ── Cooked fish recipes ───────────────────────────────────────────────

        // Tier 0
        new() { Id = "GrilledRiverTrout",   Name = "Grilled River Trout",   Description = "Simple fish over fire.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "RiverTrout",      Quantity = 1 }] },
        new() { Id = "PanFriedPerch",        Name = "Pan-Fried Perch",       Description = "Crispy-edged and quick.",  RequiresCampfire = true, Ingredients = [new() { ResourceId = "FreshwaterPerch", Quantity = 1 }] },
        new() { Id = "CatfishStew",          Name = "Catfish Stew",          Description = "Dense and sustaining.",    RequiresCampfire = true, Ingredients = [new() { ResourceId = "Catfish", Quantity = 1 }, new() { ResourceId = "WildGarlic", Quantity = 1 }, new() { ResourceId = "CattailRoot", Quantity = 1 }] },
        new() { Id = "SmokedRiverEel",       Name = "Smoked River Eel",      Description = "Slow-smoked over herbs.",  RequiresCampfire = true, Ingredients = [new() { ResourceId = "RiverEel",       Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "CrabBisque",           Name = "Crab Bisque",           Description = "Salt-simmered crab.",      RequiresCampfire = true, Ingredients = [new() { ResourceId = "FreshwaterCrab", Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "GiantCarpRoast",       Name = "Giant Carp Roast",      Description = "The whole fish, roasted.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "GiantCarp",      Quantity = 1 }] },
        new() { Id = "SaltedSandfish",       Name = "Salted Sandfish",       Description = "Pressed flat and salted.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Sandfish",       Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "GrilledMullet",        Name = "Grilled Mullet",        Description = "Thin fish, hot fire.",     RequiresCampfire = true, Ingredients = [new() { ResourceId = "TideMullet",     Quantity = 1 }] },
        new() { Id = "ShoreCrabSoup",        Name = "Shore Crab Soup",       Description = "Salt-water crab, simmered.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "ShoreCrab", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "SteamedFlounder",      Name = "Steamed Flounder",      Description = "Steamed over herbs.",      RequiresCampfire = true, Ingredients = [new() { ResourceId = "Flounder",       Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "SeahorseBroth",        Name = "Seahorse Broth",        Description = "A pale, restorative broth.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Seahorse",  Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "SwampEelStew",         Name = "Swamp Eel Stew",        Description = "Garlic-rich stew.",        RequiresCampfire = true, Ingredients = [new() { ResourceId = "SwampEel",       Quantity = 1 }, new() { ResourceId = "WildGarlic", Quantity = 1 }] },
        new() { Id = "MudfishPatties",       Name = "Mudfish Patties",       Description = "Ground with grain.",       RequiresCampfire = true, Ingredients = [new() { ResourceId = "Mudfish",        Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }] },
        new() { Id = "MarshCrabCakes",       Name = "Marsh Crab Cakes",      Description = "Crab pressed with grain.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "MarshCrab",      Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }] },

        // Tier 1
        new() { Id = "MountainTroutBraise",  Name = "Mountain Trout Braise", Description = "Salt-braised mountain trout.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "MountainTrout",  Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "SmokedSalmon",         Name = "Smoked Salmon",         Description = "Slow-smoked over herbs.",      RequiresCampfire = true, Ingredients = [new() { ResourceId = "Salmon",         Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "GlacialCharFillet",    Name = "Glacial Char Fillet",   Description = "Salt-dressed cold-water char.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "GlacialChar",    Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "GoldenTroutTartare",   Name = "Golden Trout Tartare",  Description = "Raw golden trout with salt.",   RequiresCampfire = true, Ingredients = [new() { ResourceId = "GoldenTrout",   Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "ClownfishSkewers",     Name = "Clownfish Skewers",     Description = "Bright little fish on a stick.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Clownfish",     Quantity = 1 }] },
        new() { Id = "ParrotfishCurry",      Name = "Parrotfish Curry",      Description = "Garlic and herb curry.",         RequiresCampfire = true, Ingredients = [new() { ResourceId = "Parrotfish",    Quantity = 1 }, new() { ResourceId = "WildGarlic", Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "TriggersteakFillet",   Name = "Triggerfish Steak",     Description = "Dense, aggressive fish steak.",  RequiresCampfire = true, Ingredients = [new() { ResourceId = "Triggerfish",   Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "MorayEelSaute",        Name = "Moray Eel Sauté",       Description = "Garlic-sautéed eel.",            RequiresCampfire = true, Ingredients = [new() { ResourceId = "MorayEel",      Quantity = 1 }, new() { ResourceId = "WildGarlic", Quantity = 1 }] },
        new() { Id = "LionfishFillet",       Name = "Lionfish Fillet",       Description = "Carefully deboned and herb-pressed.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Lionfish", Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "MirrorCarpBraise",     Name = "Mirror Carp Braise",    Description = "Slow-braised until it falls apart.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "MirrorCarp", Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "LakeBassStew",         Name = "Lake Bass Stew",        Description = "Root and fish, made steady.",     RequiresCampfire = true, Ingredients = [new() { ResourceId = "LakeBass",    Quantity = 1 }, new() { ResourceId = "CattailRoot", Quantity = 1 }] },
        new() { Id = "SearedMoonfish",       Name = "Seared Moonfish",       Description = "Herb-pressed and seared at dusk.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Moonfish",   Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "LegendaryMoonfishBroth", Name = "Legendary Moonfish Broth", Description = "A broth that tastes like something earned.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "LegendaryMoonfish", Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },

        // Tier 2
        new() { Id = "KelpWrappedGrouper",   Name = "Kelp-Wrapped Grouper",  Description = "Wrapped in kelp and slow-cooked.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "KelpGrouper",  Quantity = 1 }, new() { ResourceId = "Reed",  Quantity = 1 }] },
        new() { Id = "RockfishChowder",      Name = "Rockfish Chowder",      Description = "The old fish, made into chowder.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Rockfish",     Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "OctopusStew",          Name = "Octopus Stew",          Description = "Eight arms, slow-simmered.",       RequiresCampfire = true, Ingredients = [new() { ResourceId = "Octopus",      Quantity = 1 }, new() { ResourceId = "WildGarlic", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "SeaBassFillet",        Name = "Sea Bass Fillet",       Description = "Herb-crusted and reliable.",       RequiresCampfire = true, Ingredients = [new() { ResourceId = "SeaBass",      Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "SquidInkStew",         Name = "Squid Ink Stew",        Description = "The ink turned everything black.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "GiantSquid",   Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "AnglerFishFillets",    Name = "Anglerfish Fillets",    Description = "Herb-dressed deep-sea fish.",      RequiresCampfire = true, Ingredients = [new() { ResourceId = "Anglerfish",   Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "GhostfishSashimi",     Name = "Ghostfish Sashimi",     Description = "Raw and translucent, dressed in salt.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "Ghostfish",  Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "AbyssalEelRoast",      Name = "Abyssal Eel Roast",     Description = "From deep water, roasted.",        RequiresCampfire = true, Ingredients = [new() { ResourceId = "AbyssalEel",   Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "CaveFishBroth",        Name = "Cave Fish Broth",       Description = "Pale fish, pale broth.",           RequiresCampfire = true, Ingredients = [new() { ResourceId = "PaleCavefish", Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "VolcanicEelRoast",     Name = "Volcanic Eel Roast",    Description = "Roasted with sulfur.",             RequiresCampfire = true, Ingredients = [new() { ResourceId = "VolcanicEel",  Quantity = 1 }, new() { ResourceId = "Sulfur",Quantity = 1 }] },
        new() { Id = "SpicedShrimp",         Name = "Spiced Shrimp",         Description = "Herb-spiced thermal shrimp.",      RequiresCampfire = true, Ingredients = [new() { ResourceId = "ThermophilicShrimp", Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },

        // Tier 3
        new() { Id = "BioluminescentBroth",  Name = "Bioluminescent Broth",  Description = "A broth that still faintly glows.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "BioluminescentJellyfish", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "AbyssalAnglerStew",    Name = "Abyssal Anglerfish Stew", Description = "Ancient deep-sea stew.",         RequiresCampfire = true, Ingredients = [new() { ResourceId = "AbyssalAnglerfish", Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }] },
        new() { Id = "GlowingSquidBroth",    Name = "Glowing Squid Broth",   Description = "The ink glows even after cooking.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "GlowingSquid",  Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },

        // Tier 4
        new() { Id = "IceShrimpBisque",      Name = "Ice Shrimp Bisque",     Description = "Pale bisque from frozen shrimp.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "IceShrimp",    Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "CrystalFishSashimi",   Name = "Crystal Fish Sashimi",  Description = "Thin-sliced crystal fish.",       RequiresCampfire = true, Ingredients = [new() { ResourceId = "CrystalFish",  Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "FrozenEelStew",        Name = "Frozen Eel Stew",       Description = "Slow preparation, slow reward.",  RequiresCampfire = true, Ingredients = [new() { ResourceId = "FrozenEel",    Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }, new() { ResourceId = "Salt", Quantity = 1 }] },
        new() { Id = "ArcticTartare",        Name = "Arctic Tartare",        Description = "Raw arctic trout with salt.",     RequiresCampfire = true, Ingredients = [new() { ResourceId = "ArcticTrout",  Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "PolarStarBroth",       Name = "Polar Star Broth",      Description = "The star's grip, in broth form.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "PolarStar",    Quantity = 1 }, new() { ResourceId = "Salt",  Quantity = 1 }] },
        new() { Id = "AncientFishStew",      Name = "Ancient Fish Stew",     Description = "Fifty million years of patience, served hot.", RequiresCampfire = true, Ingredients = [new() { ResourceId = "AncientFish", Quantity = 1 }, new() { ResourceId = "Herbs", Quantity = 1 }, new() { ResourceId = "Grain", Quantity = 1 }] },

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

        // ── Equipment — Weapons (physical) ────────────────────────────────────
        new()
        {
            Id          = "FlintKnife",
            Name        = "Flint Knife",
            Description = "Two flint shards knapped together and wrapped tight with fiber. Not elegant. Point is sharp.",
            Ingredients =
            [
                new() { ResourceId = "Flint", Quantity = 2 },
                new() { ResourceId = "Fiber", Quantity = 2 },
            ],
            Effects = [],
        },
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
            Id               = "IronDagger",
            Name             = "Iron Dagger",
            Description      = "Two ore smelted narrow and short. Fits in a hand. Useful in tight spaces and tighter situations.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Ore",   Quantity = 2 },
                new() { ResourceId = "Fiber", Quantity = 1 },
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
        new()
        {
            Id               = "IronMace",
            Name             = "Iron Mace",
            Description      = "Three ore shaped into something that makes a point through volume and weight. Fiber wrapped grip. Honest weapon.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Ore",   Quantity = 3 },
                new() { ResourceId = "Stone", Quantity = 1 },
                new() { ResourceId = "Fiber", Quantity = 1 },
            ],
            Effects = [],
        },

        // ── Equipment — Weapons (staves) ──────────────────────────────────────
        new()
        {
            Id               = "EmberStaff",
            Name             = "Ember Staff",
            Description      = "Wood hollowed and packed with coal and amber. Warm to the touch before you even try. Fire responds to it.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",  Quantity = 2 },
                new() { ResourceId = "Coal",  Quantity = 2 },
                new() { ResourceId = "Amber", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "FrostStaff",
            Name             = "Frost Staff",
            Description      = "A branch from somewhere cold, wrapped with ice held in place by quartz. It has never been warm. Frost obeys it.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",   Quantity = 2 },
                new() { ResourceId = "Ice",    Quantity = 2 },
                new() { ResourceId = "Quartz", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "StormStaff",
            Name             = "Storm Staff",
            Description      = "Quartz-tipped and wrapped with feathers stripped from birds that flew too high. There's a low hum. Storm listens.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",     Quantity = 2 },
                new() { ResourceId = "Quartz",   Quantity = 2 },
                new() { ResourceId = "Feathers", Quantity = 2 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "VineStaff",
            Name             = "Vine Staff",
            Description      = "Living wood, still rooted in habit. Moss grows where you hold it. Nature hasn't decided you're separate from it yet.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",  Quantity = 2 },
                new() { ResourceId = "Moss",  Quantity = 2 },
                new() { ResourceId = "Herbs", Quantity = 2 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "ShadowStaff",
            Name             = "Shadow Staff",
            Description      = "Wood that absorbed something it shouldn't have, wrapped with crow feather and hollow stone. Dark. Willing.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Wood",       Quantity = 2 },
                new() { ResourceId = "Coal",       Quantity = 1 },
                new() { ResourceId = "CrowFeather",Quantity = 1 },
                new() { ResourceId = "HollowStone",Quantity = 1 },
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
        new()
        {
            Id               = "EchoMantle",
            Name             = "Echo Mantle",
            Description      = "Fiber woven with quartz dust and a cracked orb at the chest. Physical blows still land. Other things bounce.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Fiber",     Quantity = 3 },
                new() { ResourceId = "Quartz",    Quantity = 1 },
                new() { ResourceId = "CrackedOrb",Quantity = 1 },
            ],
            Effects = [],
        },

        // ── Equipment — Hats ──────────────────────────────────────────────────
        new()
        {
            Id               = "TrailCap",
            Name             = "Trail Cap",
            Description      = "Fiber and feathers stitched flat and close. Light enough to forget about. Your body remembers it anyway — keeps moving.",
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
            Id               = "MeditationCowl",
            Name             = "Meditation Cowl",
            Description      = "Deep-hooded, quartz-lined, and very quiet inside. Focus comes faster. Mana follows.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Fiber",  Quantity = 2 },
                new() { ResourceId = "Quartz", Quantity = 1 },
                new() { ResourceId = "Amber",  Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "MossHood",
            Name             = "Moss Hood",
            Description      = "Moss over reed over fiber, and still damp. Wearing something alive does something to the body. You stop bleeding as fast.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Moss",  Quantity = 3 },
                new() { ResourceId = "Reed",  Quantity = 2 },
                new() { ResourceId = "Herbs", Quantity = 1 },
            ],
            Effects = [],
        },
        new()
        {
            Id               = "HealersWrap",
            Name             = "Healer's Wrap",
            Description      = "Herb-soaked linen wound around the head and knotted at the back. Remedies applied while wearing it go further than they should.",
            RequiresWorkshop = true,
            Ingredients      =
            [
                new() { ResourceId = "Herbs", Quantity = 3 },
                new() { ResourceId = "Fiber", Quantity = 2 },
                new() { ResourceId = "Salt",  Quantity = 1 },
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
