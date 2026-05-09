using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface ISettlementRepository
{
    Task<List<Settlement>> GetAllAsync();
    Task SaveManyAsync(IEnumerable<Settlement> settlements);
    Task DeleteAllAsync();
}
