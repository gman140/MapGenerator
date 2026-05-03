using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IPlayerTileVisitRepository
{
    Task<PlayerTileVisit> RecordArrivalAsync(string playerId, int q, int r);
    Task RecordDepartureAsync(string playerId, int q, int r);
    Task<List<PlayerTileVisit>> GetVisitsForPlayerOnTileAsync(string playerId, int q, int r);
    Task DeleteAllForPlayerAsync(string playerId);
    Task CloseAllOpenVisitsAsync(string playerId);
}
