using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class CompanionRepository : ICompanionRepository
{
    private readonly MongoDbContext _ctx;

    public CompanionRepository(MongoDbContext ctx) => _ctx = ctx;

    public Task<PlayerCompanion?> GetByIdAsync(string id) =>
        _ctx.Companions.Find(c => c.Id == id).FirstOrDefaultAsync()!;

    public Task<PlayerCompanion?> GetByPlayerIdAsync(string playerId) =>
        _ctx.Companions.Find(c => c.PlayerId == playerId).FirstOrDefaultAsync()!;

    public async Task<List<PlayerCompanion>> GetManyByIdsAsync(IEnumerable<string> ids)
    {
        var idList = ids.ToList();
        if (idList.Count == 0) return [];
        return await _ctx.Companions.Find(c => idList.Contains(c.Id)).ToListAsync();
    }

    public Task SaveAsync(PlayerCompanion companion) =>
        _ctx.Companions.ReplaceOneAsync(
            c => c.Id == companion.Id,
            companion,
            new ReplaceOptions { IsUpsert = true });

    public Task DeleteAsync(string id) =>
        _ctx.Companions.DeleteOneAsync(c => c.Id == id);
}
