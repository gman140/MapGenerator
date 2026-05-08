using MapGenerator.Application.Services;
using MapGenerator.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MapGenerator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IFeatureDefinitionProvider, InMemoryFeatureDefinitionProvider>();
        services.AddSingleton<IBiomeDefinitionProvider, InMemoryBiomeDefinitionProvider>();
        services.AddSingleton<IResourceDefinitionProvider, InMemoryResourceDefinitionProvider>();
        services.AddSingleton<ICraftingRecipeProvider, InMemoryCraftingRecipeProvider>();
        services.AddSingleton<IStructureDefinitionProvider, InMemoryStructureDefinitionProvider>();
        services.AddSingleton<MapGeneratorService>();
        services.AddSingleton<PermissionService>();
        services.AddScoped<MovementService>();
        services.AddScoped<PlayerService>();
        services.AddScoped<ChatService>();
        services.AddScoped<AdminService>();
        services.AddScoped<EggService>();
        services.AddScoped<InvestigateService>();
        services.AddScoped<GatherService>();
        services.AddScoped<CraftingService>();
        services.AddScoped<StructureService>();
        services.AddScoped<TileInventoryService>();
        return services;
    }
}
