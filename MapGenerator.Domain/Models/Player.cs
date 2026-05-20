namespace MapGenerator.Domain.Models;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string BrowserId { get; set; } = string.Empty;
    public int Q { get; set; }
    public int R { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeen { get; set; }
    public long MovementCooldownUntil { get; set; } // Unix ms
    public long GatherCooldownUntil { get; set; }   // Unix ms
    public long StunnedUntil { get; set; }           // Unix ms
    public string Color { get; set; } = "#50a0f0";
    public string[] SpritePixels { get; set; } = [];
    public DateTime? LastEggLaidAt { get; set; }
    public DateTime? LastDancedAt { get; set; }
    public DateTime? LastKissedAt { get; set; }
    public int EggsDestroyed { get; set; }
    public double Satiety { get; set; } = 80.0;
    public List<ActiveBuff> ActiveBuffs { get; set; } = [];
    public Dictionary<string, int> Inventory { get; set; } = new();
    public Dictionary<string, int> CraftedItems { get; set; } = new();

    // Dungeon state — null DungeonInstanceId means on the surface
    public string? DungeonInstanceId { get; set; }
    public int DungeonFloor { get; set; }
    public int DungeonQ { get; set; }
    public int DungeonR { get; set; }

    public bool IsInDungeon => DungeonInstanceId != null;

    // Combat stats
    public int MaxHp { get; set; } = 30;
    public int CurrentHp { get; set; } = 30;
    public int MaxStamina { get; set; } = 10;
    public int CurrentStamina { get; set; } = 10;
    public int BaseAttack { get; set; } = 10;
    public int BaseDefense { get; set; } = 5;
    public float BaseDodgeChance { get; set; } = 0.10f;

    // Equipment slots (item ID or null)
    public string? EquippedWeaponId { get; set; }
    public string? EquippedArmorId { get; set; }
    public string? EquippedHatId { get; set; }

    // Combat session
    public string? ActiveCombatSessionId { get; set; }
    public bool IsInCombat => ActiveCombatSessionId != null;

    // Enemy kill tracking
    public Dictionary<string, int> EnemiesDefeated { get; set; } = new();
    public int DeathCount { get; set; }
}
