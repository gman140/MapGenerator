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
        foreach (var obs in state.Obstacles.Where(o => o.IsActive))
        {
            double sx = obs.WorldX - state.WorldOffset;
            if (sx > CanvasW + 50 || sx + obs.Width < -50) continue;

            if (obs.IsGap)
            {
                // Erase ground to show a pit
                cmds.Add(DrawCmd.Fill(sx, GroundY, obs.Width, CanvasH - GroundY, cfg.BackgroundColor));
                cmds.Add(DrawCmd.Fill(sx, GroundY, obs.Width, 4, "#000"));
            }
            else
            {
                cmds.Add(DrawCmd.Fill(sx, GroundY - obs.Height, obs.Width, obs.Height, obs.Color));
                cmds.Add(DrawCmd.Fill(sx, GroundY - obs.Height, obs.Width, 3, "#aaaaaa"));
            }
        }

        // Enemies
        foreach (var enemy in state.Enemies.Where(e => !e.IsDefeated))
        {
            double ex = enemy.ScreenX;
            if (ex < -60 || ex > CanvasW + 20) continue;

            double ew = 36, eh = 46;
            double ey = GroundY - eh;

            // Body
            cmds.Add(DrawCmd.Fill(ex, ey, ew, eh, enemy.Color));
            cmds.Add(DrawCmd.Fill(ex, ey, ew, 4, "#dd6666"));

            // HP bar
            double hpPct = (double)enemy.Hp / enemy.MaxHp;
            cmds.Add(DrawCmd.Bar(ex, ey - 10, ew, 5, hpPct, "#cc3333"));

            // Label
            cmds.Add(DrawCmd.Text("!", ex + ew / 2, ey + eh / 2 + 5, "#fff", "bold 18px monospace", "center"));
        }

        // Player: walk bob + squash/stretch
        double playerBob   = state.IsGrounded ? WalkBob(state.WalkCycleMs, 2.0, 380.0) : 0;
        double playerSy    = JumpScaleY(state.PlayerVelocityY, state.LandImpactMs);
        double playerDrawH = RunnerEngine.PlayerHeight * playerSy;
        double playerDrawY = state.PlayerY + RunnerEngine.PlayerHeight - playerDrawH + playerBob;

        if (data.PlayerSprite.Length > 0)
            cmds.Add(DrawCmd.Sprite("player", PlayerX, playerDrawY, RunnerEngine.PlayerWidth, playerDrawH));
        else
            cmds.Add(DrawCmd.Fill(PlayerX, playerDrawY, RunnerEngine.PlayerWidth, playerDrawH, "#5599ff"));

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

        // HUD: enemies remaining
        int totalEnemies   = data.Config.EnemyCount;
        int defeated       = state.Enemies.Count(e => e.IsDefeated);
        int remaining      = totalEnemies - defeated;
        double enemyProgress = totalEnemies > 0 ? (double)defeated / totalEnemies : 0;
        cmds.Add(DrawCmd.Bar(CanvasW - 160, 10, 150, 10, enemyProgress, "#cc4444"));
        string enemyLabel = remaining > 0 ? $"{remaining} enem{(remaining == 1 ? "y" : "ies")} left" : "All defeated!";
        cmds.Add(DrawCmd.Text(enemyLabel, CanvasW - 10, 36, "#ffaaaa", "11px monospace", "right"));

        // HUD: attack cooldown indicator
        if (state.AttackCooldownMs > 0)
        {
            double cdPct = state.AttackCooldownMs / 500.0;
            cmds.Add(DrawCmd.Bar(10, 44, 80, 6, 1.0 - cdPct, "#ffaa00"));
            cmds.Add(DrawCmd.Text("ATK CD", 10, 62, "#ffaa88", "10px monospace"));
        }

        // Attack swipe arc
        if (state.AttackAnimMs > 0)
        {
            double alpha = state.AttackAnimMs / 220.0;
            double cx = PlayerX + RunnerEngine.PlayerWidth + 8;
            double cy = state.PlayerY + RunnerEngine.PlayerHeight * 0.38;
            cmds.Add(DrawCmd.Arc(cx, cy, 48, -1.9, 0.5, "#ffe060", 4, alpha));
            cmds.Add(DrawCmd.Arc(cx, cy, 34, -2.1, 0.3, "#ffe060", 3, alpha * 0.75));
            cmds.Add(DrawCmd.Arc(cx, cy, 20, -1.7, 0.7, "#ffffff", 2, alpha * 0.55));
        }

        // Floating texts
        foreach (var ft in state.FloatingTexts)
            cmds.Add(DrawCmd.Text(ft.Text, ft.X, ft.Y, ft.Color, "bold 13px monospace"));

        return cmds;
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

    public static DrawCmd Fill(double x, double y, double w, double h, string color) =>
        new() { T = "fill", X = x, Y = y, W = w, H = h, C = color };

    public static DrawCmd Text(string s, double x, double y, string color, string? font = null, string? align = null) =>
        new() { T = "text", S = s, X = x, Y = y, C = color, F = font, Al = align };

    public static DrawCmd Bar(double x, double y, double w, double h, double pct, string color) =>
        new() { T = "bar", X = x, Y = y, W = w, H = h, Pct = Math.Clamp(pct, 0, 1), C = color };

    /// <summary>Draw a pre-built sprite image by key at the given position and size.</summary>
    public static DrawCmd Sprite(string key, double x, double y, double w, double h) =>
        new() { T = "sprite", S = key, X = x, Y = y, W = w, H = h };

    public static DrawCmd Arc(double cx, double cy, double radius, double a1, double a2, string color, double lineWidth, double alpha) =>
        new() { T = "arc", X = cx, Y = cy, W = radius, A1 = a1, A2 = a2, C = color, H = lineWidth, Alpha = alpha };
}
