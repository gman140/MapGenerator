using MapGenerator.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MapGenerator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<MapGeneratorService>();
        services.AddScoped<MovementService>();
        services.AddScoped<PlayerService>();
        services.AddScoped<ChatService>();
        services.AddScoped<AdminService>();
        services.AddScoped<EggService>();
        return services;
    }
}
