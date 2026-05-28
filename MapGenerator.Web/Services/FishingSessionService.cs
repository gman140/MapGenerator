using System.Collections.Concurrent;
using MapGenerator.Fishing.Models;

namespace MapGenerator.Web.Services;

/// <summary>
/// Singleton that bridges FishingInitData from the Interactive Server circuit (Game.razor)
/// to the static-SSR FishingPage that hosts the WASM FishingGame component.
/// Entries are consumed once and then discarded.
/// </summary>
public class FishingSessionService
{
    private readonly ConcurrentDictionary<string, FishingInitData> _pending = new();

    public string Store(FishingInitData data)
    {
        var key = Guid.NewGuid().ToString("N");
        _pending[key] = data;
        return key;
    }

    public FishingInitData? Take(string key)
    {
        _pending.TryRemove(key, out var data);
        return data;
    }
}
