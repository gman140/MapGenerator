using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly MongoDbContext _ctx;

    public PlayerRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        _ctx.Players.Indexes.CreateOne(new CreateIndexModel<Player>(
            Builders<Player>.IndexKeys.Ascending(p => p.Username),
            new CreateIndexOptions { Unique = true }));
        _ctx.Players.Indexes.CreateOne(new CreateIndexModel<Player>(
            Builders<Player>.IndexKeys.Ascending(p => p.BrowserId)));
    }

    public Task<Player?> GetByIdAsync(string id) =>
        _ctx.Players.Find(p => p.Id == id).FirstOrDefaultAsync()!;

    public Task<Player?> GetByBrowserIdAsync(string browserId) =>
        _ctx.Players.Find(p => p.BrowserId == browserId).FirstOrDefaultAsync()!;

    public Task<Player?> GetByUsernameAsync(string username) =>
        _ctx.Players.Find(p => p.Username == username).FirstOrDefaultAsync()!;

    public async Task<List<Player>> GetPlayersOnTileAsync(int q, int r) =>
        await _ctx.Players.Find(p => p.Q == q && p.R == r).ToListAsync();

    public async Task<Player> CreateAsync(Player player)
    {
        await _ctx.Players.InsertOneAsync(player);
        return player;
    }

    public Task UpdateAsync(Player player) =>
        _ctx.Players.ReplaceOneAsync(p => p.Id == player.Id, player);

    public Task DeleteAsync(string id) =>
        _ctx.Players.DeleteOneAsync(p => p.Id == id);

    public Task MoveAllToSpawnAsync(int spawnQ, int spawnR)
    {
        var update = Builders<Player>.Update
            .Set(p => p.Q, spawnQ)
            .Set(p => p.R, spawnR)
            .Set(p => p.MovementCooldownUntil, 0L);
        return _ctx.Players.UpdateManyAsync(_ => true, update);
    }
}
