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
    private readonly ITileInventoryRepository _tileInventoryRepo;
    private readonly ISettlementRepository _settlementRepo;
    private readonly IRoadRepository _roadRepo;
    private readonly SettlementGenerationService _settlementGenSvc;

    public AdminService(
        MapGeneratorService generator,
        IPlayerRepository playerRepo,
        IPlayerTileVisitRepository visitRepo,
        ITileNoteRepository noteRepo,
        IChatRepository chatRepo,
        ITileInventoryRepository tileInventoryRepo,
        ISettlementRepository settlementRepo,
        IRoadRepository roadRepo,
        SettlementGenerationService settlementGenSvc)
    {
        _generator        = generator;
        _playerRepo       = playerRepo;
        _visitRepo        = visitRepo;
        _noteRepo         = noteRepo;
        _chatRepo         = chatRepo;
        _tileInventoryRepo = tileInventoryRepo;
        _settlementRepo   = settlementRepo;
        _roadRepo         = roadRepo;
        _settlementGenSvc = settlementGenSvc;
    }

    public async Task<MapConfig> RegenerateMapAsync(
        int width, int height, int? seed = null, MapGenerationOptions? options = null)
    {
        await Task.WhenAll(
            _visitRepo.DeleteAllAsync(),
            _noteRepo.DeleteAllAsync(),
            _chatRepo.DeleteAllAsync(),
            _tileInventoryRepo.DeleteAllAsync(),
            _settlementRepo.DeleteAllAsync(),
            _roadRepo.DeleteAllAsync());

        var config = await _generator.GenerateMapAsync(width, height, seed, options);
        await _settlementGenSvc.GenerateAsync(config.Width, config.Height, config.Seed, options);
        await _playerRepo.MoveAllToSpawnAsync(config.SpawnQ, config.SpawnR);
        return config;
    }
}
