using MapGenerator.Application.Services;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Services;
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
        services.AddSingleton<IEquipmentDefinitionProvider, InMemoryEquipmentDefinitionProvider>();
        services.AddSingleton<IConsumableDefinitionProvider, InMemoryConsumableDefinitionProvider>();
        services.AddSingleton<IItemRegistry, ItemRegistry>();
        services.AddSingleton<ICraftingRecipeProvider, InMemoryCraftingRecipeProvider>();
        services.AddSingleton<IStructureDefinitionProvider, InMemoryStructureDefinitionProvider>();
        services.AddSingleton<ISettlementRoleDefinitionProvider, InMemorySettlementRoleDefinitionProvider>();
        services.AddSingleton<MapGeneratorService>();
        services.AddSingleton<PermissionService>();
        services.AddSingleton<SettlementCacheService>();
        services.AddScoped<SettlementGenerationService>();
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
        services.AddScoped<DanceService>();
        services.AddScoped<KissService>();
        services.AddScoped<EggExplosionService>();
        services.AddScoped<DungeonGenerationService>();
        services.AddScoped<DungeonService>();
        services.AddSingleton<IEnemyDefinitionProvider, InMemoryEnemyDefinitionProvider>();
        services.AddSingleton<IEnemyAffixProvider, InMemoryEnemyAffixProvider>();
        services.AddSingleton<ISpellDefinitionProvider, InMemorySpellDefinitionProvider>();
        services.AddSingleton<IWeaponAttackProvider, InMemoryWeaponAttackProvider>();
        services.AddSingleton<ICompanionDefinitionProvider, InMemoryCompanionDefinitionProvider>();
        services.AddSingleton<ICompanionMoveProvider, InMemoryCompanionMoveProvider>();
        services.AddScoped<EnemySpawner>();
        services.AddScoped<ICombatEngine, CombatEngine>();
        return services;
    }
}
