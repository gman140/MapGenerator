using MapGenerator.Combat.Enums;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public record SelfBuff(ModifierStat Stat, float Value, int Turns);

public class SpellDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DamageType? DamageType { get; init; }          // null = no damage (e.g. Mend, Hex)
    public DamageType? SecondaryDamageType { get; init; }
    public float SecondaryRatio { get; init; }
    public int ManaCost { get; init; }
    public float Power { get; init; }                      // damage = BaseMagic * Power * typeModifier; 0 = pure debuff/buff
    public TargetType TargetType { get; init; } = TargetType.Single;
    public int HealAmount { get; init; }                   // flat HP heal (for Self spells)
    public OnHitEffect[] OnHits { get; init; } = [];       // on-hit status effects applied to targets
    public SelfBuff[] SelfBuffs { get; init; } = [];       // stat boosts applied to the caster (Self spells)
    public float DrainRatio { get; init; }                  // heal player for this fraction of damage dealt
}
