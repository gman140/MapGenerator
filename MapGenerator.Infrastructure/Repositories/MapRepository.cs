using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class MapRepository : IMapRepository
{
    private readonly MongoDbContext _ctx;

    public MapRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        var idx = Builders<HexTile>.IndexKeys.Combine(
            Builders<HexTile>.IndexKeys.Ascending(t => t.Q),
            Builders<HexTile>.IndexKeys.Ascending(t => t.R));
        _ctx.Tiles.Indexes.CreateOne(new CreateIndexModel<HexTile>(idx, new CreateIndexOptions { Unique = true }));
    }

    public Task<HexTile?> GetTileAsync(int q, int r) =>
        _ctx.Tiles.Find(t => t.Q == q && t.R == r).FirstOrDefaultAsync()!;

    public async Task<List<HexTile>> GetTilesInRangeAsync(int minQ, int maxQ, int minR, int maxR)
    {
        var filter = Builders<HexTile>.Filter.And(
            Builders<HexTile>.Filter.Gte(t => t.Q, minQ),
            Builders<HexTile>.Filter.Lte(t => t.Q, maxQ),
            Builders<HexTile>.Filter.Gte(t => t.R, minR),
            Builders<HexTile>.Filter.Lte(t => t.R, maxR));
        return await _ctx.Tiles.Find(filter).ToListAsync();
    }

    public Task<MapConfig?> GetCurrentConfigAsync() =>
        _ctx.MapConfigs.Find(_ => true).SortByDescending(c => c.GeneratedAt).FirstOrDefaultAsync()!;

    public async Task SaveTilesAsync(IEnumerable<HexTile> tiles)
    {
        const int batch = 1000;
        var list = tiles.ToList();
        for (int i = 0; i < list.Count; i += batch)
        {
            var slice = list.GetRange(i, Math.Min(batch, list.Count - i));
            await _ctx.Tiles.InsertManyAsync(slice);
        }
    }

    public Task SaveConfigAsync(MapConfig config) => _ctx.MapConfigs.InsertOneAsync(config);

    public Task DeleteAllTilesAsync() => _ctx.Tiles.DeleteManyAsync(_ => true);

    public async Task<int> IncrementEggCountAsync(int q, int r)
    {
        var filter = Builders<HexTile>.Filter.And(
            Builders<HexTile>.Filter.Eq(t => t.Q, q),
            Builders<HexTile>.Filter.Eq(t => t.R, r));
        var update = Builders<HexTile>.Update.Inc(t => t.EggCount, 1);
        var opts = new FindOneAndUpdateOptions<HexTile> { ReturnDocument = ReturnDocument.After };
        var tile = await _ctx.Tiles.FindOneAndUpdateAsync(filter, update, opts);
        return tile?.EggCount ?? 0;
    }

    public Task PlaceSignAsync(int q, int r, string text, string authorName)
    {
        var filter = Builders<HexTile>.Filter.And(
            Builders<HexTile>.Filter.Eq(t => t.Q, q),
            Builders<HexTile>.Filter.Eq(t => t.R, r));
        var update = Builders<HexTile>.Update
            .Set(t => t.SignText, text)
            .Set(t => t.SignAuthor, authorName);
        return _ctx.Tiles.UpdateOneAsync(filter, update);
    }

    public Task SetStructureAsync(int q, int r, TileStructure structure)
    {
        var filter = Builders<HexTile>.Filter.And(
            Builders<HexTile>.Filter.Eq(t => t.Q, q),
            Builders<HexTile>.Filter.Eq(t => t.R, r));
        var update = Builders<HexTile>.Update.Set(t => t.Structure, structure);
        return _ctx.Tiles.UpdateOneAsync(filter, update);
    }

    public Task RemoveStructureAsync(int q, int r)
    {
        var filter = Builders<HexTile>.Filter.And(
            Builders<HexTile>.Filter.Eq(t => t.Q, q),
            Builders<HexTile>.Filter.Eq(t => t.R, r));
        var update = Builders<HexTile>.Update.Set(t => t.Structure, (TileStructure?)null);
        return _ctx.Tiles.UpdateOneAsync(filter, update);
    }
}
