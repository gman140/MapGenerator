namespace MapGenerator.Domain.Models;

public class DungeonInstance
{
    public string Id { get; set; } = string.Empty;
    public int EntranceQ { get; set; }
    public int EntranceR { get; set; }
    public string DungeonTheme { get; set; } = string.Empty;
    public List<DungeonFloor> Floors { get; set; } = [];
    public DateTime GeneratedAt { get; set; }
}
