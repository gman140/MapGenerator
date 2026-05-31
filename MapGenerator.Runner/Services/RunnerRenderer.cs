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

            double ey = GroundY - 46;

            if (enemy.Type == EnemyType.Shooter)
                DrawArcher(cmds, ex, ey);
            else
                DrawWarrior(cmds, ex, ey);

            double hpPct = (double)enemy.Hp / enemy.MaxHp;
            cmds.Add(DrawCmd.Bar(ex, ey - 8, 36, 4, hpPct, "#cc3333"));
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
            if (proj.X + 22 < 0 || proj.X > CanvasW + 10) continue;
            cmds.Add(DrawCmd.Fill(proj.X - 12, proj.Y - 5, 22, 10, proj.Color));
            cmds.Add(DrawCmd.Fill(proj.X - 14, proj.Y - 4, 5, 8, "#ffffff"));
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
}
