namespace MapGenerator.Fishing.Models;

public enum FishingPhase
{
    Casting,    // cast animation plays, auto-advances
    Waiting,    // bobber on water, waiting for bite
    Nibbling,   // fish nibbling, building to strike
    Striking,   // strike window open — player must press hook
    Reeling,    // hold to reel, manage tension
    Caught,     // celebration moment before result
    LineBroke,  // tension maxed, line snapped
    EscapedFish // missed all strike windows, fish left
}

public class FishingGameState
{
    public FishingPhase Phase { get; set; } = FishingPhase.Casting;

    // Active fish
    public FishDefinition? ActiveFish { get; set; }
    public int MissedStrikes { get; set; }

    // Timing counters
    public double PhaseElapsedMs { get; set; }
    public double BiteWaitMs { get; set; }     // how long to wait for bite (randomized)
    public double NibbleDurationMs { get; set; } = 1400;
    public double StrikeRemainingMs { get; set; }

    // Reeling
    public double TensionPct { get; set; }      // 0–1; snap at 1.0
    public double ReelProgressPct { get; set; } // 0–1; caught at 1.0
    public bool IsHolding { get; set; }         // player holding reel button
    public double BurstCooldownMs { get; set; } // prevents back-to-back bursts

    // Animation
    public double BobberY { get; set; }         // bobber vertical oscillation
    public double BobberAnimMs { get; set; }
    public bool NibbleDown { get; set; }        // whether bobber is dipped during nibble
    public double CelebrationMs { get; set; }

    // Session result
    public FishDefinition? CaughtFish { get; set; }

    public double LastTimestamp { get; set; } = -1;

    // Companion calls
    public string? CompanionCallText { get; set; }
    public double CompanionCallRemainingMs { get; set; }
    public double TensionCallCooldownMs { get; set; }
    public double PreviousTensionPct { get; set; }
    public bool AlmostThereCallFired { get; set; }

    // Rhythm reel mechanic
    public double ReelDisplayProgress { get; set; }  // smoothed version of ReelProgressPct, used for fish position
    public double ReelCursorPos { get; set; }        // 0–1 position across the tap bar
    public int    ReelCursorDir { get; set; } = 1;   // +1 or -1
    public double ReelZoneStart { get; set; } = 0.30; // 0–1 left edge of sweet zone
    public double ReelZoneShiftMs { get; set; }       // ms until zone jumps to new position
    public double TapFlashMs { get; set; }            // success flash countdown
    public double MissTapFlashMs { get; set; }        // miss flash countdown
}
