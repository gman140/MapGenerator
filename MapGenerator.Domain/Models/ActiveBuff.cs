using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class ActiveBuff
{
    public BuffType Type { get; set; }
    public double Magnitude { get; set; }
    public int RemainingCharges { get; set; }
}
