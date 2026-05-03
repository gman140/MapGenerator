using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class PlayerService
{
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly MapGeneratorService _mapCache;

    public PlayerService(
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        MapGeneratorService mapCache)
    {
        _playerRepo = playerRepo;
        _visitRepo = visitRepo;
        _mapCache = mapCache;
    }

    public async Task<(Player? player, string? error)> CreatePlayerAsync(string username, string browserId)
    {
        username = username.Trim();

        if (username.Length < 2)
            return (null, "Username must be at least 2 characters.");
        if (username.Length > 24)
            return (null, "Username must be 24 characters or fewer.");
        if (!username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
            return (null, "Username may only contain letters, numbers, underscores, and hyphens.");

        if (await _playerRepo.GetByUsernameAsync(username) != null)
            return (null, "That username is already taken.");

        var config = _mapCache.GetCachedConfig();
        if (config == null)
            return (null, "The world has not been generated yet. Please try again shortly.");

        var player = new Player
        {
            Username = username,
            BrowserId = browserId,
            IsAdmin = username.EndsWith("admin", StringComparison.OrdinalIgnoreCase),
            Q = config.SpawnQ,
            R = config.SpawnR,
            CreatedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow,
        };

        player = await _playerRepo.CreateAsync(player);
        await _visitRepo.RecordArrivalAsync(player.Id, player.Q, player.R);

        return (player, null);
    }

    public async Task<Player?> RestorePlayerAsync(string browserId)
    {
        var player = await _playerRepo.GetByBrowserIdAsync(browserId);
        if (player == null) return null;

        player.LastSeen = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        // Ensure there is an open visit record for their current tile
        await _visitRepo.CloseAllOpenVisitsAsync(player.Id);
        await _visitRepo.RecordArrivalAsync(player.Id, player.Q, player.R);

        return player;
    }
}
