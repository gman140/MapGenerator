using MapGenerator.Combat.Enums;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class EnemyDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; init; } = "";
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public float FleeChance { get; set; }
    public float FleeHpThreshold { get; set; }
    public List<EnemyActionEntry> ActionTable { get; set; } = [];

    // XP awarded when this enemy is defeated
    public int BaseXp { get; init; }

    // Damage type for this enemy's attacks
    public DamageType AttackDamageType { get; init; } = DamageType.Bludgeoning;

    // Type effectiveness
    public List<DamageType> Weaknesses { get; init; } = [];
    public List<DamageType> Resistances { get; init; } = [];

    // Per-type custom combat log text (falls back to generic if absent)
    public Dictionary<DamageType, string> WeaknessText { get; init; } = new();
    public Dictionary<DamageType, string> ResistanceText { get; init; } = new();

    // What the Buff action does (e.g., Wolf's Howl)
    public ModifierStat? BuffStat { get; set; }
    public float BuffValue { get; set; }
    public int BuffTurns { get; set; } = 2;

    // Bear's Rear Up: when defending, also boost outgoing damage multiplier next hit
    public float DefendDamageBonus { get; set; } = 0f;

    // Cave Troll regeneration amount
    public int RegenerateAmount { get; set; } = 0;

    // Loot dropped on death
    public List<LootEntry> LootTable { get; set; } = [];

    // Flavor text pools — one picked at random per event
    public List<string> AppearTexts { get; set; } = [];
    public List<string> AttackTexts { get; set; } = [];
    public List<string> HeavyAttackTexts { get; set; } = [];
    public List<string> DefendTexts { get; set; } = [];
    public List<string> BuffTexts { get; set; } = [];
    public List<string> RegenerateTexts { get; set; } = [];
    public List<string> FleeTexts { get; set; } = [];
    public List<string> DeathTexts { get; set; } = [];
}
