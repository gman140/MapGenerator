namespace MapGenerator.Runner.Models;

public enum EnemyType { Melee, Shooter, Thief, Boss }

public class RunnerEnemy
{
    /// <summary>Screen-space X. Enemies walk left across the screen independently of world scroll.</summary>
    public double ScreenX { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; init; }
    public int Damage { get; init; }
    public double AttackIntervalMs { get; init; } = 2500.0;
    public double AttackCooldownMs { get; set; } = 2500.0;
    public double WalkSpeedPx { get; init; } = 80.0;
    public string Color { get; init; } = "#c04040";
    public bool IsDefeated => Hp <= 0;
    public EnemyType Type { get; init; } = EnemyType.Melee;
    public double ProjectileIntervalMs { get; init; } = 1800.0;
    public double ProjectileCooldownMs { get; set; } = 1800.0;
    public double WalkCycleMs { get; set; }
    public double AttackAnimMs { get; set; }  // > 0 while the attack swipe is playing
    public bool HasAttacked { get; set; }
    public double DefeatedAnimMs { get; set; } = -1.0; // -1 = not yet triggered; >0 = ghost playing
    public double ShockwaveIntervalMs { get; init; } = double.MaxValue; // only used by Boss
    public double ShockwaveCooldownMs { get; set; }
}
