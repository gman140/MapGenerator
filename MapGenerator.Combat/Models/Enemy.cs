namespace MapGenerator.Combat.Models;

public class Enemy
{
    public string InstanceId { get; set; } = string.Empty;
    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int Attack { get; set; }
    public int  Defense     { get; set; }
    public int  Speed       { get; set; } = 5;
    public float CritChance { get; set; }
    public bool IsDefending { get; set; }
    public bool HasFled { get; set; }
    public List<CombatModifier> ActiveModifiers { get; set; } = [];
    public List<EnemyActionEntry> ActionTable { get; set; } = [];

    // Affix applied at spawn (null = no affix)
    public string? AffixId { get; set; }
    public string? AffixLabel { get; set; }

    // On-hit status effect from the affix (null = none)
    public OnHitEffect? OnHitEffect { get; set; }

    // If true, this enemy ignores the player's Steady accuracy bonus
    public bool IgnoresSteady { get; set; }
}
