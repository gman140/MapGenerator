using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class DungeonService
{
    // Axial hex directions: E, NE, NW, W, SW, SE
    private static readonly (int dq, int dr)[] Directions =
    [
        ( 1,  0), ( 1, -1), ( 0, -1),
        (-1,  0), (-1,  1), ( 0,  1),
    ];

    private readonly IDungeonRepository _dungeonRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly DungeonGenerationService _generator;
    private readonly ICombatEngine _combatEngine;
    private readonly ICombatRepository _combatRepo;

    private static readonly Dictionary<string, string> FeatureThemeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CaveEntrance"]     = "CrystalCavern",
        ["IcyCavern"]        = "FrozenVault",
        ["FrozenShrine"]     = "FrozenVault",
        ["AncientRuins"]     = "AncientTomb",
        ["RuinedTower"]      = "AncientTomb",
        ["RuinedTemple"]     = "AncientTomb",
        ["CrumbledFortress"] = "AncientTomb",
        ["StoneCircle"]      = "AncientTomb",
        ["TreeHollow"]       = "RootLabyrinth",
    };

    public DungeonService(
        IDungeonRepository dungeonRepo,
        IPlayerRepository playerRepo,
        IResourceDefinitionProvider resourceProvider,
        DungeonGenerationService generator,
        ICombatEngine combatEngine,
        ICombatRepository combatRepo)
    {
        _dungeonRepo      = dungeonRepo;
        _playerRepo       = playerRepo;
        _resourceProvider = resourceProvider;
        _generator        = generator;
        _combatEngine     = combatEngine;
        _combatRepo       = combatRepo;
    }

    // ── Entry ────────────────────────────────────────────────────────────────

    public async Task<(bool success, string message, List<(int Q, int R)> revealedRooms)> EnterDungeonAsync(Player player, string featureId)
    {
        var dungeon = await _dungeonRepo.GetByEntranceAsync(player.Q, player.R);
        if (dungeon == null)
        {
            dungeon = await _generator.GenerateAsync(player.Q, player.R, featureId);
            await _dungeonRepo.SaveAsync(dungeon);
        }

        player.DungeonInstanceId = dungeon.Id;
        player.DungeonFloor      = 1;
        player.DungeonQ          = 0;
        player.DungeonR          = 0;
        await _playerRepo.UpdateAsync(player);

        bool hasLantern = player.Inventory.GetValueOrDefault("Lantern") > 0
                          || player.CraftedItems.GetValueOrDefault("Lantern") > 0;
        List<(int Q, int R)> revealedRooms = [];
        var floor1 = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == 1);
        if (hasLantern && floor1 != null)
            revealedRooms = GetAdjacentRoomCoords(0, 0, floor1);

        string theme = FeatureThemeMap.TryGetValue(featureId, out var t) ? t : "CrystalCavern";
        return (true, EnterMessage(theme), revealedRooms);
    }

    // ── Staircase use ─────────────────────────────────────────────────────────

    public async Task<DungeonMoveResult> UseStaircaseAsync(Player player)
    {
        var dungeon = await _dungeonRepo.GetByIdAsync(player.DungeonInstanceId!);
        if (dungeon == null) return DungeonMoveResult.Fail("Dungeon not found.");

        var floor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == player.DungeonFloor);
        if (floor == null) return DungeonMoveResult.Fail("Floor not found.");

        var room = floor.Rooms.FirstOrDefault(r => r.Q == player.DungeonQ && r.R == player.DungeonR);
        if (room == null) return DungeonMoveResult.Fail("Room not found.");

        bool hasLantern = player.Inventory.GetValueOrDefault("Lantern") > 0
                          || player.CraftedItems.GetValueOrDefault("Lantern") > 0;
        var result = new DungeonMoveResult { Success = true };

        if (room.Type == DungeonRoomType.StaircaseDown)
        {
            int nextFloor = player.DungeonFloor + 1;
            var newFloor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == nextFloor);
            if (newFloor == null) return DungeonMoveResult.Fail("No floor below.");

            player.DungeonFloor = nextFloor;
            var stairUp = newFloor.Rooms.FirstOrDefault(r => r.Type == DungeonRoomType.StaircaseUp);
            player.DungeonQ = stairUp?.Q ?? 0;
            player.DungeonR = stairUp?.R ?? 0;

            if (hasLantern)
                result.RevealedRooms = GetAdjacentRoomCoords(player.DungeonQ, player.DungeonR, newFloor);

            result.RoomMessage = $"You descend. The air grows heavier. Floor {nextFloor}.";
        }
        else if (room.Type == DungeonRoomType.StaircaseUp)
        {
            if (player.DungeonFloor == 1)
            {
                ExitDungeon(player);
                result.ExitedToSurface = true;
                result.RoomMessage = "You climb out through the entrance. The open air meets you with something like relief.";
            }
            else
            {
                int prevFloorNum = player.DungeonFloor - 1;
                var prevFloor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == prevFloorNum);
                if (prevFloor == null) return DungeonMoveResult.Fail("No floor above.");

                player.DungeonFloor = prevFloorNum;
                var downRoom = prevFloor.Rooms.FirstOrDefault(r => r.Type == DungeonRoomType.StaircaseDown);
                player.DungeonQ = downRoom?.Q ?? 0;
                player.DungeonR = downRoom?.R ?? 0;

                if (hasLantern)
                    result.RevealedRooms = GetAdjacentRoomCoords(player.DungeonQ, player.DungeonR, prevFloor);

                result.RoomMessage = $"You climb back up. Floor {prevFloorNum}.";
            }
        }
        else
        {
            return DungeonMoveResult.Fail("You are not standing on a staircase.");
        }

        result.NewFloor = player.DungeonFloor;
        result.NewQ     = player.DungeonQ;
        result.NewR     = player.DungeonR;
        await _playerRepo.UpdateAsync(player);
        return result;
    }

    // ── Movement ─────────────────────────────────────────────────────────────

    public async Task<DungeonMoveResult> MoveAsync(Player player, int targetQ, int targetR, IReadOnlySet<Permission> permissions)
    {
        var dungeon = await _dungeonRepo.GetByIdAsync(player.DungeonInstanceId!);
        if (dungeon == null)
            return DungeonMoveResult.Fail("Dungeon not found.");

        var floor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == player.DungeonFloor);
        if (floor == null)
            return DungeonMoveResult.Fail("Floor not found.");

        // Validate adjacency
        int dq = targetQ - player.DungeonQ, dr = targetR - player.DungeonR;
        if (!Directions.Any(d => d == (dq, dr)))
            return DungeonMoveResult.Fail("You can only move to adjacent rooms.");

        var targetRoom = floor.Rooms.FirstOrDefault(r => r.Q == targetQ && r.R == targetR);
        if (targetRoom == null)
            return DungeonMoveResult.Fail("There is no passage in that direction.");

        // Locked door check
        if (targetRoom.Type == DungeonRoomType.LockedDoor && targetRoom.RequiredKeyId != null)
        {
            if (!player.Inventory.TryGetValue(targetRoom.RequiredKeyId, out int qty) || qty <= 0)
                return DungeonMoveResult.Fail($"The door is locked. You need a {GetKeyName(targetRoom.RequiredKeyId)} to open it.");

            // Consume key and permanently unlock
            player.Inventory[targetRoom.RequiredKeyId] = qty - 1;
            if (player.Inventory[targetRoom.RequiredKeyId] <= 0)
                player.Inventory.Remove(targetRoom.RequiredKeyId);
            targetRoom.RequiredKeyId = null;
            await _dungeonRepo.UpdateRoomAsync(dungeon.Id, floor.FloorNumber, targetRoom);
        }

        // Move player
        player.DungeonQ = targetQ;
        player.DungeonR = targetR;

        var result = new DungeonMoveResult { Success = true };

        // Apply room entry effects
        var (roomMsg, bossCombatStarted) = await ApplyRoomEntryEffects(player, dungeon, floor, targetRoom, permissions);
        result.RoomMessage    = roomMsg;
        result.CombatStarted  = bossCombatStarted;

        // Reveal neighbors if Lantern in inventory
        bool hasLantern = player.Inventory.GetValueOrDefault("Lantern") > 0
                          || player.CraftedItems.GetValueOrDefault("Lantern") > 0;
        if (hasLantern)
            result.RevealedRooms = GetAdjacentRoomCoords(targetQ, targetR, floor);

        result.NewFloor        = player.DungeonFloor;
        result.NewQ            = player.DungeonQ;
        result.NewR            = player.DungeonR;
        result.ExitedToSurface = !player.IsInDungeon;

        // Dungeon random encounter (15% on corridor moves, skip if combat already started)
        if (!result.CombatStarted && !player.IsInCombat && !result.ExitedToSurface)
        {
            var dungeonSession = _combatEngine.TryGenerateEncounter(
                player, string.Empty, inDungeon: true, dungeon.DungeonTheme, floor.FloorNumber);
            if (dungeonSession != null)
            {
                player.ActiveCombatSessionId = dungeonSession.Id;
                await _combatRepo.SaveAsync(dungeonSession);
                result.CombatStarted = true;
            }
        }

        await _playerRepo.UpdateAsync(player);
        return result;
    }

    // ── Exit (rope) ───────────────────────────────────────────────────────────

    public async Task<(bool success, string message)> UseRopeAsync(Player player)
    {
        if (!player.IsInDungeon)
            return (false, "You are not in a dungeon.");

        int qty = player.Inventory.GetValueOrDefault("Rope");
        if (qty <= 0)
            return (false, "You have no Rope.");

        player.Inventory["Rope"] = qty - 1;
        if (player.Inventory["Rope"] <= 0)
            player.Inventory.Remove("Rope");

        ExitDungeon(player);
        await _playerRepo.UpdateAsync(player);
        return (true, "You throw the rope upward. It catches on something. You climb out into the open air, blinking.");
    }

    // ── Gather (dungeon loot) ─────────────────────────────────────────────────

    public async Task<GatherResult> GatherAsync(Player player, IReadOnlySet<Permission> permissions)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        const long CooldownMs = 10_000;

        if (!permissions.Contains(Permission.IgnoreCooldowns) && now < player.GatherCooldownUntil)
        {
            double secs = (player.GatherCooldownUntil - now) / 1000.0;
            return GatherResult.Fail($"You must wait {secs:F1}s before gathering again.");
        }

        var dungeon = await _dungeonRepo.GetByIdAsync(player.DungeonInstanceId!);
        if (dungeon == null) return GatherResult.Fail("Dungeon not found.");

        var floor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == player.DungeonFloor);
        if (floor == null) return GatherResult.Fail("Floor not found.");

        var room = floor.Rooms.FirstOrDefault(r => r.Q == player.DungeonQ && r.R == player.DungeonR);
        if (room == null) return GatherResult.Fail("Room not found.");

        if (room.Type is not DungeonRoomType.Treasure and not DungeonRoomType.Boss)
            return GatherResult.Fail("There is nothing to gather here.");

        if (room.IsCleared)
            return GatherResult.Fail("This room has already been looted.");

        if (room.Loot.Count == 0)
            return GatherResult.Fail("The room is curiously empty.");

        // Dark room penalty
        bool hasLantern = player.Inventory.GetValueOrDefault("Lantern") > 0
                          || player.CraftedItems.GetValueOrDefault("Lantern") > 0;
        bool isDark = floor.Rooms.FirstOrDefault(r => r.Q == player.DungeonQ && r.R == player.DungeonR)?.Type == DungeonRoomType.Dark;
        double gatherMult = (isDark && !hasLantern) ? 0.5 : 1.0;

        // Apply gather buff
        gatherMult *= BuffService.ConsumeGatherMultiplier(player);

        var rng = new Random();
        var gathered = new List<GatheredItem>();

        foreach (var (resourceId, totalQty) in room.Loot)
        {
            // Each loot entry has a probability proportional to gather mult
            if (rng.NextDouble() > gatherMult) continue;

            var def = _resourceProvider.GetById(resourceId);
            if (def == null) continue;

            int qty = totalQty;
            gathered.Add(new GatheredItem { ResourceId = resourceId, Name = def.Name, Quantity = qty });
            player.Inventory.TryGetValue(resourceId, out int existing);
            player.Inventory[resourceId] = existing + qty;
        }

        room.IsCleared = true;
        await _dungeonRepo.UpdateRoomAsync(dungeon.Id, floor.FloorNumber, room);

        double hungerDrainMult = BuffService.ConsumeHungerDrainMultiplier(player);
        player.Satiety = Math.Max(0, player.Satiety - 2.0 * hungerDrainMult);

        long effectiveCooldown = CooldownMs;
        if (!permissions.Contains(Permission.IgnoreCooldowns))
        {
            effectiveCooldown = (long)(effectiveCooldown * HungerService.GetCooldownMultiplier(player.Satiety));
            effectiveCooldown = (long)(effectiveCooldown * BuffService.ConsumeCooldownMultiplier(player));
        }
        player.GatherCooldownUntil = permissions.Contains(Permission.IgnoreCooldowns) ? 0 : now + effectiveCooldown;
        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        string message = gathered.Count == 0
            ? "You search the room thoroughly but find nothing useful."
            : "You gather what this room held.";

        return new GatherResult { Success = true, Gathered = gathered, CooldownUntil = player.GatherCooldownUntil, Message = message };
    }

    // ── Investigate ───────────────────────────────────────────────────────────

    public async Task<string> InvestigateAsync(Player player)
    {
        var dungeon = await _dungeonRepo.GetByIdAsync(player.DungeonInstanceId!);
        if (dungeon == null) return "You look around. The dungeon is unreadable.";

        var floor = dungeon.Floors.FirstOrDefault(f => f.FloorNumber == player.DungeonFloor);
        if (floor == null) return "You look around. Your floor doesn't exist. This is alarming.";

        var room = floor.Rooms.FirstOrDefault(r => r.Q == player.DungeonQ && r.R == player.DungeonR);
        if (room == null) return "You look around. You are in a room that isn't here. You decide not to investigate further.";

        return room.Type switch
        {
            DungeonRoomType.Entrance    => "The way back to the surface is here. The opening above feels distant.",
            DungeonRoomType.StaircaseUp => player.DungeonFloor == 1
                ? "Stone steps lead upward toward the surface. The light at the top is faint but real."
                : "Stone steps lead up to the floor above.",
            DungeonRoomType.StaircaseDown => $"A shaft descends into floor {player.DungeonFloor + 1}. What's below is darker.",
            DungeonRoomType.LockedDoor    => room.RequiredKeyId == null
                ? "The door stands open. Whatever kept it locked is gone."
                : $"A heavy door blocks the way. The keyhole is shaped for a specific key — you'd know it if you had it.",
            DungeonRoomType.Trap when room.IsCleared =>
                "The hazard here has already sprung. The room is harmless now, if unpleasant.",
            DungeonRoomType.Rest when room.IsCleared =>
                "A resting spot, already used. The warmth is gone, but the shelter remains.",
            DungeonRoomType.Shrine when room.IsCleared =>
                "The shrine has given what it had to give. The glow is gone. Something was here, briefly.",
            DungeonRoomType.Shrine =>
                "A shrine carved into the rock, faintly luminous. Whatever it holds has not yet been received.",
            DungeonRoomType.Treasure when room.IsCleared =>
                "Someone has already been through here. What was here is gone.",
            DungeonRoomType.Treasure =>
                "The room holds something. You can feel it before you see it. There are items here waiting to be taken.",
            DungeonRoomType.Boss when room.IsCleared =>
                "This chamber has been stripped of everything it held. The weight of it lingers.",
            DungeonRoomType.Boss =>
                "A deeper chamber. Something accumulated here over a very long time. It can be yours. Gather it.",
            DungeonRoomType.Dark =>
                "The darkness in this room is total and deliberate. You can move through it but cannot read it.",
            DungeonRoomType.Empty =>
                "The room is empty. Damp stone, old air, a ceiling that has held for longer than you've been alive.",
            _ => "You look around. The room offers nothing, which is itself information.",
        };
    }

    // ── Room entry effects ────────────────────────────────────────────────────

    private async Task<(string? message, bool combatStarted)> ApplyRoomEntryEffects(
        Player player, DungeonInstance dungeon, DungeonFloor floor, DungeonRoom room, IReadOnlySet<Permission> permissions)
    {
        string? message = null;
        bool combatStarted = false;

        switch (room.Type)
        {
            case DungeonRoomType.Trap when !room.IsCleared:
                player.Satiety = Math.Max(0, player.Satiety - 15);
                long stunUntil = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 30_000;
                if (!permissions.Contains(Permission.IgnoreCooldowns))
                    player.StunnedUntil = Math.Max(player.StunnedUntil, stunUntil);
                room.IsCleared = true;
                await _dungeonRepo.UpdateRoomAsync(dungeon.Id, floor.FloorNumber, room);
                message = "A mechanism triggers beneath your foot. Something strikes you. You are winded and hurt. The trap is spent now.";
                break;

            case DungeonRoomType.Rest when !room.IsCleared:
                player.Satiety = Math.Min(100, player.Satiety + 20);
                room.IsCleared = true;
                await _dungeonRepo.UpdateRoomAsync(dungeon.Id, floor.FloorNumber, room);
                message = "A natural alcove, dry and sheltered. You rest briefly. Your hunger eases.";
                break;

            case DungeonRoomType.Shrine when !room.IsCleared:
                var buff = GetShrineBuff();
                BuffService.ApplyBuff(player, buff);
                room.IsCleared = true;
                await _dungeonRepo.UpdateRoomAsync(dungeon.Id, floor.FloorNumber, room);
                message = FormatShrineBuff(buff);
                break;

            case DungeonRoomType.Boss when !room.IsCleared:
                if (!player.IsInCombat)
                {
                    var bossSession = _combatEngine.StartCombat(new CombatStartContext
                    {
                        Player       = player,
                        Trigger      = CombatTrigger.RoomEntry,
                        DungeonTheme = dungeon.DungeonTheme,
                        DungeonFloor = floor.FloorNumber,
                        RoomType     = nameof(DungeonRoomType.Boss),
                    });
                    player.ActiveCombatSessionId = bossSession.Id;
                    await _combatRepo.SaveAsync(bossSession);
                    message = "Something massive stirs in the chamber. It moves toward you.";
                    combatStarted = true;
                }
                break;
        }

        return (message, combatStarted);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void ExitDungeon(Player player)
    {
        player.DungeonInstanceId = null;
        player.DungeonFloor      = 0;
        player.DungeonQ          = 0;
        player.DungeonR          = 0;
    }

    private static List<(int Q, int R)> GetAdjacentRoomCoords(int q, int r, DungeonFloor floor)
    {
        var result = new List<(int, int)>();
        foreach (var (dq, dr) in Directions)
        {
            int nq = q + dq, nr = r + dr;
            if (floor.Rooms.Any(room => room.Q == nq && room.R == nr))
                result.Add((nq, nr));
        }
        return result;
    }

    private static BuffDefinition GetShrineBuff()
    {
        var rng = new Random();
        return rng.Next(3) switch
        {
            0 => new BuffDefinition { Type = BuffType.CooldownReduction,    Magnitude = 0.60, Charges = 16 },
            1 => new BuffDefinition { Type = BuffType.GatherBonus,           Magnitude = 1.50, Charges = 10 },
            _ => new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.60, Charges = 20 },
        };
    }

    private static string FormatShrineBuff(BuffDefinition buff) => buff.Type switch
    {
        BuffType.CooldownReduction    => "The shrine pulses once. Something quickens in you. Your movements feel lighter for a while.",
        BuffType.GatherBonus          => "The shrine hums and then goes still. Your hands feel more capable. Gathering will go better.",
        BuffType.HungerDrainReduction => "The shrine offers something warm, though it has no fire. Hunger seems further away.",
        _                             => "The shrine does something. You're not certain what.",
    };

    private static string GetKeyName(string keyId) => keyId switch
    {
        "IronKey"    => "Iron Key",
        "FrostKey"   => "Frost Key",
        "AncientKey" => "Ancient Key",
        "WoodKey"    => "Wood Key",
        _            => "key",
    };

    private static string EnterMessage(string theme) => theme switch
    {
        "CrystalCavern"  => "You descend into the earth. The walls glitter faintly. The air is cold and very still.",
        "FrozenVault"    => "The entrance is sealed with cold air that hits you all at once. Below: dark, blue, old.",
        "AncientTomb"    => "The air inside is different from outside — drier, older, as if it has been sealed and waiting.",
        "RootLabyrinth"  => "The roots close above you as you descend. The smell is of wet earth and something older.",
        _                => "You step into the dark. The entrance closes above you. Not sealed — just less present than it was.",
    };
}

// ── Supporting result type ────────────────────────────────────────────────────

public class DungeonMoveResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RoomMessage { get; set; }
    public List<(int Q, int R)>? RevealedRooms { get; set; }
    public int NewFloor { get; set; }
    public int NewQ { get; set; }
    public int NewR { get; set; }
    public bool ExitedToSurface { get; set; }
    public bool CombatStarted { get; set; }

    public static DungeonMoveResult Fail(string msg) => new() { Success = false, ErrorMessage = msg };
}
