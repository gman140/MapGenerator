using MapGenerator.Combat.Models;
using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Interfaces;

public interface ICombatEngine
{
    CombatSession StartCombat(CombatStartContext context);
    CombatSession ProcessTurn(CombatSession session, Player player, CombatAction action);
    bool IsFinished(CombatSession session);
    CombatResult Resolve(CombatSession session);

    // Returns a new session if an encounter triggers, null otherwise
    CombatSession? TryGenerateEncounter(Player player, string biomeType, bool inDungeon,
                                        string? dungeonTheme, int? dungeonFloor);
}
