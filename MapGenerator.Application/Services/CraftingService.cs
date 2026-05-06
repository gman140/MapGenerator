using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class CraftingService
{
    private readonly ICraftingRecipeProvider _recipeProvider;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly IPlayerRepository _playerRepo;

    public CraftingService(
        ICraftingRecipeProvider recipeProvider,
        IResourceDefinitionProvider resourceProvider,
        IPlayerRepository playerRepo)
    {
        _recipeProvider  = recipeProvider;
        _resourceProvider = resourceProvider;
        _playerRepo      = playerRepo;
    }

    public async Task<CraftingResult> TryCraftAsync(Player player, string recipeId)
    {
        var recipe = _recipeProvider.GetById(recipeId);
        if (recipe == null)
            return Fail("Unknown recipe.");

        foreach (var ingredient in recipe.Ingredients)
        {
            player.Inventory.TryGetValue(ingredient.ResourceId, out int have);
            if (have >= ingredient.Quantity) continue;

            var res = _resourceProvider.GetById(ingredient.ResourceId);
            string name = res?.Name ?? ingredient.ResourceId;
            return Fail($"Not enough {name}. Need {ingredient.Quantity}, have {have}.");
        }

        foreach (var ingredient in recipe.Ingredients)
        {
            player.Inventory[ingredient.ResourceId] -= ingredient.Quantity;
            if (player.Inventory[ingredient.ResourceId] <= 0)
                player.Inventory.Remove(ingredient.ResourceId);
        }

        player.Inventory.TryGetValue(recipe.Id, out int existing);
        player.Inventory[recipe.Id] = existing + 1;

        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        return new CraftingResult { Success = true, CraftedItemId = recipe.Id, CraftedItemName = recipe.Name };
    }

    private static CraftingResult Fail(string msg) => new() { Success = false, ErrorMessage = msg };
}
