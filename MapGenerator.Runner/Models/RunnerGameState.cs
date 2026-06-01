namespace MapGenerator.Runner.Models;

public enum RunnerPhase { Playing, Complete, Failed }

public class RunnerGameState
{
    // Player
    public double PlayerY { get; set; }
    public double PlayerVelocityY { get; set; }
    public bool IsGrounded { get; set; } = true;
    public int PlayerHp { get; set; }
    public int PlayerMaxHp { get; set; }
    public double AttackCooldownMs { get; set; }

    // Animation
    public double AttackAnimMs { get; set; }       // swipe effect countdown
    public double WalkCycleMs { get; set; }         // drives walk bob for player
    public double LandImpactMs { get; set; }        // squash on landing

    // Companion
    public double CompanionY { get; set; }
    public double CompanionVelocityY { get; set; }
    public bool CompanionIsGrounded { get; set; } = true;
    public double CompanionJumpDelayMs { get; set; }
    public double CompanionWalkCycleMs { get; set; }
    public double CompanionLandImpactMs { get; set; }
    public double CompanionAttackCooldownMs { get; set; }

    // World scroll
    public double WorldOffset { get; set; }
    public double ScrollSpeed { get; set; }
    public double TotalDistance { get; set; }
    public double DistanceTraveled => WorldOffset;

    // Player abilities
    public bool HasDoubleJump { get; set; } = true;
    public double DodgeMs { get; set; }          // > 0 while invincible
    public double DodgeCooldownMs { get; set; }  // > 0 while recharging

    // Pickups
    public List<RunnerPickup> Pickups { get; set; } = [];
    public double AttackBoostMs { get; set; }       // > 0 while attack is doubled
    public double PickupSpawnCooldownMs { get; set; }

    // Entities
    public List<RunnerObstacle> Obstacles { get; set; } = [];
    public List<RunnerEnemy> Enemies { get; set; } = [];
    public RunnerChest? Chest { get; set; }
    public List<RunnerProjectile> Projectiles { get; set; } = [];
    public List<RunnerSpider> Spiders { get; set; } = [];
    public List<double> PendingEnemyTriggers { get; set; } = [];
    public List<FloatingText> FloatingTexts { get; set; } = [];
    public double NextObstacleWorldX { get; set; }
    public double SpiderSpawnCooldownMs { get; set; }

    // One-shot sound triggers — set by engine, cleared by the component after firing
    public bool SoundDamage { get; set; }
    public bool SoundEnemyAttack { get; set; }

    // Phase
    public RunnerPhase Phase { get; set; } = RunnerPhase.Playing;
    public double LastTimestamp { get; set; } = -1;
}
