using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class RoadRepository : IRoadRepository
{
    private readonly MongoDbContext _ctx;

    public RoadRepository(MongoDbContext ctx) => _ctx = ctx;

    public Task<List<Road>> GetAllAsync() =>
        _ctx.Roads.Find(Builders<Road>.Filter.Empty).ToListAsync();

    public Task AddAsync(Road road) =>
        _ctx.Roads.InsertOneAsync(road);

    public Task AppendPointAsync(string id, RoadPoint point)
    {
        var filter = Builders<Road>.Filter.Eq(r => r.Id, id);
        var update = Builders<Road>.Update.Push(r => r.Path, point);
        return _ctx.Roads.UpdateOneAsync(filter, update);
    }

    public Task SaveManyAsync(IEnumerable<Road> roads) =>
        _ctx.Roads.InsertManyAsync(roads);

    public Task DeleteAllAsync() =>
        _ctx.Roads.DeleteManyAsync(Builders<Road>.Filter.Empty);
}
