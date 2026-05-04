using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class TileNoteRepository : ITileNoteRepository
{
    private readonly MongoDbContext _ctx;

    public TileNoteRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        var idx = Builders<TileNote>.IndexKeys.Combine(
            Builders<TileNote>.IndexKeys.Ascending(n => n.Q),
            Builders<TileNote>.IndexKeys.Ascending(n => n.R));
        _ctx.TileNotes.Indexes.CreateOne(new CreateIndexModel<TileNote>(idx));
    }

    public Task AddNoteAsync(TileNote note) => _ctx.TileNotes.InsertOneAsync(note);

    public Task<List<TileNote>> GetNotesForTileAsync(int q, int r) =>
        _ctx.TileNotes.Find(n => n.Q == q && n.R == r)
            .SortBy(n => n.CreatedAt)
            .ToListAsync();
}
