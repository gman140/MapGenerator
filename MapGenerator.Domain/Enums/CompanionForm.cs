namespace MapGenerator.Domain.Enums;

public enum CompanionForm
{
    None,      // base — not yet evolved
    Fierce,    // +ATK×2, -DEF
    Nimble,    // +SPD, +FOC
    Sturdy,    // +DEF, +VIT
    Arcane,    // +FOC×2, +RES
    Savage,    // +ATK, +SPD, -DEF
    Guardian,  // +DEF, +VIT, +RES
    Brutal,    // +ATK×2, +VIT, -SPD
    Radiant,   // +RES×2, +DEF
}
