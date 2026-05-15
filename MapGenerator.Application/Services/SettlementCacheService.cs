using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class SettlementCacheService
{
    private static readonly (int dq, int dr)[] HexOffsets =
        [(1, 0), (0, 1), (-1, 1), (-1, 0), (0, -1), (1, -1)];

    private readonly Lock _lock = new();

    private List<Settlement> _settlements = [];
    private List<Road> _roads = [];
    private Dictionary<(int Q, int R), (Settlement Settlement, SettlementTileRole Role)> _tileMap = new();
    private HashSet<(int Q, int R)> _roadTileSet = new();
    private List<List<(int Q, int R)>> _roadPaths = [];
    private Dictionary<(int Q, int R), string> _roadTileToRoadId = new();
    private Dictionary<string, int> _roadIndexById = new();

    public void UpdateCache(List<Settlement> settlements, List<Road> roads)
    {
        var tileMap = new Dictionary<(int, int), (Settlement, SettlementTileRole)>();
        foreach (var s in settlements)
            foreach (var t in s.Tiles)
                tileMap[(t.Q, t.R)] = (s, t.Role);

        var roadTileSet = new HashSet<(int, int)>();
        var roadPaths = new List<List<(int Q, int R)>>();
        var roadTileToRoadId = new Dictionary<(int, int), string>();
        var roadIndexById = new Dictionary<string, int>();

        for (int i = 0; i < roads.Count; i++)
        {
            var road = roads[i];
            var path = road.Path.Select(p => (p.Q, p.R)).ToList();
            roadPaths.Add(path);
            roadIndexById[road.Id] = i;
            foreach (var p in path)
            {
                roadTileSet.Add(p);
                roadTileToRoadId[p] = road.Id;
            }
        }

        lock (_lock)
        {
            _settlements       = settlements;
            _roads             = roads;
            _tileMap           = tileMap;
            _roadTileSet       = roadTileSet;
            _roadPaths         = roadPaths;
            _roadTileToRoadId  = roadTileToRoadId;
            _roadIndexById     = roadIndexById;
        }
    }

    public bool IsRoadTile(int q, int r)
    {
        lock (_lock) return _roadTileSet.Contains((q, r));
    }

    public string? FindAdjacentRoadId(int q, int r)
    {
        lock (_lock)
        {
            foreach (var (dq, dr) in HexOffsets)
            {
                if (_roadTileToRoadId.TryGetValue((q + dq, r + dr), out var id))
                    return id;
            }
            return null;
        }
    }

    public void AddRoad(Road road)
    {
        var path = road.Path.Select(p => (p.Q, p.R)).ToList();
        lock (_lock)
        {
            int idx = _roadPaths.Count;
            _roadPaths.Add(path);
            _roads.Add(road);
            _roadIndexById[road.Id] = idx;
            foreach (var p in path)
            {
                _roadTileSet.Add(p);
                _roadTileToRoadId[p] = road.Id;
            }
        }
    }

    public void ExtendRoad(string roadId, int q, int r)
    {
        lock (_lock)
        {
            _roadTileSet.Add((q, r));
            _roadTileToRoadId[(q, r)] = roadId;
            if (_roadIndexById.TryGetValue(roadId, out var idx) && idx < _roadPaths.Count)
                _roadPaths[idx].Add((q, r));
        }
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
