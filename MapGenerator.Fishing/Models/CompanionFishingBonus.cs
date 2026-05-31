namespace MapGenerator.Fishing.Models;

/// <summary>
/// Fishing bonuses derived from the companion's effective stats and temperament.
/// Computed server-side and serialized into FishingInitData for the WASM session.
/// </summary>
public record CompanionFishingBonus
{
    // ── Per-point scaling constants ───────────────────────────────────────────
    // Referenced by both Game.razor (calculation) and CompanionDialog.razor (display).

    public const double FocPerPoint = 0.03;  // FOC: strike window multiplier per point
    public const double SpdPerPoint = 0.04;  // SPD: wait time reduction per point
    public const double VitPerPoint = 0.03;  // VIT: tension drain reduction per point
    public const double DefPerPoint = 0.05;  // DEF: burst strength reduction per point
    public const double AtkPerPoint = 0.02;  // ATK: flat tap progress bonus per point
    public const double ResPerPoint = 0.01;  // RES: tension break threshold raised per point

    // ── Stat-based multipliers ────────────────────────────────────────────────

    /// <summary>Multiplier on StrikeWindowMs. Above 1.0 = wider window. (FOC)</summary>
    public double StrikeMultiplier { get; init; } = 1.0;

    /// <summary>Multiplier on bite wait time. Below 1.0 = faster bites. (SPD)</summary>
    public double WaitMultiplier { get; init; } = 1.0;

    /// <summary>Multiplier on passive tension drain. Below 1.0 = less drain. (VIT)</summary>
    public double TensionMultiplier { get; init; } = 1.0;

    /// <summary>Multiplier on fish burst strength. Below 1.0 = weaker bursts. (DEF)</summary>
    public double BurstMultiplier { get; init; } = 1.0;

    /// <summary>Flat bonus added to reel progress on each successful tap. (ATK)</summary>
    public double TapBonus { get; init; } = 0.0;

    /// <summary>Tension % at which the line snaps. Default 1.0; raised by RES. (RES)</summary>
    public double TensionBreakThreshold { get; init; } = 1.0;

    // ── Temperament bonuses ───────────────────────────────────────────────────

    /// <summary>Additional rarity multiplier on rare fish pool weight. (Bold)</summary>
    public double RarityBoost { get; init; } = 1.0;

    /// <summary>Additional pool weight multiplier on chain materials. (Cunning)</summary>
    public double ChainBoost { get; init; } = 1.0;

    /// <summary>Chance each burst is fully suppressed. (Careful)</summary>
    public double BurstSuppressChance { get; init; } = 0.0;

    /// <summary>Chance each tap is treated as in-zone regardless of cursor position. (Playful)</summary>
    public double FreeTapChance { get; init; } = 0.0;

    /// <summary>When true, the hook phase starts at 0% tension instead of 15%. (Stoic)</summary>
    public bool StoicHook { get; init; }

    /// <summary>When true, the first missed strike per fish is forgiven. (Timid)</summary>
    public bool TimidFirstStrike { get; init; }

    /// <summary>Multiplier on reel cursor speed. Above 1.0 = faster cursor. (Reckless)</summary>
    public double CursorSpeedMultiplier { get; init; } = 1.0;

    /// <summary>Extra tension added on a missed tap, on top of the base 0.14. (Fierce)</summary>
    public double MissTensionBonus { get; init; } = 0.0;
}
