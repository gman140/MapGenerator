using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class DungeonGenerationService
{
    private static readonly (int dq, int dr)[] Directions =
    [
        ( 1,  0), ( 1, -1), ( 0, -1),
        (-1,  0), (-1,  1), ( 0,  1),
    ];

    private static readonly Dictionary<string, (string Theme, string KeyId)> ThemeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CaveEntrance"]     = ("CrystalCavern", "IronKey"),
        ["IcyCavern"]        = ("FrozenVault",   "FrostKey"),
        ["FrozenShrine"]     = ("FrozenVault",   "FrostKey"),
        ["AncientRuins"]     = ("AncientTomb",   "AncientKey"),
        ["RuinedTower"]      = ("AncientTomb",   "AncientKey"),
        ["RuinedTemple"]     = ("AncientTomb",   "AncientKey"),
        ["CrumbledFortress"] = ("AncientTomb",   "AncientKey"),
        ["StoneCircle"]      = ("AncientTomb",   "AncientKey"),
        ["TreeHollow"]       = ("RootLabyrinth", "WoodKey"),
    };

    private static readonly Dictionary<string, string[][]> LootPools = new()
    {
        ["CrystalCavern"] =
        [
            ["Ore", "Flint", "Coal", "Stone", "Quartz"],
            ["Amber", "HollowStone", "CrackedOrb", "DeepOre"],
            ["CaveCrystal", "GlowingMoss"],
        ],
        ["FrozenVault"] =
        [
            ["Ore", "Flint", "Ice", "Stone", "Quartz"],
            ["Amber", "FrozenFlower", "HollowStone"],
            ["FrozenRelic", "IceShard"],
        ],
        ["AncientTomb"] =
        [
            ["Stone", "Flint", "Coal", "Herbs", "Quartz"],
            ["TidalCoin", "BoneFragment", "CrackedOrb", "TarnishedRing"],
            ["AncientShard", "BoneRune", "TarnishedRelic"],
        ],
        ["RootLabyrinth"] =
        [
            ["Wood", "Fiber", "Herbs", "Moss", "Reed"],
            ["Amber", "HollowStone", "PaleMushroom"],
            ["DeepMushroom", "TangledRoot", "MossGem"],
        ],
    };

    private static readonly string[] BonusLoot =
    [
        "Rope", "Lantern", "Compass", "Amber", "Quartz", "HollowStone", "CrackedOrb",
    ];

    // Weighted room types for cluster centers only. No traps at centers — those come from the scatter pass.
    // Floor-indexed weights: [floor1, floor2, floor3].
    private static readonly (DungeonRoomType Type, int[] Weights)[] RoomTypeTable =
    [
        (DungeonRoomType.Empty,  [25, 20, 15]),
        (DungeonRoomType.Vault,  [30, 28, 25]),
        (DungeonRoomType.Dark,   [15, 15, 15]),
        (DungeonRoomType.Rest,   [18, 12,  5]),
        (DungeonRoomType.Shrine, [12, 25, 40]),
    ];

    public Task<DungeonInstance> GenerateAsync(int entranceQ, int entranceR, string featureId)
    {
        var rng = new Random();
        var (theme, keyId) = ThemeMap.TryGetValue(featureId, out var t) ? t : ("CrystalCavern", "IronKey");
        int floorCount = rng.Next(3) == 0 ? 2 : 3;

        var dungeon = new DungeonInstance
        {
            Id           = Guid.NewGuid().ToString("N"),
            EntranceQ    = entranceQ,
            EntranceR    = entranceR,
            DungeonTheme = theme,
            GeneratedAt  = DateTime.UtcNow,
        };

        for (int f = 1; f <= floorCount; f++)
            dungeon.Floors.Add(GenerateFloor(rng, f, f == floorCount, theme, keyId));

        return Task.FromResult(dungeon);
    }

    private DungeonFloor GenerateFloor(Random rng, int floorNumber, bool isDeepest, string theme, string keyId)
    {
        // ── Step 1: Place room centers ────────────────────────────────────────
        int roomCount = rng.Next(7, 12);
        var centers = PlaceRoomCenters(rng, roomCount);

        // ── Step 2: Carve room clusters ───────────────────────────────────────
        var tiles = new Dictionary<(int q, int r), DungeonRoom>();
        var clusterHexes = new HashSet<(int q, int r)>();

        foreach (var center in centers)
        {
            int radius = rng.Next(2) == 0 ? 1 : 2;
            foreach (var hex in HexesInRadius(center, radius))
            {
                tiles.TryAdd(hex, MakeRoom(hex));
                clusterHexes.Add(hex);
            }
        }

        // ── Step 3: Connect rooms with corridors ──────────────────────────────
        var edges = BuildSpanningTree(centers);

        // Add 1-2 random loop edges
        for (int i = 0, loops = rng.Next(1, 3); i < loops; i++)
        {
            var a = centers[rng.Next(centers.Count)];
            var b = centers[rng.Next(centers.Count)];
            if (a != b) edges.Add((a, b));
        }

        foreach (var (from, to) in edges)
            foreach (var hex in CarveCorridor(rng, from, to))
                tiles.TryAdd(hex, MakeRoom(hex));

        // ── Step 4: Assign room types to cluster centers ──────────────────────
        var centerSet = new HashSet<(int q, int r)>(centers);
        AssignRoomTypes(rng, tiles, centers, floorNumber, isDeepest);

        // ── Step 5: Scatter treasure and traps across non-center tiles ────────
        ScatterTilesPass(rng, tiles, centerSet);

        // ── Step 6: Place locked door tiles in corridor bottlenecks ──────────
        PlaceLockedDoors(rng, tiles, clusterHexes, floorNumber, keyId);

        // ── Step 7: Populate loot ─────────────────────────────────────────────
        foreach (var room in tiles.Values)
            if (room.Type is DungeonRoomType.Treasure or DungeonRoomType.Vault or DungeonRoomType.Boss)
                PopulateLoot(rng, room, theme, floorNumber, keyId);

        var floor = new DungeonFloor { FloorNumber = floorNumber };
        floor.Rooms.AddRange(tiles.Values);
        return floor;
    }

    // ── Room center placement ─────────────────────────────────────────────────

    private static List<(int q, int r)> PlaceRoomCenters(Random rng, int count)
    {
        const int Bound = 22;
        const int MinSpacing = 8;
        var centers = new List<(int q, int r)> { (0, 0) };

        for (int attempt = 0; attempt < 3000 && centers.Count < count; attempt++)
        {
            int q = rng.Next(-Bound, Bound + 1);
            int r = rng.Next(-Bound, Bound + 1);
            if (HexDist(0, 0, q, r) > Bound) continue;
            if (centers.All(c => HexDist(c.q, c.r, q, r) >= MinSpacing))
                centers.Add((q, r));
        }

        return centers;
    }

    private static IEnumerable<(int q, int r)> HexesInRadius((int q, int r) center, int radius)
    {
        for (int dq = -radius; dq <= radius; dq++)
        for (int dr = Math.Max(-radius, -dq - radius); dr <= Math.Min(radius, -dq + radius); dr++)
            yield return (center.q + dq, center.r + dr);
    }

    // ── Spanning tree (Prim's) ────────────────────────────────────────────────

    private static List<((int q, int r) from, (int q, int r) to)> BuildSpanningTree(List<(int q, int r)> centers)
    {
        var edges = new List<((int q, int r), (int q, int r))>();
        var connected = new HashSet<(int q, int r)> { centers[0] };
        var remaining = new List<(int q, int r)>(centers.Skip(1));

        while (remaining.Count > 0)
        {
            (int q, int r) bestA = default, bestB = default;
            int bestDist = int.MaxValue;

            foreach (var a in connected)
            foreach (var b in remaining)
            {
                int d = HexDist(a.q, a.r, b.q, b.r);
                if (d < bestDist) { bestDist = d; bestA = a; bestB = b; }
            }

            edges.Add((bestA, bestB));
            connected.Add(bestB);
            remaining.Remove(bestB);
        }

        return edges;
    }

    // ── Corridor carving (biased random walk) ─────────────────────────────────

    private static HashSet<(int q, int r)> CarveCorridor(Random rng, (int q, int r) from, (int q, int r) to)
    {
        var result = new HashSet<(int q, int r)>();
        var (q, r) = from;

        for (int step = 0; step < 500; step++)
        {
            result.Add((q, r));
            if (q == to.q && r == to.r) break;

            // Shuffle neighbors for unbiased tie-breaking, then pick closest or random
            var neighbors = Directions
                .Select(d => (q: q + d.dq, r: r + d.dr))
                .OrderBy(_ => rng.Next())
                .ToList();

            (int q, int r) next = rng.NextDouble() < 0.75
                ? neighbors.MinBy(n => HexDist(n.q, n.r, to.q, to.r))
                : neighbors[rng.Next(neighbors.Count)];

            (q, r) = next;
        }

        return result;
    }

    // ── Room type assignment ──────────────────────────────────────────────────

    private static void AssignRoomTypes(Random rng, Dictionary<(int q, int r), DungeonRoom> tiles,
        List<(int q, int r)> centers, int floorNumber, bool isDeepest)
    {
        int fi = Math.Min(floorNumber, 3) - 1;

        // Origin cluster = StaircaseUp on every floor (exit on Floor 1, return stair on deeper floors)
        if (tiles.TryGetValue(centers[0], out var stairUp))
            stairUp.Type = DungeonRoomType.StaircaseUp;

        // Farthest cluster = Boss (deepest) or StaircaseDown (non-deepest)
        var assignable = centers.Skip(1).ToList();
        if (assignable.Count == 0) return;

        var farthest = assignable.OrderByDescending(c => HexDist(0, 0, c.q, c.r)).First();
        if (tiles.TryGetValue(farthest, out var farthestRoom))
            farthestRoom.Type = isDeepest ? DungeonRoomType.Boss : DungeonRoomType.StaircaseDown;

        // Remaining centers: weighted random types
        int totalWeight = RoomTypeTable.Sum(x => x.Weights[fi]);
        foreach (var center in assignable.Where(c => c != farthest))
        {
            if (!tiles.TryGetValue(center, out var room)) continue;
            int roll = rng.Next(totalWeight), cumulative = 0;
            foreach (var (type, weights) in RoomTypeTable)
            {
                cumulative += weights[fi];
                if (roll < cumulative) { room.Type = type; break; }
            }
        }
    }

    // ── Locked door placement ─────────────────────────────────────────────────

    private static void PlaceLockedDoors(Random rng, Dictionary<(int q, int r), DungeonRoom> tiles,
        HashSet<(int q, int r)> clusterHexes, int floorNumber, string keyId)
    {
        int fi = Math.Min(floorNumber, 3) - 1;
        int doorCount = fi + 1; // 1 / 2 / 3 per floor

        // Bottleneck = corridor tile (not in any cluster) with exactly 2 adjacent floor tiles
        var candidates = tiles.Keys
            .Where(pos => !clusterHexes.Contains(pos)
                && Directions.Count(d => tiles.ContainsKey((pos.q + d.dq, pos.r + d.dr))) == 2)
            .OrderBy(_ => rng.Next())
            .Take(doorCount * 4)
            .ToList();

        int placed = 0;
        foreach (var pos in candidates)
        {
            if (placed >= doorCount) break;
            tiles[pos].Type = DungeonRoomType.LockedDoor;
            tiles[pos].RequiredKeyId = keyId;
            placed++;
        }
    }

    // ── Scatter pass ──────────────────────────────────────────────────────────

    private static void ScatterTilesPass(Random rng, Dictionary<(int q, int r), DungeonRoom> tiles,
        HashSet<(int q, int r)> centerSet)
    {
        const double TreasureChance = 0.20;
        const double TrapChance     = 0.15;

        foreach (var room in tiles.Values)
        {
            if (room.Type != DungeonRoomType.Empty) continue;
            if (centerSet.Contains((room.Q, room.R))) continue;

            double roll = rng.NextDouble();
            if (roll < TreasureChance)
                room.Type = DungeonRoomType.Treasure;
            else if (roll < TreasureChance + TrapChance)
                room.Type = DungeonRoomType.Trap;
        }
    }

    // ── Loot population ───────────────────────────────────────────────────────

    private static void PopulateLoot(Random rng, DungeonRoom room, string theme, int floorNumber, string keyId)
    {
        if (!LootPools.TryGetValue(theme, out var pools)) return;

        bool isBoss  = room.Type == DungeonRoomType.Boss;
        bool isVault = room.Type == DungeonRoomType.Vault;

        // Boss gets the most, vault gets a solid handful, scattered treasure gets one to 3 picks
        int picks = isBoss ? floorNumber + 2 : isVault ? rng.Next(2, 5) : rng.Next(1, 3);
        for (int i = 0; i < picks; i++)
        {
            int tier = rng.Next(0, Math.Min(floorNumber, 3));
            var pool = pools[tier];
            string item = pool[rng.Next(pool.Length)];
            room.Loot[item] = room.Loot.GetValueOrDefault(item) + 1;
        }

        double bonusChance = isBoss || isVault ? 0.55 : 0.20;
        if (rng.NextDouble() < bonusChance)
        {
            string bonus = BonusLoot[rng.Next(BonusLoot.Length)];
            room.Loot[bonus] = room.Loot.GetValueOrDefault(bonus) + 1;
        }

        if (isBoss)
            room.Loot[keyId] = 1;
        else if (isVault && rng.NextDouble() < 0.22)
            room.Loot[keyId] = 1;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static DungeonRoom MakeRoom((int q, int r) pos) =>
        new() { Q = pos.q, R = pos.r, Type = DungeonRoomType.Empty };

    private static int HexDist(int q1, int r1, int q2, int r2)
    {
        int dq = q2 - q1, dr = r2 - r1;
        return (Math.Abs(dq) + Math.Abs(dr) + Math.Abs(dq + dr)) / 2;
    }
}
