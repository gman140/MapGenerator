namespace MapGenerator.Domain.Models;

public class TileInventory
{
    public string Id { get; set; } = string.Empty;
    public int Q { get; set; }
    public int R { get; set; }
    public Dictionary<string, int> Items { get; set; } = new();
}
