using System.Collections.Concurrent;
using MapGenerator.Runner.Models;

namespace MapGenerator.Web.Services;

/// <summary>
/// Singleton that bridges data between the Interactive Server circuit (Game.razor)
/// and the static-SSR RunnerPage that hosts the WASM RunnerGame component.
/// Entries are consumed once and then discarded.
/// </summary>
public class ChallengeSessionService
{
    private readonly ConcurrentDictionary<string, RunnerInitData> _pending = new();

    public string Store(RunnerInitData data)
    {
        var key = Guid.NewGuid().ToString("N");
        _pending[key] = data;
        return key;
    }

    public RunnerInitData? Take(string key)
    {
        _pending.TryRemove(key, out var data);
        return data;
    }
}
