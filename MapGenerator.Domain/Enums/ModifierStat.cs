namespace MapGenerator.Domain.Enums;

public enum ModifierStat
{
    // ── Core stats ───────────────────────────────────────────────────────────
    Attack,
    Defense,
    Speed,
    Magic,
    Resistance,

    // ── Accuracy ─────────────────────────────────────────────────────────────
    // Positive = more accurate; negative = more likely to miss. Summed into accuracyMod in CombatMath.RollHit.
    Accuracy,

    // ── Regeneration ─────────────────────────────────────────────────────────
    StaminaRegen,
    ManaRegen,
    HpRegen,
    HealBonus,

    // ── Status / DoT (PvE CombatModifier usage) ──────────────────────────────
    Disease,        // HP% DoT per tick
    Burn,           // HP% DoT per tick
    Venom,          // HP% DoT per tick (scales 1.5×)
    StaminaDrain,   // stamina drained per tick

    // ── Special ──────────────────────────────────────────────────────────────
    DamageMultiplier, // one-shot damage boost (cleared after use)
    Stun,             // 50% skip chance while present
}
