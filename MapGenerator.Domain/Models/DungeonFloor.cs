namespace MapGenerator.Domain.Models;

public class DungeonFloor
{
    public int FloorNumber { get; set; }
    public List<DungeonRoom> Rooms { get; set; } = [];
}
