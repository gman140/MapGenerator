namespace MapGenerator.Combat.Enums;

public enum CombatActionType
{
    Attack,
    HeavyAttack,
    Steady,     // replaces Dodge; improves accuracy of next attack/spell
    Defend,
    UseItem,
    Flee,
    Spell,
    Refocus,
}
