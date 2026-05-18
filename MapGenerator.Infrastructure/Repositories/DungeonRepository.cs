using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class DungeonRepository : IDungeonRepository
{
    private readonly MongoDbContext _ctx;

    public DungeonRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        _ctx.Dungeons.Indexes.CreateOne(new CreateIndexModel<DungeonInstance>(
            Builders<DungeonInstance>.IndexKeys
                .Ascending(d => d.EntranceQ)
                .Ascending(d => d.EntranceR),
            new CreateIndexOptions { Unique = true }));
    }

    public Task<DungeonInstance?> GetByEntranceAsync(int q, int r) =>
        _ctx.Dungeons.Find(d => d.EntranceQ == q && d.EntranceR == r).FirstOrDefaultAsync()!;

    public Task<DungeonInstance?> GetByIdAsync(string id) =>
        _ctx.Dungeons.Find(d => d.Id == id).FirstOrDefaultAsync()!;

    public Task SaveAsync(DungeonInstance dungeon) =>
        _ctx.Dungeons.ReplaceOneAsync(
            d => d.Id == dungeon.Id,
            dungeon,
            new ReplaceOptions { IsUpsert = true });

    public async Task UpdateRoomAsync(string dungeonId, int floorNumber, DungeonRoom room)
    {
        var dungeon = await _ctx.Dungeons.Find(d => d.Id == dungeonId).FirstOrDefaultAsync();
        if (dungeon == null) return;

        var floor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
        if (floor == null) return;

        var idx = floor.Rooms.FindIndex(r => r.Q == room.Q && r.R == room.R);
        if (idx >= 0)
            floor.Rooms[idx] = room;

        await _ctx.Dungeons.ReplaceOneAsync(d => d.Id == dungeonId, dungeon);
    }
}
