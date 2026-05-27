using MapGenerator.Combat.Enums;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class CombatSession
{
    public string Id { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;

    // Player HP
    public int PlayerHp { get; set; }
    public int PlayerMaxHp { get; set; }

    // Player stamina
    public int PlayerStamina { get; set; }
    public int PlayerMaxStamina { get; set; }

    // Snapshotted base stats (at combat start)
    public int   PlayerBaseAttack      { get; set; }
    public int   PlayerBaseDefense     { get; set; }
    public int   PlayerBaseResistance  { get; set; }
    public int   PlayerBaseSpeed       { get; set; }
    public float PlayerBaseCritChance  { get; set; }
    public DamageType PlayerWeaponDamageType { get; set; } = DamageType.Bludgeoning;

    // Mana
    public int PlayerMana    { get; set; }
    public int PlayerMaxMana { get; set; }
    public int PlayerBaseMagic { get; set; }

    // Turn flags (cleared at end of each turn)
    public bool PlayerDefending { get; set; }
    // Steady: set by the Steady action; clears after the player's next attack/spell
    public bool PlayerSteady { get; set; }

    // Active modifiers on the player (equipment = permanent, buffs = temporary)
    public List<CombatModifier> ActiveModifiers { get; set; } = [];

    public List<Enemy> Enemies { get; set; } = [];
    public int TurnNumber { get; set; }
    public CombatPhase Phase { get; set; }
    public List<string> Log { get; set; } = [];

    // True when the player successfully used the Flee action
    public bool PlayerFled { get; set; }

    // Context for display
    public string? ContextLabel { get; set; }

    // Companion (invulnerable; acts each turn if present)
    public bool CompanionPresent { get; set; }
    public string? CompanionDefinitionId { get; set; }
    public List<string> CompanionMoveIds { get; set; } = [];
    public int CompanionBaseAttack { get; set; }
    public int CompanionBaseSpeed { get; set; }
    public string CompanionName { get; set; } = string.Empty;
}
