using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Interfaces;

public interface ICombatRepository
{
    Task<CombatSession?> GetByIdAsync(string id);
    Task SaveAsync(CombatSession session);
    Task DeleteAsync(string id);
}
