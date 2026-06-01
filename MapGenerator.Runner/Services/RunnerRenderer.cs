using MapGenerator.Runner.Models;

namespace MapGenerator.Runner.Services;

/// <summary>
/// Builds a flat list of draw commands from the current game state.
/// Each command is a plain object serialized to JSON and executed by runner.js.
/// </summary>
public static class RunnerRenderer
{
    private const double GroundY = RunnerEngine.GroundY;
    private const double CanvasW = RunnerEngine.CanvasWidth;
    private const double CanvasH = RunnerEngine.CanvasHeight;
    private const double PlayerX = RunnerEngine.PlayerScreenX;

    public static List<DrawCmd> Build(RunnerGameState state, RunnerInitData data)
    {
        var cmds = new List<DrawCmd>();
        var cfg = data.Config;

        // Background
        cmds.Add(DrawCmd.Fill(0, 0, CanvasW, CanvasH, cfg.BackgroundColor));

        // Ground
        cmds.Add(DrawCmd.Fill(0, GroundY, CanvasW, CanvasH - GroundY, cfg.GroundColor));

        // Obstacles
        foreach (var obs in state.Obstacles)
        {
            double sx = obs.WorldX - state.WorldOffset;
            if (sx > CanvasW + 50 || sx + obs.Width < -50) continue;

            if (obs.IsGap)
            {
                DrawSpikes(cmds, sx, obs.Width);
            }
            else
            {
                DrawBoulder(cmds, sx, obs.Width, obs.Height);
            }
        }

        // Defeated enemy fade-out (rendered under live enemies)
        foreach (var enemy in state.Enemies.Where(e => e.IsDefeated && e.DefeatedAnimMs > 0))
        {
            double t      = 1.0 - enemy.DefeatedAnimMs / 600.0;
            double alpha  = 1.0 - t;
            double rise   = t * 60.0;
            bool isBoss   = enemy.Type == EnemyType.Boss;
            double enemyH = isBoss ? 68.0 : 46.0;
            double ey     = GroundY - enemyH - rise;
            double ex     = enemy.ScreenX;

            var ghostCmds = new List<DrawCmd>();
            if (isBoss)                            DrawBoss(ghostCmds, ex, ey);
            else if (enemy.Type == EnemyType.Shooter) DrawArcher(ghostCmds, ex, ey);
            else if (enemy.Type == EnemyType.Thief)   DrawThief(ghostCmds, ex, ey);
            else                                       DrawWarrior(ghostCmds, ex, ey);

            foreach (var cmd in ghostCmds) cmd.Alpha *= alpha;
            cmds.AddRange(ghostCmds);
        }

        // Enemies
        foreach (var enemy in state.Enemies.Where(e => !e.IsDefeated))
        {
            double ex = enemy.ScreenX;
            if (ex < -80 || ex > CanvasW + 20) continue;

            bool isBoss = enemy.Type == EnemyType.Boss;
            double enemyH = isBoss ? 68.0 : 46.0;
            double enemyW = isBoss ? 54.0 : 36.0;

            // Walk bob
            double walkBob = isBoss
                ? Math.Sin(2 * Math.PI * enemy.WalkCycleMs / 700.0) * 1.5
                : Math.Sin(2 * Math.PI * enemy.WalkCycleMs / 460.0) * 2.5;
            double ey = GroundY - enemyH + walkBob;

            // Windup lean: whole body shifts right (away from player) as attack charges,
            // then snaps back to zero the instant the swipe fires
            bool inMeleeRange = enemy.ScreenX < RunnerEngine.PlayerScreenX + RunnerEngine.AttackRange
                             && enemy.ScreenX > RunnerEngine.PlayerScreenX - 20;
            bool windingUp  = inMeleeRange && enemy.AttackCooldownMs < 450
                           && enemy.Type != EnemyType.Shooter && enemy.Type != EnemyType.Boss;
            bool bossWindup = inMeleeRange && isBoss && enemy.AttackCooldownMs < 600;
            double threshold = isBoss ? 600.0 : 450.0;

            double leanOffset = 0;
            if ((windingUp || bossWindup) && enemy.AttackAnimMs <= 0)
            {
                double windAlpha = 1.0 - enemy.AttackCooldownMs / threshold;
                leanOffset = windAlpha * (isBoss ? 10.0 : 6.0);
            }
            double drawEx = ex + leanOffset;

            bool isBlocking = enemy.CanBlock
                && enemy.BlockRecoveryMs <= 0
                && enemy.AttackCooldownMs > threshold
                && enemy.AttackAnimMs <= 0;

            // Block aura (drawn behind body)
            if (isBlocking)
            {
                double auraCx = ex + enemyW / 2;
                double auraCy = ey + enemyH / 2;
                double auraR  = (isBoss ? 44.0 : 30.0);
                var aura = DrawCmd.Circle(auraCx, auraCy, auraR, "#2255bb");
                aura.Alpha = 0.25;
                cmds.Add(aura);
            }

            // Draw body (at leaned position)
            if (isBoss)
                DrawBoss(cmds, drawEx, ey);
            else if (enemy.Type == EnemyType.Shooter)
                DrawArcher(cmds, drawEx, ey);
            else if (enemy.Type == EnemyType.Thief)
                DrawThief(cmds, drawEx, ey);
            else
                DrawWarrior(cmds, drawEx, ey);

            // Attack swipe — crescent pointing left, fires when AttackAnimMs > 0
            if (enemy.AttackAnimMs > 0)
            {
                double animAlpha = enemy.AttackAnimMs / 200.0;
                double swipeCx   = drawEx + enemyW * 0.28;
                double swipeCy   = ey + enemyH * 0.42;
                double innerR    = isBoss ? 30.0 : 18.0;
                double outerR    = isBoss ? 54.0 : 36.0;
                double sAStart   = Math.PI - 1.0;
                double sAEnd     = Math.PI + 1.0;
                const int sN     = 6;

                double[] sPts = new double[sN * 4];
                for (int k = 0; k < sN; k++)
                {
                    double a = sAStart + (sAEnd - sAStart) * k / (sN - 1);
                    sPts[k * 2]     = swipeCx + outerR * Math.Cos(a);
                    sPts[k * 2 + 1] = swipeCy + outerR * Math.Sin(a);
                }
                for (int k = 0; k < sN; k++)
                {
                    double a = sAEnd - (sAEnd - sAStart) * k / (sN - 1);
                    sPts[sN * 2 + k * 2]     = swipeCx + innerR * Math.Cos(a);
                    sPts[sN * 2 + k * 2 + 1] = swipeCy + innerR * Math.Sin(a);
                }
                var swipe = DrawCmd.Poly(sPts, isBoss ? "#ff2200" : "#ff5500");
                swipe.Alpha = animAlpha;
                cmds.Add(swipe);

                double[] sRim = new double[sN * 4];
                for (int k = 0; k < sN; k++)
                {
                    double a = sAStart + (sAEnd - sAStart) * k / (sN - 1);
                    sRim[k * 2]     = swipeCx + outerR * Math.Cos(a);
                    sRim[k * 2 + 1] = swipeCy + outerR * Math.Sin(a);
                }
                for (int k = 0; k < sN; k++)
                {
                    double a = sAEnd - (sAEnd - sAStart) * k / (sN - 1);
                    sRim[sN * 2 + k * 2]     = swipeCx + (outerR - 5) * Math.Cos(a);
                    sRim[sN * 2 + k * 2 + 1] = swipeCy + (outerR - 5) * Math.Sin(a);
                }
                var rimCmd = DrawCmd.Poly(sRim, "#ffcc88");
                rimCmd.Alpha = animAlpha * 0.6;
                cmds.Add(rimCmd);
            }

            double hpPct    = (double)enemy.Hp / enemy.MaxHp;
            string barColor = isBoss ? "#cc0000" : "#cc3333";
            double barY     = ey - (isBoss ? 10 : 8);
            cmds.Add(DrawCmd.Bar(ex - (isBoss ? 9 : 0), barY, enemyW, isBoss ? 6 : 4, hpPct, barColor));
            if (isBoss)
                cmds.Add(DrawCmd.Text("BOSS", ex + enemyW / 2 - 9, barY - 2, "#ff4444", "bold 10px monospace"));

            double indX = ex + enemyW * 0.5 - (isBoss ? 0 : 2);
            double indY = barY - 14;

            if (isBlocking)
            {
                // Shield icon
                double shX = indX, shY = indY - 4;
                cmds.Add(DrawCmd.Poly([
                    shX - 6, shY - 5,  shX + 6, shY - 5,
                    shX + 7, shY + 0,  shX + 5, shY + 5,
                    shX,     shY + 9,
                    shX - 5, shY + 5,  shX - 7, shY + 0,
                ], "#3377dd"));
                cmds.Add(DrawCmd.Poly([
                    shX - 3, shY - 3,  shX + 3, shY - 3,
                    shX + 4, shY + 1,  shX + 2, shY + 5,
                    shX,     shY + 7,
                    shX - 2, shY + 5,  shX - 4, shY + 1,
                ], "#88bbff"));
            }
            else if ((windingUp || bossWindup) && enemy.AttackAnimMs <= 0)
            {
                // Windup "!" indicator
                double windAlpha = 1.0 - enemy.AttackCooldownMs / threshold;
                cmds.Add(DrawCmd.Arc(indX, indY, 9, 0, Math.PI * 2, "#ff2200", 2, windAlpha * 0.9));
                var excl = DrawCmd.Text("!", indX, indY + 5, "#ff4444", "bold 14px monospace", "center");
                excl.Alpha = windAlpha;
                cmds.Add(excl);
            }
        }

        // Treasure chest
        if (state.Chest != null)
        {
            double csx = state.Chest.WorldX - state.WorldOffset;
            if (csx < CanvasW + 40 && csx + RunnerEngine.ChestWidth > -10)
                DrawChest(cmds, csx, GroundY - RunnerEngine.ChestHeight);
        }

        // Projectiles
        foreach (var proj in state.Projectiles)
        {
            if (proj.X + 26 < 0 || proj.X > CanvasW + 10) continue;
            if (proj.IsShockwave)
            {
                // Ground-level shockwave from boss: wider, orange glow
                cmds.Add(DrawCmd.Fill(proj.X - 16, proj.Y - 10, 32, 18, proj.Color));
                cmds.Add(DrawCmd.Fill(proj.X - 20, proj.Y - 6, 8,  10, "#ffcc44")); // leading edge
                cmds.Add(DrawCmd.Fill(proj.X - 14, proj.Y - 14, 24,  4, "#ffcc44")); // spark on top
            }
            else
            {
                cmds.Add(DrawCmd.Fill(proj.X - 12, proj.Y - 5, 22, 10, proj.Color));
                cmds.Add(DrawCmd.Fill(proj.X - 14, proj.Y - 4,  5,  8, "#ffffff"));
            }
        }

        // Pickups (scrolling world-space; drawn before spiders so player appears on top)
        foreach (var pickup in state.Pickups)
        {
            double px = pickup.WorldX - state.WorldOffset;
            if (px + RunnerEngine.PickupWidth < -10 || px > CanvasW + 10) continue;
            double py = GroundY - RunnerEngine.PickupHeight;
            DrawPickup(cmds, px, py, pickup.Type);
        }

        // Spiders (web + oscillating body + 8 legs)
        foreach (var spider in state.Spiders)
        {
            double bx = spider.WorldX - state.WorldOffset;
            if (bx + 25 < 0 || bx - 20 > CanvasW) continue;

            double by = RunnerEngine.SpiderCenterY
                      + RunnerEngine.SpiderAmplitude
                      * Math.Sin(2 * Math.PI * spider.PhaseMs / RunnerEngine.SpiderPeriodMs);

            DrawSpider(cmds, bx, by);
        }

        // Player: walk bob + squash/stretch
        double playerBob   = state.IsGrounded ? WalkBob(state.WalkCycleMs, 2.0, 380.0) : 0;
        double playerSy    = JumpScaleY(state.PlayerVelocityY, state.LandImpactMs);
        double playerDrawH = RunnerEngine.PlayerHeight * playerSy;
        double playerDrawY = state.PlayerY + RunnerEngine.PlayerHeight - playerDrawH + playerBob;

        double playerAlpha = state.DodgeMs > 0 ? 0.38 : 1.0;
        DrawCmd playerCmd  = data.PlayerSprite.Length > 0
            ? DrawCmd.Sprite("player", PlayerX, playerDrawY, RunnerEngine.PlayerWidth, playerDrawH)
            : DrawCmd.Fill(PlayerX, playerDrawY, RunnerEngine.PlayerWidth, playerDrawH, "#5599ff");
        playerCmd.Alpha = playerAlpha;
        cmds.Add(playerCmd);

        // Companion: walk bob (opposite phase) + squash/stretch, 2px/cell = 16px
        const double CompDrawSize = 8 * 4; // 3px/cell = 24px
        double companionBob   = state.CompanionIsGrounded ? WalkBob(state.CompanionWalkCycleMs + 190.0, 1.5, 380.0) : 0;
        double companionSy    = JumpScaleY(state.CompanionVelocityY, state.CompanionLandImpactMs);
        double compDrawH      = CompDrawSize * companionSy;
        double companionX     = PlayerX - 30;
        double companionDrawY = state.CompanionY + RunnerEngine.CompanionHeight - compDrawH + companionBob;

        if (data.CompanionSprite.Length > 0)
            cmds.Add(DrawCmd.Sprite("companion", companionX, companionDrawY, CompDrawSize, compDrawH));
        else if (!string.IsNullOrEmpty(data.CompanionName))
            cmds.Add(DrawCmd.Fill(companionX, companionDrawY, CompDrawSize, compDrawH, "#80cc80"));

        // HUD: player HP bar
        cmds.Add(DrawCmd.Bar(10, 10, 140, 12, (double)state.PlayerHp / state.PlayerMaxHp, "#44cc44"));
        cmds.Add(DrawCmd.Text($"HP {state.PlayerHp}/{state.PlayerMaxHp}", 10, 36, "#aaffaa", "11px monospace"));

        // HUD: enemies remaining / chest indicator
        int totalEnemies   = data.Config.EnemyCount;
        int defeated       = state.Enemies.Count(e => e.IsDefeated);
        int remaining      = totalEnemies - defeated;
        double enemyProgress = totalEnemies > 0 ? (double)defeated / totalEnemies : 0;

        if (state.Chest != null)
        {
            // Chest exists — show progress bar in gold and a "find the chest" nudge
            double chestSx = state.Chest.WorldX - state.WorldOffset;
            cmds.Add(DrawCmd.Bar(CanvasW - 160, 10, 150, 10, 1.0, "#eecc44"));
            if (chestSx > CanvasW)
                cmds.Add(DrawCmd.Text("▶ Treasure ahead!", CanvasW - 10, 36, "#eecc44", "bold 11px monospace", "right"));
            else
                cmds.Add(DrawCmd.Text("Reach the chest!", CanvasW - 10, 36, "#eecc44", "bold 11px monospace", "right"));
        }
        else
        {
            cmds.Add(DrawCmd.Bar(CanvasW - 160, 10, 150, 10, enemyProgress, "#cc4444"));
            string enemyLabel = remaining > 0 ? $"{remaining} enem{(remaining == 1 ? "y" : "ies")} left" : "All defeated!";
            cmds.Add(DrawCmd.Text(enemyLabel, CanvasW - 10, 36, "#ffaaaa", "11px monospace", "right"));
        }

        // HUD: dodge bar (always shown)
        double dodgeCharge = state.DodgeCooldownMs > 0
            ? 1.0 - state.DodgeCooldownMs / RunnerEngine.DodgeRechargeDurationMs
            : 1.0;
        string dodgeBarColor = state.DodgeMs > 0 ? "#88eeff" : (state.DodgeCooldownMs > 0 ? "#224466" : "#44aaff");
        cmds.Add(DrawCmd.Bar(10, 44, 80, 6, dodgeCharge, dodgeBarColor));
        string dodgeLabel = state.DodgeMs > 0 ? "DODGING" : (state.DodgeCooldownMs > 0 ? "DODGE CD" : "DODGE ✓");
        cmds.Add(DrawCmd.Text(dodgeLabel, 10, 62, "#88ccff", "10px monospace"));

        // HUD: attack boost timer
        if (state.AttackBoostMs > 0)
        {
            double boostPct = state.AttackBoostMs / RunnerEngine.AttackBoostDurationMs;
            cmds.Add(DrawCmd.Bar(10, 66, 80, 6, boostPct, "#ff8800"));
            cmds.Add(DrawCmd.Text("ATK ×2", 10, 84, "#ffcc44", "bold 10px monospace"));
        }

        // HUD: attack cooldown indicator
        if (state.AttackCooldownMs > 0)
        {
            double cdPct = state.AttackCooldownMs / 500.0;
            double cdY = state.AttackBoostMs > 0 ? 88 : 66;
            cmds.Add(DrawCmd.Bar(10, cdY, 80, 6, 1.0 - cdPct, "#ffaa00"));
            cmds.Add(DrawCmd.Text("ATK CD", 10, cdY + 18, "#ffaa88", "10px monospace"));
        }

        // Attack swipe — crescent slash polygon
        if (state.AttackAnimMs > 0)
        {
            double alpha = state.AttackAnimMs / 220.0;
            double cx    = PlayerX + RunnerEngine.PlayerWidth + 10;
            double cy    = state.PlayerY + RunnerEngine.PlayerHeight * 0.38;

            const double InnerR = 24, OuterR = 48;
            const double AStart = -1.4, AEnd = 0.9;
            const int N = 7;

            // Build crescent: N points along outer arc, then N along inner arc reversed
            double[] pts = new double[N * 4];
            for (int k = 0; k < N; k++)
            {
                double a = AStart + (AEnd - AStart) * k / (N - 1);
                pts[k * 2]     = cx + OuterR * Math.Cos(a);
                pts[k * 2 + 1] = cy + OuterR * Math.Sin(a);
            }
            for (int k = 0; k < N; k++)
            {
                double a = AEnd - (AEnd - AStart) * k / (N - 1);
                pts[N * 2 + k * 2]     = cx + InnerR * Math.Cos(a);
                pts[N * 2 + k * 2 + 1] = cy + InnerR * Math.Sin(a);
            }
            var slash = DrawCmd.Poly(pts, "#ffdd44");
            slash.Alpha = alpha;
            cmds.Add(slash);

            // Bright edge highlight (thin outer rim)
            double[] rim = new double[N * 4];
            for (int k = 0; k < N; k++)
            {
                double a = AStart + (AEnd - AStart) * k / (N - 1);
                rim[k * 2]     = cx + OuterR * Math.Cos(a);
                rim[k * 2 + 1] = cy + OuterR * Math.Sin(a);
            }
            for (int k = 0; k < N; k++)
            {
                double a = AEnd - (AEnd - AStart) * k / (N - 1);
                rim[N * 2 + k * 2]     = cx + (OuterR - 5) * Math.Cos(a);
                rim[N * 2 + k * 2 + 1] = cy + (OuterR - 5) * Math.Sin(a);
            }
            var edge = DrawCmd.Poly(rim, "#ffffff");
            edge.Alpha = alpha * 0.65;
            cmds.Add(edge);

            // Impact burst at the tip of the slash
            double impX = cx + OuterR * Math.Cos(AEnd);
            double impY = cy + OuterR * Math.Sin(AEnd);
            var star = DrawCmd.Poly([impX-10, impY, impX, impY-10, impX+10, impY, impX, impY+10], "#ffffff");
            star.Alpha = alpha;
            cmds.Add(star);
            var innerStar = DrawCmd.Poly([impX-5, impY-5, impX+5, impY-5, impX+5, impY+5, impX-5, impY+5], "#ffee88");
            innerStar.Alpha = alpha * 0.75;
            cmds.Add(innerStar);
        }

        // Floating texts
        foreach (var ft in state.FloatingTexts)
            cmds.Add(DrawCmd.Text(ft.Text, ft.X, ft.Y, ft.Color, "bold 13px monospace"));

        return cmds;
    }

    // ── Obstacle drawing ─────────────────────────────────────────────────────

    /// <summary>
    /// Stone spike field. Fills the obstacle width with 4 unevenly-spaced spikes,
    /// each split into a lit left face and a shadowed right face.
    /// </summary>
    private static void DrawSpikes(List<DrawCmd> cmds, double sx, double width)
    {
        // (tip-offset-from-sx, base-left-offset, base-right-offset, height)
        (double tip, double bl, double br, double h)[] spikes =
        [
            (7,   0,  15, 22),
            (21,  13, 30, 28),
            (35,  27, 44, 20),
            (50,  42, width, 25),
        ];

        foreach (var (tip, bl, br, h) in spikes)
        {
            double tx = sx + tip, bly = GroundY, ty = GroundY - h;
            // Left face — lighter
            cmds.Add(DrawCmd.Poly([sx + bl, bly, tx, bly, tx, ty], "#8a8a9e"));
            // Right face — darker
            cmds.Add(DrawCmd.Poly([tx, bly, sx + br, bly, tx, ty], "#3e3e52"));
            // Tip highlight sliver
            cmds.Add(DrawCmd.Poly([tx - 1, ty + 3, tx + 1, ty + 3, tx, ty], "#c0c0d8"));
        }
    }

    /// <summary>
    /// Rocky boulder. Irregular polygon silhouette with top-surface highlight,
    /// left-face shading, and a diagonal crack.
    /// </summary>
    private static void DrawBoulder(List<DrawCmd> cmds, double sx, double w, double h)
    {
        double top = GroundY - h;

        // Main body — irregular octagon
        cmds.Add(DrawCmd.Poly([
            sx + 3,     GroundY,
            sx + w - 2, GroundY,
            sx + w,     GroundY - h * 0.28,
            sx + w - 3, GroundY - h * 0.84,
            sx + w * 0.6, top,
            sx + w * 0.25, top + h * 0.04,
            sx + 2,     GroundY - h * 0.72,
            sx,         GroundY - h * 0.36,
        ], "#808090"));

        // Top-surface highlight
        cmds.Add(DrawCmd.Poly([
            sx + w * 0.25, top + h * 0.04,
            sx + w * 0.6,  top,
            sx + w - 3,    GroundY - h * 0.84,
            sx + w * 0.72, GroundY - h * 0.65,
            sx + w * 0.3,  GroundY - h * 0.62,
        ], "#a8a8bc"));

        // Left face shading
        cmds.Add(DrawCmd.Poly([
            sx,         GroundY - h * 0.36,
            sx + 2,     GroundY - h * 0.72,
            sx + w * 0.25, top + h * 0.04,
            sx + w * 0.3,  GroundY - h * 0.62,
            sx + w * 0.14, GroundY - h * 0.4,
        ], "#606070"));

        // Crack (diagonal dark stripe)
        cmds.Add(DrawCmd.Poly([
            sx + w * 0.42, top + h * 0.06,
            sx + w * 0.46, top + h * 0.06,
            sx + w * 0.62, GroundY - h * 0.35,
            sx + w * 0.58, GroundY - h * 0.35,
        ], "#3a3a4a"));
    }

    // ── Pickup drawing ───────────────────────────────────────────────────────

    private static void DrawPickup(List<DrawCmd> cmds, double px, double py, PickupType type)
    {
        if (type == PickupType.HealthPotion)
        {
            // Red glass bottle: narrow neck, round body
            cmds.Add(DrawCmd.Fill(px + 4, py,      6, 6,  "#882222")); // cork/cap
            cmds.Add(DrawCmd.Fill(px + 5, py + 5,  4, 4,  "#cc3333")); // neck
            cmds.Add(DrawCmd.Poly([px+2, py+8, px+12, py+8, px+14, py+12,
                                   px+12, py+20, px+2, py+20, px, py+12], "#cc2222")); // body
            cmds.Add(DrawCmd.Poly([px+4, py+9, px+9, py+9, px+10, py+12,
                                   px+8, py+18, px+4, py+18, px+2, py+12], "#ee4444")); // highlight
            cmds.Add(DrawCmd.Fill(px + 5, py + 10, 3, 5, "#ff8888")); // inner shine
        }
        else // AttackBoost
        {
            // Orange/gold diamond crystal
            double cx = px + 7, cy = py + 11;
            cmds.Add(DrawCmd.Poly([cx, cy-11, cx+7, cy-3, cx+7, cy+4, cx, cy+11, cx-7, cy+4, cx-7, cy-3], "#cc6600")); // outer
            cmds.Add(DrawCmd.Poly([cx, cy-9,  cx+5, cy-2, cx+5, cy+3, cx, cy+9,  cx-5, cy+3, cx-5, cy-2], "#ff8800")); // mid
            cmds.Add(DrawCmd.Poly([cx-2, cy-4, cx+3, cy-4, cx+4, cy+1, cx, cy+6, cx-4, cy+1], "#ffcc44")); // inner bright
            cmds.Add(DrawCmd.Fill(cx - 1, cy - 7, 2, 4, "#ffffff")); // top shine
        }
    }

    // ── Spider drawing ───────────────────────────────────────────────────────

    /// <summary>
    /// Spider hanging on a web. bx/by = body center in screen space.
    /// Faces left (front legs reach toward the player, abdomen trails right).
    /// </summary>
    private static void DrawSpider(List<DrawCmd> cmds, double bx, double by)
    {
        // Web string from ceiling to body top
        cmds.Add(DrawCmd.Fill(bx - 1, 0, 2, by - 6, "#ccbb88"));

        // Back legs (trailing right, toward canvas right — drawn first so body overlaps)
        cmds.Add(DrawCmd.Poly([bx+4, by-4, bx+5, by-3, bx+18, by-10, bx+19, by-11], "#1a0a00"));
        cmds.Add(DrawCmd.Poly([bx+6, by-1, bx+7, by,   bx+20, by- 2, bx+21, by- 3], "#1a0a00"));
        cmds.Add(DrawCmd.Poly([bx+6, by+3, bx+7, by+4, bx+19, by+10, bx+18, by+11], "#1a0a00"));
        cmds.Add(DrawCmd.Poly([bx+4, by+7, bx+5, by+8, bx+14, by+17, bx+13, by+18], "#1a0a00"));

        // Abdomen (larger rear section)
        cmds.Add(DrawCmd.Poly([
            bx+ 3, by- 3,
            bx+ 9, by- 5,
            bx+15, by- 3,
            bx+17, by+ 2,
            bx+15, by+ 7,
            bx+ 9, by+ 9,
            bx+ 3, by+ 7,
        ], "#2a1505"));
        // Abdomen highlight stripe
        cmds.Add(DrawCmd.Poly([bx+6, by-4, bx+12, by-3, bx+13, by, bx+6, by-1], "#3a2008"));

        // Cephalothorax (head + thorax, forward section)
        cmds.Add(DrawCmd.Poly([
            bx- 5, by- 2,
            bx- 2, by- 6,
            bx+ 4, by- 6,
            bx+ 6, by- 2,
            bx+ 5, by+ 4,
            bx- 1, by+ 5,
            bx- 5, by+ 2,
        ], "#1a0a00"));

        // Eyes (red, glowing)
        cmds.Add(DrawCmd.Fill(bx - 4, by - 5, 2, 2, "#cc2200"));
        cmds.Add(DrawCmd.Fill(bx - 1, by - 6, 2, 2, "#cc2200"));

        // Front legs (reaching left toward player — drawn on top)
        cmds.Add(DrawCmd.Poly([bx-3, by-5, bx-2, by-4, bx-16, by-11, bx-17, by-12], "#1a0a00"));
        cmds.Add(DrawCmd.Poly([bx-5, by-1, bx-4, by,   bx-18, by- 2, bx-19, by- 3], "#1a0a00"));
        cmds.Add(DrawCmd.Poly([bx-5, by+3, bx-4, by+4, bx-16, by+10, bx-17, by+ 9], "#1a0a00"));
        cmds.Add(DrawCmd.Poly([bx-3, by+5, bx-2, by+6, bx-11, by+15, bx-12, by+14], "#1a0a00"));
    }

    // ── Chest drawing ────────────────────────────────────────────────────────

    /// <summary>
    /// Treasure chest, 30×32 bounding box. cx,cy = top-left corner.
    /// </summary>
    private static void DrawChest(List<DrawCmd> cmds, double cx, double cy)
    {
        const double W = 30, bodyH = 20, lidH = 12;

        // Body
        cmds.Add(DrawCmd.Fill(cx,      cy + lidH, W,     bodyH, "#7a4a1e"));
        cmds.Add(DrawCmd.Fill(cx + 22, cy + lidH, W - 22, bodyH, "#5a3010")); // right shading

        // Lid (trapezoid — slightly wider than body)
        cmds.Add(DrawCmd.Poly([cx - 1, cy + lidH, cx + W + 1, cy + lidH, cx + W - 1, cy, cx + 1, cy], "#9b6328"));
        cmds.Add(DrawCmd.Poly([cx + 21, cy + lidH, cx + W + 1, cy + lidH, cx + W - 1, cy, cx + 20, cy], "#7a4a1e")); // lid shading

        // Gold bands
        cmds.Add(DrawCmd.Fill(cx - 1, cy + lidH - 2, W + 2, 4, "#eecc44")); // seam band
        cmds.Add(DrawCmd.Fill(cx,     cy + lidH + 9,  W,    3, "#eecc44")); // body band

        // Lock
        cmds.Add(DrawCmd.Fill(cx + 11, cy + lidH - 5, 8, 7, "#eecc44")); // lock plate
        cmds.Add(DrawCmd.Fill(cx + 13, cy + lidH - 7, 4, 3, "#eecc44")); // lock shackle
        cmds.Add(DrawCmd.Fill(cx + 14, cy + lidH - 3, 2, 3, "#5a3010")); // keyhole
    }

    // ── Enemy drawing ─────────────────────────────────────────────────────────

    /// <summary>
    /// Enormous dark-armoured boss, 54×68 bounding box relative to (ex, ey).
    /// Faces LEFT. Horned helmet, massive shoulder plates, glowing axe.
    /// </summary>
    private static void DrawBoss(List<DrawCmd> cmds, double ex, double ey)
    {
        // --- Legs ---
        cmds.Add(DrawCmd.Poly([ex+12, ey+54, ex+26, ey+54, ex+24, ey+68, ex+10, ey+68], "#1a1a2a")); // left
        cmds.Add(DrawCmd.Poly([ex+28, ey+54, ex+44, ey+54, ex+42, ey+68, ex+26, ey+68], "#1a1a2a")); // right
        cmds.Add(DrawCmd.Fill(ex+ 8, ey+62, 20, 6, "#0d0d18")); // boot left
        cmds.Add(DrawCmd.Fill(ex+24, ey+62, 20, 6, "#0d0d18")); // boot right

        // --- Greaves / lower armor ---
        cmds.Add(DrawCmd.Poly([ex+10, ey+46, ex+44, ey+46, ex+42, ey+54, ex+12, ey+54], "#252535"));

        // --- Belt / waist ---
        cmds.Add(DrawCmd.Poly([ex+10, ey+42, ex+44, ey+42, ex+42, ey+46, ex+12, ey+46], "#440022"));
        cmds.Add(DrawCmd.Fill(ex+24, ey+42, 6, 4, "#880044")); // buckle glow

        // --- Torso ---
        cmds.Add(DrawCmd.Poly([ex+ 8, ey+24, ex+46, ey+24, ex+44, ey+42, ex+10, ey+42], "#1a1a2a"));
        // Chest plates (two panels)
        cmds.Add(DrawCmd.Poly([ex+10, ey+24, ex+26, ey+24, ex+24, ey+40, ex+12, ey+40], "#252535"));
        cmds.Add(DrawCmd.Poly([ex+28, ey+24, ex+44, ey+24, ex+42, ey+40, ex+30, ey+40], "#252535"));
        // Center spine glow
        cmds.Add(DrawCmd.Fill(ex+25, ey+24, 4, 18, "#880022"));

        // --- Shoulder pauldrons (massive, flanking the body) ---
        cmds.Add(DrawCmd.Poly([ex- 8, ey+20, ex+14, ey+16, ex+16, ey+34, ex+ 2, ey+38], "#330044")); // left
        cmds.Add(DrawCmd.Poly([ex+38, ey+16, ex+62, ey+20, ex+52, ey+38, ex+38, ey+34], "#330044")); // right
        // Pauldron highlights
        cmds.Add(DrawCmd.Poly([ex- 6, ey+22, ex+12, ey+18, ex+14, ey+28, ex+ 2, ey+30], "#440066"));
        cmds.Add(DrawCmd.Poly([ex+40, ey+18, ex+58, ey+22, ex+50, ey+30, ex+40, ey+28], "#440066"));

        // --- Helmet (large, angular) ---
        cmds.Add(DrawCmd.Poly([
            ex- 4, ey+18, ex+ 2, ey+ 6, ex+ 8, ey+ 2,
            ex+32, ey+ 0, ex+50, ey+ 4, ex+54, ey+12,
            ex+52, ey+22, ex+ 4, ey+22,
        ], "#1e1e30"));
        // Upper face-plate highlight
        cmds.Add(DrawCmd.Poly([ex+8, ey+3, ex+32, ey+1, ex+48, ey+5, ex+50, ey+12, ex+10, ey+12], "#2a2a48"));

        // --- Horns (red, curving up) ---
        cmds.Add(DrawCmd.Poly([ex+ 4, ey+ 6, ex+10, ey+ 0, ex+ 8, ey-12, ex+ 2, ey+ 2], "#cc2200")); // left
        cmds.Add(DrawCmd.Poly([ex+44, ey+ 0, ex+50, ey+ 6, ex+52, ey+ 2, ex+46, ey-12], "#cc2200")); // right

        // --- Eyes / visor (glowing red slit) ---
        cmds.Add(DrawCmd.Fill(ex+ 2, ey+12, 22, 5, "#880000"));
        cmds.Add(DrawCmd.Fill(ex+ 2, ey+13, 20, 3, "#cc0000"));
        cmds.Add(DrawCmd.Fill(ex+ 4, ey+14,  8, 1, "#ff2200")); // bright left eye

        // --- Axe weapon (huge, extends left) ---
        // Handle
        cmds.Add(DrawCmd.Fill(ex+10, ey+18, 10, 40, "#554433"));
        // Axe head (large dark blade)
        cmds.Add(DrawCmd.Poly([ex-22, ey+28, ex+10, ey+16, ex+12, ey+50, ex+ 4, ey+54], "#1a1a2a"));
        // Blade edge (silver)
        cmds.Add(DrawCmd.Poly([ex-18, ey+30, ex+10, ey+18, ex+10, ey+22, ex-10, ey+30], "#9999cc"));
        cmds.Add(DrawCmd.Poly([ex-12, ey+36, ex+10, ey+44, ex+10, ey+50, ex- 4, ey+50], "#666688"));
        // Crossguard
        cmds.Add(DrawCmd.Fill(ex+ 8, ey+14, 6, 6, "#eecc44")); // top guard
        cmds.Add(DrawCmd.Fill(ex+ 4, ey+22, 12, 4, "#eecc44")); // horizontal guard
    }

    /// <summary>
    /// Heavily-armoured melee warrior, 36×46 bounding box relative to (ex, ey).
    /// Profile faces LEFT: helmet visor and sword tip both point toward the player.
    /// </summary>
    private static void DrawWarrior(List<DrawCmd> cmds, double ex, double ey)
    {
        // --- Legs ---
        cmds.Add(DrawCmd.Poly([ex+7, ey+40, ex+16, ey+40, ex+15, ey+46, ex+6, ey+46],   "#bb2222")); // left
        cmds.Add(DrawCmd.Poly([ex+19, ey+40, ex+28, ey+40, ex+27, ey+46, ex+18, ey+46], "#bb2222")); // right
        // Boot caps
        cmds.Add(DrawCmd.Fill(ex+5,  ey+43, 12, 3, "#331108"));
        cmds.Add(DrawCmd.Fill(ex+17, ey+43, 12, 3, "#331108"));

        // --- Belt ---
        cmds.Add(DrawCmd.Poly([ex+6, ey+36, ex+29, ey+36, ex+27, ey+40, ex+8, ey+40], "#553322"));
        cmds.Add(DrawCmd.Fill(ex+16, ey+36, 5, 4, "#eecc44")); // buckle

        // --- Torso (trapezoid: wide at shoulders, narrower at waist) ---
        cmds.Add(DrawCmd.Poly([ex+4, ey+18, ex+32, ey+18, ex+29, ey+36, ex+6, ey+36], "#cc3333"));
        // Right-side shading
        cmds.Add(DrawCmd.Poly([ex+24, ey+18, ex+32, ey+18, ex+29, ey+36, ex+22, ey+36], "#991111"));
        // Chest cross emblem
        cmds.Add(DrawCmd.Fill(ex+13, ey+22, 10, 2, "#eecc44")); // horizontal bar
        cmds.Add(DrawCmd.Fill(ex+17, ey+19, 2,  9, "#eecc44")); // vertical bar

        // --- Left shoulder pauldron (triangle jutting forward/left) ---
        cmds.Add(DrawCmd.Poly([ex-4, ey+21, ex+8, ey+15, ex+8, ey+27], "#aa2222"));

        // --- Helmet (angular octagon with left-pointing visor) ---
        cmds.Add(DrawCmd.Poly([
            ex+ 0, ey+ 6,
            ex+ 4, ey+ 1,
            ex+20, ey+ 0,
            ex+24, ey+ 4,
            ex+24, ey+16,
            ex+20, ey+18,
            ex+ 0, ey+18,
            ex- 3, ey+12,
        ], "#777788"));
        // Upper face-plate highlight
        cmds.Add(DrawCmd.Poly([ex+4, ey+2, ex+20, ey+1, ex+22, ey+8, ex+6, ey+9], "#9999bb"));
        // Visor slit
        cmds.Add(DrawCmd.Fill(ex+0, ey+8, 15, 4, "#111122"));

        // --- Sword ---
        // Handle (behind body, drawn first)
        cmds.Add(DrawCmd.Fill(ex+10, ey+20, 8, 8, "#886644"));
        // Cross-guard
        cmds.Add(DrawCmd.Fill(ex+ 8, ey+17, 4, 14, "#eecc44")); // vertical
        cmds.Add(DrawCmd.Fill(ex+ 4, ey+23, 8,  4, "#eecc44")); // horizontal
        // Blade (triangle, tip points left)
        cmds.Add(DrawCmd.Poly([ex-10, ey+25, ex+8, ey+21, ex+8, ey+29], "#ccccdd"));
        // Blade highlight edge
        cmds.Add(DrawCmd.Poly([ex- 8, ey+25, ex+8, ey+21, ex+8, ey+22], "#ffffff"));
    }

    /// <summary>
    /// Lean hooded archer, 36×46 bounding box relative to (ex, ey).
    /// Profile faces LEFT: pointed hood and drawn bow both point toward the player.
    /// </summary>
    private static void DrawArcher(List<DrawCmd> cmds, double ex, double ey)
    {
        // --- Quiver on back (drawn first so body overlaps it) ---
        cmds.Add(DrawCmd.Poly([ex+24, ey+8, ex+32, ey+6, ex+34, ey+22, ex+24, ey+24], "#8b4513"));
        cmds.Add(DrawCmd.Fill(ex+25, ey+4, 2, 6, "#ccccdd")); // arrow
        cmds.Add(DrawCmd.Fill(ex+28, ey+3, 2, 6, "#ccccdd")); // arrow
        cmds.Add(DrawCmd.Fill(ex+31, ey+4, 2, 5, "#ccccdd")); // arrow

        // --- Cloak flap (back, visible behind body) ---
        cmds.Add(DrawCmd.Poly([ex+22, ey+18, ex+36, ey+22, ex+34, ey+38, ex+22, ey+36], "#4a3020"));

        // --- Legs ---
        cmds.Add(DrawCmd.Poly([ex+7, ey+38, ex+16, ey+38, ex+15, ey+46, ex+6, ey+46],   "#664433")); // left
        cmds.Add(DrawCmd.Poly([ex+18, ey+38, ex+26, ey+38, ex+25, ey+46, ex+17, ey+46], "#664433")); // right
        // Boot caps
        cmds.Add(DrawCmd.Fill(ex+5,  ey+43, 11, 3, "#2a1810"));
        cmds.Add(DrawCmd.Fill(ex+16, ey+43, 11, 3, "#2a1810"));

        // --- Belt ---
        cmds.Add(DrawCmd.Poly([ex+6, ey+34, ex+24, ey+34, ex+22, ey+38, ex+8, ey+38], "#553322"));

        // --- Torso (slim trapezoid) ---
        cmds.Add(DrawCmd.Poly([ex+4, ey+18, ex+24, ey+18, ex+22, ey+34, ex+6, ey+34], "#8b6644"));
        // Leather strap diagonal
        cmds.Add(DrawCmd.Poly([ex+4, ey+18, ex+14, ey+18, ex+10, ey+32, ex+6, ey+32], "#664433"));

        // --- Hood (elongated point jutting left — key silhouette) ---
        cmds.Add(DrawCmd.Poly([
            ex- 4, ey+14,   // sharp point facing left
            ex+ 0, ey+ 4,
            ex+ 6, ey+ 0,
            ex+20, ey+ 0,
            ex+24, ey+ 6,
            ex+22, ey+18,
            ex+ 0, ey+18,
        ], "#3a6622"));
        // Hood highlight ridge
        cmds.Add(DrawCmd.Poly([ex+4, ey+3, ex+18, ey+2, ex+20, ey+9, ex+8, ey+11], "#4d8830"));
        // Shadow under hood brim (face area)
        cmds.Add(DrawCmd.Fill(ex+0, ey+10, 10, 6, "#1a2a10"));
        // Single visible eye
        cmds.Add(DrawCmd.Fill(ex+2, ey+12, 2, 2, "#ffddcc"));

        // --- Bow (arc on left side, convex toward player) ---
        // Angles sweep through π (leftward) so the curve faces left, string faces right.
        double bowCx = ex + 14, bowCy = ey + 24;
        cmds.Add(DrawCmd.Arc(bowCx, bowCy, 18, Math.PI - 1.15, Math.PI + 1.15, "#9b6328", 3, 1.0));
        // Bowstring: vertical line at the endpoint x (bowCx - 18·cos(1.15) ≈ bowCx - 7.4)
        cmds.Add(DrawCmd.Fill(ex + 7, ey + 8, 1, 32, "#cccccc"));
    }

    /// <summary>
    /// Fast dark thief, 36×46 bounding box relative to (ex, ey).
    /// Slim build, glowing red eyes, short purple-guard dagger.
    /// </summary>
    private static void DrawThief(List<DrawCmd> cmds, double ex, double ey)
    {
        // --- Legs (slim) ---
        cmds.Add(DrawCmd.Poly([ex+ 8, ey+38, ex+14, ey+38, ex+13, ey+46, ex+ 7, ey+46], "#1a1528"));
        cmds.Add(DrawCmd.Poly([ex+16, ey+38, ex+22, ey+38, ex+21, ey+46, ex+15, ey+46], "#1a1528"));
        cmds.Add(DrawCmd.Fill(ex+ 6, ey+43,  9, 3, "#0d0b15")); // boot left
        cmds.Add(DrawCmd.Fill(ex+14, ey+43,  9, 3, "#0d0b15")); // boot right

        // --- Belt with coin pouch ---
        cmds.Add(DrawCmd.Poly([ex+7, ey+34, ex+22, ey+34, ex+20, ey+38, ex+9, ey+38], "#884488"));
        cmds.Add(DrawCmd.Fill(ex+7, ey+34, 5, 5, "#553322")); // pouch

        // --- Body (slim, slight forward lean) ---
        cmds.Add(DrawCmd.Poly([ex+4, ey+16, ex+24, ey+16, ex+22, ey+34, ex+7, ey+34], "#2a2035"));
        cmds.Add(DrawCmd.Poly([ex+18, ey+16, ex+24, ey+16, ex+22, ey+34, ex+16, ey+34], "#1a1025")); // shading
        cmds.Add(DrawCmd.Poly([ex+ 4, ey+16, ex+ 8, ey+16, ex+ 9, ey+34, ex+ 7, ey+34], "#553366")); // purple cloak edge

        // --- Hood (angular, aggressive front point) ---
        cmds.Add(DrawCmd.Poly([
            ex- 2, ey+14,
            ex+  0, ey+ 6,
            ex+  5, ey+ 2,
            ex+ 18, ey+ 0,
            ex+ 22, ey+ 4,
            ex+ 20, ey+16,
            ex+  0, ey+16,
        ], "#2a2035"));
        cmds.Add(DrawCmd.Poly([ex+4, ey+3, ex+16, ey+2, ex+18, ey+8, ex+6, ey+10], "#3a3050")); // ridge
        cmds.Add(DrawCmd.Fill(ex+0, ey+8, 11, 6, "#0d0b15")); // dark face
        // Glowing red eyes
        cmds.Add(DrawCmd.Fill(ex+1, ey+10, 3, 2, "#cc2222"));
        cmds.Add(DrawCmd.Fill(ex+5, ey+10, 2, 2, "#cc2222"));

        // --- Dagger (short, purple guard) ---
        cmds.Add(DrawCmd.Fill(ex+8,  ey+22, 5, 6, "#553322")); // handle
        cmds.Add(DrawCmd.Fill(ex+6,  ey+21, 3, 8, "#884488")); // guard vertical
        cmds.Add(DrawCmd.Fill(ex+4,  ey+24, 5, 3, "#884488")); // guard horizontal
        cmds.Add(DrawCmd.Poly([ex-4, ey+26, ex+6, ey+23, ex+6, ey+29], "#ccccdd")); // blade
        cmds.Add(DrawCmd.Poly([ex-2, ey+26, ex+6, ey+23, ex+6, ey+24], "#ffffff")); // edge highlight
    }

    private static double WalkBob(double cycleMs, double amplitude, double periodMs) =>
        Math.Sin(cycleMs * 2 * Math.PI / periodMs) * amplitude;

    private static double JumpScaleY(double velocityY, double landImpactMs)
    {
        if (landImpactMs > 0) return 0.72 + 0.28 * (1.0 - landImpactMs / 150.0);
        if (velocityY < -100) return 1.18;  // going up: stretch
        if (velocityY >  100) return 0.88;  // falling: squash
        return 1.0;
    }
}

/// <summary>Minimal draw command — serialized to JSON and interpreted by runner.js.</summary>
public class DrawCmd
{
    public string T { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }
    public double W { get; set; }   // width or arc radius
    public double H { get; set; }   // height or arc line width
    public double A1 { get; set; }  // arc start angle
    public double A2 { get; set; }  // arc end angle
    public double Alpha { get; set; } = 1.0;
    public string C { get; set; } = "";
    public string? S { get; set; }   // text content or sprite key
    public string? F { get; set; }   // font
    public string? Al { get; set; }  // text align
    public double Pct { get; set; }  // bar fill fraction
    public double[]? Pts { get; set; }  // polygon point pairs [x0,y0,x1,y1,...]

    public static DrawCmd Fill(double x, double y, double w, double h, string color) =>
        new() { T = "fill", X = x, Y = y, W = w, H = h, C = color };

    public static DrawCmd Text(string s, double x, double y, string color, string? font = null, string? align = null) =>
        new() { T = "text", S = s, X = x, Y = y, C = color, F = font, Al = align };

    public static DrawCmd Bar(double x, double y, double w, double h, double pct, string color) =>
        new() { T = "bar", X = x, Y = y, W = w, H = h, Pct = Math.Clamp(pct, 0, 1), C = color };

    /// <summary>Draw a pre-built sprite image by key at the given position and size.</summary>
    public static DrawCmd Sprite(string key, double x, double y, double w, double h) =>
        new() { T = "sprite", S = key, X = x, Y = y, W = w, H = h };

    /// <summary>Filled polygon. pts is a flat [x0,y0,x1,y1,...] array.</summary>
    public static DrawCmd Poly(double[] pts, string fill) =>
        new() { T = "poly", Pts = pts, C = fill };

    public static DrawCmd Arc(double cx, double cy, double radius, double a1, double a2, string color, double lineWidth, double alpha) =>
        new() { T = "arc", X = cx, Y = cy, W = radius, A1 = a1, A2 = a2, C = color, H = lineWidth, Alpha = alpha };

    public static DrawCmd Circle(double cx, double cy, double radius, string color) =>
        new() { T = "circle", X = cx, Y = cy, W = radius, C = color };
}
