using MapGenerator.Application.Services;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Web.Services;

public class GameSessionService : IAsyncDisposable
{
    private readonly PlayerService _playerSvc;
    private readonly ChatService _chatSvc;
    private readonly MovementService _movementSvc;
    private readonly EggService _eggSvc;
    private readonly GameBroadcastService _broadcast;
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;

    public Player? Player { get; private set; }
    public bool IsLoaded { get; private set; }

    public GameSessionService(
        PlayerService playerSvc,
        ChatService chatSvc,
        MovementService movementSvc,
        EggService eggSvc,
        GameBroadcastService broadcast,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo)
    {
        _playerSvc = playerSvc;
        _chatSvc = chatSvc;
        _movementSvc = movementSvc;
        _eggSvc = eggSvc;
        _broadcast = broadcast;
        _playerRepo = playerRepo;
        _visitRepo = visitRepo;
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
        var result = await _movementSvc.TryMoveAsync(Player, targetQ, targetR, oceanConfirmed);

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
        var result = await _eggSvc.LayEggAsync(Player);
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

    public ValueTask DisposeAsync()
    {
        if (Player != null)
            _broadcast.PlayerWentOffline(Player.Id);
        return ValueTask.CompletedTask;
    }
}
