using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class PlayerTileVisitRepository : IPlayerTileVisitRepository
{
    private readonly MongoDbContext _ctx;

    public PlayerTileVisitRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        _ctx.Visits.Indexes.CreateOne(new CreateIndexModel<PlayerTileVisit>(
            Builders<PlayerTileVisit>.IndexKeys.Combine(
                Builders<PlayerTileVisit>.IndexKeys.Ascending(v => v.PlayerId),
                Builders<PlayerTileVisit>.IndexKeys.Ascending(v => v.Q),
                Builders<PlayerTileVisit>.IndexKeys.Ascending(v => v.R),
                Builders<PlayerTileVisit>.IndexKeys.Ascending(v => v.ArrivedAt))));
    }

    public async Task<PlayerTileVisit> RecordArrivalAsync(string playerId, int q, int r)
    {
        var visit = new PlayerTileVisit
        {
            PlayerId = playerId,
            Q = q,
            R = r,
            ArrivedAt = DateTime.UtcNow
        };
        await _ctx.Visits.InsertOneAsync(visit);
        return visit;
    }

    public Task RecordDepartureAsync(string playerId, int q, int r)
    {
        var filter = Builders<PlayerTileVisit>.Filter.And(
            Builders<PlayerTileVisit>.Filter.Eq(v => v.PlayerId, playerId),
            Builders<PlayerTileVisit>.Filter.Eq(v => v.Q, q),
            Builders<PlayerTileVisit>.Filter.Eq(v => v.R, r),
            Builders<PlayerTileVisit>.Filter.Eq(v => v.LeftAt, null));
        var update = Builders<PlayerTileVisit>.Update.Set(v => v.LeftAt, DateTime.UtcNow);
        return _ctx.Visits.UpdateManyAsync(filter, update);
    }

    public Task<List<PlayerTileVisit>> GetVisitsForPlayerOnTileAsync(string playerId, int q, int r) =>
        _ctx.Visits.Find(v => v.PlayerId == playerId && v.Q == q && v.R == r)
            .SortBy(v => v.ArrivedAt)
            .ToListAsync();

    public async Task<int> CountVisitsAsync(string playerId, int q, int r) =>
        (int)await _ctx.Visits.CountDocumentsAsync(v => v.PlayerId == playerId && v.Q == q && v.R == r);

    public Task DeleteAllForPlayerAsync(string playerId) =>
        _ctx.Visits.DeleteManyAsync(v => v.PlayerId == playerId);

    public Task CloseAllOpenVisitsAsync(string playerId)
    {
        var filter = Builders<PlayerTileVisit>.Filter.And(
            Builders<PlayerTileVisit>.Filter.Eq(v => v.PlayerId, playerId),
            Builders<PlayerTileVisit>.Filter.Eq(v => v.LeftAt, null));
        var update = Builders<PlayerTileVisit>.Update.Set(v => v.LeftAt, DateTime.UtcNow);
        return _ctx.Visits.UpdateManyAsync(filter, update);
    }

    public Task DeleteAllAsync() =>
        _ctx.Visits.DeleteManyAsync(Builders<PlayerTileVisit>.Filter.Empty);

    public async Task<List<(int Q, int R)>> GetVisitedCoordsAsync(string playerId)
    {
        var docs = await _ctx.Visits
            .Find(v => v.PlayerId == playerId)
            .Project(v => new { v.Q, v.R })
            .ToListAsync();
        return docs.Select(d => (d.Q, d.R)).Distinct().ToList();
    }
}
