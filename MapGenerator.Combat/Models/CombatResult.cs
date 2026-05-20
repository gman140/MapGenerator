namespace MapGenerator.Combat.Models;

public class CombatResult
{
    public bool Victory { get; set; }
    public bool Fled { get; set; }
    public bool PlayerDied { get; set; }
    public int HpRemaining { get; set; }
    public int StaminaRemaining { get; set; }
    public int ManaRemaining { get; set; }
    public int XpGained { get; set; }
    public bool LeveledUp { get; set; }
    public int NewLevel { get; set; }
    public int StatPointsGained { get; set; }
    public Dictionary<string, int> LootGained { get; set; } = new();
    public Dictionary<string, int> EnemiesDefeated { get; set; } = new();
    public string? SummaryMessage { get; set; }
}
