using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class AdminService
{
    private readonly MapGeneratorService _generator;
    private readonly IPlayerRepository _playerRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly ITileNoteRepository _noteRepo;
    private readonly IChatRepository _chatRepo;

    public AdminService(
        MapGeneratorService generator,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        ITileNoteRepository noteRepo,
        IChatRepository chatRepo)
    {
        _generator  = generator;
        _playerRepo = playerRepo;
        _visitRepo  = visitRepo;
        _noteRepo   = noteRepo;
        _chatRepo   = chatRepo;
    }

    public async Task<MapConfig> RegenerateMapAsync(
        int width, int height, int? seed = null, MapGenerationOptions? options = null)
    {
        await Task.WhenAll(
            _visitRepo.DeleteAllAsync(),
            _noteRepo.DeleteAllAsync(),
            _chatRepo.DeleteAllAsync());

        var config = await _generator.GenerateMapAsync(width, height, seed, options);
        await _playerRepo.MoveAllToSpawnAsync(config.SpawnQ, config.SpawnR);
        return config;
    }
}
