using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IDungeonRepository
{
    Task<DungeonInstance?> GetByEntranceAsync(int q, int r);
    Task<DungeonInstance?> GetByIdAsync(string id);
    Task SaveAsync(DungeonInstance dungeon);
    Task UpdateRoomAsync(string dungeonId, int floorNumber, DungeonRoom room);
}
