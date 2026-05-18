using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class DungeonGenerationService
{
    // Axial hex directions: E, NE, NW, W, SW, SE
    private static readonly (int dq, int dr)[] Directions =
    [
        ( 1,  0), ( 1, -1), ( 0, -1),
        (-1,  0), (-1,  1), ( 0,  1),
    ];

    // Map entrance feature IDs to dungeon themes and key types.
    private static readonly Dictionary<string, (string Theme, string KeyId)> ThemeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CaveEntrance"] = ("CrystalCavern", "IronKey"),
        ["IcyCavern"]    = ("FrozenVault",   "FrostKey"),
        ["FrozenShrine"] = ("FrozenVault",   "FrostKey"),
        ["AncientRuins"] = ("AncientTomb",   "AncientKey"),
        ["RuinedTower"]  = ("AncientTomb",   "AncientKey"),
        ["RuinedTemple"] = ("AncientTomb",   "AncientKey"),
        ["CrumbledFortress"] = ("AncientTomb", "AncientKey"),
        ["StoneCircle"]  = ("AncientTomb",   "AncientKey"),
        ["TreeHollow"]   = ("RootLabyrinth", "WoodKey"),
    };

    // Loot pools by theme and tier.
    // Tier 0 = common, 1 = uncommon, 2 = rare/dungeon-exclusive
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

    // Craftable items that can appear as bonus loot anywhere.
    private static readonly string[] BonusLoot =
    [
        "Rope", "Lantern", "Compass", "Amber", "Quartz", "HollowStone", "CrackedOrb",
    ];

    // Room type weights per floor (1-indexed). Weights for:
    // Empty, Trap, Dark, Treasure, Rest, Shrine, LockedDoor
    private static readonly (DungeonRoomType Type, int[] Weights)[] DistributionTable =
    [
        (DungeonRoomType.Empty,      [45, 35, 30]),
        (DungeonRoomType.Trap,       [10, 18, 25]),
        (DungeonRoomType.Dark,       [10, 12, 10]),
        (DungeonRoomType.Treasure,   [12, 12, 10]),
        (DungeonRoomType.Rest,       [10,  6,  4]),
        (DungeonRoomType.Shrine,     [ 6,  6,  5]),
        (DungeonRoomType.LockedDoor, [ 4,  6,  8]),
    ];

    public Task<DungeonInstance> GenerateAsync(int entranceQ, int entranceR, string featureId)
    {
        var rng = new Random();
        var (theme, keyId) = ThemeMap.TryGetValue(featureId, out var t) ? t : ("CrystalCavern", "IronKey");
        int floorCount = rng.Next(0, 3) == 0 ? 2 : 3; // 1/3 chance of 2 floors, else 3

        var dungeon = new DungeonInstance
        {
            Id          = Guid.NewGuid().ToString("N"),
            EntranceQ   = entranceQ,
            EntranceR   = entranceR,
            DungeonTheme = theme,
            GeneratedAt = DateTime.UtcNow,
        };

        for (int f = 1; f <= floorCount; f++)
        {
            bool isDeepest = f == floorCount;
            var floor = GenerateFloor(rng, f, isDeepest, theme, keyId, floorCount);
            dungeon.Floors.Add(floor);
        }

        return Task.FromResult(dungeon);
    }

    private DungeonFloor GenerateFloor(Random rng, int floorNumber, bool isDeepest, string theme, string keyId, int totalFloors)
    {
        var floor = new DungeonFloor { FloorNumber = floorNumber };
        var rooms = new Dictionary<(int, int), DungeonRoom>();

        // Entrance at (0,0)
        var entrance = new DungeonRoom
        {
            Q    = 0,
            R    = 0,
            Type = DungeonRoomType.StaircaseUp,
        };
        rooms[(0, 0)] = entrance;

        // BFS expansion
        var frontier = new Queue<(int q, int r)>();
        frontier.Enqueue((0, 0));
        int targetSize = rng.Next(25, 36);

        while (rooms.Count < targetSize && frontier.Count > 0)
        {
            var (q, r) = frontier.Dequeue();
            int maxNew = rng.Next(1, 5);
            var shuffled = Directions.OrderBy(_ => rng.Next()).Take(maxNew);

            foreach (var (dq, dr) in shuffled)
            {
                if (rooms.Count >= targetSize) break;
                int nq = q + dq, nr = r + dr;
                if (rooms.ContainsKey((nq, nr))) continue;

                var neighbor = new DungeonRoom { Q = nq, R = nr, Type = DungeonRoomType.Empty };
                rooms[(nq, nr)] = neighbor;

                // Wire corridors both ways
                int dirIdx      = Array.IndexOf(Directions, (dq, dr));
                int reverseDirIdx = (dirIdx + 3) % 6;
                rooms[(q, r)].PassableDirections.Add(dirIdx);
                neighbor.PassableDirections.Add(reverseDirIdx);

                frontier.Enqueue((nq, nr));
            }
        }

        // Assign room types
        AssignRoomTypes(rng, rooms, floorNumber, isDeepest, theme, keyId, totalFloors);

        floor.Rooms.AddRange(rooms.Values);
        return floor;
    }

    private void AssignRoomTypes(Random rng, Dictionary<(int, int), DungeonRoom> rooms,
        int floorNumber, bool isDeepest, string theme, string keyId, int totalFloors)
    {
        int fi = Math.Min(floorNumber, 3) - 1; // 0-indexed, capped at 2

        // Collect assignable rooms (not entrance/staircase)
        var assignable = rooms.Values
            .Where(r => r.Type is not DungeonRoomType.Entrance and not DungeonRoomType.StaircaseUp)
            .ToList();

        // Find the farthest room from (0,0) for StaircaseDown / Boss
        var farthest = assignable
            .OrderByDescending(r => HexDistance(0, 0, r.Q, r.R))
            .FirstOrDefault();

        if (farthest != null)
        {
            if (isDeepest)
                farthest.Type = DungeonRoomType.Boss;
            else
                farthest.Type = DungeonRoomType.StaircaseDown;

            assignable.Remove(farthest);
        }

        // Guarantee StaircaseUp on floors 2+ (entrance tile handles floor 1 exit)
        if (!isDeepest && floorNumber > 1)
        {
            var stairUp = assignable.FirstOrDefault();
            if (stairUp != null)
            {
                stairUp.Type = DungeonRoomType.StaircaseUp;
                assignable.Remove(stairUp);
            }
        }

        // Place LockedDoor rooms as blockers on corridors to good areas
        int lockedDoorCount = DistributionTable.First(d => d.Type == DungeonRoomType.LockedDoor).Weights[fi] / 3;
        var lockedDoorCandidates = assignable.Where(r => r.PassableDirections.Count >= 2).Take(lockedDoorCount * 2).ToList();
        int placed = 0;
        foreach (var r in lockedDoorCandidates)
        {
            if (placed >= lockedDoorCount) break;
            r.Type = DungeonRoomType.LockedDoor;
            r.RequiredKeyId = keyId;
            assignable.Remove(r);
            placed++;
        }

        // Build weighted pool for remaining rooms
        var weightedTypes = DistributionTable
            .Where(d => d.Type is not DungeonRoomType.LockedDoor)
            .Select(d => (d.Type, Weight: d.Weights[fi]))
            .ToList();
        int totalWeight = weightedTypes.Sum(x => x.Weight);

        foreach (var room in assignable)
        {
            int roll = rng.Next(totalWeight);
            int cumulative = 0;
            foreach (var (type, weight) in weightedTypes)
            {
                cumulative += weight;
                if (roll < cumulative)
                {
                    room.Type = type;
                    break;
                }
            }
        }

        // Assign loot to Treasure and Boss rooms
        foreach (var room in rooms.Values)
        {
            if (room.Type is DungeonRoomType.Treasure or DungeonRoomType.Boss)
            {
                bool isBoss = room.Type == DungeonRoomType.Boss;
                PopulateLoot(rng, room, theme, floorNumber, isBoss, keyId);
            }
        }
    }

    private void PopulateLoot(Random rng, DungeonRoom room, string theme, int floorNumber, bool isBoss, string keyId)
    {
        if (!LootPools.TryGetValue(theme, out var pools)) return;

        int picks = isBoss ? floorNumber + 2 : rng.Next(1, 3);

        for (int i = 0; i < picks; i++)
        {
            // Higher floor = better loot tier
            int maxTier = Math.Min(floorNumber, 3);
            int tier = rng.Next(0, maxTier);
            var pool = pools[tier];
            string item = pool[rng.Next(pool.Length)];
            room.Loot[item] = room.Loot.GetValueOrDefault(item) + 1;
        }

        // Chance of bonus craftable loot
        if (rng.NextDouble() < 0.35)
        {
            string bonus = BonusLoot[rng.Next(BonusLoot.Length)];
            room.Loot[bonus] = room.Loot.GetValueOrDefault(bonus) + 1;
        }

        // Boss room always drops the theme key (first-time only enforced by IsCleared logic)
        if (isBoss)
        {
            room.Loot[keyId] = 1;
        }
    }

    private static int HexDistance(int q1, int r1, int q2, int r2)
    {
        int dq = q2 - q1, dr = r2 - r1;
        return (Math.Abs(dq) + Math.Abs(dr) + Math.Abs(dq + dr)) / 2;
    }
}
