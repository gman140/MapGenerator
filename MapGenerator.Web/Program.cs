using MapGenerator.Application;
using MapGenerator.Application.Services;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Infrastructure;
using MapGenerator.Web.Components;
using MapGenerator.Web.Hubs;
using MapGenerator.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSignalR();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddSingleton<GameBroadcastService>();
builder.Services.AddScoped<GameSessionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapHub<GameHub>("/gamehub");

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Seed map and settlements on startup
using (var scope = app.Services.CreateScope())
{
    var generator = scope.ServiceProvider.GetRequiredService<MapGeneratorService>();
    var existing  = await generator.LoadCacheAsync();

    if (existing == null)
    {
        int width  = builder.Configuration.GetValue("Map:DefaultWidth",  300);
        int height = builder.Configuration.GetValue("Map:DefaultHeight", 300);
        existing   = await generator.GenerateMapAsync(width, height);

        var settlementGen = scope.ServiceProvider.GetRequiredService<SettlementGenerationService>();
        await settlementGen.GenerateAsync(existing.Width, existing.Height, existing.Seed, existing.Options);
    }
    else
    {
        var settlementRepo = scope.ServiceProvider.GetRequiredService<ISettlementRepository>();
        var roadRepo       = scope.ServiceProvider.GetRequiredService<IRoadRepository>();
        var settlements    = await settlementRepo.GetAllAsync();
        var roads          = await roadRepo.GetAllAsync();

        var cache = app.Services.GetRequiredService<SettlementCacheService>();
        if (settlements.Count == 0)
        {
            var settlementGen = scope.ServiceProvider.GetRequiredService<SettlementGenerationService>();
            await settlementGen.GenerateAsync(existing.Width, existing.Height, existing.Seed, existing.Options);
        }
        else
        {
            cache.UpdateCache(settlements, roads);
        }
    }
}

app.Run();
