namespace MapGenerator.Domain.Models;

public class FoodDefinition
{
    public string ResourceId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public double SatietyRestore { get; init; }
    public string[] EatMessages { get; init; } = [];
    public BuffDefinition? Buff { get; init; }
}
