using MapGenerator.Domain.Interfaces;
using MapGenerator.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapGenerator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var dbName = config["MongoDB:Database"] ?? "MapGenerator";

        services.AddSingleton(new MongoDbContext(connStr, dbName));
        services.AddScoped<IMapRepository, MapRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IPlayerTileVisitRepository, PlayerTileVisitRepository>();
        services.AddScoped<ITileNoteRepository, TileNoteRepository>();
        services.AddScoped<ITileInventoryRepository, TileInventoryRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<IRoadRepository, RoadRepository>();
        return services;
    }
}
