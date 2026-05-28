using MapGenerator.Fishing.Models;

namespace MapGenerator.Fishing.Services;

public static class FishingEngine
{
    private const double CastingDurationMs = 1100.0;
    private const double BurstCooldownBaseMs = 1500.0;
    private const double ReelSlipRate = 0.05;       // reel progress lost per second when not holding
    private const double TensionRecoveryRate = 0.10; // tension recovered per second when not holding

    private static readonly Random _rng = new();

    public static FishingGameState Initialize(FishingInitData data) =>
        new() { Phase = FishingPhase.Casting };

    public static void Tick(FishingGameState state, FishingInitData data, double timestamp)
    {
        if (IsTerminal(state)) return;

        double deltaMs = state.LastTimestamp < 0 ? 0 : Math.Min(timestamp - state.LastTimestamp, 100);
        state.LastTimestamp = timestamp;
        double dt = deltaMs / 1000.0;

        state.PhaseElapsedMs += deltaMs;
        state.BobberAnimMs   += deltaMs;

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
                TickReeling(state, deltaMs, dt);
                break;

            case FishingPhase.Caught:
                state.CelebrationMs += deltaMs;
                break;
        }
    }

    public static void Hook(FishingGameState state)
    {
        if (state.Phase != FishingPhase.Striking) return;
        state.Phase           = FishingPhase.Reeling;
        state.PhaseElapsedMs  = 0;
        state.TensionPct      = 0.15;
        state.ReelProgressPct = 0;
        state.BurstCooldownMs = BurstCooldownBaseMs;
    }

    public static void SetHolding(FishingGameState state, bool holding) =>
        state.IsHolding = holding;

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
            };
        return new FishingResult();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static void EnterWaiting(FishingGameState state, FishingInitData data)
    {
        state.Phase          = FishingPhase.Waiting;
        state.PhaseElapsedMs = 0;
        double baseWait      = 3000.0 + _rng.NextDouble() * 5000.0;
        state.BiteWaitMs     = baseWait * data.WaitTimeMultiplier;
        state.ActiveFish     = null;
    }

    private static void EnterNibbling(FishingGameState state, FishingInitData data)
    {
        state.Phase          = FishingPhase.Nibbling;
        state.PhaseElapsedMs = 0;
        state.MissedStrikes  = 0;
        state.ActiveFish     = PickFish(data);
    }

    private static void EnterStriking(FishingGameState state, FishingInitData data)
    {
        state.Phase             = FishingPhase.Striking;
        state.PhaseElapsedMs    = 0;
        double window           = state.ActiveFish?.StrikeWindowMs ?? 700;
        state.StrikeRemainingMs = window * data.StrikeWindowMultiplier;
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
            // Give another nibble with the same fish
            state.Phase          = FishingPhase.Nibbling;
            state.PhaseElapsedMs = 0;
        }
    }

    private static void TickBobber(FishingGameState state, double dt, bool nibbling)
    {
        double bob = Math.Sin(state.BobberAnimMs * 0.0025) * 3.0;
        if (nibbling)
            bob += 7.0 * Math.Abs(Math.Sin(state.BobberAnimMs * 0.007));
        state.BobberY = bob;
    }

    private static void TickReeling(FishingGameState state, double deltaMs, double dt)
    {
        var fish = state.ActiveFish;
        if (fish == null) { state.Phase = FishingPhase.EscapedFish; return; }

        if (state.IsHolding)
        {
            state.ReelProgressPct += fish.ReelRate * dt;
            state.TensionPct      += (fish.TensionDrainRate + fish.ReelResistance) * dt;
        }
        else
        {
            state.TensionPct      = Math.Max(0, state.TensionPct - TensionRecoveryRate * dt);
            state.ReelProgressPct = Math.Max(0, state.ReelProgressPct - ReelSlipRate * dt);
        }

        // Fish burst
        state.BurstCooldownMs = Math.Max(0, state.BurstCooldownMs - deltaMs);
        if (state.BurstCooldownMs <= 0 && _rng.NextDouble() < fish.BurstChancePerSec * dt)
        {
            state.TensionPct      += fish.BurstStrength;
            state.BurstCooldownMs  = BurstCooldownBaseMs;
        }

        state.TensionPct      = Math.Clamp(state.TensionPct, 0, 1.0);
        state.ReelProgressPct = Math.Clamp(state.ReelProgressPct, 0, 1.0);

        if (state.TensionPct >= 1.0)
        {
            state.Phase = FishingPhase.LineBroke;
        }
        else if (state.ReelProgressPct >= 1.0)
        {
            state.Phase        = FishingPhase.Caught;
            state.CaughtFish   = fish;
            state.CelebrationMs = 0;
        }
    }

    private static FishDefinition? PickFish(FishingInitData data)
    {
        var pool = data.FishPool;
        if (pool.Length == 0) return null;

        double totalWeight = pool.Sum(f =>
            f.RarityWeight < 1.5 ? f.RarityWeight * data.RarityMultiplier : f.RarityWeight);

        double roll = _rng.NextDouble() * totalWeight;
        foreach (var f in pool)
        {
            double w = f.RarityWeight < 1.5 ? f.RarityWeight * data.RarityMultiplier : f.RarityWeight;
            roll -= w;
            if (roll <= 0) return f;
        }
        return pool[^1];
    }
}
