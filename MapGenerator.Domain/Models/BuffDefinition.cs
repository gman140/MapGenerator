using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class BuffDefinition
{
    public BuffType Type { get; init; }
    public double Magnitude { get; init; }  // multiplier applied to target value
    public int Charges { get; init; }
}
