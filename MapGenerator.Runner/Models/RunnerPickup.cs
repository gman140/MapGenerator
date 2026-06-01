namespace MapGenerator.Runner.Models;

public enum PickupType { HealthPotion, AttackBoost }

public class RunnerPickup
{
    /// <summary>World-space X left edge. Scrolls left with the world like obstacles.</summary>
    public double WorldX { get; set; }
    public PickupType Type { get; set; }
}
