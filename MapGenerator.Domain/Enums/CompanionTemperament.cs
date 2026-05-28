namespace MapGenerator.Domain.Enums;

public enum CompanionTemperament
{
    Bold,     // +ATK, -DEF — charges in headfirst
    Timid,    // +DEF, -ATK — cautious and defensive
    Reckless, // +ATK, +SPD, -DEF — all-out offense
    Careful,  // +DEF, +RES, -SPD — patient and resistant
    Cunning,  // +FOC, +SPD, -VIT — crit-focused, fragile
    Stoic,    // +RES, +DEF, -FOC — immovable and resistant
    Playful,  // +SPD, +FOC, -ATK — fast and unpredictable
    Fierce,   // +ATK, +ATK, -RES, -DEF — extreme offense
}
