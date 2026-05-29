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
    private const double PlayerY    = WaterY - 52;  // bottom of sprite lands at waterline

    public static List<FishDrawCmd> Build(FishingGameState state, FishingInitData data)
    {
        var cmds = new List<FishDrawCmd>();

        DrawBackground(cmds, data, state);

        if (state.Phase == FishingPhase.Reeling)
        {
            DrawFishApproaching(cmds, state);
            DrawFishingLineToFish(cmds, state);
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

        // Water surface shimmer
        cmds.Add(FishDrawCmd.Fill(0, WaterY, CanvasWidth, 6, data.WaterSurfaceColor));

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

    private static void DrawFishingLine(List<FishDrawCmd> cmds, FishingGameState state)
    {
        double bobberY = BobberBaseY + state.BobberY;

        if (state.Phase == FishingPhase.Casting)
        {
            // Arc cast animation — line sweeps from player to bobber
            double t = Math.Min(state.PhaseElapsedMs / 1100.0, 1.0);
            double midX = RodTipX + (BobberX - RodTipX) * t;
            double midY = RodTipY + (bobberY - RodTipY) * t - (60 * Math.Sin(t * Math.PI));
            cmds.Add(FishDrawCmd.Line(RodTipX, RodTipY, midX, midY, "#d0c8a0", 1.5));
        }
        else
        {
            // Straight line to bobber
            cmds.Add(FishDrawCmd.Line(RodTipX, RodTipY, BobberX, bobberY, "#d0c8a0", 1.5));
        }
    }

    private static void DrawBobber(List<FishDrawCmd> cmds, FishingGameState state)
    {
        double bobberY = BobberBaseY + state.BobberY;
        if (state.Phase == FishingPhase.Casting)
        {
            double t = Math.Min(state.PhaseElapsedMs / 1100.0, 1.0);
            bobberY = RodTipY + (BobberBaseY - RodTipY) * t - (60 * Math.Sin(t * Math.PI));
        }

        // Bobber stick
        cmds.Add(FishDrawCmd.Fill(BobberX - 1, bobberY - 9, 2, 9, "#e0d0a0"));
        // Bobber top (red)
        cmds.Add(FishDrawCmd.Circle(BobberX, bobberY - 5, 5, "#dd3333"));
        // Bobber bottom (white, in water)
        cmds.Add(FishDrawCmd.Circle(BobberX, bobberY + 3, 5, "#eeeeee"));

        // Striking: bobber flash
        if (state.Phase == FishingPhase.Striking)
        {
            bool flash = (state.PhaseElapsedMs / 150) % 2 < 1;
            if (flash)
                cmds.Add(FishDrawCmd.Circle(BobberX, bobberY - 1, 9, "#ffdd44", 0.7));
        }
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
        string fishName = state.ActiveFish?.Name ?? "Fish";

        // Fish name at top — above the speech bubble area
        cmds.Add(FishDrawCmd.Text(fishName, CanvasWidth / 2, 22, "#ffd080", "bold 13px monospace", "center"));

        // Tension bar pinned to bottom strip, well below the scene
        string tensionColor = state.TensionPct > 0.75 ? "#ff4422" : state.TensionPct > 0.5 ? "#ffaa22" : "#44cc44";
        cmds.Add(FishDrawCmd.Text($"Tension  {state.TensionPct * 100:F0}%", 20, CanvasHeight - 52, "#ddbbaa", "10px monospace"));
        cmds.Add(FishDrawCmd.Bar(20, CanvasHeight - 42, CanvasWidth - 40, 12, state.TensionPct, tensionColor));

        // Hold indicator
        string holdHint = state.IsHolding ? "REELING ✓" : "Let go to ease tension";
        string holdColor = state.IsHolding ? "#44ff88" : "#aaaaaa";
        cmds.Add(FishDrawCmd.Text(holdHint, CanvasWidth / 2, CanvasHeight - 16, holdColor, "12px monospace", "center"));
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
        double x = BobberX - (BobberX - (PlayerX + 115)) * state.ReelProgressPct;
        double y = WaterY + 34 + Math.Sin(state.BobberAnimMs * 0.003) * 5;
        if (state.TensionPct > 0.60)
            x += Math.Sin(state.BobberAnimMs * 0.05) * (state.TensionPct - 0.60) * 14;
        return (x, y);
    }

    private static void DrawFishingLineToFish(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var (fx, fy) = FishPosition(state);
        cmds.Add(FishDrawCmd.Line(RodTipX, RodTipY, fx, fy, "#d0c8a0", 1.5));
    }

    private static void DrawFishApproaching(List<FishDrawCmd> cmds, FishingGameState state)
    {
        var fish = state.ActiveFish;
        if (fish == null) return;

        var (fx, fy) = FishPosition(state);
        double bodyR  = 7 + Math.Clamp(fish.TensionDrainRate * 55, 0, 13);
        double alpha  = 0.78 + state.ReelProgressPct * 0.20;
        string color  = state.TensionPct > 0.70 ? "#3a1500" : "#0e2a3a";

        // Body — two overlapping circles create an oval (head left, tail right)
        cmds.Add(FishDrawCmd.Circle(fx + bodyR * 0.3, fy, bodyR,        color, alpha));
        cmds.Add(FishDrawCmd.Circle(fx + bodyR * 0.9, fy, bodyR * 0.65, color, alpha));

        // Tail fin — V shape on the right
        double tx = fx + bodyR * 1.65;
        cmds.Add(FishDrawCmd.Line(tx, fy, tx + bodyR * 0.75, fy - bodyR * 0.8, color, 2.0));
        cmds.Add(FishDrawCmd.Line(tx, fy, tx + bodyR * 0.75, fy + bodyR * 0.8, color, 2.0));

        // Dorsal fin
        cmds.Add(FishDrawCmd.Line(fx + bodyR * 0.3, fy - bodyR,       fx + bodyR * 0.85, fy - bodyR * 1.35, color, 1.5));
        cmds.Add(FishDrawCmd.Line(fx + bodyR * 0.85, fy - bodyR * 1.35, fx + bodyR * 1.15, fy - bodyR,      color, 1.5));

        // Eye
        cmds.Add(FishDrawCmd.Circle(fx + bodyR * 0.05, fy - bodyR * 0.20, 2.5, "#ffffff",    alpha));
        cmds.Add(FishDrawCmd.Circle(fx + bodyR * 0.08, fy - bodyR * 0.20, 1.2, "#001020",    alpha));

        // Highlight flash as fish nears (progress > 80%)
        if (state.ReelProgressPct > 0.80)
        {
            double flashAlpha = (state.ReelProgressPct - 0.80) / 0.20 * 0.35;
            cmds.Add(FishDrawCmd.Circle(fx + bodyR * 0.3, fy, bodyR + 3, "#ffffff", flashAlpha));
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
}
