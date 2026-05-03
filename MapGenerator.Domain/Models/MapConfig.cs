namespace MapGenerator.Domain.Models;

public class MapConfig
{
    public string Id { get; set; } = string.Empty;
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 300;
    public int Seed { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int SpawnQ { get; set; }
    public int SpawnR { get; set; }
    public MapGenerationOptions Options { get; set; } = new();
}
