namespace MapGenerator.Runner.Models;

/// <summary>
/// Everything the WASM RunnerGame component needs to start a run.
/// Serialized by Blazor when crossing the Server→WASM boundary.
/// </summary>
public class RunnerInitData
{
    public RunnerConfig Config { get; init; } = new();
    public string[] PlayerSprite { get; init; } = [];
    public string[] CompanionSprite { get; init; } = [];
    public string CompanionName { get; init; } = "";
}
