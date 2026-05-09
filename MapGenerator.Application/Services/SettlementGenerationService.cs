using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class SettlementGenerationService
{
    private readonly MapGeneratorService _mapCache;
    private readonly ISettlementRepository _settlementRepo;
    private readonly IRoadRepository _roadRepo;
    private readonly SettlementCacheService _settlementCache;

    private static readonly (int dq, int dr)[] Neighbors =
        [(-1, 0), (1, 0), (0, -1), (0, 1), (-1, 1), (1, -1)];

    private static readonly string[] Prefixes =
    [
        "Stone", "Green", "Black", "Iron", "River", "Silver", "White", "Oak",
        "Elder", "Moor", "Ash", "Briar", "Crow", "Dawn", "Frost", "Amber",
        "Bitter", "Golden", "Hollow", "Marsh", "Raven", "Shadow", "Thistle", "Wind"
    ];

    private static readonly string[] Suffixes =
    [
        "haven", "bridge", "ford", "wick", "bury", "ton", "ham", "field",
        "wood", "mere", "holm", "gate", "cross", "vale", "cliff", "keep",
        "moor", "fall", "mill", "crest", "hollow", "rest", "shore", "water"
    ];

    public SettlementGenerationService(
        MapGeneratorService mapCache,
        ISettlementRepository settlementRepo,
        IRoadRepository roadRepo,
        SettlementCacheService settlementCache)
    {
        _mapCache        = mapCache;
        _settlementRepo  = settlementRepo;
        _roadRepo        = roadRepo;
        _settlementCache = settlementCache;
    }

    public async Task GenerateAsync(int width, int height, int seed, MapGenerationOptions? options)
    {
        options ??= new();
        var rng = new Random(seed + 0x5E771E);

        int count = options.SettlementCount > 0
            ? options.SettlementCount
            : Math.Max(3, width * height / 4000);

        // Collect habitable candidates
        var candidates = new List<HexTile>();
        for (int q = 0; q < width; q++)
            for (int r = 0; r < height; r++)
            {
                var t = _mapCache.GetCachedTile(q, r);
                if (t != null && IsHabitable(t.Biome)) candidates.Add(t);
            }

        Shuffle(candidates, rng);

        // Pick well-spaced centers
        var centers = PickCenters(candidates, count);
        if (centers.Count == 0) return;

        // Assign types (10% City, 20% Town, 30% Village, 40% Hamlet)
        var typePool = new List<SettlementType>();
        int cities   = Math.Max(1, (int)(centers.Count * 0.10));
        int towns    = Math.Max(1, (int)(centers.Count * 0.20));
        int villages = Math.Max(1, (int)(centers.Count * 0.30));
        for (int i = 0; i < cities;               i++) typePool.Add(SettlementType.City);
        for (int i = 0; i < towns;                i++) typePool.Add(SettlementType.Town);
        for (int i = 0; i < villages;             i++) typePool.Add(SettlementType.Village);
        while (typePool.Count < centers.Count)         typePool.Add(SettlementType.Hamlet);
        Shuffle(typePool, rng);

        var usedNames = new HashSet<string>();
        var settlements = centers.Select((c, i) => new Settlement
        {
            Name    = GenerateName(rng, usedNames),
            Type    = typePool[i],
            CenterQ = c.Q,
            CenterR = c.R,
        }).ToList();

        // Expand each settlement's tile footprint
        var claimed = new HashSet<(int, int)>();
        foreach (var s in settlements)
        {
            s.Tiles = ExpandSettlement(s, claimed, rng, width, height);
            foreach (var t in s.Tiles) claimed.Add((t.Q, t.R));
        }

        // MST to decide which pairs get roads
        var edges = ComputeMSTEdges(settlements);

        // A* path for each MST edge
        var roadTileSet = new HashSet<(int, int)>();
        var roads = new List<Road>();
        foreach (var (from, to) in edges)
        {
            var path = FindPath(from.CenterQ, from.CenterR, to.CenterQ, to.CenterR,
                                roadTileSet, width, height);
            if (path == null) continue;
            var road = new Road
            {
                FromSettlementId = from.Id,
                ToSettlementId   = to.Id,
                Path             = path.Select(p => new RoadPoint { Q = p.Q, R = p.R }).ToList()
            };
            roads.Add(road);
            foreach (var p in path) roadTileSet.Add((p.Q, p.R));
        }

        await _settlementRepo.SaveManyAsync(settlements);
        await _roadRepo.SaveManyAsync(roads);
        _settlementCache.UpdateCache(settlements, roads);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static bool IsHabitable(BiomeType b) => b is
        BiomeType.Grassland or BiomeType.Plains or BiomeType.Forest or
        BiomeType.Savanna   or BiomeType.Jungle  or BiomeType.Beach  or
        BiomeType.Tundra;

    private static List<HexTile> PickCenters(List<HexTile> candidates, int count)
    {
        const int MinDist = 18;
        var result = new List<HexTile>();
        foreach (var c in candidates)
        {
            if (result.Count >= count) break;
            if (result.All(r => HexDist(r.Q, r.R, c.Q, c.R) >= MinDist))
                result.Add(c);
        }
        return result;
    }

    private List<SettlementTile> ExpandSettlement(
        Settlement s, HashSet<(int, int)> claimed, Random rng, int width, int height)
    {
        int max = s.Type switch
        {
            SettlementType.City    => rng.Next(17, 23),
            SettlementType.Town    => rng.Next(10, 15),
            SettlementType.Village => rng.Next(5,  9),
            _                      => rng.Next(2,  4),
        };

        var tiles = new List<SettlementTile>
            { new() { Q = s.CenterQ, R = s.CenterR, Role = SettlementTileRole.Center } };
        var visited = new HashSet<(int, int)> { (s.CenterQ, s.CenterR) };
        var queue = new Queue<(int Q, int R)>();
        queue.Enqueue((s.CenterQ, s.CenterR));

        while (queue.Count > 0 && tiles.Count < max)
        {
            var (q, r) = queue.Dequeue();
            var shuffled = Neighbors.OrderBy(_ => rng.Next()).ToArray();
            foreach (var (dq, dr) in shuffled)
            {
                if (tiles.Count >= max) break;
                int nq = q + dq, nr = r + dr;
                if (nq < 0 || nr < 0 || nq >= width || nr >= height) continue;
                if (visited.Contains((nq, nr)) || claimed.Contains((nq, nr))) continue;
                var tile = _mapCache.GetCachedTile(nq, nr);
                if (tile == null || tile.Biome is BiomeType.Ocean or BiomeType.Lake or BiomeType.Glacier) continue;
                visited.Add((nq, nr));
                tiles.Add(new SettlementTile { Q = nq, R = nr, Role = AssignRole(s.Type, tiles.Count) });
                queue.Enqueue((nq, nr));
            }
        }

        return tiles;
    }

    private static SettlementTileRole AssignRole(SettlementType type, int index) =>
        (type, index) switch
        {
            (SettlementType.Hamlet, _)  => SettlementTileRole.Farm,
            (SettlementType.Village, 1) => SettlementTileRole.Market,
            (SettlementType.Village, 2) => SettlementTileRole.Residential,
            (SettlementType.Village, _) => index % 2 == 0 ? SettlementTileRole.Residential : SettlementTileRole.Farm,
            (SettlementType.Town,    1) => SettlementTileRole.Market,
            (SettlementType.Town,    2) => SettlementTileRole.Inn,
            (SettlementType.Town,    3) => SettlementTileRole.Guard,
            (SettlementType.Town,    _) => index % 3 == 0 ? SettlementTileRole.Farm : SettlementTileRole.Residential,
            (SettlementType.City,    1) => SettlementTileRole.Market,
            (SettlementType.City,    2) => SettlementTileRole.Guard,
            (SettlementType.City,    3) => SettlementTileRole.Inn,
            (SettlementType.City,    4) => SettlementTileRole.Market,
            (SettlementType.City,    5) => SettlementTileRole.Mill,
            (SettlementType.City,    _) => (index % 4) switch
            {
                0 => SettlementTileRole.Residential,
                1 => SettlementTileRole.Farm,
                2 => SettlementTileRole.Guard,
                _ => SettlementTileRole.Farm,
            },
            _ => SettlementTileRole.Farm,
        };

    // ── MST via Kruskal's ──────────────────────────────────────────────────────

    private static List<(Settlement From, Settlement To)> ComputeMSTEdges(List<Settlement> settlements)
    {
        var edges = new List<(float Dist, int I, int J)>();
        for (int i = 0; i < settlements.Count; i++)
            for (int j = i + 1; j < settlements.Count; j++)
                edges.Add((HexDist(settlements[i].CenterQ, settlements[i].CenterR,
                                   settlements[j].CenterQ, settlements[j].CenterR), i, j));

        edges.Sort((a, b) => a.Dist.CompareTo(b.Dist));

        int[] parent = Enumerable.Range(0, settlements.Count).ToArray();
        int Find(int x) => parent[x] == x ? x : parent[x] = Find(parent[x]);

        var result = new List<(Settlement, Settlement)>();
        foreach (var (_, i, j) in edges)
        {
            if (Find(i) != Find(j))
            {
                result.Add((settlements[i], settlements[j]));
                parent[Find(i)] = Find(j);
            }
        }
        return result;
    }

    // ── A* Pathfinding ─────────────────────────────────────────────────────────

    private List<(int Q, int R)>? FindPath(
        int fromQ, int fromR, int toQ, int toR,
        HashSet<(int, int)> existingRoad, int width, int height)
    {
        var open    = new PriorityQueue<(int Q, int R), float>();
        var gCost   = new Dictionary<(int, int), float>();
        var cameFrom = new Dictionary<(int, int), (int, int)>();
        const int MaxIter = 30_000;
        int iter = 0;

        var start = (fromQ, fromR);
        var goal  = (toQ, toR);
        gCost[start] = 0;
        open.Enqueue(start, HexDist(fromQ, fromR, toQ, toR));

        while (open.Count > 0 && iter++ < MaxIter)
        {
            var cur = open.Dequeue();
            if (cur == goal) return Reconstruct(cameFrom, goal);

            foreach (var (dq, dr) in Neighbors)
            {
                int nq = cur.Q + dq, nr = cur.R + dr;
                if (nq < 0 || nr < 0 || nq >= width || nr >= height) continue;

                var tile = _mapCache.GetCachedTile(nq, nr);
                if (tile == null || tile.Biome is BiomeType.Ocean or BiomeType.Lake) continue;

                float cost = tile.Biome switch
                {
                    BiomeType.Mountain or BiomeType.Volcano => 4f,
                    BiomeType.Glacier                       => 6f,
                    BiomeType.River                         => 1.5f,
                    BiomeType.Swamp or BiomeType.Marsh      => 2f,
                    _                                       => 1f,
                };
                if (existingRoad.Contains((nq, nr))) cost *= 0.3f;

                float ng = gCost[cur] + cost;
                var nb = (nq, nr);
                if (!gCost.TryGetValue(nb, out float existing) || ng < existing)
                {
                    gCost[nb]    = ng;
                    cameFrom[nb] = cur;
                    open.Enqueue(nb, ng + HexDist(nq, nr, toQ, toR));
                }
            }
        }
        return null;
    }

    private static List<(int Q, int R)> Reconstruct(
        Dictionary<(int, int), (int, int)> cameFrom, (int Q, int R) current)
    {
        var path = new List<(int Q, int R)> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            path.Add(prev);
            current = prev;
        }
        path.Reverse();
        return path;
    }

    // ── Utilities ──────────────────────────────────────────────────────────────

    private static int HexDist(int q1, int r1, int q2, int r2)
    {
        int dq = q2 - q1, dr = r2 - r1;
        return Math.Max(Math.Abs(dq), Math.Max(Math.Abs(dr), Math.Abs(dq + dr)));
    }

    private static void Shuffle<T>(List<T> list, Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static string GenerateName(Random rng, HashSet<string> used)
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            var name = Prefixes[rng.Next(Prefixes.Length)] + Suffixes[rng.Next(Suffixes.Length)];
            if (used.Add(name)) return name;
        }
        return $"Settlement {used.Count + 1}";
    }
}
