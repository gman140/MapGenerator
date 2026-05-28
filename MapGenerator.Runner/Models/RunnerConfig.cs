namespace MapGenerator.Runner.Models;

/// <summary>
/// Pure game-mechanics configuration for a single challenge run.
/// Built by the server from a FeatureChallengeConfig + player context.
/// Passed as a parameter to the WASM RunnerGame component.
/// </summary>
public class RunnerConfig
{
    public string Label { get; init; } = "Challenge";
    public int PlayerMaxHp { get; init; } = 50;
    public int PlayerAttackStat { get; init; } = 10;
    public int CompanionAttackStat { get; init; } = 0;
    public int EnemyHp { get; init; } = 30;
    public int EnemyDamage { get; init; } = 8;
    public int EnemyCount { get; init; } = 3;
    public int ObstacleCount { get; init; } = 6;
    public double TotalDistancePx { get; init; } = 4000.0;
    public double InitialScrollSpeed { get; init; } = 180.0;
    public string[] RewardPool { get; init; } = [];
    public int RewardCount { get; init; } = 2;
    public string BackgroundColor { get; init; } = "#1a1a2e";
    public string GroundColor { get; init; } = "#3a3a5a";
    public string ObstacleColor { get; init; } = "#808090";
    public string EnemyColor { get; init; } = "#c04040";
}
