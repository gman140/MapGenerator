namespace MapGenerator.Combat.Models;

public enum CombatEventKind
{
    PlayerUpdate,      // HP, stamina, or flag changed on player
    EnemyUpdate,       // HP changed on a specific enemy
    PlayerAttack,      // player is attacking — trigger player strike animation
    ShakePlayer,       // player was hit — trigger shake animation
    ShakeEnemy,        // enemy was hit — trigger shake animation
    EnemyDied,         // enemy HP reached 0 — trigger death animation
    EnemyFled,         // enemy fled — trigger flee animation
    Pause,             // no state change, just wait for dramatic effect
    StatusApplied,     // a status effect was applied to the player
    StatusTick,        // a status effect dealt damage/drained stamina this turn
    PlayerManaUpdate,  // mana changed
    CompanionAction,   // companion took an action this turn
    PlayerBounce,      // player took a non-damage action — trigger bounce animation
    EnemyBounce,       // enemy took a non-damage action — trigger bounce animation
    CompanionBounce,   // companion took a non-damage action — trigger bounce animation
}

public class CombatEvent
{
    public CombatEventKind Kind { get; set; }

    // Optional log line to append to the combat log when this event fires
    public string? Log { get; set; }

    // For enemy-targeted events
    public string? EnemyInstanceId { get; set; }

    // Player state (−1 = no change)
    public int PlayerHp { get; set; } = -1;
    public int PlayerStamina { get; set; } = -1;
    public int PlayerMana { get; set; } = -1;
    public bool? PlayerDefending { get; set; }
    public bool? PlayerDodging { get; set; }

    // Enemy state (−1 = no change)
    public int EnemyHp { get; set; } = -1;

    // How long the UI should wait after processing this event (ms)
    public int DelayMs { get; set; }
}
