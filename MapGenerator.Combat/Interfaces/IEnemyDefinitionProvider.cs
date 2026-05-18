using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Interfaces;

public interface IEnemyDefinitionProvider
{
    IReadOnlyList<EnemyDefinition> All { get; }
    EnemyDefinition? GetById(string id);
}
