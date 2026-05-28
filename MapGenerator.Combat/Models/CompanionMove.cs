using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class CompanionMove
{
    public string Id          { get; init; } = string.Empty;
    public string Name        { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    // Empty = usable by all element types
    public IReadOnlyList<DamageType> EligibleTypes { get; init; } = [];

    // ── Damage component (Power = 0 → no damage) ────────────────────────────
    public DamageType DamageType { get; init; }
    public float      Power      { get; init; }
    public bool       HitsAll    { get; init; }

    // ── Heal component (0 → no heal) ─────────────────────────────────────────
    public float HealAmount { get; init; }

    // ── Status effects (any mix of self-buffs and enemy debuffs/statuses) ────
    public IReadOnlyList<MoveEffect> Effects { get; init; } = [];

    // ── Convenience queries ───────────────────────────────────────────────────
    public bool HasDamage    => Power > 0f;
    public bool HasHeal      => HealAmount > 0f;
    public bool HasSelfBuff  => Effects.Any(e => e.Target == MoveEffectTarget.Self);
    public bool HasEnemyEffect => Effects.Any(e => e.Target == MoveEffectTarget.Enemy);
}
