namespace MapGenerator.Combat.Enums;

public enum CombatActionType
{
    Attack,        // dispatched via CombatAction.WeaponAttackId; stamina cost from WeaponAttackDefinition
    Steady,        // improves accuracy of next attack/spell
    Defend,
    UseItem,
    Flee,
    Spell,
    Refocus,
}
