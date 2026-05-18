using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Models;

public class Enemy
{
    public string InstanceId { get; set; } = string.Empty;
    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public bool IsDefending { get; set; }
    public bool HasFled { get; set; }
    public List<CombatModifier> ActiveModifiers { get; set; } = [];
    public List<EnemyActionEntry> ActionTable { get; set; } = [];
}
