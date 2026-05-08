using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface ITileInventoryRepository
{
    Task<TileInventory?> GetAsync(int q, int r);
    Task AddItemsAsync(int q, int r, string itemId, int quantity);
    Task<bool> RemoveItemsAsync(int q, int r, string itemId, int quantity);
    Task DeleteAllAsync();
}
