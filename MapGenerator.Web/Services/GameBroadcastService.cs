using MapGenerator.Domain.Models;
using MapGenerator.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MapGenerator.Web.Services;

/// <summary>
/// Singleton that wires Blazor-circuit code to SignalR group broadcasts and in-process C# events.
/// Blazor components subscribe to the C# events for immediate in-process delivery;
/// SignalR broadcasts handle future external clients and cross-process scenarios.
/// </summary>
public class GameBroadcastService
{
    private readonly IHubContext<GameHub> _hub;

    // In-process events consumed by Blazor Server circuits
    public event Action<string, int, int, int, int>? PlayerMoved;   // playerId, oldQ, oldR, newQ, newR
    public event Action<ChatMessage>? MessageReceived;
    public event Action<string, string>? PlayerConnected;            // playerId, username
    public event Action<string>? PlayerDisconnected;                 // playerId
    public event Action<MapConfig>? MapRegenerated;

    // In-memory online player presence: playerId -> (username, q, r)
    private readonly Dictionary<string, (string Username, int Q, int R)> _online = [];
    private readonly Lock _lock = new();

    public GameBroadcastService(IHubContext<GameHub> hub) => _hub = hub;

    public void PlayerCameOnline(string playerId, string username, int q, int r)
    {
        lock (_lock) _online[playerId] = (username, q, r);
        PlayerConnected?.Invoke(playerId, username);
    }

    public void PlayerWentOffline(string playerId)
    {
        lock (_lock) _online.Remove(playerId);
        PlayerDisconnected?.Invoke(playerId);
    }

    public List<(string Id, string Username, int Q, int R)> GetOnlinePlayers()
    {
        lock (_lock) return _online.Select(kv => (kv.Key, kv.Value.Username, kv.Value.Q, kv.Value.R)).ToList();
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
        lock (_lock) _online[playerId] = (username, newQ, newR);
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
}
