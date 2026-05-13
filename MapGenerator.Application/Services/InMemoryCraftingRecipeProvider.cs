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
            Effects = [ItemEffect.ClearMovementCooldown],
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
