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

    public Task SaveManyAsync(IEnumerable<Road> roads) =>
        _ctx.Roads.InsertManyAsync(roads);

    public Task DeleteAllAsync() =>
        _ctx.Roads.DeleteManyAsync(Builders<Road>.Filter.Empty);
}
