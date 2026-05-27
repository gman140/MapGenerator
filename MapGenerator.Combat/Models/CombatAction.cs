using MapGenerator.Combat.Enums;

namespace MapGenerator.Combat.Models;

public class CombatAction
{
    public CombatActionType Type { get; set; }
    public string? TargetEnemyId { get; set; }
    public string? ItemId { get; set; }
    public string? SpellId { get; set; }
    public string? WeaponAttackId { get; set; }
}
