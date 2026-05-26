using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class StatusDefinition
{
    public CompanionStatus Id              { get; init; }
    public string          DisplayName     { get; init; } = "";
    // Fraction of max HP dealt as damage at the start of the afflicted unit's turn
    public float           DoTMultiplier   { get; init; } = 0f;
    // Multiplier applied to outgoing attack damage while afflicted (Burned = 0.8)
    public float           AttackMultiplier { get; init; } = 1f;
    // Probability of skipping the unit's turn entirely
    public float           SkipChance      { get; init; } = 0f;
}

public static class StatusRegistry
{
    private static readonly Dictionary<CompanionStatus, StatusDefinition> _all = new()
    {
        [CompanionStatus.None]      = new() { Id = CompanionStatus.None },
        [CompanionStatus.Poisoned]  = new() { Id = CompanionStatus.Poisoned,  DisplayName = "Poison",    DoTMultiplier = 0.06f },
        [CompanionStatus.Burned]    = new() { Id = CompanionStatus.Burned,    DisplayName = "Burn",      DoTMultiplier = 0.06f, AttackMultiplier = 0.8f },
        [CompanionStatus.Chilled]   = new() { Id = CompanionStatus.Chilled,   DisplayName = "Chill" },
        [CompanionStatus.Paralyzed] = new() { Id = CompanionStatus.Paralyzed, DisplayName = "Paralysis", SkipChance = 0.25f },
        [CompanionStatus.Cursed]    = new() { Id = CompanionStatus.Cursed,    DisplayName = "Curse" },
    };

    public static StatusDefinition Get(CompanionStatus s) => _all[s];
}
