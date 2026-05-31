namespace MapGenerator.Fishing.Models;

/// <summary>
/// Everything the WASM FishingGame component needs to start a session.
/// Serialized by Blazor when crossing the Server→WASM boundary.
/// </summary>
public class FishingInitData
{
    public string LocationLabel { get; init; } = "Fishing Spot";
    public string BackgroundColor { get; init; } = "#1a2a3a";
    public string WaterColor { get; init; } = "#1e5080";
    public string WaterSurfaceColor { get; init; } = "#2a70b0";

    /// <summary>Pole tier the player is fishing with. Affects fish pool and boons.</summary>
    public int PoleTier { get; init; }

    /// <summary>Multiplier on StrikeWindowMs (pole boon). 1.0 = no bonus.</summary>
    public double StrikeWindowMultiplier { get; init; } = 1.0;

    /// <summary>Multiplier applied to RarityWeight of rare fish. Pole Tier 3+ bonus.</summary>
    public double RarityMultiplier { get; init; } = 1.0;

    /// <summary>Multiplier on bite wait time. Values below 1.0 mean faster bites (Light Lure boon).</summary>
    public double WaitTimeMultiplier { get; init; } = 1.0;

    /// <summary>Current consecutive catch count. Bonus applies at 3+.</summary>
    public int FishingStreak { get; init; }

    public bool StreakBonusActive => FishingStreak >= 3;

    /// <summary>Fish pool for this location, filtered to the player's pole tier.</summary>
    public FishDefinition[] FishPool { get; init; } = [];

    public string[] PlayerSprite { get; init; } = [];
    public string[] CompanionSprite { get; init; } = [];
    public string CompanionName { get; init; } = "";
}
