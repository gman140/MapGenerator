using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IRoadRepository
{
    Task<List<Road>> GetAllAsync();
    Task AddAsync(Road road);
    Task AppendPointAsync(string id, RoadPoint point);
    Task SaveManyAsync(IEnumerable<Road> roads);
    Task DeleteAllAsync();
}
