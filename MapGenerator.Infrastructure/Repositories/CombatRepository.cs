using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class CombatRepository : ICombatRepository
{
    private readonly MongoDbContext _ctx;

    public CombatRepository(MongoDbContext ctx) => _ctx = ctx;

    public Task<CombatSession?> GetByIdAsync(string id) =>
        _ctx.CombatSessions.Find(s => s.Id == id).FirstOrDefaultAsync()!;

    public Task SaveAsync(CombatSession session) =>
        _ctx.CombatSessions.ReplaceOneAsync(
            s => s.Id == session.Id,
            session,
            new ReplaceOptions { IsUpsert = true });

    public Task DeleteAsync(string id) =>
        _ctx.CombatSessions.DeleteOneAsync(s => s.Id == id);
}
