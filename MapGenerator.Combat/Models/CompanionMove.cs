using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class CompanionMove
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DamageType DamageType { get; init; }
    public float Power { get; init; }
    public bool HitsAll { get; init; }
}
