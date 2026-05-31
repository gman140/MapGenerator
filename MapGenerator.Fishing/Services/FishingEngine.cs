using MapGenerator.Fishing.Models;

namespace MapGenerator.Fishing.Services;

public static class FishingEngine
{
    private const double CastingDurationMs      = 1100.0;
    private const double BurstCooldownBaseMs    = 1500.0;
    private const double ReelSlipRate           = 0.05;
    private const double TensionRecoveryRate    = 0.10;
    private const double CallDurationMs         = 3400.0;
    private const double TensionCallCooldownMs  = 2800.0;

    private static readonly Random _rng = new();

    // ── Call message pools ────────────────────────────────────────────────────

    private static readonly string[] CallsCasting  = ["Here we go.", "Let's see what's biting.", "Good cast.", "Perfect spot."];
    private static readonly string[] CallsWaiting  = ["Any moment now...", "Patience.", "Something's around here.", "The water looks good."];
    private static readonly string[] CallsNibbling = ["Something's there!", "Get ready to hook it!", "Here it comes—"];
    private static readonly string[] CallsStriking = ["Now!", "Hook it!", "Now, now—", "Don't miss!"];
    private static readonly string[] CallsReeling  = ["You got one!", "Reel it in!", "Don't let it go!"];
    private static readonly string[] CallsTensionMed  = ["Ease up!", "Let it run a bit!", "Not so hard—"];
    private static readonly string[] CallsTensionHigh = ["Careful, the line—", "Back off!", "It's going to snap!"];
    private static readonly string[] CallsTensionDrop = ["Now pull!", "It's tiring—reel!", "Go, go!"];
    private static readonly string[] CallsAlmost   = ["Almost there!", "Keep going!", "You've almost got it!"];
    private static readonly string[] CallsCaught   = ["Yes! You got it!", "Nice one!", "Look at that thing!"];

    // ── Public API ────────────────────────────────────────────────────────────

    public static FishingGameState Initialize(FishingInitData data)
    {
        var state = new FishingGameState { Phase = FishingPhase.Casting };
        TriggerCall(state, data, CallsCasting);
        return state;
    }

    public static void Tick(FishingGameState state, FishingInitData data, double timestamp)
    {
        if (IsTerminal(state)) return;

        double deltaMs = state.LastTimestamp < 0 ? 0 : Math.Min(timestamp - state.LastTimestamp, 100);
        state.LastTimestamp = timestamp;
        double dt = deltaMs / 1000.0;

        state.PhaseElapsedMs          += deltaMs;
        state.BobberAnimMs            += deltaMs;
        state.CompanionCallRemainingMs = Math.Max(0, state.CompanionCallRemainingMs - deltaMs);
        state.TensionCallCooldownMs    = Math.Max(0, state.TensionCallCooldownMs - deltaMs);

        switch (state.Phase)
        {
            case FishingPhase.Casting:
                if (state.PhaseElapsedMs >= CastingDurationMs)
                    EnterWaiting(state, data);
                break;

            case FishingPhase.Waiting:
                TickBobber(state, dt, false);
                if (state.PhaseElapsedMs >= state.BiteWaitMs)
                    EnterNibbling(state, data);
                break;

            case FishingPhase.Nibbling:
                TickBobber(state, dt, true);
                if (state.PhaseElapsedMs >= state.NibbleDurationMs)
                    EnterStriking(state, data);
                break;

            case FishingPhase.Striking:
                TickBobber(state, dt, true);
                state.StrikeRemainingMs -= deltaMs;
                if (state.StrikeRemainingMs <= 0)
                    HandleMissedStrike(state, data);
                break;

            case FishingPhase.Reeling:
                TickReeling(state, data, deltaMs, dt);
                break;

            case FishingPhase.Caught:
                state.CelebrationMs += deltaMs;
                break;
        }
    }

    public static void Hook(FishingGameState state, FishingInitData data)
    {
        if (state.Phase != FishingPhase.Striking) return;
        state.Phase           = FishingPhase.Reeling;
        state.PhaseElapsedMs  = 0;
        state.TensionPct         = 0.15;
        state.ReelProgressPct    = 0;
        state.ReelDisplayProgress = 0;
        state.BurstCooldownMs    = BurstCooldownBaseMs;
        state.PreviousTensionPct   = 0.15;
        state.AlmostThereCallFired = false;
        // Rhythm reel init
        state.ReelCursorPos   = 0;
        state.ReelCursorDir   = 1;
        state.ReelZoneStart   = 0.30;
        state.ReelZoneShiftMs = 2000 + _rng.NextDouble() * 1500;
        state.TapFlashMs      = 0;
        state.MissTapFlashMs  = 0;
        TriggerCall(state, data, CallsReeling);
    }

    /// <summary>Player taps during reeling — checks cursor-in-zone and applies reward or penalty.</summary>
    public static void Tap(FishingGameState state, FishingInitData data)
    {
        if (state.Phase != FishingPhase.Reeling) return;
        var fish = state.ActiveFish;
        if (fish == null) return;

        double baseZoneW      = ZoneBaseWidth(fish);
        double effectiveZoneW = baseZoneW * (1.0 - state.TensionPct * 0.45);
        bool inZone           = state.ReelCursorPos >= state.ReelZoneStart &&
                                state.ReelCursorPos <= state.ReelZoneStart + effectiveZoneW;

        if (inZone)
        {
            state.ReelProgressPct = Math.Clamp(state.ReelProgressPct + 0.10 + (1 - state.TensionPct) * 0.05, 0, 1);
            state.TensionPct      = Math.Max(0, state.TensionPct - 0.06);
            state.TapFlashMs      = 260;
        }
        else
        {
            state.TensionPct = Math.Clamp(state.TensionPct + 0.14, 0, 1);
            state.MissTapFlashMs = 220;
        }

        // Check terminal conditions immediately — the next tick's passive slip runs before
        // TickReeling's own check and would pull ReelProgressPct back below 1.0 before it fires.
        if (state.TensionPct >= 1.0)
        {
            state.Phase = FishingPhase.LineBroke;
        }
        else if (state.ReelProgressPct >= 1.0)
        {
            state.Phase          = FishingPhase.Caught;
            state.CaughtFish     = fish;
            state.CaughtWeightKg = RollCatchWeight(fish);
            state.CelebrationMs  = 0;
            TriggerCall(state, data, CallsCaught);
        }
    }

    public static void SetHolding(FishingGameState state, bool holding) { } // no-op — replaced by Tap()

    public static bool IsTerminal(FishingGameState state) =>
        state.Phase is FishingPhase.LineBroke or FishingPhase.EscapedFish ||
        (state.Phase == FishingPhase.Caught && state.CelebrationMs >= 2000);

    public static FishingResult BuildResult(FishingGameState state)
    {
        if (state.Phase == FishingPhase.Caught && state.CaughtFish != null)
            return new FishingResult
            {
                CaughtFishId   = state.CaughtFish.Id,
                CaughtFishName = state.CaughtFish.Name,
                CaughtWeightKg = state.CaughtWeightKg,
            };
        return new FishingResult();
    }

    // ── Phase transitions ────────────────────────────────────────────────────

    private static void EnterWaiting(FishingGameState state, FishingInitData data)
    {
        state.Phase          = FishingPhase.Waiting;
        state.PhaseElapsedMs = 0;
        double baseWait      = 3000.0 + _rng.NextDouble() * 5000.0;
        state.BiteWaitMs     = baseWait * data.WaitTimeMultiplier;
        state.ActiveFish     = null;
        TriggerCall(state, data, CallsWaiting);
    }

    private static void EnterNibbling(FishingGameState state, FishingInitData data)
    {
        state.Phase          = FishingPhase.Nibbling;
        state.PhaseElapsedMs = 0;
        state.MissedStrikes  = 0;
        state.ActiveFish     = PickFish(data);
        TriggerCall(state, data, CallsNibbling);
    }

    private static void EnterStriking(FishingGameState state, FishingInitData data)
    {
        state.Phase             = FishingPhase.Striking;
        state.PhaseElapsedMs    = 0;
        double window           = state.ActiveFish?.StrikeWindowMs ?? 700;
        double streakBonus      = data.StreakBonusActive ? 1.25 : 1.0;
        state.StrikeRemainingMs = window * data.StrikeWindowMultiplier * streakBonus;
        TriggerCall(state, data, CallsStriking);
    }

    private static void HandleMissedStrike(FishingGameState state, FishingInitData data)
    {
        state.MissedStrikes++;
        if (state.MissedStrikes >= (state.ActiveFish?.MaxMissedStrikes ?? 2))
        {
            state.Phase = FishingPhase.EscapedFish;
        }
        else
        {
            state.Phase          = FishingPhase.Nibbling;
            state.PhaseElapsedMs = 0;
            TriggerCall(state, data, CallsNibbling);
        }
    }

    // ── Tick helpers ─────────────────────────────────────────────────────────

    private static void TickBobber(FishingGameState state, double dt, bool nibbling)
    {
        double bob = Math.Sin(state.BobberAnimMs * 0.0025) * 3.0;
        if (nibbling)
            bob += 7.0 * Math.Abs(Math.Sin(state.BobberAnimMs * 0.007));
        state.BobberY = bob;
    }

    private static void TickReeling(FishingGameState state, FishingInitData data, double deltaMs, double dt)
    {
        var fish = state.ActiveFish;
        if (fish == null) { state.Phase = FishingPhase.EscapedFish; return; }

        // ── Rhythm cursor ─────────────────────────────────────────────────────
        double cursorSpeed = 0.50 + fish.TensionDrainRate * 5.5;
        state.ReelCursorPos += state.ReelCursorDir * cursorSpeed * dt;
        if (state.ReelCursorPos >= 1.0) { state.ReelCursorPos = 1.0; state.ReelCursorDir = -1; }
        if (state.ReelCursorPos <= 0.0) { state.ReelCursorPos = 0.0; state.ReelCursorDir  =  1; }

        // Zone shifts to a new random position periodically
        state.ReelZoneShiftMs -= deltaMs;
        if (state.ReelZoneShiftMs <= 0)
        {
            double zw = ZoneBaseWidth(fish);
            state.ReelZoneStart   = _rng.NextDouble() * (1.0 - zw);
            state.ReelZoneShiftMs = 1800 + _rng.NextDouble() * 2000;
        }

        // Flash countdowns
        state.TapFlashMs     = Math.Max(0, state.TapFlashMs     - deltaMs);
        state.MissTapFlashMs = Math.Max(0, state.MissTapFlashMs - deltaMs);

        // ── Smooth display position ───────────────────────────────────────────
        // ReelDisplayProgress chases ReelProgressPct at a fixed speed so the fish
        // darts forward visibly on each tap instead of teleporting.
        const double CatchUpSpeed = 0.90; // units/sec — a 0.10 tap takes ~110 ms to complete
        double disp  = state.ReelDisplayProgress;
        double tgt   = state.ReelProgressPct;
        double delta = tgt - disp;
        state.ReelDisplayProgress = Math.Clamp(
            disp + Math.Sign(delta) * Math.Min(Math.Abs(delta), CatchUpSpeed * dt), 0, 1);

        // ── Passive forces ────────────────────────────────────────────────────
        double prevTension        = state.TensionPct;
        double tensionMult        = data.StreakBonusActive ? 0.70 : 1.0;
        state.ReelProgressPct     = Math.Max(0, state.ReelProgressPct - 0.020 * dt);
        state.TensionPct          = Math.Clamp(state.TensionPct + fish.TensionDrainRate * 0.40 * tensionMult * dt, 0, 1);

        // Fish burst
        state.BurstCooldownMs = Math.Max(0, state.BurstCooldownMs - deltaMs);
        if (state.BurstCooldownMs <= 0 && _rng.NextDouble() < fish.BurstChancePerSec * dt)
        {
            state.TensionPct      = Math.Clamp(state.TensionPct + fish.BurstStrength, 0, 1);
            state.BurstCooldownMs = BurstCooldownBaseMs;
        }

        // ── Companion tension calls ───────────────────────────────────────────
        if (state.TensionCallCooldownMs <= 0)
        {
            if (state.TensionPct >= 0.80 && prevTension < 0.80)
            {
                TriggerCall(state, data, CallsTensionHigh);
                state.TensionCallCooldownMs = TensionCallCooldownMs;
            }
            else if (state.TensionPct >= 0.65 && prevTension < 0.65)
            {
                TriggerCall(state, data, CallsTensionMed);
                state.TensionCallCooldownMs = TensionCallCooldownMs;
            }
            else if (prevTension >= 0.70 && state.TensionPct < 0.40)
            {
                TriggerCall(state, data, CallsTensionDrop);
                state.TensionCallCooldownMs = TensionCallCooldownMs;
            }
        }

        if (!state.AlmostThereCallFired && state.ReelProgressPct >= 0.75)
        {
            TriggerCall(state, data, CallsAlmost);
            state.AlmostThereCallFired = true;
        }

        state.PreviousTensionPct = state.TensionPct;

        if (state.TensionPct >= 1.0)
        {
            state.Phase = FishingPhase.LineBroke;
        }
        else if (state.ReelProgressPct >= 1.0)
        {
            state.Phase          = FishingPhase.Caught;
            state.CaughtFish     = fish;
            state.CaughtWeightKg = RollCatchWeight(fish);
            state.CelebrationMs  = 0;
            TriggerCall(state, data, CallsCaught);
        }
    }

    // Width of the sweet zone for a given fish (base, before tension narrowing)
    private static double ZoneBaseWidth(FishDefinition fish) =>
        Math.Clamp(0.36 - fish.TensionDrainRate * 0.85, 0.14, 0.36);

    private static FishDefinition? PickFish(FishingInitData data)
    {
        var pool = data.FishPool;
        if (pool.Length == 0) return null;

        double totalWeight = pool.Sum(f => FishWeight(f, data));
        double roll        = _rng.NextDouble() * totalWeight;
        foreach (var f in pool)
        {
            roll -= FishWeight(f, data);
            if (roll <= 0) return f;
        }
        return pool[^1];
    }

    private static double FishWeight(FishDefinition fish, FishingInitData data)
    {
        double w = fish.RarityWeight;

        // Pole rarity multiplier — applies only to rare entries (RarityWeight < 1.5)
        if (fish.RarityWeight < 1.5f)
            w *= data.RarityMultiplier;

        // Pearl Spinner: boosts all rare entries
        if (fish.RarityWeight < 1.5f)
            w *= data.LureRarityBoost;

        // Category lure: boosts matching habitat
        if (data.LureCategory != null && fish.FishCategory == data.LureCategory)
            w *= data.LureCategoryBoost;

        // Streak bonus: boosts chain materials so skilled players progress faster
        if (data.StreakBonusActive && fish.IsChainMaterial)
            w *= 1.5;

        return w;
    }

    private static void TriggerCall(FishingGameState state, FishingInitData data, string[] options)
    {
        if (string.IsNullOrEmpty(data.CompanionName)) return;
        state.CompanionCallText       = options[_rng.Next(options.Length)];
        state.CompanionCallRemainingMs = CallDurationMs;
    }

    private static double RollCatchWeight(FishDefinition fish)
    {
        // Explicit overrides for creatures whose size doesn't follow difficulty stats
        (double min, double max) = fish.Id switch
        {
            "Seahorse"                          => (0.02, 0.09),
            "ShoreCrab" or "FreshwaterCrab"
                         or "MarshCrab"         => (0.05, 0.35),
            "ThermophilicShrimp" or "IceShrimp" => (0.01, 0.04),
            "PolarStar"                         => (0.15, 0.80),
            "BioluminescentJellyfish"            => (0.40, 2.50),
            "RiverScale" or "DeepwaterPearl"
                         or "CoralChip"         => (0.01, 0.05),
            "AncientLure" or "GlacierShard"     => (0.02, 0.08),
            "Catfish"                           => (1.50, 9.00),
            "GiantCarp"                         => (8.00, 35.0),
            "Salmon"                            => (2.00, 12.0),
            "MirrorCarp"                        => (4.00, 22.0),
            "MorayEel" or "AbyssalEel"
                        or "FrozenEel"          => (2.00, 14.0),
            "GiantSquid"                        => (20.0, 100.0),
            "Moonfish"                          => (8.00, 30.0),
            "LegendaryMoonfish"                 => (25.0, 90.0),
            "AncientFish"                       => (18.0, 75.0),
            _                                   => (0.00, 0.00),
        };

        if (min == 0)
        {
            // Auto-compute from fight difficulty and rarity
            double difficulty = fish.TensionDrainRate * 45 + fish.ReelResistance * 30;
            double rarityMult = 1.0 / Math.Sqrt(fish.RarityWeight + 0.05);
            min = Math.Max(0.10, difficulty * rarityMult * 0.12);
            max = min * (2.0 + rarityMult * 0.8);
        }

        return min + _rng.NextDouble() * (max - min);
    }
}
