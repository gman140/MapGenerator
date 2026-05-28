using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public record WeaponAttackEffect(CompanionStatus Status, float Chance, int Duration);

public class WeaponAttackDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int StaminaCost { get; init; }
    public float Power { get; init; } = 1.0f;
    public bool UsesMagic { get; init; }
    public bool HitsAll { get; init; }
    public float LifestealRatio { get; init; }
    public IReadOnlyList<WeaponAttackEffect> Effects { get; init; } = [];
}
