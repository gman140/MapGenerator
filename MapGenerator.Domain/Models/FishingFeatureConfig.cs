namespace MapGenerator.Domain.Models;

public class FishingFeatureConfig
{
    public string LocationLabel { get; init; } = "Fishing Spot";
    public string BackgroundColor { get; init; } = "#1a2a3a";
    public string WaterColor { get; init; } = "#1e5080";
    public string WaterSurfaceColor { get; init; } = "#2a70b0";

    /// <summary>Minimum pole tier required to fish at this location.</summary>
    public int RequiredPoleTier { get; init; }

    /// <summary>Fish IDs available at this location (filtered by pole tier when building the pool).</summary>
    public string[] FishIds { get; init; } = [];
}
