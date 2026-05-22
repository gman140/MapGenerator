using MapGenerator.Combat.Enums;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class CompanionMove
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public CompanionMoveKind Kind { get; init; } = CompanionMoveKind.Attack;
    // Empty = usable by all element types
    public IReadOnlyList<DamageType> EligibleTypes { get; init; } = [];

    // Attack fields
    public DamageType DamageType { get; init; }
    public float Power { get; init; }
    public bool HitsAll { get; init; }

    // Buff / Debuff fields (Kind == PlayerBuff or EnemyDebuff)
    // EffectStat == null on a PlayerBuff means "heal HP"
    public ModifierStat? EffectStat { get; init; }
    public float EffectValue { get; init; }   // positive = buff/heal, negative = debuff
    public int EffectTurns { get; init; }
}
