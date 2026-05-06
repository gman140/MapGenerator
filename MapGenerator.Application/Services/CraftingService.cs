using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class CraftingService
{
    private readonly ICraftingRecipeProvider _recipeProvider;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly IPlayerRepository _playerRepo;
    private readonly MapGeneratorService _mapCache;

    public CraftingService(
        ICraftingRecipeProvider recipeProvider,
        IResourceDefinitionProvider resourceProvider,
        IPlayerRepository playerRepo,
        MapGeneratorService mapCache)
    {
        _recipeProvider   = recipeProvider;
        _resourceProvider = resourceProvider;
        _playerRepo       = playerRepo;
        _mapCache         = mapCache;
    }

    public async Task<CraftingResult> TryCraftAsync(Player player, string recipeId)
    {
        var recipe = _recipeProvider.GetById(recipeId);
        if (recipe == null)
            return Fail("Unknown recipe.");

        bool hasWorkshop = _mapCache.GetCachedTile(player.Q, player.R)?.Structure?.Type == StructureType.Workshop;

        foreach (var ingredient in recipe.Ingredients)
        {
            int required = RequiredQty(ingredient.Quantity, hasWorkshop);
            player.Inventory.TryGetValue(ingredient.ResourceId, out int have);
            if (have >= required) continue;
            var res = _resourceProvider.GetById(ingredient.ResourceId);
            return Fail($"Not enough {res?.Name ?? ingredient.ResourceId}. Need {required}, have {have}.");
        }

        foreach (var ingredient in recipe.Ingredients)
        {
            int required = RequiredQty(ingredient.Quantity, hasWorkshop);
            player.Inventory[ingredient.ResourceId] -= required;
            if (player.Inventory[ingredient.ResourceId] <= 0)
                player.Inventory.Remove(ingredient.ResourceId);
        }

        player.Inventory.TryGetValue(recipe.Id, out int existing);
        player.Inventory[recipe.Id] = existing + 1;

        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        return new CraftingResult { Success = true, CraftedItemId = recipe.Id, CraftedItemName = recipe.Name };
    }

    private static int RequiredQty(int baseQty, bool hasWorkshop) =>
        hasWorkshop ? (int)Math.Ceiling(baseQty / 2.0) : baseQty;

    private static CraftingResult Fail(string msg) => new() { Success = false, ErrorMessage = msg };
}
