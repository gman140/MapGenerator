using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class TileInventoryService
{
    private readonly ITileInventoryRepository _tileInventoryRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly ICraftingRecipeProvider _recipeProvider;

    public TileInventoryService(
        ITileInventoryRepository tileInventoryRepo,
        IPlayerRepository playerRepo,
        IResourceDefinitionProvider resourceProvider,
        ICraftingRecipeProvider recipeProvider)
    {
        _tileInventoryRepo = tileInventoryRepo;
        _playerRepo        = playerRepo;
        _resourceProvider  = resourceProvider;
        _recipeProvider    = recipeProvider;
    }

    public Task<TileInventory?> GetTileInventoryAsync(int q, int r) =>
        _tileInventoryRepo.GetAsync(q, r);

    public async Task<string?> DropItemsAsync(Player player, string itemId, int quantity)
    {
        if (quantity <= 0) return "Invalid quantity.";
        if (!player.Inventory.TryGetValue(itemId, out int have) || have < quantity)
            return "You don't have enough of that item.";

        if (have == quantity)
            player.Inventory.Remove(itemId);
        else
            player.Inventory[itemId] = have - quantity;

        await _playerRepo.UpdateAsync(player);
        await _tileInventoryRepo.AddItemsAsync(player.Q, player.R, itemId, quantity);
        return null;
    }

    public async Task<string?> PickUpItemsAsync(Player player, string itemId, int quantity)
    {
        if (quantity <= 0) return "Invalid quantity.";

        bool ok = await _tileInventoryRepo.RemoveItemsAsync(player.Q, player.R, itemId, quantity);
        if (!ok) return "Not enough of that item here.";

        player.Inventory.TryGetValue(itemId, out int have);
        player.Inventory[itemId] = have + quantity;
        await _playerRepo.UpdateAsync(player);
        return null;
    }

    public string GetItemName(string itemId) =>
        _resourceProvider.GetById(itemId)?.Name ?? _recipeProvider.GetById(itemId)?.Name ?? itemId;

    public string? GetItemDescription(string itemId) =>
        _resourceProvider.GetById(itemId)?.Description ?? _recipeProvider.GetById(itemId)?.Description;
}
