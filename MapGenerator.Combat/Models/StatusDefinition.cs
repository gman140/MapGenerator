using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class StatusDefinition
{
    public CompanionStatus Id          { get; init; }
    public string          DisplayName { get; init; } = "";

    // ── Damage-over-time ─────────────────────────────────────────────────────
    // Fraction of max HP dealt at the start of the afflicted unit's turn
    public float DoTMultiplier { get; init; }

    // ── Turn impairment ──────────────────────────────────────────────────────
    public float SkipChance { get; init; }

    // ── Stat modifiers (applied as % of BASE stat, ignoring other active mods)
    // Positive = buff; negative = debuff. Min = minimum absolute-value magnitude.
    public float AtkPct { get; init; }
    public int   AtkMin { get; init; }
    public float DefPct { get; init; }
    public int   DefMin { get; init; }
    public float SpdPct { get; init; }
    public int   SpdMin { get; init; }

    // ── Accuracy ─────────────────────────────────────────────────────────────
    // Added to the afflicted unit's extraMissChance on all outgoing attacks
    public float MissChanceAdd { get; init; }
}

public static class StatusRegistry
{
    private static readonly Dictionary<CompanionStatus, StatusDefinition> _all = new()
    {
        [CompanionStatus.None]      = new() { Id = CompanionStatus.None },

        // DoT statuses
        [CompanionStatus.Poisoned]  = new() { Id = CompanionStatus.Poisoned,  DisplayName = "Poison",
            DoTMultiplier = 0.06f },
        [CompanionStatus.Burned]    = new() { Id = CompanionStatus.Burned,    DisplayName = "Burn",
            DoTMultiplier = 0.06f, AtkPct = -0.20f, AtkMin = 2 },

        // Turn impairment
        [CompanionStatus.Paralyzed] = new() { Id = CompanionStatus.Paralyzed, DisplayName = "Paralysis",
            SkipChance = 0.25f },
        [CompanionStatus.Chilled]   = new() { Id = CompanionStatus.Chilled,   DisplayName = "Chill",
            SpdPct = -0.35f, SpdMin = 2 },

        // Accuracy impairment
        [CompanionStatus.Cursed]    = new() { Id = CompanionStatus.Cursed,    DisplayName = "Curse",
            MissChanceAdd = 0.20f },

        // Offensive buffs
        [CompanionStatus.Rallied]   = new() { Id = CompanionStatus.Rallied,   DisplayName = "Rallied",
            AtkPct = 0.25f, AtkMin = 2 },
        [CompanionStatus.Enraged]   = new() { Id = CompanionStatus.Enraged,   DisplayName = "Enraged",
            AtkPct = 0.35f, AtkMin = 3 },

        // Defensive buffs
        [CompanionStatus.Hardened]  = new() { Id = CompanionStatus.Hardened,  DisplayName = "Hardened",
            DefPct = 0.25f, DefMin = 2 },
        [CompanionStatus.Fortified] = new() { Id = CompanionStatus.Fortified, DisplayName = "Fortified",
            DefPct = 0.35f, DefMin = 3 },

        // Speed buff
        [CompanionStatus.Hastened]  = new() { Id = CompanionStatus.Hastened,  DisplayName = "Hastened",
            SpdPct = 0.30f, SpdMin = 2 },

        // Debuffs
        [CompanionStatus.Weakened]  = new() { Id = CompanionStatus.Weakened,  DisplayName = "Weakened",
            AtkPct = -0.20f, AtkMin = 2 },
        [CompanionStatus.Exposed]   = new() { Id = CompanionStatus.Exposed,   DisplayName = "Exposed",
            DefPct = -0.25f, DefMin = 3 },
        [CompanionStatus.Shattered] = new() { Id = CompanionStatus.Shattered, DisplayName = "Shattered",
            DefPct = -0.35f, DefMin = 4 },
    };

    public static StatusDefinition Get(CompanionStatus s) => _all[s];
    public static IReadOnlyCollection<StatusDefinition> All => _all.Values;
}
