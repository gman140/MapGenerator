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

    // Personality — assigned randomly on hatch, never changes
    public CompanionTemperament Temperament { get; set; } = CompanionTemperament.Bold;

    // Form — assigned randomly when an Evolution Stone is used
    public CompanionForm Form { get; set; } = CompanionForm.None;

    // Seeded from definition at hatch
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, int> Stats { get; set; } = []; // keys: ATK VIT DEF SPD FOC RES

    // Element types — seeded from definition at hatch, expanded via Elemental Core
    public List<DamageType> ElementTypes { get; set; } = [];

    // Battle record
    public int BattleWins { get; set; } = 0;
    public int BattleLosses { get; set; } = 0;
}
