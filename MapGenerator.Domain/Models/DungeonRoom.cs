using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class DungeonRoom
{
    public int Q { get; set; }
    public int R { get; set; }
    public DungeonRoomType Type { get; set; }
    public bool IsCleared { get; set; }
public Dictionary<string, int> Loot { get; set; } = new();
    public string? RequiredKeyId { get; set; }
}
