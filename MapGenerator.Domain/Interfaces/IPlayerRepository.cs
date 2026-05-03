using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(string id);
    Task<Player?> GetByBrowserIdAsync(string browserId);
    Task<Player?> GetByUsernameAsync(string username);
    Task<List<Player>> GetPlayersOnTileAsync(int q, int r);
    Task<Player> CreateAsync(Player player);
    Task UpdateAsync(Player player);
    Task DeleteAsync(string id);
    Task MoveAllToSpawnAsync(int spawnQ, int spawnR);
}
