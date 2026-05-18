using MapGenerator.Combat.Enums;

namespace MapGenerator.Combat.Models;

public class EnemyActionEntry
{
    public EnemyActionType Action { get; set; }
    public int Weight { get; set; }
}
