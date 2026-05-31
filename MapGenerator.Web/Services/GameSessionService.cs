using MapGenerator.Application.Services;
using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MapGenerator.Fishing.Interfaces;

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
    private readonly IConsumableDefinitionProvider _consumableProvider;
    private readonly IEquipmentDefinitionProvider _equipmentProvider;
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly ITileNoteRepository _noteRepo;
    private readonly IMapRepository _mapRepo;
    private readonly IRoadRepository _roadRepo;
    private readonly DungeonService _dungeonSvc;
    private readonly IDungeonRepository _dungeonRepo;
    private readonly ICombatEngine _combatEngine;
    private readonly ICombatRepository _combatRepo;
    private readonly ITileInventoryRepository _tileInventoryRepo;
    private readonly ICompanionRepository _companionRepo;
    private readonly ICompanionDefinitionProvider _companionDefProvider;
    private readonly ICompanionMoveProvider _companionMoveProvider;
    private readonly CompanionBattleService _battleSvc;
    private readonly CompanionService _companionSvc;
    private readonly IFishDefinitionProvider _fishDefProvider;

    public Player? Player { get; private set; }
    public bool IsLoaded { get; private set; }
    public bool IsStunned => Player != null && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < Player.StunnedUntil;
    public PlayerCompanion? Companion { get; private set; }

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
        IConsumableDefinitionProvider consumableProvider,
        IEquipmentDefinitionProvider equipmentProvider,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        ITileNoteRepository noteRepo,
        IMapRepository mapRepo,
        IRoadRepository roadRepo,
        DungeonService dungeonSvc,
        IDungeonRepository dungeonRepo,
        ICombatEngine combatEngine,
        ICombatRepository combatRepo,
        ITileInventoryRepository tileInventoryRepo,
        ICompanionRepository companionRepo,
        ICompanionDefinitionProvider companionDefProvider,
        ICompanionMoveProvider companionMoveProvider,
        CompanionBattleService battleSvc,
        CompanionService companionSvc,
        IFishDefinitionProvider fishDefProvider)
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
        _consumableProvider = consumableProvider;
        _equipmentProvider  = equipmentProvider;
        _playerRepo      = playerRepo;
        _visitRepo       = visitRepo;
        _noteRepo        = noteRepo;
        _mapRepo         = mapRepo;
        _roadRepo        = roadRepo;
        _dungeonSvc      = dungeonSvc;
        _dungeonRepo     = dungeonRepo;
        _combatEngine      = combatEngine;
        _combatRepo        = combatRepo;
        _tileInventoryRepo = tileInventoryRepo;
        _companionRepo        = companionRepo;
        _companionDefProvider = companionDefProvider;
        _companionMoveProvider = companionMoveProvider;
        _battleSvc            = battleSvc;
        _companionSvc         = companionSvc;
        _fishDefProvider      = fishDefProvider;
    }

    public async Task InitAsync(string browserId)
    {
        Player = await _playerSvc.RestorePlayerAsync(browserId);
        if (Player != null)
        {
            if (Player.SpritePixels.Length == 0)
            {
                Player.SpritePixels = GenerateDefaultSprite(Player.Color);
                await _playerRepo.UpdateAsync(Player);
            }
            _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color, Player.SpritePixels, Player.EggsDestroyed, Player.CompanionId != null);

            if (Player.CompanionId != null)
                await LoadCompanionAsync(Player.CompanionId);
        }
        IsLoaded = true;
    }

    private async Task LoadCompanionAsync(string companionId)
    {
        Companion = await _companionRepo.GetByIdAsync(companionId);
    }

    public async Task<(bool ok, string? error)> CreatePlayerAsync(string username, string browserId)
    {
        var (player, error) = await _playerSvc.CreatePlayerAsync(username, browserId);
        if (player == null) return (false, error);
        Player = player;
        if (Player.SpritePixels.Length == 0)
        {
            Player.SpritePixels = GenerateDefaultSprite(Player.Color);
            await _playerRepo.UpdateAsync(Player);
        }
        _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color, Player.SpritePixels, Player.EggsDestroyed);
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
            await RespawnAsync(oldQ, oldR);
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
        _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color, Player.SpritePixels, Player.EggsDestroyed);
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

    public async Task UpdateSpriteAsync(string[] pixels)
    {
        if (Player == null) return;
        Player.SpritePixels = pixels;
        await _playerRepo.UpdateAsync(Player);
        _broadcast.UpdatePlayerSprite(Player.Id, pixels);
    }

    public static string[] GenerateDefaultSprite(string color)
    {
        string filled = color.Length == 7 ? color + "ff" : color;
        var skinColors = new List<string>(["#ffd5adff", "#baec8bff", "#743800ff", "#be5600ff", "#ffffffff", "#fd50508f"]);
        string skinFilled = skinColors[Random.Shared.Next(skinColors.Count)];
        var pixels = new string[256];
        var blackPixels = new HashSet<int>([5,6,7,8,9,10,20,27,67,70,73,76,83,86,89,92,99,108,116,123,131,133,134,135,136,137,138,140,146,157,162,165,170,173,179,180,187,188,196,203,212,215,216,219,229,230,233,234]);
        var filledPixels = new HashSet<int>([35,36,37,38,39,40,41,42,43,44,46,51,52,53,54,55,56,57,58,59,60,61,78]);
        var skinPixels = new HashSet<int>([21,22,23,24,25,26,68,69,71,72,74,75,84,85,87,88,90,91,100,101,102,103,104,105,106,107,117,118,119,120,121,122,132,139,147,148,149,150,151,152,153,154,155,156,163,164,166,167,168,169,171,172,181,182,183,184,185,186,197,198,199,200,201,202,213,214,217,218]);
        for (int i = 0; i < 256; i++) {
            pixels[i] = "";
            if (blackPixels.Contains(i))
                pixels[i] = "#000000ff";
            else if (filledPixels.Contains(i))
                pixels[i] = filled;
            else if (skinPixels.Contains(i))
                pixels[i] = skinFilled;
        }
        return pixels;
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

        var (flavor, notes, beacon, reveal) = await _investigateSvc.InvestigateAsync(Player);
        return (flavor, notes, beacon, reveal);
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
        {
            Player.GatherCooldownUntil = surfaceResult.CooldownUntil;

            if (Companion != null)
            {
                var (bonusText, itemId) = _companionSvc.RollGatherBonus(Companion, Random.Shared);
                if (bonusText != null)
                {
                    surfaceResult.CompanionBonusMessage = bonusText;
                    if (itemId != null)
                    {
                        Player.Inventory.TryGetValue(itemId, out int qty);
                        Player.Inventory[itemId] = qty + 1;
                        surfaceResult.CompanionBonusMessage += $" (Found: {itemId})";
                        Player.LastSeen = DateTime.UtcNow;
                        await _playerRepo.UpdateAsync(Player);
                    }
                }
            }
        }
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

    public bool TileHasEgg()
    {
        if (Player == null || Player.IsInDungeon) return false;
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        return tile != null && tile.EggCount > 0;
    }

    // ── Companion ──────────────────────────────────────────────────────────────

    public async Task<(bool success, string message)> HatchCompanionAsync()
    {
        if (Player == null) return (false, "Not logged in.");
        if (IsStunned) return (false, "You are stunned and cannot act.");
        if (Player.CompanionId != null) return (false, "You already have a companion. Release it first.");

        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile == null || tile.EggCount <= 0) return (false, "There are no eggs here to hatch.");

        var allDefs = _companionDefProvider.GetAll();
        if (allDefs.Count == 0) return (false, "No companion types available.");
        var def = allDefs[Random.Shared.Next(allDefs.Count)];

        var rng = Random.Shared;
        var eligibleMoves = _companionMoveProvider.GetEligibleFor(def.ElementTypes);
        string? attackId = eligibleMoves.Where(m => m.HasDamage && !m.HasEnemyEffect)
                                        .OrderBy(_ => rng.Next()).Select(m => m.Id).FirstOrDefault();
        string? buffId   = eligibleMoves.Where(m => (m.HasSelfBuff || m.HasHeal) && !m.HasDamage)
                                        .OrderBy(_ => rng.Next()).Select(m => m.Id).FirstOrDefault();
        string? debuffId = eligibleMoves.Where(m => m.HasEnemyEffect && !m.HasDamage)
                                        .OrderBy(_ => rng.Next()).Select(m => m.Id).FirstOrDefault();
        var selectedMoves = new[] { attackId, buffId, debuffId }.Where(id => id != null).Select(id => id!).ToList();

        var temperaments = Enum.GetValues<Domain.Enums.CompanionTemperament>();
        var companion = new PlayerCompanion
        {
            Id           = Guid.NewGuid().ToString("N"),
            PlayerId     = Player.Id,
            DefinitionId = def.Id,
            Nickname     = def.Name,
            MoveIds      = selectedMoves,
            SpritePixels = (string[])def.DefaultSprite.Clone(),
            ElementTypes = [.. def.ElementTypes],
            Description  = def.Description,
            Stats        = new Dictionary<string, int>
            {
                ["ATK"] = def.BaseAttack,
                ["VIT"] = def.BaseVitality,
                ["DEF"] = def.BaseDefense,
                ["SPD"] = def.BaseSpeed,
                ["FOC"] = def.BaseFocus,
                ["RES"] = def.BaseResist,
            },
            Temperament  = temperaments[rng.Next(temperaments.Length)],
        };

        await _companionRepo.SaveAsync(companion);
        Player.CompanionId = companion.Id;
        Player.LastSeen    = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);

        var newEggCount = await _mapRepo.DecrementEggCountAsync(tile.Q, tile.R);
        _mapCache.UpdateCachedEggCount(tile.Q, tile.R, newEggCount);

        Companion = companion;
        _broadcast.UpdatePlayerCompanionStatus(Player.Id, true);

        return (true, $"The egg stirs and cracks open. A {def.Name} emerges, blinks at you once, and decides to follow.");
    }

    public async Task UpdateCompanionSpriteAsync(string[] pixels)
    {
        if (Companion == null) return;
        Companion.SpritePixels = pixels;
        await _companionRepo.SaveAsync(Companion);
    }

    public async Task UpdateCompanionNicknameAsync(string nickname)
    {
        if (Companion == null) return;
        Companion.Nickname = nickname.Trim();
        await _companionRepo.SaveAsync(Companion);
    }

    public async Task<string?> UpdateCompanionMovesAsync(List<string> moveIds)
    {
        if (Companion == null) return "No companion.";
        int maxMoves = FormRegistry.Get(Companion.Form).MaxMoves;
        if (moveIds.Count > maxMoves) return $"This companion can have at most {maxMoves} moves.";
        var eligible = _companionMoveProvider.GetEligibleFor(Companion.ElementTypes).Select(m => m.Id).ToHashSet();
        foreach (var id in moveIds)
        {
            if (!eligible.Contains(id)) return $"Move '{id}' is not eligible for this companion.";
        }
        Companion.MoveIds = moveIds;
        await _companionRepo.SaveAsync(Companion);
        return null;
    }

    // Helper: build the battle stats record for the current player's companion
    private CompanionBattleStats BuildBattleStats()
    {
        var c = Companion!;
        return new CompanionBattleStats(
            c.Stats.GetValueOrDefault("ATK"),
            c.Stats.GetValueOrDefault("VIT"), c.Stats.GetValueOrDefault("DEF"),
            c.Stats.GetValueOrDefault("SPD"), c.Stats.GetValueOrDefault("FOC"),
            c.Stats.GetValueOrDefault("RES"),
            c.Temperament, c.Form, c.ElementTypes, [.. c.MoveIds]);
    }

    public (bool ok, string? error) ChallengeCompanionBattle(string targetId, string targetName)
    {
        if (Player == null) return (false, "Not logged in.");
        if (Companion == null) return (false, "You don't have a companion.");
        if (targetId == Player.Id) return (false, "You can't challenge yourself.");

        var challenge = new CompanionBattleChallenge(
            Player.Id, Player.Username, targetId,
            Companion.Nickname, Companion.SpritePixels,
            BuildBattleStats(),
            DateTimeOffset.UtcNow.AddSeconds(30));

        var (ok, error) = _battleSvc.TryCreateChallenge(challenge);
        if (!ok) return (false, error);

        _broadcast.NotifyCompanionChallengeReceived(targetId, challenge);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> AcceptCompanionBattleAsync(string challengerId)
    {
        if (Player == null) return (false, "Not logged in.");
        if (Companion == null) return (false, "You don't have a companion.");

        var challenge = _battleSvc.TakeChallenge(Player.Id);
        if (challenge == null) return (false, "Challenge has expired.");
        if (challenge.ChallengerId != challengerId) return (false, "Challenge mismatch.");

        var result = _battleSvc.SimulateBattle(
            challenge,
            Player.Id, Player.Username, Companion.Nickname, Companion.SpritePixels,
            BuildBattleStats());

        bool won = result.WinnerId == Player.Id;
        bool draw = result.WinnerId == null;
        if (won) Companion.BattleWins++;
        else if (!draw) Companion.BattleLosses++;
        await _companionRepo.SaveAsync(Companion);

        _broadcast.NotifyCompanionBattleCompleted(challenge.ChallengerId, result);
        _broadcast.NotifyCompanionBattleCompleted(Player.Id, result);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> UseElementalCoreAsync(DamageType newElement)
    {
        if (Player == null) return (false, "Not logged in.");
        if (Companion == null) return (false, "You don't have a companion.");
        Player.Inventory.TryGetValue("ElementalCore", out int coreQty);
        if (coreQty <= 0) return (false, "You don't have an Elemental Core.");

        var err = await _companionSvc.UseElementalCoreAsync(Companion, newElement);
        if (err != null) return (false, err);

        if (coreQty == 1) Player.Inventory.Remove("ElementalCore");
        else Player.Inventory["ElementalCore"] = coreQty - 1;
        Player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);
        return (true, $"{Companion.Nickname} now has the {newElement} element!");
    }

    public async Task<(bool ok, string? error)> UseEvolutionStoneAsync()
    {
        if (Player == null) return (false, "Not logged in.");
        if (Companion == null) return (false, "You don't have a companion.");
        Player.Inventory.TryGetValue("EvolutionStone", out int stoneQty);
        if (stoneQty <= 0) return (false, "You don't have an Evolution Stone.");

        var err = await _companionSvc.UseEvolutionStoneAsync(Companion);
        if (err != null) return (false, err);

        if (stoneQty == 1) Player.Inventory.Remove("EvolutionStone");
        else Player.Inventory["EvolutionStone"] = stoneQty - 1;
        Player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);
        return (true, $"{Companion.Nickname} became the {Companion.Form} form!");
    }

    public void DeclineCompanionBattle()
    {
        if (Player == null) return;
        var challenge = _battleSvc.TakeChallenge(Player.Id);
        if (challenge != null)
            _broadcast.NotifyCompanionChallengeDeclined(challenge.ChallengerId);
    }

    public async Task<(bool success, string message)> ReleaseCompanionAsync()
    {
        if (Player == null) return (false, "Not logged in.");
        if (Player.CompanionId == null) return (false, "You don't have a companion.");

        string name = Companion?.Nickname ?? "companion";
        await _companionRepo.DeleteAsync(Player.CompanionId);
        Player.CompanionId = null;
        Player.LastSeen    = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(Player);

        Companion = null;
        _broadcast.UpdatePlayerCompanionStatus(Player.Id, false);

        return (true, $"You bid farewell. Your {name} wanders off into the world.");
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

    public async Task<(bool success, string message)> UseAsync(string itemId)
    {
        if (Player == null) return (false, "Not logged in.");

        // Check consumable definition first
        var def = _consumableProvider.GetById(itemId);
        if (def != null && def.SatietyRestore > 0)
        {
            Player.Inventory.TryGetValue(itemId, out int qty);
            if (qty <= 0) return (false, $"You don't have any {def.Name}.");
            if (qty == 1) Player.Inventory.Remove(itemId);
            else Player.Inventory[itemId] = qty - 1;

            Player.Satiety = Math.Min(100, Player.Satiety + def.SatietyRestore);
            if (def.Buff != null)
                BuffService.ApplyBuff(Player, def.Buff);
            Player.LastSeen = DateTime.UtcNow;
            await _playerRepo.UpdateAsync(Player);

            var rng = new Random();
            var message = def.UseMessages.Length > 0
                ? def.UseMessages[rng.Next(def.UseMessages.Length)]
                : $"You eat the {def.Name}.";
            return (true, message);
        }

        // Raw fish fallback — any caught fish is edible raw for modest satiety
        var fishDef = _fishDefProvider.GetById(itemId);
        if (fishDef != null && !fishDef.IsChainMaterial)
        {
            int qty = Player.Inventory.GetValueOrDefault(itemId);
            if (qty <= 0) return (false, $"You don't have any {fishDef.Name}.");
            if (qty == 1) Player.Inventory.Remove(itemId);
            else Player.Inventory[itemId] = qty - 1;

            double satiety = fishDef.RarityWeight switch
            {
                >= 2.0f => 12,
                >= 1.0f => 18,
                _       => 25,
            };
            Player.Satiety = Math.Min(100, Player.Satiety + satiety);
            Player.LastSeen = DateTime.UtcNow;
            await _playerRepo.UpdateAsync(Player);
            return (true, $"You eat the {fishDef.Name} raw. It is not good. It is sufficient.");
        }

        return (false, "You cannot eat that.");
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
        return tile?.FeatureId != null
               && DungeonGenerationService.DungeonEntranceIds.Contains(tile.FeatureId);
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
        var session = await _combatRepo.GetByIdAsync(Player.ActiveCombatSessionId);
        if (session == null) return null;

        // Session was already finished but player exited before confirming end screen — clean it up.
        if (_combatEngine.IsFinished(session))
        {
            await ApplyCombatResultAsync(session);
            return null;
        }

        if (Companion != null && !session.CompanionPresent)
        {
            session.CompanionPresent       = true;
            session.CompanionDefinitionId  = Companion.DefinitionId;
            session.CompanionMoveIds       = [.. Companion.MoveIds];
            session.CompanionBaseAttack    = Companion.Stats.GetValueOrDefault("ATK");
            session.CompanionBaseSpeed     = Companion.Stats.GetValueOrDefault("SPD");
            session.CompanionName          = Companion.Nickname;
            await _combatRepo.SaveAsync(session);
        }

        return session;
    }

    public async Task<CombatTurnResult?> ProcessCombatTurnAsync(CombatAction action)
    {
        if (Player?.ActiveCombatSessionId == null) return null;
        var session = await _combatRepo.GetByIdAsync(Player.ActiveCombatSessionId);
        if (session == null) return null;

        var result = _combatEngine.ProcessTurn(session, Player, action);
        await _combatRepo.SaveAsync(result.Session);
        return result;
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

    // Shared respawn logic — used by combat death and drowning.
    // Drops all items at (dropQ, dropR), resets player to spawn, saves, and broadcasts move.
    public async Task RespawnAsync(int dropQ, int dropR)
    {
        if (Player == null) return;

        foreach (var (itemId, qty) in Player.Inventory)
            await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, itemId, qty);
        foreach (var (itemId, qty) in Player.CraftedItems)
            await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, itemId, qty);

        if (Player.EquippedWeaponId != null)
        {
            await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, Player.EquippedWeaponId, 1);
            Player.EquippedWeaponId = null;
        }
        if (Player.EquippedArmorId != null)
        {
            await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, Player.EquippedArmorId, 1);
            Player.EquippedArmorId = null;
        }
        if (Player.EquippedLureId != null)
        {
            await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, Player.EquippedLureId, 1);
            Player.EquippedLureId = null;
        }
        if (Player.EquippedHatId != null)
        {
            await _tileInventoryRepo.AddItemsAsync(dropQ, dropR, Player.EquippedHatId, 1);
            Player.EquippedHatId = null;
        }

        int oldQ = Player.Q, oldR = Player.R;
        var config  = _mapCache.GetCachedConfig();
        Player.Q    = config?.SpawnQ ?? 0;
        Player.R    = config?.SpawnR ?? 0;

        Player.DungeonInstanceId = null;
        Player.DungeonFloor      = 0;
        Player.DungeonQ          = 0;
        Player.DungeonR          = 0;
        Player.Satiety           = 50;
        Player.Inventory.Clear();
        Player.CraftedItems.Clear();
        Player.ActiveBuffs.Clear();
        Player.CurrentHp         = Player.MaxHp;
        Player.CurrentStamina    = Player.MaxStamina;
        Player.DeathCount++;

        await _playerRepo.UpdateAsync(Player);
        await _broadcast.NotifyPlayerMovedAsync(Player.Id, Player.Username, oldQ, oldR, Player.Q, Player.R);
    }

    private async Task ApplyCombatResultAsync(CombatSession session)
    {
        if (Player == null) return;
        var result = _combatEngine.Resolve(session);

        Player.CurrentHp      = result.HpRemaining;
        Player.CurrentStamina = result.StaminaRemaining;
        Player.CurrentMana    = result.ManaRemaining;

        if (result.PlayerDied)
        {
            if (Player.IsAdmin)
            {
                Player.DungeonInstanceId = null;
                Player.DungeonFloor      = 0;
                Player.DungeonQ          = 0;
                Player.DungeonR          = 0;
                Player.CurrentHp         = Player.MaxHp;
                Player.CurrentStamina    = Player.MaxStamina;
                Player.ActiveCombatSessionId = null;
                await _playerRepo.UpdateAsync(Player);
            }
            else
            {
                int dropQ = Player.DungeonInstanceId != null ? Player.DungeonQ : Player.Q;
                int dropR = Player.DungeonInstanceId != null ? Player.DungeonR : Player.R;
                Player.ActiveCombatSessionId = null;
                await RespawnAsync(dropQ, dropR);
            }
        }
        else
        {
            foreach (var (id, qty) in result.LootGained)
                Player.Inventory[id] = Player.Inventory.GetValueOrDefault(id) + qty;
            foreach (var (defId, count) in result.EnemiesDefeated)
                Player.EnemiesDefeated[defId] = Player.EnemiesDefeated.GetValueOrDefault(defId) + count;

            // Apply XP and level-up
            if (result.XpGained > 0)
            {
                Player.Experience += result.XpGained;
                int levelUps = 0;
                while (Player.Level < 20)
                {
                    int xpRequired = Player.Level * 60;
                    if (Player.Experience < xpRequired) break;
                    Player.Experience -= xpRequired;
                    Player.Level++;
                    levelUps++;
                    Player.CurrentHp = Player.MaxHp;
                    Player.CurrentMana = Player.MaxMana;
                }
                if (levelUps > 0)
                {
                    Player.UnspentStatPoints += levelUps * 3;
                    result.LeveledUp      = true;
                    result.NewLevel       = Player.Level;
                    result.StatPointsGained = levelUps * 3;
                }
            }

            Player.ActiveCombatSessionId = null;
            await _playerRepo.UpdateAsync(Player);
        }

        await _combatRepo.DeleteAsync(session.Id);
    }

    // ── Stat Point Allocation ────────────────────────────────────────────────

    private static readonly string[] TieredStats = ["BaseAttack", "BaseDefense", "BaseResistance", "BaseMagic", "BaseSpeed", "BaseCritChance"];

    public static int GetStatPointCost(string stat, int currentPurchases) => stat switch
    {
        "MaxHp" => 1,
        _ when TieredStats.Contains(stat) => currentPurchases < 5 ? 1 : currentPurchases < 10 ? 2 : 3,
        _ => 99,
    };

    public async Task<string?> AllocateStatPointAsync(string stat)
    {
        if (Player == null) return "Not logged in.";
        if (Player.UnspentStatPoints <= 0) return "No unspent stat points.";

        int purchases = Player.StatPurchases.GetValueOrDefault(stat);
        int cost = GetStatPointCost(stat, purchases);
        if (Player.UnspentStatPoints < cost) return $"Not enough points. {stat} costs {cost} at this level.";

        Player.UnspentStatPoints -= cost;
        Player.StatPurchases[stat] = purchases + 1;

        switch (stat)
        {
            case "MaxHp":             Player.MaxHp += 2; break;
            case "BaseAttack":        Player.BaseAttack++;        break;
            case "BaseDefense":       Player.BaseDefense++;       break;
            case "BaseResistance":    Player.BaseResistance++;    break;
            case "BaseMagic":         Player.BaseMagic++;         break;
            case "BaseSpeed":         Player.BaseSpeed++;         break;
            case "BaseCritChance":    Player.BaseCritChance += 0.01f; break;
            default: return "Unknown stat.";
        }

        await _playerRepo.UpdateAsync(Player);
        return null;
    }

    // ── Equipment ─────────────────────────────────────────────────────────────

    public async Task<string?> EquipItemAsync(string itemId)
    {
        if (Player == null) return "Not logged in.";
        var def = _equipmentProvider.GetById(itemId);
        if (def == null) return "That item cannot be equipped.";

        int qty = Player.Inventory.GetValueOrDefault(itemId)
                + Player.CraftedItems.GetValueOrDefault(itemId);
        if (qty <= 0) return "You don't have that item.";

        switch (def.EquipmentSlot)
        {
            case "Weapon": Player.EquippedWeaponId = itemId; break;
            case "Armor":  Player.EquippedArmorId  = itemId; break;
            case "Hat":    Player.EquippedHatId     = itemId; break;
            case "Lure":   Player.EquippedLureId    = itemId; break;
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
            case "Lure":   Player.EquippedLureId   = null; break;
            default: return "Unknown slot.";
        }
        await _playerRepo.UpdateAsync(Player);
        return null;
    }

    // ── Challenge rewards ─────────────────────────────────────────────────────

    public async Task GrantChallengeRewardsAsync(IEnumerable<(string ItemId, int Quantity)> rewards)
    {
        foreach (var (id, qty) in rewards)
            Player!.Inventory[id] = Player.Inventory.GetValueOrDefault(id) + qty;
        await _playerRepo.UpdateAsync(Player!);
    }

    public async Task<bool> RecordFishCatchAsync(string fishId, double weightKg)
    {
        Player!.Inventory[fishId] = Player.Inventory.GetValueOrDefault(fishId) + 1;
        bool newRecord = false;
        if (Player.FishLog.TryGetValue(fishId, out var entry))
        {
            entry.TotalCaught++;
            if (weightKg > entry.PersonalBestWeightKg)
            {
                entry.PersonalBestWeightKg = weightKg;
                newRecord = true;
            }
        }
        else
        {
            Player.FishLog[fishId] = new() { FirstCaughtAt = DateTime.UtcNow, TotalCaught = 1, PersonalBestWeightKg = weightKg };
            newRecord = true;
        }
        Player!.FishingStreak++;

        // Consume one charge from any active fishing food buffs
        BuffService.ConsumeFishingRarityCharge(Player!);
        BuffService.ConsumeFishingStrikeCharge(Player!);

        // Byproduct drops tied to fish rarity
        var fishDef = _fishDefProvider.GetById(fishId);
        if (fishDef != null && !fishDef.IsChainMaterial)
        {
            string? drop = fishDef.RarityWeight switch
            {
                >= 2.0f => RollChance(0.35) ? "FishScale" : null,
                >= 1.0f => RollChance(0.25) ? "FishFin" : null,
                _       => RollChance(0.30) ? "FishFin" : null,
            };
            if (drop != null)
                Player.Inventory[drop] = Player.Inventory.GetValueOrDefault(drop) + 1;
            if ((fishDef.FishCategory == "Saltwater" || fishDef.FishCategory == "Deep") && RollChance(0.20))
                Player.Inventory["BrineCrystal"] = Player.Inventory.GetValueOrDefault("BrineCrystal") + 1;
        }

        await _playerRepo.UpdateAsync(Player!);
        return newRecord;
    }

    private static readonly Random _rng = new();
    private static bool RollChance(double probability) => _rng.NextDouble() < probability;

    public async Task ResetFishingStreakAsync()
    {
        if (Player == null || Player.FishingStreak == 0) return;
        Player.FishingStreak = 0;
        await _playerRepo.UpdateAsync(Player);
    }

    public async Task<Dictionary<string, (string PlayerName, double WeightKg)>> GetFishWorldRecordsAsync()
    {
        var players = await _playerRepo.GetAllAsync();
        var result  = new Dictionary<string, (string PlayerName, double WeightKg)>();
        foreach (var player in players)
        {
            foreach (var (fishId, entry) in player.FishLog)
            {
                if (!result.TryGetValue(fishId, out var current) || entry.PersonalBestWeightKg > current.WeightKg)
                    result[fishId] = (player.Username, entry.PersonalBestWeightKg);
            }
        }
        return result;
    }

    // ── Dispose ───────────────────────────────────────────────────────────────

    public ValueTask DisposeAsync()
    {
        if (Player != null)
            _broadcast.PlayerWentOffline(Player.Id);
        return ValueTask.CompletedTask;
    }
}
