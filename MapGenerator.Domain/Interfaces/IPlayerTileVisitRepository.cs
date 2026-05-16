using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IPlayerTileVisitRepository
{
    Task<PlayerTileVisit> RecordArrivalAsync(string playerId, int q, int r);
    Task RecordDepartureAsync(string playerId, int q, int r);
    Task<List<PlayerTileVisit>> GetVisitsForPlayerOnTileAsync(string playerId, int q, int r);
    Task<int> CountVisitsAsync(string playerId, int q, int r);
    Task DeleteAllForPlayerAsync(string playerId);
    Task CloseAllOpenVisitsAsync(string playerId);
    Task<List<(int Q, int R)>> GetVisitedCoordsAsync(string playerId);
    Task RecordRevealedCoordsAsync(string playerId, IEnumerable<(int Q, int R)> coords);
    Task DeleteAllAsync();
}
