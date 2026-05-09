using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class SettlementRepository : ISettlementRepository
{
    private readonly MongoDbContext _ctx;

    public SettlementRepository(MongoDbContext ctx) => _ctx = ctx;

    public Task<List<Settlement>> GetAllAsync() =>
        _ctx.Settlements.Find(Builders<Settlement>.Filter.Empty).ToListAsync();

    public Task SaveManyAsync(IEnumerable<Settlement> settlements) =>
        _ctx.Settlements.InsertManyAsync(settlements);

    public Task DeleteAllAsync() =>
        _ctx.Settlements.DeleteManyAsync(Builders<Settlement>.Filter.Empty);
}
