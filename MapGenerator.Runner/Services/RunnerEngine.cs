using MapGenerator.Runner.Models;

namespace MapGenerator.Runner.Services;

public static class RunnerEngine
{
    // Layout constants (canvas 800×300)
    public const double CanvasWidth = 800;
    public const double CanvasHeight = 300;
    public const double GroundY = 260;          // y-coordinate of the ground surface
    public const double PlayerScreenX = 80;     // player is fixed horizontally
    public const double PlayerWidth = 48;
    public const double PlayerHeight = 48;
    public const double CompanionWidth = 32;
    public const double CompanionHeight = 32;

    private const double Gravity = 1800.0;
    private const double JumpVelocity = -650.0;
    private const double BoulderWidth = 40.0;
    private const double BoulderHeight = 50.0;
    private const double GapWidth = 60.0;
    private const double AttackRange = 90.0;
    private const double ProjectileY = GroundY - 28;     // flies at mid-body height; player must jump to dodge
    private const double AttackCooldownMs = 500.0;
    private const double CompanionAttackCooldownMs = 2200.0;
    private const double SpeedAcceleration = 8.0; // px/s per second

    public static RunnerGameState Initialize(RunnerInitData data)
    {
        var cfg = data.Config;

        // Spread enemy spawn triggers evenly; obstacles spawn dynamically during play
        var triggers = new List<double>();
        double spacing = cfg.TotalDistancePx / (cfg.EnemyCount + 1.0);
        for (int i = 0; i < cfg.EnemyCount; i++)
            triggers.Add(spacing * (i + 1));

        return new RunnerGameState
        {
            PlayerY = GroundY - PlayerHeight,
            PlayerHp = cfg.PlayerMaxHp,
            PlayerMaxHp = cfg.PlayerMaxHp,
            CompanionY = GroundY - CompanionHeight,
            CompanionIsGrounded = true,
            ScrollSpeed = cfg.InitialScrollSpeed,
            TotalDistance = cfg.TotalDistancePx,
            CompanionAttackCooldownMs = CompanionAttackCooldownMs,
            NextObstacleWorldX = 520.0,  // first obstacle appears ~520px ahead
            PendingEnemyTriggers = triggers,
        };
    }

    public static void Tick(RunnerGameState state, RunnerConfig cfg, double timestamp)
    {
        if (state.Phase != RunnerPhase.Playing) return;

        double deltaMs = state.LastTimestamp < 0 ? 16 : Math.Min(timestamp - state.LastTimestamp, 100);
        state.LastTimestamp = timestamp;
        double dt = deltaMs / 1000.0;

        // Scroll
        state.WorldOffset += state.ScrollSpeed * dt;
        state.ScrollSpeed += SpeedAcceleration * dt;

        // Spawn enemies when distance thresholds are crossed
        for (int i = state.PendingEnemyTriggers.Count - 1; i >= 0; i--)
        {
            if (state.WorldOffset >= state.PendingEnemyTriggers[i])
            {
                bool isShooter = state.Enemies.Count % 2 == 1;
                state.Enemies.Add(new RunnerEnemy
                {
                    ScreenX = CanvasWidth + 20,
                    Hp = cfg.EnemyHp,
                    MaxHp = cfg.EnemyHp,
                    Damage = cfg.EnemyDamage,
                    Color = isShooter ? "#c07820" : cfg.EnemyColor,
                    Type = isShooter ? EnemyType.Shooter : EnemyType.Melee,
                });
                state.PendingEnemyTriggers.RemoveAt(i);
            }
        }

        // Physics — track grounded state before update for landing detection
        bool wasGrounded = state.IsGrounded;
        bool companionWasGrounded = state.CompanionIsGrounded;

        if (!state.IsGrounded)
        {
            state.PlayerVelocityY += Gravity * dt;
            state.PlayerY += state.PlayerVelocityY * dt;
        }
        double standingY = GroundY - PlayerHeight;
        if (state.PlayerY >= standingY)
        {
            state.PlayerY = standingY;
            state.PlayerVelocityY = 0;
            state.IsGrounded = true;
        }

        if (!wasGrounded && state.IsGrounded)
            state.LandImpactMs = 150.0;

        // Companion delayed jump
        if (state.CompanionJumpDelayMs > 0)
        {
            state.CompanionJumpDelayMs -= deltaMs;
            if (state.CompanionJumpDelayMs <= 0 && state.CompanionIsGrounded)
            {
                state.CompanionIsGrounded = false;
                state.CompanionVelocityY = JumpVelocity;
            }
        }

        // Companion physics
        if (!state.CompanionIsGrounded)
        {
            state.CompanionVelocityY += Gravity * dt;
            state.CompanionY += state.CompanionVelocityY * dt;
        }
        double companionStandingY = GroundY - CompanionHeight;
        if (state.CompanionY >= companionStandingY)
        {
            state.CompanionY = companionStandingY;
            state.CompanionVelocityY = 0;
            state.CompanionIsGrounded = true;
        }

        if (!companionWasGrounded && state.CompanionIsGrounded)
            state.CompanionLandImpactMs = 120.0;

        // Walk cycles (advance when grounded)
        if (state.IsGrounded)          state.WalkCycleMs += deltaMs;
        if (state.CompanionIsGrounded) state.CompanionWalkCycleMs += deltaMs;

        // Animation countdowns
        state.AttackAnimMs        = Math.Max(0, state.AttackAnimMs - deltaMs);
        state.LandImpactMs        = Math.Max(0, state.LandImpactMs - deltaMs);
        state.CompanionLandImpactMs = Math.Max(0, state.CompanionLandImpactMs - deltaMs);

        // Dynamic obstacle spawning — keep spawning as long as enemies remain
        bool enemiesRemain = state.PendingEnemyTriggers.Count > 0 || state.Enemies.Any(e => !e.IsDefeated);
        if (enemiesRemain)
        {
            while (state.NextObstacleWorldX - state.WorldOffset < CanvasWidth + 500)
            {
                bool isGap = Random.Shared.NextDouble() < 0.35;
                state.Obstacles.Add(new RunnerObstacle
                {
                    WorldX = state.NextObstacleWorldX,
                    Width  = isGap ? GapWidth : BoulderWidth,
                    Height = isGap ? 0 : BoulderHeight,
                    IsGap  = isGap,
                    Color  = cfg.ObstacleColor,
                });
                state.NextObstacleWorldX += 350.0 + Random.Shared.NextDouble() * 300.0;
            }
        }

        // Remove obstacles that have scrolled fully off-screen to the left
        state.Obstacles.RemoveAll(o => o.WorldX + o.Width < state.WorldOffset - 80);

        // Tick hit cooldowns so each obstacle can only damage the player once per crossing
        foreach (var obs in state.Obstacles)
            obs.HitCooldownMs = Math.Max(0, obs.HitCooldownMs - deltaMs);

        // Obstacle collision
        double playerLeft = PlayerScreenX;
        double playerRight = PlayerScreenX + PlayerWidth;
        double playerBottom = state.PlayerY + PlayerHeight;

        foreach (var obs in state.Obstacles.Where(o => o.HitCooldownMs <= 0))
        {
            double obsLeft = obs.WorldX - state.WorldOffset;
            double obsRight = obsLeft + obs.Width;

            bool horizontalOverlap = playerRight > obsLeft + 4 && playerLeft < obsRight - 4;
            if (!horizontalOverlap) continue;

            if (obs.IsGap)
            {
                if (state.IsGrounded)
                {
                    DamagePlayer(state, cfg.EnemyDamage * 2, "#ff8844");
                    obs.HitCooldownMs = 600.0;
                }
            }
            else
            {
                double obstacleTop = GroundY - obs.Height;
                if (playerBottom > obstacleTop + 4 && state.PlayerY < GroundY)
                {
                    DamagePlayer(state, cfg.EnemyDamage, "#ff8844");
                    obs.HitCooldownMs = 600.0;
                }
            }
        }

        // Enemy logic
        state.AttackCooldownMs = Math.Max(0, state.AttackCooldownMs - deltaMs);

        foreach (var enemy in state.Enemies.Where(e => !e.IsDefeated))
        {
            bool playerInEnemyRange = enemy.ScreenX < PlayerScreenX + AttackRange &&
                                      enemy.ScreenX > PlayerScreenX - 20;

            if (!playerInEnemyRange)
            {
                enemy.ScreenX -= enemy.WalkSpeedPx * dt;

                // Shooter enemies fire projectiles while walking toward the player
                if (enemy.Type == EnemyType.Shooter)
                {
                    enemy.ProjectileCooldownMs -= deltaMs;
                    if (enemy.ProjectileCooldownMs <= 0)
                    {
                        state.Projectiles.Add(new RunnerProjectile
                        {
                            X = enemy.ScreenX - 4,
                            Y = ProjectileY,
                            Damage = enemy.Damage,
                        });
                        enemy.ProjectileCooldownMs = enemy.ProjectileIntervalMs;
                    }
                }
            }
            else
            {
                enemy.AttackCooldownMs -= deltaMs;
                if (enemy.AttackCooldownMs <= 0)
                {
                    DamagePlayer(state, enemy.Damage, "#ff4444");
                    enemy.AttackCooldownMs = enemy.AttackIntervalMs;
                }
            }
        }

        // Projectile movement and collision
        for (int i = state.Projectiles.Count - 1; i >= 0; i--)
        {
            var proj = state.Projectiles[i];
            proj.X -= proj.SpeedPx * dt;

            if (proj.X + 12 < 0)
            {
                state.Projectiles.RemoveAt(i);
                continue;
            }

            // Collision with player (±10 horizontal, ±5 vertical around projectile center)
            if (proj.X + 10 > playerLeft &&
                proj.X - 10 < playerRight &&
                proj.Y + 5  > state.PlayerY &&
                proj.Y - 5  < playerBottom)
            {
                DamagePlayer(state, proj.Damage, "#ffaa00");
                state.Projectiles.RemoveAt(i);
            }
        }

        // Companion auto-attack
        state.CompanionAttackCooldownMs -= deltaMs;
        if (state.CompanionAttackCooldownMs <= 0)
        {
            var target = state.Enemies
                .Where(e => !e.IsDefeated && e.ScreenX < PlayerScreenX + 180)
                .MinBy(e => e.ScreenX);
            if (target != null)
            {
                int dmg = Math.Max(1, cfg.CompanionAttackStat / 3 + 3);
                target.Hp = Math.Max(0, target.Hp - dmg);
                AddFloat(state, target.ScreenX + 18, GroundY - 60, $"-{dmg}", "#a0e0ff");
            }
            state.CompanionAttackCooldownMs = CompanionAttackCooldownMs;
        }

        // Floating text lifetime
        foreach (var ft in state.FloatingTexts)
        {
            ft.Y -= 55.0 * dt;
            ft.LifetimeMs -= deltaMs;
        }
        state.FloatingTexts.RemoveAll(ft => ft.LifetimeMs <= 0);

        // Win / lose — win requires all enemies defeated (obstacles keep coming until then)
        if (state.PlayerHp <= 0)
            state.Phase = RunnerPhase.Failed;
        else if (state.PendingEnemyTriggers.Count == 0 &&
                 state.Enemies.Count > 0 &&
                 !state.Enemies.Any(e => !e.IsDefeated))
            state.Phase = RunnerPhase.Complete;
    }

    public static void Jump(RunnerGameState state)
    {
        if (state.Phase != RunnerPhase.Playing || !state.IsGrounded) return;
        state.IsGrounded = false;
        state.PlayerVelocityY = JumpVelocity;
        state.CompanionJumpDelayMs = 150.0; // companion jumps 150 ms after player
    }

    public static void Attack(RunnerGameState state, int playerAttackStat)
    {
        if (state.Phase != RunnerPhase.Playing || state.AttackCooldownMs > 0) return;

        var target = state.Enemies
            .Where(e => !e.IsDefeated && e.ScreenX < PlayerScreenX + AttackRange && e.ScreenX > PlayerScreenX - 20)
            .MinBy(e => e.ScreenX);

        if (target == null) return;

        int dmg = Math.Max(1, playerAttackStat / 2 + 5);
        target.Hp = Math.Max(0, target.Hp - dmg);
        state.AttackCooldownMs = AttackCooldownMs;
        state.AttackAnimMs = 220.0;
        AddFloat(state, target.ScreenX + 18, GroundY - 70, $"-{dmg}", "#ffdd44");
    }

    public static RunnerResult BuildResult(RunnerGameState state, RunnerConfig cfg)
    {
        if (!state.Phase.Equals(RunnerPhase.Complete))
            return new RunnerResult { Won = false };

        var rng = new Random();
        var rewards = new List<RunnerReward>();
        if (cfg.RewardPool.Length > 0)
        {
            var pool = cfg.RewardPool.ToList();
            for (int i = 0; i < cfg.RewardCount && pool.Count > 0; i++)
            {
                int idx = rng.Next(pool.Count);
                rewards.Add(new RunnerReward { ItemId = pool[idx], Quantity = 1 });
                pool.RemoveAt(idx);
            }
        }
        return new RunnerResult { Won = true, Rewards = rewards };
    }

    private static void DamagePlayer(RunnerGameState state, int damage, string color)
    {
        state.PlayerHp = Math.Max(0, state.PlayerHp - damage);
        AddFloat(state, PlayerScreenX, state.PlayerY - 8, $"-{damage}", color);
    }

    private static void AddFloat(RunnerGameState state, double x, double y, string text, string color) =>
        state.FloatingTexts.Add(new FloatingText { X = x, Y = y, Text = text, Color = color });
}
