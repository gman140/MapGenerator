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
    public  const double AttackRange = 90.0;
    private const double ProjectileY = GroundY - 28;     // flies at mid-body height; player must jump to dodge
    private const double AttackCooldownMs    = 500.0;
    private const double AxeVelocityX        = 340.0;
    private const double AxeInitialVelocityY = -200.0;
    private const double AxeGravity          = 520.0;
    private const double AxeRotationSpeed    = 6.0;
    private const double CompanionAttackCooldownMs = 2200.0;
    private const double SpeedAcceleration = 8.0;
    public  const double ChestWidth  = 30.0;
    public  const double ChestHeight = 32.0;
    private const double ChestSpawnAheadPx = 680.0;

    // Thief
    private const double ThiefSpeedPx = 210.0;

    // Dodge
    public  const double DodgeDurationMs   = 400.0;
    public  const double DodgeRechargeDurationMs = 2500.0;

    // Spiders
    public  const double SpiderRadius        = 9.0;
    public  const double SpiderCenterY       = GroundY - 45;  // 215 — mid-oscillation height
    public  const double SpiderAmplitude     = 22.0;          // bobs between 193 (high/safe) and 237 (low/hit)
    public  const double SpiderPeriodMs      = 2800.0;        // full up-down cycle duration
    private const double SpiderSpawnInterval = 4500.0;        // base ms between spiders

    // (Platforms removed — to be revisited)

    // Boss
    private const double BossWalkSpeedPx     = 55.0;
    private const double BossShockwaveInterval = 5500.0;
    private const double BossShockwaveY      = GroundY - 12; // ground-level wave; player must jump
    private const double BossShockwaveSpeed  = 460.0;

    // Pickups
    public  const double PickupWidth            = 14.0;
    public  const double PickupHeight           = 22.0;
    public  const double AttackBoostDurationMs  = 8000.0;
    private const double PickupSpawnInterval    = 5500.0;

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
            NextObstacleWorldX = 520.0,
            PendingEnemyTriggers = triggers,
            SpiderSpawnCooldownMs  = 2000.0,
            PickupSpawnCooldownMs  = 4000.0,  // first pickup after 4 s
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
                // Last enemy of a full run (≥3 enemies) is always a boss
                bool isBoss = cfg.EnemyCount >= 3 && state.Enemies.Count == cfg.EnemyCount - 1;

                EnemyType type;
                if (isBoss)
                    type = EnemyType.Boss;
                else
                    type = (state.Enemies.Count % 3) switch
                    {
                        0 => EnemyType.Melee,
                        1 => EnemyType.Shooter,
                        _ => EnemyType.Thief,
                    };

                int hp = type switch
                {
                    EnemyType.Boss  => cfg.EnemyHp * 4,
                    EnemyType.Thief => Math.Max(1, cfg.EnemyHp * 3 / 5),
                    _               => cfg.EnemyHp,
                };

                state.Enemies.Add(new RunnerEnemy
                {
                    ScreenX             = CanvasWidth + 20,
                    Hp                  = hp,
                    MaxHp               = hp,
                    Damage              = type == EnemyType.Boss ? cfg.EnemyDamage * 2 : cfg.EnemyDamage,
                    WalkSpeedPx         = type == EnemyType.Boss  ? BossWalkSpeedPx
                                        : type == EnemyType.Thief ? ThiefSpeedPx : 80.0,
                    AttackIntervalMs    = type == EnemyType.Boss ? 1500.0 : type == EnemyType.Thief ? 2000.0 : 2500.0,
                    AttackCooldownMs    = type == EnemyType.Boss ? 1500.0 : type == EnemyType.Thief ? 2000.0 : 2500.0,
                    CanBlock            = type == EnemyType.Melee || type == EnemyType.Boss,
                    ShockwaveIntervalMs = type == EnemyType.Boss ? BossShockwaveInterval : double.MaxValue,
                    ShockwaveCooldownMs = type == EnemyType.Boss ? 3000.0 : 0.0, // first shockwave after 3 s
                    Color = type switch
                    {
                        EnemyType.Boss    => "#1a1a2a",
                        EnemyType.Thief   => "#2a2035",
                        EnemyType.Shooter => "#c07820",
                        _                 => cfg.EnemyColor,
                    },
                    Type = type,
                });
                state.PendingEnemyTriggers.RemoveAt(i);
            }
        }

        // Physics
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
        {
            state.LandImpactMs  = 150.0;
            state.HasDoubleJump = true;
        }

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

        // Animation + ability countdowns
        state.AttackAnimMs          = Math.Max(0, state.AttackAnimMs - deltaMs);
        state.LandImpactMs          = Math.Max(0, state.LandImpactMs - deltaMs);
        state.CompanionLandImpactMs = Math.Max(0, state.CompanionLandImpactMs - deltaMs);
        state.DodgeMs               = Math.Max(0, state.DodgeMs - deltaMs);
        state.DodgeCooldownMs       = Math.Max(0, state.DodgeCooldownMs - deltaMs);
        state.DamageInvincibilityMs = Math.Max(0, state.DamageInvincibilityMs - deltaMs);
        state.AttackBoostMs         = Math.Max(0, state.AttackBoostMs - deltaMs);
        foreach (var e in state.Enemies)
        {
            e.AttackAnimMs    = Math.Max(0, e.AttackAnimMs - deltaMs);
            e.BlockRecoveryMs = Math.Max(0, e.BlockRecoveryMs - deltaMs);
            if (e.IsDefeated)
            {
                if (e.DefeatedAnimMs < 0)       e.DefeatedAnimMs = 600.0;
                else if (e.DefeatedAnimMs > 0)  e.DefeatedAnimMs = Math.Max(0, e.DefeatedAnimMs - deltaMs);
            }
        }

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
                enemy.WalkCycleMs += deltaMs;

                // Boss fires ground shockwaves while walking toward the player
                if (enemy.Type == EnemyType.Boss)
                {
                    enemy.ShockwaveCooldownMs -= deltaMs;
                    if (enemy.ShockwaveCooldownMs <= 0)
                    {
                        state.Projectiles.Add(new RunnerProjectile
                        {
                            X          = enemy.ScreenX - 4,
                            Y          = BossShockwaveY,
                            Damage     = enemy.Damage / 2,
                            SpeedPx    = BossShockwaveSpeed,
                            IsShockwave = true,
                            Color      = "#ff5500",
                        });
                        enemy.ShockwaveCooldownMs = enemy.ShockwaveIntervalMs;
                    }
                }

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
                        state.SoundEnemyAttack     = true;
                        enemy.ProjectileCooldownMs = enemy.ProjectileIntervalMs;
                    }
                }
            }
            else
            {
                enemy.AttackCooldownMs -= deltaMs;
                if (enemy.AttackCooldownMs <= 0)
                {
                    enemy.AttackAnimMs      = 200.0;
                    enemy.HasAttacked       = true;
                    enemy.BlockRecoveryMs   = 400.0;
                    state.SoundEnemyAttack  = true;
                    DamagePlayer(state, enemy.Damage, "#ff4444");
                    enemy.AttackCooldownMs  = enemy.AttackIntervalMs;
                }
            }
        }

        // Projectile movement and collision
        for (int i = state.Projectiles.Count - 1; i >= 0; i--)
        {
            var proj = state.Projectiles[i];

            if (proj.IsPlayerAxe)
            {
                proj.VelocityY += AxeGravity * dt;
                proj.X         += proj.VelocityX * dt;
                proj.Y         += proj.VelocityY * dt;
                proj.Rotation  += AxeRotationSpeed * dt;

                if (proj.X > CanvasWidth + 20 || proj.Y > GroundY + 10)
                {
                    state.Projectiles.RemoveAt(i);
                    continue;
                }

                bool hit = false;
                foreach (var enemy in state.Enemies.Where(e => !e.IsDefeated))
                {
                    double ew  = enemy.Type == EnemyType.Boss ? 54.0 : 36.0;
                    double eh  = enemy.Type == EnemyType.Boss ? 68.0 : 46.0;
                    double ecx = enemy.ScreenX + ew * 0.5;
                    double ecy = GroundY - eh * 0.5;
                    double dx  = proj.X - ecx, dy = proj.Y - ecy;
                    if (dx * dx + dy * dy < ew * 0.55 * (ew * 0.55))
                    {
                        double wt       = enemy.Type == EnemyType.Boss ? 600.0 : 450.0;
                        bool blocking   = enemy.CanBlock && enemy.BlockRecoveryMs <= 0
                                       && enemy.AttackCooldownMs > wt && enemy.AttackAnimMs <= 0;
                        int dmg         = blocking ? Math.Max(1, proj.Damage / 4) : proj.Damage;
                        enemy.Hp        = Math.Max(0, enemy.Hp - dmg);
                        string col      = blocking ? "#6699cc" : "#ffaa44";
                        AddFloat(state, enemy.ScreenX + 18, GroundY - 70, $"-{dmg}", col);
                        if (blocking) AddFloat(state, enemy.ScreenX + 18, GroundY - 85, "BLOCK!", "#4488ff");
                        hit = true;
                        break;
                    }
                }
                if (hit) state.Projectiles.RemoveAt(i);
            }
            else if (proj.IsCompanionBolt)
            {
                proj.X += proj.SpeedPx * dt;

                if (proj.X > CanvasWidth + 20)
                {
                    state.Projectiles.RemoveAt(i);
                    continue;
                }

                bool hit = false;
                foreach (var enemy in state.Enemies.Where(e => !e.IsDefeated))
                {
                    double ew  = enemy.Type == EnemyType.Boss ? 54.0 : 36.0;
                    double eh  = enemy.Type == EnemyType.Boss ? 68.0 : 46.0;
                    if (proj.X + 10 > enemy.ScreenX      && proj.X - 10 < enemy.ScreenX + ew &&
                        proj.Y +  5 > GroundY - eh       && proj.Y -  5 < GroundY)
                    {
                        double wt     = enemy.Type == EnemyType.Boss ? 600.0 : 450.0;
                        bool blocking = enemy.CanBlock && enemy.BlockRecoveryMs <= 0
                                     && enemy.AttackCooldownMs > wt && enemy.AttackAnimMs <= 0;
                        int dmg       = blocking ? Math.Max(1, proj.Damage / 4) : proj.Damage;
                        enemy.Hp      = Math.Max(0, enemy.Hp - dmg);
                        AddFloat(state, enemy.ScreenX + 18, GroundY - 60, $"-{dmg}", blocking ? "#6699cc" : "#a0e0ff");
                        if (blocking) AddFloat(state, enemy.ScreenX + 18, GroundY - 75, "BLOCK!", "#4488ff");
                        hit = true;
                        break;
                    }
                }
                if (hit) state.Projectiles.RemoveAt(i);
            }
            else
            {
                proj.X -= proj.SpeedPx * dt;

                if (proj.X + 12 < 0)
                {
                    state.Projectiles.RemoveAt(i);
                    continue;
                }

                // Collision with player
                if (proj.X + 10 > playerLeft &&
                    proj.X - 10 < playerRight &&
                    proj.Y + 5  > state.PlayerY &&
                    proj.Y - 5  < playerBottom)
                {
                    DamagePlayer(state, proj.Damage, "#ffaa00");
                    state.Projectiles.RemoveAt(i);
                }
            }
        }

        // Spider spawn + update + collision
        bool bossActive = state.Enemies.Any(e => e.Type == EnemyType.Boss && !e.IsDefeated);
        state.SpiderSpawnCooldownMs -= deltaMs;
        if (state.SpiderSpawnCooldownMs <= 0 && enemiesRemain && !bossActive)
        {
            state.Spiders.Add(new RunnerSpider
            {
                WorldX  = state.WorldOffset + CanvasWidth + 60,
                PhaseMs = Random.Shared.NextDouble() * SpiderPeriodMs, // random start phase
            });
            state.SpiderSpawnCooldownMs = SpiderSpawnInterval + Random.Shared.NextDouble() * 2000.0;
        }

        for (int i = state.Spiders.Count - 1; i >= 0; i--)
        {
            var spider = state.Spiders[i];
            spider.PhaseMs += deltaMs;

            double sx = spider.WorldX - state.WorldOffset;
            if (sx + SpiderRadius < -20)        // scrolled fully off-screen left
            {
                state.Spiders.RemoveAt(i);
                continue;
            }

            double sy = SpiderCenterY + SpiderAmplitude * Math.Sin(2 * Math.PI * spider.PhaseMs / SpiderPeriodMs);

            bool sHit = sx + SpiderRadius > playerLeft  && sx - SpiderRadius < playerRight
                     && sy + SpiderRadius > state.PlayerY && sy - SpiderRadius < playerBottom;
            if (sHit)
                DamagePlayer(state, cfg.EnemyDamage, "#88ff44");
        }

        // Pickup spawn + collection
        state.PickupSpawnCooldownMs -= deltaMs;
        if (state.PickupSpawnCooldownMs <= 0 && enemiesRemain)
        {
            var ptype = Random.Shared.NextDouble() < 0.55 ? PickupType.HealthPotion : PickupType.AttackBoost;
            // Try up to 5 candidate positions, each 100px apart, skipping any that overlap an obstacle
            bool placed = false;
            for (int attempt = 0; attempt < 5; attempt++)
            {
                double candidateX = state.WorldOffset + CanvasWidth + 180 + attempt * 100;
                bool conflicts = state.Obstacles.Any(o =>
                    candidateX - 10 < o.WorldX + o.Width &&
                    candidateX + PickupWidth + 10 > o.WorldX);
                if (!conflicts)
                {
                    state.Pickups.Add(new RunnerPickup { WorldX = candidateX, Type = ptype });
                    placed = true;
                    break;
                }
            }
            // If all candidates were blocked, retry sooner rather than waiting the full interval
            state.PickupSpawnCooldownMs = placed
                ? PickupSpawnInterval + Random.Shared.NextDouble() * 3000.0
                : 1500.0;
        }

        state.Pickups.RemoveAll(p => p.WorldX + PickupWidth < state.WorldOffset - 20);

        for (int i = state.Pickups.Count - 1; i >= 0; i--)
        {
            var pickup = state.Pickups[i];
            double px = pickup.WorldX - state.WorldOffset;
            bool hOverlap = px + PickupWidth > playerLeft && px < playerRight;
            if (!hOverlap || !state.IsGrounded) continue;

            if (pickup.Type == PickupType.HealthPotion)
            {
                int heal = Math.Max(5, state.PlayerMaxHp / 4);
                state.PlayerHp = Math.Min(state.PlayerMaxHp, state.PlayerHp + heal);
                AddFloat(state, PlayerScreenX, state.PlayerY - 20, $"+{heal} HP", "#44ff88");
            }
            else
            {
                state.AttackBoostMs = AttackBoostDurationMs;
                AddFloat(state, PlayerScreenX, state.PlayerY - 20, "ATK BOOST!", "#ffaa00");
            }
            state.Pickups.RemoveAt(i);
        }

        // Companion auto-attack — fires a projectile toward the nearest visible enemy
        state.CompanionAttackCooldownMs -= deltaMs;
        if (state.CompanionAttackCooldownMs <= 0)
        {
            bool anyEnemy = state.Enemies.Any(e => !e.IsDefeated && e.ScreenX < CanvasWidth + 10);
            if (anyEnemy)
            {
                int dmg = Math.Max(1, cfg.CompanionAttackStat / 3 + 3);
                state.Projectiles.Add(new RunnerProjectile
                {
                    X               = PlayerScreenX - 16,
                    Y               = ProjectileY,
                    SpeedPx         = 300.0,
                    Damage          = dmg,
                    IsCompanionBolt = true,
                });
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

        // Spawn chest once all enemies are defeated; obstacles already stop via enemiesRemain check above
        bool allDefeated = state.PendingEnemyTriggers.Count == 0 &&
                           state.Enemies.Count > 0 &&
                           !state.Enemies.Any(e => !e.IsDefeated);
        if (allDefeated && state.Chest == null)
            state.Chest = new RunnerChest { WorldX = state.WorldOffset + ChestSpawnAheadPx };

        // Win / lose
        if (state.PlayerHp <= 0)
            state.Phase = RunnerPhase.Failed;
        else if (state.Chest != null)
        {
            double chestScreenX = state.Chest.WorldX - state.WorldOffset;
            if (chestScreenX <= PlayerScreenX + PlayerWidth)
                state.Phase = RunnerPhase.Complete;
        }
    }

    public static void Jump(RunnerGameState state)
    {
        if (state.Phase != RunnerPhase.Playing) return;
        if (state.IsGrounded)
        {
            state.IsGrounded = false;
            state.PlayerVelocityY = JumpVelocity;
            state.CompanionJumpDelayMs = 150.0;
        }
        else if (state.HasDoubleJump)
        {
            state.PlayerVelocityY = JumpVelocity * 0.85;  // slightly weaker second jump
            state.HasDoubleJump = false;
        }
    }

    public static void Dodge(RunnerGameState state)
    {
        if (state.Phase != RunnerPhase.Playing) return;
        if (state.DodgeCooldownMs > 0 || state.DodgeMs > 0) return;
        state.DodgeMs         = DodgeDurationMs;
        state.DodgeCooldownMs = DodgeRechargeDurationMs;
    }

    public static void Attack(RunnerGameState state, int playerAttackStat)
    {
        if (state.Phase != RunnerPhase.Playing || state.AttackCooldownMs > 0) return;

        var target = state.Enemies
            .Where(e => !e.IsDefeated && e.ScreenX < PlayerScreenX + AttackRange && e.ScreenX > PlayerScreenX - 20)
            .MinBy(e => e.ScreenX);

        if (target == null)
        {
            // No melee target — throw a ranged axe
            int axeDmg = Math.Max(1, playerAttackStat / 2 + 3);
            if (state.AttackBoostMs > 0) axeDmg *= 2;
            state.Projectiles.Add(new RunnerProjectile
            {
                X           = PlayerScreenX + PlayerWidth + 4,
                Y           = state.PlayerY + PlayerHeight * 0.35,
                IsPlayerAxe = true,
                VelocityX   = AxeVelocityX,
                VelocityY   = AxeInitialVelocityY + state.PlayerVelocityY * 0.3,
                Damage      = axeDmg,
            });
            state.AttackCooldownMs = AttackCooldownMs;
            return;
        }

        int dmg = Math.Max(1, playerAttackStat / 2 + 5);
        if (state.AttackBoostMs > 0) dmg *= 2;

        double windupThreshold = target.Type == EnemyType.Boss ? 600.0 : 450.0;
        bool isBlocking = target.CanBlock
            && target.BlockRecoveryMs <= 0
            && target.AttackCooldownMs > windupThreshold
            && target.AttackAnimMs <= 0;

        if (isBlocking) dmg = Math.Max(1, dmg / 4);

        target.Hp = Math.Max(0, target.Hp - dmg);
        state.AttackCooldownMs = AttackCooldownMs;
        state.AttackAnimMs     = 220.0;

        if (isBlocking)
        {
            AddFloat(state, target.ScreenX + 18, GroundY - 70, $"-{dmg}", "#6699cc");
            AddFloat(state, target.ScreenX + 18, GroundY - 85, "BLOCK!", "#4488ff");
        }
        else
        {
            AddFloat(state, target.ScreenX + 18, GroundY - 70, $"-{dmg}", "#ffdd44");
        }

        // Parry sweet-spot: player strikes within 400 ms of the enemy's last attack → knockback
        const double ParryWindowMs = 400.0;
        if (target.HasAttacked && target.AttackCooldownMs > target.AttackIntervalMs - ParryWindowMs)
        {
            target.ScreenX += 80.0;
            AddFloat(state, target.ScreenX - 20, GroundY - 85, "COUNTER!", "#ff8800");
        }
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
        if (state.DodgeMs > 0)
        {
            AddFloat(state, PlayerScreenX, state.PlayerY - 20, "DODGE!", "#44eeff");
            return;
        }
        if (state.DamageInvincibilityMs > 0) return;

        state.SoundDamage           = true;
        state.DamageInvincibilityMs = 800.0;
        state.PlayerHp              = Math.Max(0, state.PlayerHp - damage);
        AddFloat(state, PlayerScreenX, state.PlayerY - 8, $"-{damage}", color);
    }

    private static void AddFloat(RunnerGameState state, double x, double y, string text, string color) =>
        state.FloatingTexts.Add(new FloatingText { X = x, Y = y, Text = text, Color = color });
}
