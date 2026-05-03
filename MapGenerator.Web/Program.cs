using MapGenerator.Application;
using MapGenerator.Application.Services;
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

// Seed map if none exists
using (var scope = app.Services.CreateScope())
{
    var generator = scope.ServiceProvider.GetRequiredService<MapGeneratorService>();
    var existing = await generator.LoadCacheAsync();
    if (existing == null)
    {
        int width = builder.Configuration.GetValue("Map:DefaultWidth", 300);
        int height = builder.Configuration.GetValue("Map:DefaultHeight", 300);
        await generator.GenerateMapAsync(width, height);
    }
}

app.Run();
