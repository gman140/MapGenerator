using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class ConsumableDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    // Out-of-combat use (food / drink)
    public double SatietyRestore { get; init; }
    public string[] UseMessages { get; init; } = [];
    public BuffDefinition? Buff { get; init; }

    // In-combat use
    public bool UsableInCombat { get; init; }
    public int CombatHpRestore { get; init; }
    public int CombatStaminaRestore { get; init; }
    public ModifierStat? CombatBuffStat { get; init; }
    public float CombatBuffValue { get; init; }
    public int CombatBuffTurns { get; init; }
    public string? CombatBuffLabel { get; init; }
    public ModifierStat[]? ClearsStatuses { get; init; }
}
