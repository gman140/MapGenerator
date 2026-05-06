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
    Task<int> IncrementEggCountAsync(int q, int r);
    Task PlaceSignAsync(int q, int r, string text, string authorName);
    Task SetStructureAsync(int q, int r, TileStructure structure);
    Task RemoveStructureAsync(int q, int r);
}
