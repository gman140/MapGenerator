using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Interfaces;

public interface IEnemyAffixProvider
{
    EnemyAffix? GetById(string id);
    IReadOnlyList<EnemyAffix> GetAll();
}
