namespace MapGenerator.Fishing.Models;

/// <summary>
/// Defines a fish species or catchable treasure.
/// All fight behavior is encoded as plain properties — no type dispatch needed.
/// </summary>
public class FishDefinition
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";

    /// <summary>Minimum pole tier required to catch this fish.</summary>
    public int PoleTier { get; init; }

    /// <summary>Relative probability of appearing in a pool. Higher = more common.</summary>
    public float RarityWeight { get; init; } = 1f;

    /// <summary>True for pole-upgrade chain materials (e.g. River Scale). Not a fish to eat.</summary>
    public bool IsChainMaterial { get; init; }

    /// <summary>Habitat category used by lures: "Freshwater", "Saltwater", "Deep", or "Cold". Null for chain materials.</summary>
    public string? FishCategory { get; init; }

    // ── Fight parameters ────────────────────────────────────────────────────

    /// <summary>Seconds between bobber appearing and fish striking.</summary>
    public double WaitTimeMinSec { get; init; } = 2.0;
    public double WaitTimeMaxSec { get; init; } = 6.0;

    /// <summary>How long the hook-set window is open (ms).</summary>
    public double StrikeWindowMs { get; init; } = 700.0;

    /// <summary>How many missed strikes before the fish leaves.</summary>
    public int MaxMissedStrikes { get; init; } = 2;

    /// <summary>Tension % per second the fish passively drains while being reeled.</summary>
    public double TensionDrainRate { get; init; } = 0.08;

    /// <summary>Additional tension % per second added while player holds the reel button.</summary>
    public double ReelResistance { get; init; } = 0.12;

    /// <summary>Chance per second the fish makes a sudden tension burst.</summary>
    public double BurstChancePerSec { get; init; } = 0.15;

    /// <summary>Tension % added instantly when a burst fires.</summary>
    public double BurstStrength { get; init; } = 0.18;

    /// <summary>Reel progress % per second when player holds and tension is manageable.</summary>
    public double ReelRate { get; init; } = 0.14;
}
