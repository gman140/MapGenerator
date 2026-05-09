namespace MapGenerator.Domain.Models;

public class MapGenerationOptions
{
    /// <summary>Elevation below which tiles become ocean/lake (0–1). Higher = more water coverage.</summary>
    public float SeaLevel { get; set; } = 0.42f;

    /// <summary>Elevation above which tiles become mountain (0–1).</summary>
    public float MountainLevel { get; set; } = 0.70f;

    /// <summary>Elevation above which tiles become snow (0–1).</summary>
    public float SnowLevel { get; set; } = 0.84f;

    /// <summary>Frequency of the elevation noise. Higher = more zoomed-out features (fewer, larger landmasses).</summary>
    public float ElevationScale { get; set; } = 2.8f;

    /// <summary>Frequency of the moisture noise. Higher = finer moisture variation.</summary>
    public float MoistureScale { get; set; } = 3.2f;

    /// <summary>Strength of domain-warp distortion applied to elevation. Higher = more organic, twisted coastlines.</summary>
    public float WarpStrength { get; set; } = 0.9f;

    /// <summary>Shifts overall moisture up or down (-0.4 to 0.4). Positive = lusher; negative = drier.</summary>
    public float MoistureBias { get; set; } = 0.0f;

    /// <summary>Number of rivers to generate. 0 = auto (roughly 1 per 2000 tiles).</summary>
    public int RiverCount { get; set; } = 0;

    /// <summary>Moisture must be below this for a high-elevation tile to become Volcano instead of Mountain (0–1). Higher = more volcanoes.</summary>
    public float VolcanoMoistureMax { get; set; } = 0.20f;

    /// <summary>How far above SnowLevel elevation must reach to become Glacier instead of Snow (0–1). Lower = more glacier, less snow.</summary>
    public float GlacierElevationOffset { get; set; } = 0.06f;

    /// <summary>Number of settlements to generate. 0 = auto (~1 per 4000 tiles).</summary>
    public int SettlementCount { get; set; } = 0;
}
