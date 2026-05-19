using MapGenerator.Application.Services;
using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Web.Services;

public class GameSessionService : IAsyncDisposable
{
    private readonly PlayerService _playerSvc;
    private readonly ChatService _chatSvc;
    private readonly MovementService _movementSvc;
    private readonly EggService _eggSvc;
    private readonly DanceService _danceSvc;
    private readonly KissService _kissSvc;
    private readonly InvestigateService _investigateSvc;
    private readonly GatherService _gatherSvc;
    private readonly CraftingService _craftingSvc;
    private readonly StructureService _structureSvc;
    private readonly EggExplosionService _explosionSvc;
    private readonly PermissionService _permissionSvc;
    private readonly MapGeneratorService _mapCache;
    private readonly GameBroadcastService _broadcast;
    private readonly SettlementCacheService _settlementCache;
    private readonly IFoodDefinitionProvider _foodProvider;
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly ITileNoteRepository _noteRepo;
    private readonly IMapRepository _mapRepo;
    private readonly IRoadRepository _roadRepo;
    private readonly DungeonService _dungeonSvc;
    private readonly IDungeonRepository _dungeonRepo;
    private readonly ICombatEngine _combatEngine;
    private readonly ICombatRepository _combatRepo;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly ITileInventoryRepository _tileInventoryRepo;

    public Player? Player { get; private set; }
    public bool IsLoaded { get; private set; }
    public bool IsStunned => Player != null && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < Player.StunnedUntil;

    public GameSessionService(
        PlayerService playerSvc,
        ChatService chatSvc,
        MovementService movementSvc,
        EggService eggSvc,
        DanceService danceSvc,
        KissService kissSvc,
        InvestigateService investigateSvc,
        GatherService gatherSvc,
        CraftingService craftingSvc,
        StructureService structureSvc,
        EggExplosionService explosionSvc,
        PermissionService permissionSvc,
        MapGeneratorService mapCache,
        GameBroadcastService broadcast,
        SettlementCacheService settlementCache,
        IFoodDefinitionProvider foodProvider,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        ITileNoteRepository noteRepo,
        IMapRepository mapRepo,
        IRoadRepository roadRepo,
        DungeonService dungeonSvc,
        IDungeonRepository dungeonRepo,
        ICombatEngine combatEngine,
        ICombatRepository combatRepo,
        IResourceDefinitionProvider resourceProvider,
        ITileInventoryRepository tileInventoryRepo)
    {
        _playerSvc       = playerSvc;
        _chatSvc         = chatSvc;
        _movementSvc     = movementSvc;
        _eggSvc          = eggSvc;
        _danceSvc        = danceSvc;
        _kissSvc         = kissSvc;
        _investigateSvc  = investigateSvc;
        _gatherSvc       = gatherSvc;
        _craftingSvc     = craftingSvc;
        _structureSvc    = structureSvc;
        _explosionSvc    = explosionSvc;
        _permissionSvc   = permissionSvc;
        _mapCache        = mapCache;
        _broadcast       = broadcast;
        _settlementCache = settlementCache;
        _foodProvider    = foodProvider;
        _playerRepo      = playerRepo;
        _visitRepo       = visitRepo;
        _noteRepo        = noteRepo;
        _mapRepo         = mapRepo;
        _roadRepo        = roadRepo;
        _dungeonSvc      = dungeonSvc;
        _dungeonRepo     = dungeonRepo;
        _combatEngine       = combatEngine;
        _combatRepo         = combatRepo;
        _resourceProvider   = resourceProvider;
        _tileInventoryRepo  = tileInventoryRepo;
    }

    public async Task InitAsync(string browserId)
    {
        Player = await _playerSvc.RestorePlayerAsync(browserId);
        if (Player != null)
            _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color, Player.EggsDestroyed);
        IsLoaded = true;
    }

    public async Task<(bool ok, string? error)> CreatePlayerAsync(string username, string browserId)
    {
        var (player, error) = await _playerSvc.CreatePlayerAsync(username, browserId);
        if (player == null) return (false, error);
        Player = player;
        _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color, Player.EggsDestroyed);
        return (true, null);
    }

    public async Task<MovementResult> MoveAsync(int targetQ, int targetR, bool oceanConfirmed = false)
    {
        if (Player == null) return new MovementResult { ErrorMessage = "Not logged in." };
        if (IsStunned) return new MovementResult { ErrorMessage = "You are stunned and cannot move." };

        int oldQ = Player.Q, oldR = Player.R;
        var permissions = _permissionSvc.GetPermissions(Player);
        var result = await _movementSvc.TryMoveAsync(Player, permissions, targetQ, targetR, oceanConfirmed);

        if (result.PlayerDrowned)
        {
            _broadcast.PlayerWentOffline(Player.Id);
            Player = null;
            return result;
        }

        if (result.Success)
        {
            Player.Q = result.NewQ!.Value;
            Player.R = result.NewR!.Value;
            Player.MovementCooldownUntil = result.CooldownUntil ?? 0;
            await _broadcast.NotifyPlayerMovedAsync(Player.Id, Player.Username, oldQ, oldR, Player.Q, Player.R);
        }

        return result;
    }

    public async Task HandleMapResetAsync()
    {
        if (Player == null) return;
        await _visitRepo.RecordDepartureAsync(Player.Id, Player.Q, Player.R);
        var updated = await _playerRepo.GetByIdAsync(Player.Id);
        if (updated == null) return;
        Player = updated;
        await _visitRepo.RecordArrivalAsync(Player.Id, Player.Q, Player.R);
        _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color, Player.EggsDestroyed);
    }

    public async Task<(bool success, string kisserMsg)> KissAsync(string targetId, string targetName)
    {
        if (Player == null) return (false, "Not logged in.");
        if (IsStunned) return (false, "You are stunned and cannot act.");
        var permissions = _permissionSvc.GetPermissions(Player);
        var result = await _kissSvc.KissAsync(Player, permissions, targetId, targetName);
        if (result.success)
            _broadcast.NotifyPlayerKissed(Player.Id, Player.Username, targetId, targetName, Player.Q, Player.R, result.kisseeMsg, result.observerMsg);
        return (result.success, result.kisserMsg);
    }

    public async Task<(bool success, string? error)> BuildRoadAsync()
    {
        if (Player == null) return (false, "Not logged in.");
        if (IsStunned) return (false, "You are stunned and cannot act.");

        const int woodCost = 3, stoneCost = 2;
        Player.Inventory.TryGetValue("Wood", out int wood);
        Player.Inventory.TryGetValue("Stone", out int stone);
        if (wood < woodCost)  return (false, $"Not enough Wood. Need {woodCost}, have {wood}.");
        if (stone < stoneCost) return (false, $"Not enough Stone. Need {stoneCost}, have {stone}.");

        Player.Inventory["Wood"]  = wood  - woodCost;
        if (Player.Inventory["Wood"]  <= 0) Player.Inventory.Remove("Wood");
        Player.Inventory["Stone"] = stone - stoneCost;
        if (Player.Inventory["Stone"] <= 0) Player.Inventory.Remove("Stone");

        var point = new RoadPoint { Q = Player.Q, R = Player.R };
        var adjacentId = _settlementCache.FindAdjacentRoadId(Player.Q, Player.R);

        if (adjacentId != null)
        {
            await _roadRepo.AppendPointAsync(adjacentId, point);
            _settlementCache.ExtendRoad(adjacentId, Player.Q, Player.R);
        }
        else
        {
            var road = new Road
            {
                FromSettlementId = string.Empty,
                ToSettlementId   = string.Empty,
                Path             = [point],
            };
            await _roadRepo.AddAsync(road);
            _settlementCache.AddRoad(road);
        }

        Player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);
        _broadcast.NotifyRoadsChanged();
        return (true, null);
    }

    public async Task<(bool exploded, string message)> TryExplodeEggsAsync()
    {
        if (Player == null) return (false, string.Empty);
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile == null || tile.EggCount <= 0) return (false, string.Empty);

        var (exploded, message, biomeChanges) = _explosionSvc.TryExplode(Player, tile);
        if (!exploded) return (false, string.Empty);

        foreach (var (q, r, biome) in biomeChanges)
        {
            await _mapRepo.UpdateTileBiomeAndFeatureAsync(q, r, biome, null);
            _mapCache.UpdateCachedBiomeAndFeature(q, r, biome, null);
        }

        var newCount = await _mapRepo.DecrementEggCountAsync(tile.Q, tile.R);
        _mapCache.UpdateCachedEggCount(tile.Q, tile.R, newCount);

        Player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);
        _broadcast.UpdatePlayerEggsDestroyed(Player.Id, Player.EggsDestroyed);
        _broadcast.NotifyEggExploded(Player.Id, Player.Q, Player.R);
        return (true, message);
    }

    public async Task ConsumeGardenProductionAsync()
    {
        if (Player == null) return;
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile?.Structure?.Type == StructureType.Garden)
        {
            await _structureSvc.GenerateGardenProductionAsync(tile);
            _broadcast.NotifyTileInventoryChanged(tile.Q, tile.R);
        }
    }

    public async Task<(bool success, string message, bool eggDestroyed, int newEggCount)> DanceAsync()
    {
        if (Player == null) return (false, "Not logged in.", false, 0);
        if (IsStunned) return (false, "You are stunned and cannot act.", false, 0);
        var permissions = _permissionSvc.GetPermissions(Player);
        var result = await _danceSvc.DanceAsync(Player, permissions);
        if (result.success)
        {
            if (result.eggDestroyed)
                _broadcast.UpdatePlayerEggsDestroyed(Player.Id, Player.EggsDestroyed);
            _broadcast.NotifyPlayerDanced(Player.Id, Player.Username, Player.Q, Player.R);
        }
        return result;
    }

    public async Task<(bool success, string message, int eggCount)> LayEggAsync()
    {
        if (Player == null) return (false, "Not logged in.", 0);
        if (IsStunned) return (false, "You are stunned and cannot act.", 0);
        var permissions = _permissionSvc.GetPermissions(Player);
        var result = await _eggSvc.LayEggAsync(Player, permissions);
        if (result.success)
            _broadcast.NotifyEggLaid(Player.Id, Player.Q, Player.R, result.eggCount);
        return result;
    }

    public async Task UpdateColorAsync(string color)
    {
        if (Player == null) return;
        Player.Color = color;
        await _playerRepo.UpdateAsync(Player);
        _broadcast.UpdatePlayerColor(Player.Id, color);
    }

    public async Task<ChatMessage?> SendLocalMessageAsync(string content)
    {
        if (Player == null) return null;
        var msg = await _chatSvc.SendLocalAsync(Player, content);
        await _broadcast.BroadcastLocalMessageAsync(msg);
        return msg;
    }

    public async Task<ChatMessage?> SendWorldMessageAsync(string content)
    {
        if (Player == null) return null;
        var msg = await _chatSvc.SendWorldAsync(Player, content);
        await _broadcast.BroadcastWorldMessageAsync(msg);
        return msg;
    }

    public Task<List<ChatMessage>> GetTileChatAsync() =>
        Player != null
            ? _chatSvc.GetTileHistoryAsync(Player.Id, Player.Q, Player.R)
            : Task.FromResult(new List<ChatMessage>());

    public Task<List<ChatMessage>> GetWorldChatAsync() => _chatSvc.GetWorldHistoryAsync();

    public async Task<(string flavorText, List<TileNote> notes, string? beaconMessage, HashSet<(int, int)>? beaconReveal)>
        InvestigateAsync()
    {
        if (Player == null) return ("You are not sure who you are.", [], null, null);
        if (IsStunned) return ("You are stunned and cannot focus.", [], null, null);
        if (Player.IsInDungeon)
        {
            var msg = await _dungeonSvc.InvestigateAsync(Player);
            return (msg, [], null, null);
        }
        return await _investigateSvc.InvestigateAsync(Player);
    }

    public async Task<GatherResult> GatherAsync()
    {
        if (Player == null) return GatherResult.Fail("Not logged in.");
        if (IsStunned) return GatherResult.Fail("You are stunned and cannot act.");
        var permissions = _permissionSvc.GetPermissions(Player);
        if (Player.IsInDungeon)
        {
            var result = await _dungeonSvc.GatherAsync(Player, permissions);
            if (result.Success) Player.GatherCooldownUntil = result.CooldownUntil;
            return result;
        }
        var surfaceResult = await _gatherSvc.TryGatherAsync(Player, permissions);
        if (surfaceResult.Success)
            Player.GatherCooldownUntil = surfaceResult.CooldownUntil;
        return surfaceResult;
    }

    public async Task<bool> UsePoulticeAsync()
    {
        if (Player == null) return false;
        if (IsStunned) return false;
        Player.Inventory.TryGetValue("Poultice", out int qty);
        if (qty <= 0) return false;

        if (qty == 1) Player.Inventory.Remove("Poultice");
        else Player.Inventory["Poultice"] = qty - 1;

        Player.MovementCooldownUntil = 0;
        Player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);
        return true;
    }

    public async Task<(bool success, string? error)> BuildStructureAsync(StructureType type)
    {
        if (Player == null) return (false, "Not logged in.");
        if (IsStunned) return (false, "You are stunned and cannot act.");
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile == null) return (false, "Cannot build here.");
        return await _structureSvc.TryBuildAsync(Player, type, tile);
    }

    public async Task<bool> DestroyStructureAsync()
    {
        if (Player == null) return false;
        if (IsStunned) return false;
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile?.Structure == null) return false;
        await _structureSvc.DestroyAsync(tile);
        return true;
    }

    public async Task<CraftingResult> CraftAsync(string recipeId)
    {
        if (Player == null) return new CraftingResult { Success = false, ErrorMessage = "Not logged in." };
        if (IsStunned) return new CraftingResult { Success = false, ErrorMessage = "You are stunned and cannot act." };
        return await _craftingSvc.TryCraftAsync(Player, recipeId);
    }

    public bool TileHasResources()
    {
        if (Player == null) return false;
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        return tile != null && _gatherSvc.TileHasResources(tile);
    }

    public async Task<HashSet<(int, int)>?> LoadRevealedTilesAsync()
    {
        if (Player == null || Player.IsAdmin) return null;
        var coords = await _visitRepo.GetVisitedCoordsAsync(Player.Id);
        var set = new HashSet<(int, int)>();
        foreach (var (q, r) in coords)
        {
            set.Add((q, r));
            foreach (var (dq, dr) in MapGeneratorService.HexNeighborOffsets())
                set.Add((q + dq, r + dr));
        }
        // Always ensure current position is revealed
        set.Add((Player.Q, Player.R));
        foreach (var (dq, dr) in MapGeneratorService.HexNeighborOffsets())
            set.Add((Player.Q + dq, Player.R + dr));
        return set;
    }

    public Task PersistRevealedCoordsAsync(IEnumerable<(int, int)> coords)
        => Player != null ? _visitRepo.RecordRevealedCoordsAsync(Player.Id, coords) : Task.CompletedTask;

    public async Task<(bool success, string? error)> EditTileAsync(BiomeType biome, string? featureId)
    {
        if (Player == null) return (false, "Not logged in.");
        if (IsStunned) return (false, "You are stunned and cannot act.");
        if (!Player.IsAdmin) return (false, "Insufficient permissions.");
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile == null) return (false, "Tile not found.");
        await _mapRepo.UpdateTileBiomeAndFeatureAsync(Player.Q, Player.R, biome, featureId);
        _mapCache.UpdateCachedBiomeAndFeature(Player.Q, Player.R, biome, featureId);
        return (true, null);
    }

    public async Task LeaveNoteAsync(string content)
    {
        if (Player == null || string.IsNullOrWhiteSpace(content) || IsStunned) return;
        content = content.Trim();
        if (content.Length > 200) content = content[..200];
        await _noteRepo.AddNoteAsync(new TileNote
        {
            Q = Player.Q, R = Player.R,
            AuthorId = Player.Id, AuthorName = Player.Username,
            Content = content, CreatedAt = DateTime.UtcNow
        });
    }

    public async Task PlantSignAsync(string content)
    {
        if (Player == null || string.IsNullOrWhiteSpace(content) || IsStunned) return;
        content = content.Trim();
        if (content.Length > 100) content = content[..100];
        await _mapRepo.PlaceSignAsync(Player.Q, Player.R, content, Player.Username);
        _mapCache.UpdateCachedSign(Player.Q, Player.R, content, Player.Username);
    }

    public async Task<(bool success, string message)> EatAsync(string resourceId)
    {
        if (Player == null) return (false, "Not logged in.");
        var foodDef = _foodProvider.GetById(resourceId);
        if (foodDef == null) return (false, "You cannot eat that.");

        Player.Inventory.TryGetValue(resourceId, out int qty);
        if (qty <= 0) return (false, $"You don't have any {foodDef.Name}.");

        if (qty == 1) Player.Inventory.Remove(resourceId);
        else Player.Inventory[resourceId] = qty - 1;

        Player.Satiety = Math.Min(100, Player.Satiety + foodDef.SatietyRestore);
        if (foodDef.Buff != null)
            BuffService.ApplyBuff(Player, foodDef.Buff);
        Player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);

        var rng = new Random();
        var message = foodDef.EatMessages.Length > 0
            ? foodDef.EatMessages[rng.Next(foodDef.EatMessages.Length)]
            : $"You eat the {foodDef.Name}.";
        return (true, message);
    }

    // ── Dungeon ───────────────────────────────────────────────────────────────

    public async Task<(bool success, string message, List<(int Q, int R)> revealedRooms)> EnterDungeonAsync()
    {
        if (Player == null) return (false, "Not logged in.", []);
        if (IsStunned) return (false, "You are stunned and cannot act.", []);
        if (Player.IsInDungeon) return (false, "You are already in a dungeon.", []);
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile?.FeatureId == null) return (false, "There is no dungeon entrance here.", []);
        return await _dungeonSvc.EnterDungeonAsync(Player, tile.FeatureId);
    }

    public async Task<DungeonMoveResult> UseStaircaseAsync()
    {
        if (Player == null) return DungeonMoveResult.Fail("Not logged in.");
        if (IsStunned) return DungeonMoveResult.Fail("You are stunned and cannot move.");
        if (!Player.IsInDungeon) return DungeonMoveResult.Fail("You are not in a dungeon.");
        return await _dungeonSvc.UseStaircaseAsync(Player);
    }

    public async Task<DungeonMoveResult> DungeonMoveAsync(int targetQ, int targetR)
    {
        if (Player == null) return DungeonMoveResult.Fail("Not logged in.");
        if (IsStunned) return DungeonMoveResult.Fail("You are stunned and cannot move.");
        if (!Player.IsInDungeon) return DungeonMoveResult.Fail("You are not in a dungeon.");
        var permissions = _permissionSvc.GetPermissions(Player);
        return await _dungeonSvc.MoveAsync(Player, targetQ, targetR, permissions);
    }

    public async Task<(bool success, string message)> UseRopeAsync()
    {
        if (Player == null) return (false, "Not logged in.");
        if (IsStunned) return (false, "You are stunned and cannot act.");
        return await _dungeonSvc.UseRopeAsync(Player);
    }

    public async Task<DungeonFloor?> GetCurrentDungeonFloorAsync()
    {
        if (Player == null || !Player.IsInDungeon) return null;
        var dungeon = await _dungeonRepo.GetByIdAsync(Player.DungeonInstanceId!);
        return dungeon?.Floors.FirstOrDefault(f => f.FloorNumber == Player.DungeonFloor);
    }

    public bool IsOnDungeonEntrance()
    {
        if (Player == null || Player.IsInDungeon) return false;
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        return tile?.FeatureId is "CaveEntrance" or "IcyCavern" or "FrozenShrine"
               or "AncientRuins" or "RuinedTower" or "RuinedTemple" or "CrumbledFortress"
               or "StoneCircle" or "TreeHollow";
    }

    public bool DungeonTileHasGather()
    {
        if (Player == null || !Player.IsInDungeon) return false;
        return true; // Determined at gather-time by DungeonService
    }

    // ── Combat ────────────────────────────────────────────────────────────────

    public async Task StartDebugCombatAsync()
    {
        if (Player == null || Player.IsInCombat) return;
        var session = _combatEngine.StartCombat(new CombatStartContext
        {
            Player    = Player,
            Trigger   = CombatTrigger.RandomEncounter,
            BiomeType = "Forest",
        });
        Player.ActiveCombatSessionId = session.Id;
        await _combatRepo.SaveAsync(session);
        await _playerRepo.UpdateAsync(Player);
    }

    public async Task<CombatSession?> GetActiveCombatAsync()
    {
        if (Player?.ActiveCombatSessionId == null) return null;
        return await _combatRepo.GetByIdAsync(Player.ActiveCombatSessionId);
    }

    public async Task<CombatSession?> ProcessCombatTurnAsync(CombatAction action)
    {
        if (Player?.ActiveCombatSessionId == null) return null;
        var session = await _combatRepo.GetByIdAsync(Player.ActiveCombatSessionId);
        if (session == null) return null;

        session = _combatEngine.ProcessTurn(session, Player, action);
        await _combatRepo.SaveAsync(session);
        // Result is not applied here — UI shows the end screen first, then calls FinalizeCombatAsync
        return session;
    }

    public async Task<CombatResult?> GetPendingCombatResultAsync()
    {
        if (Player?.ActiveCombatSessionId == null) return null;
        var session = await _combatRepo.GetByIdAsync(Player.ActiveCombatSessionId);
        if (session == null || !_combatEngine.IsFinished(session)) return null;
        return _combatEngine.Resolve(session);
    }

    public async Task FinalizeCombatAsync()
    {
        if (Player?.ActiveCombatSessionId == null) return;
        var session = await _combatRepo.GetByIdAsync(Player.ActiveCombatSessionId);
        if (session == null) return;
        await ApplyCombatResultAsync(session);
    }

    private async Task ApplyCombatResultAsync(CombatSession session)
    {
        if (Player == null) return;
        var result = _combatEngine.Resolve(session);

        Player.CurrentHp    = result.HpRemaining;
        Player.CurrentStamina = result.StaminaRemaining;

        if (result.PlayerDied)
        {
            if (Player.IsAdmin)
            {
                // Admins respawn on the world map without losing anything
                Player.DungeonInstanceId = null;
                Player.DungeonFloor      = 0;
                Player.DungeonQ          = 0;
                Player.DungeonR          = 0;
                Player.CurrentHp         = Player.MaxHp;
                Player.CurrentStamina    = Player.MaxStamina;
            }
            else
            {
                // Drop inventory onto the tile before clearing
                int dropQ = Player.DungeonInstanceId != null ? Player.DungeonQ : Player.Q;
                int dropR = Player.DungeonInstanceId != null ? Player.DungeonR : Player.R;

                foreach (var (itemId, qty) in Player.Inventory)
                    await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, itemId, qty);
                foreach (var (itemId, qty) in Player.CraftedItems)
                    await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, itemId, qty);

                // Same reset as drowning
                Player.Q              = 0;
                Player.R              = 0;
                Player.DungeonInstanceId = null;
                Player.DungeonFloor   = 0;
                Player.DungeonQ       = 0;
                Player.DungeonR       = 0;
                Player.Satiety        = 50;
                Player.Inventory.Clear();
                Player.CraftedItems.Clear();
                Player.CurrentHp      = Player.MaxHp;
                Player.CurrentStamina = Player.MaxStamina;
            }
        }
        else
        {
            // Grant loot
            foreach (var (id, qty) in result.LootGained)
                Player.Inventory[id] = Player.Inventory.GetValueOrDefault(id) + qty;

            // Record enemies defeated
            foreach (var (defId, count) in result.EnemiesDefeated)
                Player.EnemiesDefeated[defId] = Player.EnemiesDefeated.GetValueOrDefault(defId) + count;
        }

        Player.ActiveCombatSessionId = null;
        await _playerRepo.UpdateAsync(Player);
        await _combatRepo.DeleteAsync(session.Id);
    }

    // ── Equipment ─────────────────────────────────────────────────────────────

    public async Task<string?> EquipItemAsync(string itemId)
    {
        if (Player == null) return "Not logged in.";
        var def = _resourceProvider.GetById(itemId);
        if (def?.EquipmentSlot == null) return "That item cannot be equipped.";

        int qty = Player.Inventory.GetValueOrDefault(itemId)
                + Player.CraftedItems.GetValueOrDefault(itemId);
        if (qty <= 0) return "You don't have that item.";

        switch (def.EquipmentSlot)
        {
            case "Weapon": Player.EquippedWeaponId = itemId; break;
            case "Armor":  Player.EquippedArmorId  = itemId; break;
            case "Hat":    Player.EquippedHatId    = itemId; break;
            default: return "Unknown equipment slot.";
        }

        await _playerRepo.UpdateAsync(Player);
        return null;
    }

    public async Task<string?> UnequipItemAsync(string slot)
    {
        if (Player == null) return "Not logged in.";
        switch (slot)
        {
            case "Weapon": Player.EquippedWeaponId = null; break;
            case "Armor":  Player.EquippedArmorId  = null; break;
            case "Hat":    Player.EquippedHatId    = null; break;
            default: return "Unknown slot.";
        }
        await _playerRepo.UpdateAsync(Player);
        return null;
    }

    // ── Dispose ───────────────────────────────────────────────────────────────

    public ValueTask DisposeAsync()
    {
        if (Player != null)
            _broadcast.PlayerWentOffline(Player.Id);
        return ValueTask.CompletedTask;
    }
}
