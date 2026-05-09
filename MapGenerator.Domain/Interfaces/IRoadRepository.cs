using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IRoadRepository
{
    Task<List<Road>> GetAllAsync();
    Task SaveManyAsync(IEnumerable<Road> roads);
    Task DeleteAllAsync();
}
