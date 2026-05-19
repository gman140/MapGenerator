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

    // Affix applied at spawn (null = no affix)
    public string? AffixId { get; set; }
    public string? AffixLabel { get; set; }

    // On-hit status effect from the affix (null = none)
    public OnHitEffect? OnHitEffect { get; set; }

    // If true, player dodge chance is bypassed for all this enemy's attacks
    public bool ActionsCantBeDodged { get; set; }
}
