using MapGenerator.Combat.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Models;

public class CombatStartContext
{
    public Player Player { get; set; } = null!;
    public CombatTrigger Trigger { get; set; }
    public string? DungeonTheme { get; set; }
    public int? DungeonFloor { get; set; }
    public string? BiomeType { get; set; }
    public string? RoomType { get; set; }
}
