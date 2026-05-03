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

    // playerId -> (username, q, r, color)
    private readonly Dictionary<string, (string Username, int Q, int R, string Color)> _online = [];
    private readonly Lock _lock = new();

    public GameBroadcastService(IHubContext<GameHub> hub) => _hub = hub;

    public void PlayerCameOnline(string playerId, string username, int q, int r, string color)
    {
        lock (_lock) _online[playerId] = (username, q, r, color);
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
                _online[playerId] = (p.Username, p.Q, p.R, color);
        }
        PlayerColorChanged?.Invoke(playerId, color);
    }

    public List<(string Id, string Username, int Q, int R, string Color)> GetOnlinePlayers()
    {
        lock (_lock) return _online.Select(kv => (kv.Key, kv.Value.Username, kv.Value.Q, kv.Value.R, kv.Value.Color)).ToList();
    }

    public List<(string Id, string Username)> GetOnlinePlayersOnTile(int q, int r)
    {
        lock (_lock)
            return _online
                .Where(kv => kv.Value.Q == q && kv.Value.R == r)
                .Select(kv => (kv.Key, kv.Value.Username))
                .ToList();
    }

    public async Task NotifyPlayerMovedAsync(string playerId, string username, int oldQ, int oldR, int newQ, int newR)
    {
        lock (_lock)
        {
            if (_online.TryGetValue(playerId, out var p))
                _online[playerId] = (p.Username, newQ, newR, p.Color);
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
}
