using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class TileInventoryRepository : ITileInventoryRepository
{
    private readonly MongoDbContext _ctx;

    public TileInventoryRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        var idx = Builders<TileInventory>.IndexKeys.Combine(
            Builders<TileInventory>.IndexKeys.Ascending(t => t.Q),
            Builders<TileInventory>.IndexKeys.Ascending(t => t.R));
        _ctx.TileInventories.Indexes.CreateOne(
            new CreateIndexModel<TileInventory>(idx, new CreateIndexOptions { Unique = true }));
    }

    private FilterDefinition<TileInventory> CoordFilter(int q, int r) =>
        Builders<TileInventory>.Filter.And(
            Builders<TileInventory>.Filter.Eq(t => t.Q, q),
            Builders<TileInventory>.Filter.Eq(t => t.R, r));

    public Task<TileInventory?> GetAsync(int q, int r) =>
        _ctx.TileInventories.Find(CoordFilter(q, r)).FirstOrDefaultAsync()!;

    public async Task AddItemsAsync(int q, int r, string itemId, int quantity)
    {
        var filter = CoordFilter(q, r);
        var doc = await _ctx.TileInventories.Find(filter).FirstOrDefaultAsync();
        if (doc == null)
        {
            doc = new TileInventory { Q = q, R = r };
            doc.Items[itemId] = quantity;
            await _ctx.TileInventories.InsertOneAsync(doc);
        }
        else
        {
            doc.Items.TryGetValue(itemId, out int existing);
            doc.Items[itemId] = existing + quantity;
            await _ctx.TileInventories.ReplaceOneAsync(filter, doc);
        }
    }

    public async Task<bool> RemoveItemsAsync(int q, int r, string itemId, int quantity)
    {
        var filter = CoordFilter(q, r);
        var doc = await _ctx.TileInventories.Find(filter).FirstOrDefaultAsync();
        if (doc == null || !doc.Items.TryGetValue(itemId, out int have) || have < quantity)
            return false;

        if (have == quantity)
            doc.Items.Remove(itemId);
        else
            doc.Items[itemId] = have - quantity;

        await _ctx.TileInventories.ReplaceOneAsync(filter, doc);
        return true;
    }

    public Task DeleteAllAsync() =>
        _ctx.TileInventories.DeleteManyAsync(Builders<TileInventory>.Filter.Empty);
}
