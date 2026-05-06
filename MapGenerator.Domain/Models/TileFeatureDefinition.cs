using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class TileFeatureDefinition
{
    public string Id { get; init; } = string.Empty;
    public string[] DisplayNames { get; init; } = [];
    public FeatureCategory Category { get; init; }
    public BiomeType[] AllowedBiomes { get; init; } = [];
    public float Probability { get; init; }
    public string[] PartA { get; init; } = [];
    public string[] PartB { get; init; } = [];
    public ResourceYield[] ResourceYields { get; init; } = [];

    public string GetDisplayName(int q, int r) =>
        DisplayNames.Length == 0 ? Id : DisplayNames[Math.Abs(q * 7 + r * 13) % DisplayNames.Length];
}
