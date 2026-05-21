using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface ICompanionRepository
{
    Task<PlayerCompanion?> GetByIdAsync(string id);
    Task<PlayerCompanion?> GetByPlayerIdAsync(string playerId);
    Task<List<PlayerCompanion>> GetManyByIdsAsync(IEnumerable<string> ids);
    Task SaveAsync(PlayerCompanion companion);
    Task DeleteAsync(string id);
}
