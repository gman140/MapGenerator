namespace MapGenerator.Domain.Enums;

public enum CompanionStatus
{
    None,

    // ── Damage-over-time ─────────────────────────────────────────────────────
    Poisoned,   // Nature: 6% max-HP DoT per tick
    Burned,     // Fire:   6% max-HP DoT + −20% ATK

    // ── Turn impairment ──────────────────────────────────────────────────────
    Paralyzed,  // Storm:  25% chance to skip turn
    Chilled,    // Frost:  −35% SPD (min −2), harder to land attacks

    // ── Accuracy impairment ──────────────────────────────────────────────────
    Cursed,     // Dark:   +20% miss chance on all outgoing attacks

    // ── Offensive buffs ──────────────────────────────────────────────────────
    Rallied,    // Universal: +25% ATK (min +2)
    Enraged,    // Fire/Storm: +35% ATK (min +3)

    // ── Defensive buffs ─────────────────────────────────────────────────────
    Hardened,   // Nature:  +25% DEF (min +2)
    Fortified,  // Frost:   +35% DEF (min +3)

    // ── Speed buff ───────────────────────────────────────────────────────────
    Hastened,   // Fire/Storm/Dark: +30% SPD (min +2), replaces DodgeChance buffs

    // ── Debuffs ──────────────────────────────────────────────────────────────
    Weakened,   // ATK −20% (min −2)
    Exposed,    // DEF −25% (min −3)
    Shattered,  // DEF −35% (min −4) — Storm/Dark stronger debuff
}
