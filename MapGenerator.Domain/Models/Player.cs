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
    public string Color { get; set; } = "#50a0f0";
    public DateTime? LastEggLaidAt { get; set; }
    public Dictionary<string, int> Inventory { get; set; } = new();
}
