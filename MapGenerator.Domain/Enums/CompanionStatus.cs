namespace MapGenerator.Domain.Enums;

public enum CompanionStatus
{
    None,
    Poisoned,   // Nature: -HP per turn
    Burned,     // Fire: -HP per turn + reduced ATK
    Chilled,    // Frost: reduced SPD (applied as speed mod)
    Paralyzed,  // Storm: 25% chance to skip each turn
    Cursed,     // Dark: reduced ATK + DEF
}
