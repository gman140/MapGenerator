using MapGenerator.Combat.Enums;

namespace MapGenerator.Combat.Models;

public class EnemyAffix
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    // Stat multipliers / bonuses applied at spawn
    public float HpMultiplier { get; set; } = 1f;
    public float AttackMultiplier { get; set; } = 1f;
    public int AttackBonus { get; set; }
    public int DefenseBonus { get; set; }

    // On-hit status effect (null = none)
    public OnHitEffect? OnHit { get; set; }

    // Extra actions appended to the enemy's action table at spawn
    public List<EnemyActionEntry> BonusActions { get; set; } = [];

    // If true, the player's dodge chance is ignored for all this enemy's attacks
    public bool ActionsCantBeDodged { get; set; }
}
