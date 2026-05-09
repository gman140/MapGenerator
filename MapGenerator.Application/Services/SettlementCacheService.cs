using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class SettlementCacheService
{
    private readonly Lock _lock = new();

    private List<Settlement> _settlements = [];
    private List<Road> _roads = [];
    private Dictionary<(int Q, int R), (Settlement Settlement, SettlementTileRole Role)> _tileMap = new();
    private HashSet<(int Q, int R)> _roadTileSet = new();
    private List<List<(int Q, int R)>> _roadPaths = [];

    public void UpdateCache(List<Settlement> settlements, List<Road> roads)
    {
        var tileMap = new Dictionary<(int, int), (Settlement, SettlementTileRole)>();
        foreach (var s in settlements)
            foreach (var t in s.Tiles)
                tileMap[(t.Q, t.R)] = (s, t.Role);

        var roadTileSet = new HashSet<(int, int)>();
        var roadPaths = new List<List<(int Q, int R)>>();
        foreach (var road in roads)
        {
            var path = road.Path.Select(p => (p.Q, p.R)).ToList();
            roadPaths.Add(path);
            foreach (var p in path) roadTileSet.Add(p);
        }

        lock (_lock)
        {
            _settlements  = settlements;
            _roads        = roads;
            _tileMap      = tileMap;
            _roadTileSet  = roadTileSet;
            _roadPaths    = roadPaths;
        }
    }

    public bool IsRoadTile(int q, int r)
    {
        lock (_lock) return _roadTileSet.Contains((q, r));
    }

    public (string? Name, SettlementTileRole? Role) GetSettlementTile(int q, int r)
    {
        lock (_lock)
        {
            if (_tileMap.TryGetValue((q, r), out var entry))
                return (entry.Settlement.Name, entry.Role);
            return (null, null);
        }
    }

    public (string? Name, SettlementTileRole? Role, SettlementType? Type) GetSettlementInfo(int q, int r)
    {
        lock (_lock)
        {
            if (_tileMap.TryGetValue((q, r), out var entry))
                return (entry.Settlement.Name, entry.Role, entry.Settlement.Type);
            return (null, null, null);
        }
    }

    public IReadOnlyList<List<(int Q, int R)>> RoadPaths
    {
        get { lock (_lock) return _roadPaths; }
    }
}
