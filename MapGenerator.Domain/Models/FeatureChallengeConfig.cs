namespace MapGenerator.Domain.Models;

/// <summary>
/// Configuration stored on a TileFeatureDefinition that marks it as having a runner challenge.
/// The server converts this into a RunnerInitData when the player enters the challenge.
/// </summary>
public class FeatureChallengeConfig
{
    public string Label { get; init; } = "Challenge";
    public int PlayerMaxHp { get; init; } = 50;
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
