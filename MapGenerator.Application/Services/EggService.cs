using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class EggService
{
    private static readonly TimeSpan EggCooldown = TimeSpan.FromMinutes(1);

    private readonly IMapRepository _mapRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly MapGeneratorService _mapCache;

    public EggService(IMapRepository mapRepo, IPlayerRepository playerRepo, MapGeneratorService mapCache)
    {
        _mapRepo = mapRepo;
        _playerRepo = playerRepo;
        _mapCache = mapCache;
    }

    public async Task<(bool success, string message, int eggCount)> LayEggAsync(Player player)
    {
        if (player.LastEggLaidAt.HasValue)
        {
            var remaining = EggCooldown - (DateTime.UtcNow - player.LastEggLaidAt.Value);
            if (remaining > TimeSpan.Zero)
                return (false, $"You need to rest {remaining.TotalSeconds:F0}s before laying another egg.", 0);
        }

        var newCount = await _mapRepo.IncrementEggCountAsync(player.Q, player.R);
        _mapCache.UpdateCachedEggCount(player.Q, player.R, newCount);
        player.LastEggLaidAt = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        return (true, GetEggDescriptor(newCount), newCount);
    }

    public static string GetEggDescriptor(int count) => count switch
    {
        1  => "A single, lonely egg sits here.",
        2  => "Two eggs. A modest collection.",
        3  => "Three eggs. Someone is committed.",
        4  => "Four eggs. This is getting weird.",
        5  => "Five eggs. What are you doing.",
        6  => "Six eggs. The ground is running out of room.",
        7  => "Seven eggs. A sacred number, maybe?",
        8  => "Eight eggs. An entire octet of eggs.",
        9  => "Nine eggs. Approaching dangerous levels.",
        10 => "Ten eggs. You have a problem.",
        >= 20 and < 50  => $"{count} eggs. This tile is an egg sanctuary.",
        >= 50 and < 100 => $"{count} eggs. Geologists will wonder about this for centuries.",
        >= 100          => $"{count} eggs. You have achieved egg singularity.",
        _               => $"{count} eggs cluster here.",
    };
}
