using MapGenerator.Combat.Enums;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class SpellDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DamageType? DamageType { get; init; }          // null = no damage (e.g. Mend)
    public DamageType? SecondaryDamageType { get; init; }
    public float SecondaryRatio { get; init; }
    public int ManaCost { get; init; }
    public float Power { get; init; }                      // damage = BaseMagic * Power * typeModifier
    public TargetType TargetType { get; init; } = TargetType.Single;
    public int HealAmount { get; init; }                   // flat HP heal (for Self spells)
    public OnHitEffect? OnHit { get; init; }
}
