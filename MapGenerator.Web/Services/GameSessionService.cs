using MapGenerator.Application.Services;
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
    private readonly InvestigateService _investigateSvc;
    private readonly GatherService _gatherSvc;
    private readonly CraftingService _craftingSvc;
    private readonly StructureService _structureSvc;
    private readonly PermissionService _permissionSvc;
    private readonly MapGeneratorService _mapCache;
    private readonly GameBroadcastService _broadcast;
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly ITileNoteRepository _noteRepo;
    private readonly IMapRepository _mapRepo;

    public Player? Player { get; private set; }
    public bool IsLoaded { get; private set; }

    public GameSessionService(
        PlayerService playerSvc,
        ChatService chatSvc,
        MovementService movementSvc,
        EggService eggSvc,
        InvestigateService investigateSvc,
        GatherService gatherSvc,
        CraftingService craftingSvc,
        StructureService structureSvc,
        PermissionService permissionSvc,
        MapGeneratorService mapCache,
        GameBroadcastService broadcast,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        ITileNoteRepository noteRepo,
        IMapRepository mapRepo)
    {
        _playerSvc      = playerSvc;
        _chatSvc        = chatSvc;
        _movementSvc    = movementSvc;
        _eggSvc         = eggSvc;
        _investigateSvc = investigateSvc;
        _gatherSvc      = gatherSvc;
        _craftingSvc    = craftingSvc;
        _structureSvc   = structureSvc;
        _permissionSvc  = permissionSvc;
        _mapCache       = mapCache;
        _broadcast      = broadcast;
        _playerRepo     = playerRepo;
        _visitRepo      = visitRepo;
        _noteRepo       = noteRepo;
        _mapRepo        = mapRepo;
    }

    public async Task InitAsync(string browserId)
    {
        Player = await _playerSvc.RestorePlayerAsync(browserId);
        if (Player != null)
            _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color);
        IsLoaded = true;
    }

    public async Task<(bool ok, string? error)> CreatePlayerAsync(string username, string browserId)
    {
        var (player, error) = await _playerSvc.CreatePlayerAsync(username, browserId);
        if (player == null) return (false, error);
        Player = player;
        _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color);
        return (true, null);
    }

    public async Task<MovementResult> MoveAsync(int targetQ, int targetR, bool oceanConfirmed = false)
    {
        if (Player == null) return new MovementResult { ErrorMessage = "Not logged in." };

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
        _broadcast.PlayerCameOnline(Player.Id, Player.Username, Player.Q, Player.R, Player.Color);
    }

    public async Task<(bool success, string message, int eggCount)> LayEggAsync()
    {
        if (Player == null) return (false, "Not logged in.", 0);
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

    public async Task<(string flavorText, List<TileNote> notes)> InvestigateAsync()
    {
        if (Player == null) return ("You are not sure who you are.", []);
        return await _investigateSvc.InvestigateAsync(Player);
    }

    public async Task<GatherResult> GatherAsync()
    {
        if (Player == null) return new GatherResult { Success = false, ErrorMessage = "Not logged in." };
        var permissions = _permissionSvc.GetPermissions(Player);
        var result = await _gatherSvc.TryGatherAsync(Player, permissions);
        if (result.Success)
            Player.GatherCooldownUntil = result.CooldownUntil;
        return result;
    }

    public async Task<bool> UsePoulticeAsync()
    {
        if (Player == null) return false;
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
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile == null) return (false, "Cannot build here.");
        return await _structureSvc.TryBuildAsync(Player, type, tile);
    }

    public async Task<bool> DestroyStructureAsync()
    {
        if (Player == null) return false;
        var tile = _mapCache.GetCachedTile(Player.Q, Player.R);
        if (tile?.Structure == null) return false;
        await _structureSvc.DestroyAsync(tile);
        return true;
    }

    public async Task<CraftingResult> CraftAsync(string recipeId)
    {
        if (Player == null) return new CraftingResult { Success = false, ErrorMessage = "Not logged in." };
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

    public async Task LeaveNoteAsync(string content)
    {
        if (Player == null || string.IsNullOrWhiteSpace(content)) return;
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
        if (Player == null || string.IsNullOrWhiteSpace(content)) return;
        content = content.Trim();
        if (content.Length > 100) content = content[..100];
        await _mapRepo.PlaceSignAsync(Player.Q, Player.R, content, Player.Username);
        _mapCache.UpdateCachedSign(Player.Q, Player.R, content, Player.Username);
    }

    public ValueTask DisposeAsync()
    {
        if (Player != null)
            _broadcast.PlayerWentOffline(Player.Id);
        return ValueTask.CompletedTask;
    }
}
