using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class PlayerCompanion
{
    public string Id { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public string DefinitionId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public List<string> MoveIds { get; set; } = [];
    public string[] SpritePixels { get; set; } = [];

    // Tier
    public CompanionTier Tier { get; set; } = CompanionTier.T1;

    // Personality — assigned randomly on hatch, never changes
    public CompanionTemperament Temperament { get; set; } = CompanionTemperament.Bold;

    // Seeded from definition at hatch
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, int> Stats { get; set; } = []; // keys: ATK VIT DEF SPD FOC RES

    // Element types — seeded from definition at hatch, expanded via Elemental Core
    public List<DamageType> ElementTypes { get; set; } = [];

    // Battle record
    public int BattleWins { get; set; } = 0;
    public int BattleLosses { get; set; } = 0;

    // Stat points — earned by using items (ElementalCore +3, EvolutionStone +5)
    public int BonusStatPoints { get; set; } = 0;

    // Manually allocated stat points
    // Keys: "ATK", "VIT", "DEF", "SPD", "FOC", "RES"
    public Dictionary<string, int> AllocatedStats { get; set; } = [];

    public int GetAllocated(string stat) =>
        AllocatedStats.TryGetValue(stat, out var v) ? v : 0;

    public int SpentStatPoints => AllocatedStats.Values.Sum();
    public int AvailableStatPoints => BonusStatPoints - SpentStatPoints;
}
