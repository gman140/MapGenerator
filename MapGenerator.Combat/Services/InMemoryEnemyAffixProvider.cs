using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;


namespace MapGenerator.Combat.Services;

public class InMemoryEnemyAffixProvider : IEnemyAffixProvider
{
    private static readonly List<EnemyAffix> All =
    [
        new EnemyAffix
        {
            Id              = "rabid",
            Label           = "Rabid",
            HpMultiplier    = 1.10f,
            AttackMultiplier = 1.05f,
            OnHit = new OnHitEffect
            {
                StatusType = ModifierStat.Disease,
                Chance     = 0.35f,
                Value      = 0.04f,   // 4% max HP per turn
                Turns      = 3,
            },
        },
        new EnemyAffix
        {
            Id              = "burning",
            Label           = "Burning",
            HpMultiplier    = 1.05f,
            AttackMultiplier = 1.15f,
            OnHit = new OnHitEffect
            {
                StatusType = ModifierStat.Burn,
                Chance     = 0.45f,
                Value      = 0.05f,   // 5% max HP per turn
                Turns      = 3,
            },
        },
        new EnemyAffix
        {
            Id              = "venomous",
            Label           = "Venomous",
            HpMultiplier    = 1.15f,
            AttackMultiplier = 1.0f,
            OnHit = new OnHitEffect
            {
                StatusType = ModifierStat.Venom,
                Chance     = 0.38f,
                Value      = 0.03f,   // 3% max HP per turn
                Turns      = 4,
            },
        },
        new EnemyAffix
        {
            Id           = "armored",
            Label        = "Armored",
            HpMultiplier = 1.20f,
            DefenseBonus = 8,
        },
        new EnemyAffix
        {
            Id                    = "spectral",
            Label                 = "Spectral",
            HpMultiplier          = 1.15f,
            AttackMultiplier      = 1.10f,
            ActionsCantBeDodged   = true,
        },
        new EnemyAffix
        {
            Id              = "enraged",
            Label           = "Enraged",
            HpMultiplier    = 1.20f,
            AttackMultiplier = 1.30f,
            AttackBonus     = 5,
            BonusActions =
            [
                new EnemyActionEntry { Action = EnemyActionType.HeavyAttack, Weight = 15 },
            ],
        },
        new EnemyAffix
        {
            Id              = "cursed",
            Label           = "Cursed",
            HpMultiplier    = 1.10f,
            AttackMultiplier = 1.05f,
            OnHit = new OnHitEffect
            {
                StatusType = ModifierStat.StaminaDrain,
                Chance     = 0.50f,
                Value      = 2f,     // 2 stamina drained per turn
                Turns      = 2,
            },
        },
        new EnemyAffix
        {
            Id              = "frenzied",
            Label           = "Frenzied",
            HpMultiplier    = 1.05f,
            AttackMultiplier = 1.15f,
            BonusActions =
            [
                new EnemyActionEntry { Action = EnemyActionType.DoubleStrike, Weight = 20 },
            ],
        },
    ];

    private static readonly Dictionary<string, EnemyAffix> ById =
        All.ToDictionary(a => a.Id, StringComparer.OrdinalIgnoreCase);

    public EnemyAffix? GetById(string id) => ById.GetValueOrDefault(id);
    public IReadOnlyList<EnemyAffix> GetAll() => All;
}
