using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IMapRepository
{
    Task<HexTile?> GetTileAsync(int q, int r);
    Task<List<HexTile>> GetTilesInRangeAsync(int minQ, int maxQ, int minR, int maxR);
    Task<MapConfig?> GetCurrentConfigAsync();
    Task SaveTilesAsync(IEnumerable<HexTile> tiles);
    Task SaveConfigAsync(MapConfig config);
    Task DeleteAllTilesAsync();
}
