using MapGenerator.Domain.Models;
using MapGenerator.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MapGenerator.Web.Services;

public class GameBroadcastService
{
    private readonly IHubContext<GameHub> _hub;

    public event Action<string, int, int, int, int>? PlayerMoved;   // playerId, oldQ, oldR, newQ, newR
    public event Action<ChatMessage>? MessageReceived;
    public event Action<string, string>? PlayerConnected;            // playerId, username
    public event Action<string>? PlayerDisconnected;                 // playerId
    public event Action<MapConfig>? MapRegenerated;
    public event Action<string, string>? PlayerColorChanged;         // playerId, newColor
    public event Action<string, int, int, int>? EggLaid;             // playerId, q, r, eggCount
    public event Action<string, string, int, int>? PlayerDanced;     // playerId, username, q, r
    public event Action<string, string, string, string, int, int, string, string>? PlayerKissed; // kisserId, kisserName, kisseeId, kisseeName, q, r, kisseeMsg, observerMsg
    public event Action<int, int>? TileInventoryChanged;             // q, r
    public event Action? RoadsChanged;
    public event Action<string, int, int>? EggExploded;             // playerId, q, r
    public event Action<string, CompanionBattleChallenge>? CompanionChallengeReceived; // targetId, challenge
    public event Action<string>? CompanionChallengeDeclined;                            // challengerId
    public event Action<string, CompanionBattleResult>? CompanionBattleCompleted;       // playerId, result

    // playerId -> (username, q, r, color, spritePixels, eggsDestroyed, hasCompanion)
    private readonly Dictionary<string, (string Username, int Q, int R, string Color, string[] SpritePixels, int EggsDestroyed, bool HasCompanion)> _online = [];
    private readonly Lock _lock = new();

    public GameBroadcastService(IHubContext<GameHub> hub) => _hub = hub;

    public void PlayerCameOnline(string playerId, string username, int q, int r, string color, string[] spritePixels, int eggsDestroyed = 0, bool hasCompanion = false)
    {
        lock (_lock) _online[playerId] = (username, q, r, color, spritePixels, eggsDestroyed, hasCompanion);
        PlayerConnected?.Invoke(playerId, username);
    }

    public void PlayerWentOffline(string playerId)
    {
        lock (_lock) _online.Remove(playerId);
        PlayerDisconnected?.Invoke(playerId);
    }

    public void UpdatePlayerColor(string playerId, string color)
    {
        lock (_lock)
        {
            if (_online.TryGetValue(playerId, out var p))
                _online[playerId] = (p.Username, p.Q, p.R, color, p.SpritePixels, p.EggsDestroyed, p.HasCompanion);
        }
        PlayerColorChanged?.Invoke(playerId, color);
    }

    public void UpdatePlayerSprite(string playerId, string[] pixels)
    {
        lock (_lock)
        {
            if (_online.TryGetValue(playerId, out var p))
                _online[playerId] = (p.Username, p.Q, p.R, p.Color, pixels, p.EggsDestroyed, p.HasCompanion);
        }
        PlayerColorChanged?.Invoke(playerId, string.Empty);
    }

    public void UpdatePlayerEggsDestroyed(string playerId, int eggsDestroyed)
    {
        lock (_lock)
        {
            if (_online.TryGetValue(playerId, out var p))
                _online[playerId] = (p.Username, p.Q, p.R, p.Color, p.SpritePixels, eggsDestroyed, p.HasCompanion);
        }
    }

    public void UpdatePlayerCompanionStatus(string playerId, bool hasCompanion)
    {
        lock (_lock)
        {
            if (_online.TryGetValue(playerId, out var p))
                _online[playerId] = (p.Username, p.Q, p.R, p.Color, p.SpritePixels, p.EggsDestroyed, hasCompanion);
        }
    }

    public List<(string Id, string Username, int Q, int R, string Color, string[] SpritePixels)> GetOnlinePlayers()
    {
        lock (_lock) return _online.Select(kv => (kv.Key, kv.Value.Username, kv.Value.Q, kv.Value.R, kv.Value.Color, kv.Value.SpritePixels)).ToList();
    }

    public List<(string Id, string Username, int EggsDestroyed, bool HasCompanion)> GetOnlinePlayersOnTile(int q, int r)
    {
        lock (_lock)
            return _online
                .Where(kv => kv.Value.Q == q && kv.Value.R == r)
                .Select(kv => (kv.Key, kv.Value.Username, kv.Value.EggsDestroyed, kv.Value.HasCompanion))
                .ToList();
    }

    public async Task NotifyPlayerMovedAsync(string playerId, string username, int oldQ, int oldR, int newQ, int newR)
    {
        lock (_lock)
        {
            if (_online.TryGetValue(playerId, out var p))
                _online[playerId] = (p.Username, newQ, newR, p.Color, p.SpritePixels, p.EggsDestroyed, p.HasCompanion);
        }
        PlayerMoved?.Invoke(playerId, oldQ, oldR, newQ, newR);

        await _hub.Clients.Group(GameHub.TileKey(oldQ, oldR))
            .SendAsync("PlayerLeft", playerId, username);
        await _hub.Clients.Group(GameHub.TileKey(newQ, newR))
            .SendAsync("PlayerArrived", playerId, username);
    }

    public async Task BroadcastLocalMessageAsync(ChatMessage message)
    {
        MessageReceived?.Invoke(message);
        await _hub.Clients.Group(GameHub.TileKey(message.TileQ!.Value, message.TileR!.Value))
            .SendAsync("LocalMessage", message.SenderName, message.Content, message.SentAt);
    }

    public async Task BroadcastWorldMessageAsync(ChatMessage message)
    {
        MessageReceived?.Invoke(message);
        await _hub.Clients.All.SendAsync("WorldMessage", message.SenderName, message.Content, message.SentAt);
    }

    public async Task BroadcastMapRegeneratedAsync(MapConfig config)
    {
        MapRegenerated?.Invoke(config);
        await _hub.Clients.All.SendAsync("MapRegenerated");
    }

    public void NotifyEggLaid(string playerId, int q, int r, int eggCount)
    {
        EggLaid?.Invoke(playerId, q, r, eggCount);
    }

    public void NotifyPlayerDanced(string playerId, string username, int q, int r)
    {
        PlayerDanced?.Invoke(playerId, username, q, r);
    }

    public void NotifyPlayerKissed(string kisserId, string kisserName, string kisseeId, string kisseeName, int q, int r, string kisseeMsg, string observerMsg)
    {
        PlayerKissed?.Invoke(kisserId, kisserName, kisseeId, kisseeName, q, r, kisseeMsg, observerMsg);
    }

    public void NotifyTileInventoryChanged(int q, int r)
    {
        TileInventoryChanged?.Invoke(q, r);
    }

    public void NotifyRoadsChanged() => RoadsChanged?.Invoke();

    public void NotifyEggExploded(string playerId, int q, int r) =>
        EggExploded?.Invoke(playerId, q, r);

    public void NotifyCompanionChallengeReceived(string targetId, CompanionBattleChallenge challenge) =>
        CompanionChallengeReceived?.Invoke(targetId, challenge);

    public void NotifyCompanionChallengeDeclined(string challengerId) =>
        CompanionChallengeDeclined?.Invoke(challengerId);

    public void NotifyCompanionBattleCompleted(string playerId, CompanionBattleResult result) =>
        CompanionBattleCompleted?.Invoke(playerId, result);
}
