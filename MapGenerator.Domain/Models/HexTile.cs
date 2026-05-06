using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class HexTile
{
    public string Id { get; set; } = string.Empty;
    public int Q { get; set; }
    public int R { get; set; }
    public BiomeType Biome { get; set; }
    public string? FeatureId { get; set; }
    public float Elevation { get; set; }
    public float Moisture { get; set; }
    public int EggCount { get; set; }
    public string? SignText { get; set; }
    public string? SignAuthor { get; set; }
    public TileStructure? Structure { get; set; }
}
