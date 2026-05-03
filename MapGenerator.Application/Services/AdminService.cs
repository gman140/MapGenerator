using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class AdminService
{
    private readonly MapGeneratorService _generator;
    private readonly IPlayerRepository _playerRepo;

    public AdminService(MapGeneratorService generator, IPlayerRepository playerRepo)
    {
        _generator = generator;
        _playerRepo = playerRepo;
    }

    public async Task<MapConfig> RegenerateMapAsync(
        int width, int height, int? seed = null, MapGenerationOptions? options = null)
    {
        var config = await _generator.GenerateMapAsync(width, height, seed, options);
        await _playerRepo.MoveAllToSpawnAsync(config.SpawnQ, config.SpawnR);
        return config;
    }
}
