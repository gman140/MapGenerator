using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class OnHitEffect
{
    public ModifierStat StatusType { get; set; }
    public float Chance { get; set; }   // 0.0–1.0
    public float Value { get; set; }    // damage fraction or stamina amount per turn
    public int Turns { get; set; }
}
