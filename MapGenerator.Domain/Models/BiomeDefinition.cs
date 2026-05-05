using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class BiomeDefinition
{
    public BiomeType Type { get; init; }
    public long CooldownMs { get; init; }
    public (byte R, byte G, byte B) MinimapRgb { get; init; }
    public string[] InvestigatePartA { get; init; } = [];
    public string[] InvestigatePartB { get; init; } = [];
    public string[] NeighborText { get; init; } = [];
    public int NeighborPriority { get; init; }
}
