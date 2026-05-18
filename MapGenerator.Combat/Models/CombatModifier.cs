using MapGenerator.Combat.Enums;

namespace MapGenerator.Combat.Models;

public class CombatModifier
{
    public string Id { get; set; } = string.Empty;
    public ModifierTarget Target { get; set; }
    public ModifierStat Stat { get; set; }
    public float Value { get; set; }
    public int? TurnsRemaining { get; set; }   // null = permanent (equipment)
    public string? Source { get; set; }
}
