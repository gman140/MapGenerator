using Microsoft.AspNetCore.SignalR;

namespace MapGenerator.Web.Hubs;

public class GameHub : Hub
{
    // Groups players by tile key so the server can broadcast tile-scoped events
    public Task JoinTileGroup(int q, int r) =>
        Groups.AddToGroupAsync(Context.ConnectionId, TileKey(q, r));

    public Task LeaveTileGroup(int q, int r) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, TileKey(q, r));

    public static string TileKey(int q, int r) => $"tile_{q}_{r}";
}
