using MapGenerator.Fishing.Models;

namespace MapGenerator.Fishing.Services;

public static class FishingRenderer
{
    public const double CanvasWidth  = 600;
    public const double CanvasHeight = 300;

    private const double WaterY      = 130; // water surface y
    private const double RodTipX    = 90;
    private const double RodTipY    = 55;
    private const double BobberX    = 310;
    private const double BobberBaseY = WaterY + 14;
    private const double PlayerX    = 18;
    private const double PlayerY    = WaterY - 48;  // bottom of sprite lands at waterline

    public static List<FishDrawCmd> Build(FishingGameState state, FishingInitData data)
    {
        var cmds = new List<FishDrawCmd>();

        DrawBackground(cmds, data, state);

        if (state.Phase == FishingPhase.Reeling)
        {
            DrawFishApproaching(cmds, state);
            DrawFishingLineToFish(cmds, state);
        }
        else if (state.Phase == FishingPhase.Caught)
        {
            DrawCaughtLine(cmds, state);
        }
        else
        {
            DrawFishingLine(cmds, state);
            DrawBobber(cmds, state);
        }

        DrawPlayer(cmds, data);
        DrawCompanionCall(cmds, state, data);

        switch (state.Phase)
        {
            case FishingPhase.Casting:
                DrawCastingHint(cmds, state);
                break;
            case FishingPhase.Waiting:
                DrawWaitingHint(cmds);
                break;
            case FishingPhase.Nibbling:
                DrawFishDepthHint(cmds, state);
                DrawNibblingHint(cmds, state);
                break;
            case FishingPhase.Striking:
                DrawFishDepthHint(cmds, state);
                DrawStrikeAlert(cmds, state);
                break;
            case FishingPhase.Reeling:
                DrawReelingHUD(cmds, state);
                break;
            case FishingPhase.Caught:
                DrawCaughtCelebration(cmds, state);
                break;
        }

        return cmds;
    }

    // ── Scene ──────────────────────────────────────────────────────────────────

    private static void DrawBackground(List<FishDrawCmd> cmds, FishingInitData data, FishingGameState state)
    {
        // Sky
        cmds.Add(FishDrawCmd.Fill(0, 0, CanvasWidth, WaterY, data.BackgroundColor));

        // Water body
        cmds.Add(FishDrawCmd.Fill(0, WaterY, CanvasWidth, CanvasHeight - WaterY, data.WaterColor));

        // Animated water surface
        DrawWaterSurface(cmds, data, state);

        // Tier label
        string tierLabel = data.PoleTier switch
        {
            0 => "Fishing Rod",
            1 => "Reinforced Rod",
            2 => "Light Lure",
            3 => "Lucky Rod",
            4 => "Ice Auger",
            _ => "Fishing",
        };
        cmds.Add(FishDrawCmd.Text($"🎣 {tierLabel}", CanvasWidth - 8, 22, "#a0c8e0", "11px monospace", "right"));
    }

    /// <summary>Single source of truth for bobber position — handles both cast arc and idle float.</summary>
    private static (double x, double y) GetBobberPosition(FishingGameState state)
    {
        if (state.Phase == FishingPhase.Casting)
        {
            double t = Math.Min(state.PhaseElapsedMs / 1100.0, 1.0);
            return (
                RodTipX + (BobberX - RodTipX) * t,
                RodTipY + (BobberBaseY - RodTipY) * t - 60 * Math.Sin(t * Math.PI)
            );
        }
        return (BobberX, BobberBaseY + state.BobberY);
    }

    private static void DrawFishingLine(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var (bx, by) = GetBobberPosition(state);
        if (state.Phase == FishingPhase.Casting)
            cmds.Add(FishDrawCmd.Line(RodTipX, RodTipY, bx, by, "#d0c8a0", 1.5));
        else
            DrawCurvedLine(cmds, RodTipX, RodTipY, bx, by, 22.0, "#d0c8a0", 1.5);
    }

    private static void DrawBobber(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var (bx, by) = GetBobberPosition(state);

        cmds.Add(FishDrawCmd.Fill(bx - 1, by - 9, 2, 9, "#e0d0a0"));
        cmds.Add(FishDrawCmd.Circle(bx, by - 5, 5, "#dd3333"));
        cmds.Add(FishDrawCmd.Circle(bx, by + 3, 5, "#eeeeee"));

        if (state.Phase == FishingPhase.Striking)
        {
            bool flash = (state.PhaseElapsedMs / 150) % 2 < 1;
            if (flash)
                cmds.Add(FishDrawCmd.Circle(bx, by - 1, 9, "#ffdd44", 0.7));
        }
    }

    private static void DrawCaughtLine(List<FishDrawCmd> cmds, FishingGameState state)
    {
        const double hangX = 112;
        const double hangY = WaterY - 20;

        DrawCurvedLine(cmds, RodTipX, RodTipY, hangX, hangY, 10.0, "#d0c8a0", 1.5);

        var fish = state.CaughtFish;
        if (fish == null) return;

        double bodyR = 7 + Math.Clamp(fish.TensionDrainRate * 55, 0, 13);

        // Fish hangs vertically: head near hook, tail below
        cmds.Add(FishDrawCmd.Circle(hangX, hangY + bodyR * 0.45, bodyR * 0.85, "#0e2a3a", 0.88));
        cmds.Add(FishDrawCmd.Circle(hangX, hangY + bodyR * 1.15, bodyR * 0.62, "#0e2a3a", 0.88));

        // Tail spread at bottom
        double tailY = hangY + bodyR * 1.85;
        cmds.Add(FishDrawCmd.Line(hangX, tailY, hangX - bodyR * 0.7, tailY + bodyR * 0.65, "#0e2a3a", 2.0));
        cmds.Add(FishDrawCmd.Line(hangX, tailY, hangX + bodyR * 0.7, tailY + bodyR * 0.65, "#0e2a3a", 2.0));

        // Eye
        cmds.Add(FishDrawCmd.Circle(hangX - bodyR * 0.22, hangY + bodyR * 0.20, 2.2, "#ffffff", 0.88));
        cmds.Add(FishDrawCmd.Circle(hangX - bodyR * 0.18, hangY + bodyR * 0.20, 1.0, "#001020", 0.88));
    }

    private static void DrawPlayer(List<FishDrawCmd> cmds, FishingInitData data)
    {
        // Companion behind player
        if (data.CompanionSprite.Length > 0)
            cmds.Add(FishDrawCmd.Sprite("companion", PlayerX + 48, PlayerY + 16, 32, 32));

        if (data.PlayerSprite.Length > 0)
            cmds.Add(FishDrawCmd.Sprite("player", PlayerX, PlayerY, 48, 48));
        else
            cmds.Add(FishDrawCmd.Fill(PlayerX, PlayerY, 32, 48, "#5599ff"));

        // Fishing rod
        cmds.Add(FishDrawCmd.Line(PlayerX + 40, PlayerY + 24, RodTipX, RodTipY, "#8B6914", 2.5));
    }

    private static void DrawCompanionCall(List<FishDrawCmd> cmds, FishingGameState state, FishingInitData data)
    {
        if (string.IsNullOrEmpty(state.CompanionCallText) ||
            string.IsNullOrEmpty(data.CompanionName)       ||
            state.CompanionCallRemainingMs <= 0) return;

        double alpha = Math.Min(1.0, state.CompanionCallRemainingMs / 600.0); // fade last 600 ms
        string label = $"{data.CompanionName}: {state.CompanionCallText}";

        // Estimate bubble width (≈6.3px per char at 11px monospace)
        double bubbleW = label.Length * 6.3 + 16;
        double bubbleX = PlayerX + 50;  // anchor near companion
        double bubbleY = PlayerY - 4;   // above companion area

        // Keep bubble inside canvas
        if (bubbleX + bubbleW > CanvasWidth - 8)
            bubbleX = CanvasWidth - bubbleW - 8;

        cmds.Add(FishDrawCmd.Fill(bubbleX, bubbleY - 20, bubbleW, 22, "#0d1f0d", 0.88 * alpha));
        cmds.Add(new FishDrawCmd
        {
            T = "text", S = label,
            X = bubbleX + 8, Y = bubbleY - 4,
            C = "#aaffaa", F = "11px monospace",
            Alpha = alpha,
        });
    }

    // ── Phase UIs ─────────────────────────────────────────────────────────────

    private static void DrawCastingHint(List<FishDrawCmd> cmds, FishingGameState state)
    {
        double alpha = Math.Min(state.PhaseElapsedMs / 400.0, 1.0);
        cmds.Add(FishDrawCmd.Text("Casting...", CanvasWidth / 2, CanvasHeight - 24, "#88ccee", "13px monospace", "center"));
    }

    private static void DrawWaitingHint(List<FishDrawCmd> cmds)
    {
        cmds.Add(FishDrawCmd.Text("Waiting for a bite...", CanvasWidth / 2, CanvasHeight - 24,
            "#88aacc", "12px monospace", "center"));
    }

    private static void DrawNibblingHint(List<FishDrawCmd> cmds, FishingGameState state)
    {
        string fishName = state.ActiveFish?.Name ?? "Something";
        bool flash = (state.PhaseElapsedMs / 200) % 2 < 1;

        cmds.Add(FishDrawCmd.Text($"{fishName} is nibbling!", CanvasWidth / 2, CanvasHeight - 44,
            "#ffd060", "bold 14px monospace", "center"));
        if (flash)
            cmds.Add(FishDrawCmd.Text("Get ready to hook!", CanvasWidth / 2, CanvasHeight - 24,
                "#ffaa30", "12px monospace", "center"));
    }

    private static void DrawStrikeAlert(List<FishDrawCmd> cmds, FishingGameState state)
    {
        string fishName = state.ActiveFish?.Name ?? "Fish";
        bool flash = (state.PhaseElapsedMs / 80) % 2 < 1;

        // Bright alert
        double alertAlpha = flash ? 0.9 : 0.5;
        cmds.Add(FishDrawCmd.Fill(0, CanvasHeight - 60, CanvasWidth, 60, "#331100", alertAlpha));
        cmds.Add(FishDrawCmd.Text("⚡ HOOK NOW!", CanvasWidth / 2, CanvasHeight - 36,
            flash ? "#ffee00" : "#ffaa00", "bold 20px monospace", "center"));
        cmds.Add(FishDrawCmd.Text(fishName, CanvasWidth / 2, CanvasHeight - 14,
            "#ffcc88", "12px monospace", "center"));

        // Strike window bar (shrinking)
        double featureDef = state.StrikeRemainingMs;
        double maxWindow = state.ActiveFish?.StrikeWindowMs ?? 700;
        double pct = Math.Clamp(featureDef / maxWindow, 0, 1);
        cmds.Add(FishDrawCmd.Bar(20, 8, CanvasWidth - 40, 10, pct, "#ffdd44"));
        cmds.Add(FishDrawCmd.Text("Strike window", 20, 30, "#ddcc88", "10px monospace"));
    }

    private static void DrawReelingHUD(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var fish = state.ActiveFish;
        if (fish == null) return;

        // Fish name at top
        cmds.Add(FishDrawCmd.Text(fish.Name, CanvasWidth / 2, 22, "#ffd080", "bold 13px monospace", "center"));

        // ── Rhythm tap bar ────────────────────────────────────────────────────
        const double barX = 20, barW = CanvasWidth - 40, barH = 20;
        const double barY = CanvasHeight - 52;

        // Background
        cmds.Add(FishDrawCmd.Fill(barX, barY, barW, barH, "#111820"));

        // Sweet zone — color shifts green→orange→red as tension rises; narrows at high tension
        double baseZoneW      = Math.Clamp(0.36 - fish.TensionDrainRate * 0.85, 0.14, 0.36);
        double effectiveZoneW = baseZoneW * (1.0 - state.TensionPct * 0.45);
        double zoneX          = barX + state.ReelZoneStart * barW;
        double zoneW          = effectiveZoneW * barW;
        string zoneColor      = state.TensionPct > 0.75 ? "#ee4422"
                              : state.TensionPct > 0.50 ? "#ee9922"
                              : "#44ee88";

        cmds.Add(FishDrawCmd.Fill(zoneX, barY, zoneW, barH, zoneColor, 0.80));

        // Miss flash — brief red wash over the whole bar
        if (state.MissTapFlashMs > 0)
            cmds.Add(FishDrawCmd.Fill(barX, barY, barW, barH, "#ff2200", state.MissTapFlashMs / 220.0 * 0.55));

        // Hit flash — white pulse on the zone
        if (state.TapFlashMs > 0)
            cmds.Add(FishDrawCmd.Fill(zoneX, barY, zoneW, barH, "#ffffff", state.TapFlashMs / 260.0 * 0.65));

        // Cursor — white vertical bar, slightly taller than the bar
        double cursorX = barX + state.ReelCursorPos * barW - 2;
        cmds.Add(FishDrawCmd.Fill(cursorX, barY - 3, 4, barH + 6, "#ffffff"));

        // Labels
        cmds.Add(FishDrawCmd.Text("Tap in the zone!", CanvasWidth / 2, barY - 7, "#88aabb", "10px monospace", "center"));
        cmds.Add(FishDrawCmd.Text($"Tension  {state.TensionPct * 100:F0}%", 20, CanvasHeight - 16, "#ddbbaa", "10px monospace"));
    }

    private static void DrawCaughtCelebration(List<FishDrawCmd> cmds, FishingGameState state)
    {
        string fishName = state.CaughtFish?.Name ?? "Fish";

        // Sparkle particles (fake with circles at random fixed positions driven by time)
        double t = state.CelebrationMs;
        double[] sparkleX = [180, 280, 350, 420, 300, 240, 390];
        double[] sparkleY = [170, 140, 200, 160, 220, 190, 150];
        for (int i = 0; i < sparkleX.Length; i++)
        {
            double alpha = Math.Abs(Math.Sin((t / 180.0) + i * 0.9)) * 0.85;
            double r = 3.0 + Math.Abs(Math.Sin((t / 220.0) + i)) * 5;
            string sc = i % 3 == 0 ? "#ffee44" : i % 3 == 1 ? "#44ffaa" : "#ff88cc";
            cmds.Add(FishDrawCmd.Circle(sparkleX[i], sparkleY[i], r, sc, alpha));
        }

        cmds.Add(FishDrawCmd.Text("★ Caught!", CanvasWidth / 2, CanvasHeight / 2 - 20,
            "#ffee44", "bold 24px monospace", "center"));
        cmds.Add(FishDrawCmd.Text(fishName, CanvasWidth / 2, CanvasHeight / 2 + 14,
            "#ffd080", "bold 16px monospace", "center"));
    }

    // ── Fish silhouette helpers ───────────────────────────────────────────────

    private static (double x, double y) FishPosition(FishingGameState state)
    {
        double x = BobberX - (BobberX - (PlayerX + 115)) * state.ReelDisplayProgress;
        double y = WaterY + 34 + Math.Sin(state.BobberAnimMs * 0.003) * 5;
        if (state.TensionPct > 0.60)
            x += Math.Sin(state.BobberAnimMs * 0.05) * (state.TensionPct - 0.60) * 14;
        return (x, y);
    }

    private static void DrawFishingLineToFish(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var (fx, fy) = FishPosition(state);
        double sag = 14.0 * (1.0 - state.TensionPct * 0.85); // nearly straight when tension peaks
        DrawCurvedLine(cmds, RodTipX, RodTipY, fx, fy, sag, "#d0c8a0", 1.5);
    }

    private static void DrawFishApproaching(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var fish = state.ActiveFish;
        if (fish == null) return;

        var (fx, fy) = FishPosition(state);
        double bodyR = 7 + Math.Clamp(fish.TensionDrainRate * 55, 0, 13);
        double alpha = 0.78 + state.ReelDisplayProgress * 0.20;
        string color = state.TensionPct > 0.70 ? "#3a1500" : "#0e2a3a";

        // ── Swimming animation ────────────────────────────────────────────────
        double t = state.BobberAnimMs;
        double tiltAngle = 0.20 * Math.Sin(t * 0.0040);               // body tilt cycle
        double tailWag   = 0.24 * Math.Sin(t * 0.0080 + Math.PI / 2); // tail 2× faster, 90° offset

        // High-tension thrash: rapid tilt added when tension > 55%
        if (state.TensionPct > 0.55)
        {
            double thrashT = (state.TensionPct - 0.55) / 0.45;
            tiltAngle += 0.16 * Math.Sin(t * 0.026) * thrashT;
        }

        double cosA = Math.Cos(tiltAngle);
        double sinA = Math.Sin(tiltAngle);
        double cx   = fx + bodyR * 0.5; // rotation pivot (fish center)

        // Rotates a local-space offset around the fish center
        (double x, double y) R(double dx, double dy) =>
            (cx + dx * cosA - dy * sinA,
             fy + dx * sinA + dy * cosA);

        // ── Body ──────────────────────────────────────────────────────────────
        var (b1x, b1y) = R(-bodyR * 0.20, 0);
        var (b2x, b2y) = R( bodyR * 0.40, 0);
        cmds.Add(FishDrawCmd.Circle(b1x, b1y, bodyR,        color, alpha));
        cmds.Add(FishDrawCmd.Circle(b2x, b2y, bodyR * 0.65, color, alpha));

        // ── Tail (wag shifts both fins together) ──────────────────────────────
        var (tbx, tby) = R(bodyR * 1.15, 0);
        double wagOff  = tailWag * bodyR * 0.55;
        var (t1x, t1y) = R(bodyR * 1.65, -bodyR * 0.80 + wagOff);
        var (t2x, t2y) = R(bodyR * 1.65,  bodyR * 0.80 + wagOff);
        cmds.Add(FishDrawCmd.Line(tbx, tby, t1x, t1y, color, 2.0));
        cmds.Add(FishDrawCmd.Line(tbx, tby, t2x, t2y, color, 2.0));

        // ── Dorsal fin ────────────────────────────────────────────────────────
        var (d1x, d1y) = R(-bodyR * 0.20, -bodyR);
        var (d2x, d2y) = R( bodyR * 0.35, -bodyR * 1.35);
        var (d3x, d3y) = R( bodyR * 0.65, -bodyR);
        cmds.Add(FishDrawCmd.Line(d1x, d1y, d2x, d2y, color, 1.5));
        cmds.Add(FishDrawCmd.Line(d2x, d2y, d3x, d3y, color, 1.5));

        // ── Eye ───────────────────────────────────────────────────────────────
        var (ex, ey) = R(-bodyR * 0.45, -bodyR * 0.18);
        cmds.Add(FishDrawCmd.Circle(ex,              ey,              2.5, "#ffffff", alpha));
        cmds.Add(FishDrawCmd.Circle(ex + cosA * 0.4, ey + sinA * 0.4, 1.2, "#001020", alpha));

        // ── Approach flash ────────────────────────────────────────────────────
        if (state.ReelDisplayProgress > 0.80)
        {
            double flashAlpha = (state.ReelDisplayProgress - 0.80) / 0.20 * 0.35;
            cmds.Add(FishDrawCmd.Circle(b1x, b1y, bodyR + 3, "#ffffff", flashAlpha));
        }
    }

    // Faint silhouette lurking deep under the bobber during Nibbling/Striking
    private static void DrawFishDepthHint(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var fish = state.ActiveFish;
        if (fish == null) return;

        double fishY  = WaterY + 72;
        double bodyR  = 6 + Math.Clamp(fish.TensionDrainRate * 55, 0, 12);
        double pulse  = Math.Sin(state.BobberAnimMs * 0.004);
        double alpha  = 0.18 + pulse * 0.07;

        cmds.Add(FishDrawCmd.Circle(BobberX + bodyR * 0.3, fishY, bodyR,        "#0a1a28", alpha));
        cmds.Add(FishDrawCmd.Circle(BobberX + bodyR * 0.9, fishY, bodyR * 0.65, "#0a1a28", alpha));
        double tx = BobberX + bodyR * 1.65;
        cmds.Add(FishDrawCmd.Line(tx, fishY, tx + bodyR * 0.7, fishY - bodyR * 0.75, "#0a1a28", 1.5));
        cmds.Add(FishDrawCmd.Line(tx, fishY, tx + bodyR * 0.7, fishY + bodyR * 0.75, "#0a1a28", 1.5));
    }

    // ── Shared drawing helpers ────────────────────────────────────────────────

    /// <summary>Draws a fishing line that droops under gravity using 7 parabolic segments.</summary>
    private static void DrawCurvedLine(List<FishDrawCmd> cmds,
        double x1, double y1, double x2, double y2,
        double sag, string color, double lineWidth)
    {
        const int segments = 7;
        double px = x1, py = y1;
        for (int i = 1; i <= segments; i++)
        {
            double t  = (double)i / segments;
            double nx = x1 + (x2 - x1) * t;
            double ny = y1 + (y2 - y1) * t + sag * Math.Sin(t * Math.PI);
            cmds.Add(FishDrawCmd.Line(px, py, nx, ny, color, lineWidth));
            px = nx; py = ny;
        }
    }

    /// <summary>
    /// Draws the full-width animated water surface as 40 connected line segments following
    /// three overlapping sine waves, giving an organic side-view ripple effect.
    /// </summary>
    private static void DrawWaterSurface(List<FishDrawCmd> cmds, FishingInitData data, FishingGameState state)
    {
        const int    steps = 40;
        const double stepW = CanvasWidth / steps;

        double p1 = state.BobberAnimMs * 0.0015;          // primary wave
        double p2 = state.BobberAnimMs * 0.0009 + 2.1;    // secondary — different speed + phase offset
        double p3 = state.BobberAnimMs * 0.0005 + 4.4;    // subtle third harmonic

        double WaveY(double x) =>
            WaterY
            + 2.4 * Math.Sin(x * 0.015 + p1)
            + 1.0 * Math.Sin(x * 0.028 + p2)
            + 0.5 * Math.Sin(x * 0.044 + p3);

        double px = 0, py = WaveY(0);
        for (int i = 1; i <= steps; i++)
        {
            double nx = i * stepW;
            double ny = WaveY(nx);
            cmds.Add(FishDrawCmd.Line(px, py, nx, ny, data.WaterSurfaceColor, 6.0));
            px = nx; py = ny;
        }
    }
}
