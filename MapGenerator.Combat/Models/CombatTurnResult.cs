namespace MapGenerator.Combat.Models;

public class CombatTurnResult
{
    public CombatSession Session { get; set; } = null!;
    public List<CombatEvent> Events { get; set; } = [];
}
