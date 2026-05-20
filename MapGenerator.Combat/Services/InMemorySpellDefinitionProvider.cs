using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemorySpellDefinitionProvider : ISpellDefinitionProvider
{
    private static readonly SpellDefinition[] _definitions =
    [
        new()
        {
            Id          = "firebolt",
            Name        = "Firebolt",
            Description = "A bolt of searing flame. Burns the target over time.",
            DamageType  = DamageType.Fire,
            ManaCost    = 3,
            Power       = 1.4f,
            TargetType  = TargetType.Single,
            OnHit       = new() { StatusType = ModifierStat.Burn, Chance = 0.30f, Value = 0.04f, Turns = 3 },
        },
        new()
        {
            Id          = "frost_ray",
            Name        = "Frost Ray",
            Description = "A freezing beam that chills the target, reducing its stamina recovery.",
            DamageType  = DamageType.Frost,
            ManaCost    = 3,
            Power       = 1.2f,
            TargetType  = TargetType.Single,
            OnHit       = new() { StatusType = ModifierStat.StaminaRegen, Chance = 1.0f, Value = -2f, Turns = 2 },
        },
        new()
        {
            Id          = "thunder",
            Name        = "Thunder",
            Description = "A burst of storm energy that strikes all enemies simultaneously.",
            DamageType  = DamageType.Storm,
            ManaCost    = 5,
            Power       = 1.0f,
            TargetType  = TargetType.AllEnemies,
        },
        new()
        {
            Id          = "mend",
            Name        = "Mend",
            Description = "Channel restorative energy to heal your wounds.",
            DamageType  = null,
            ManaCost    = 2,
            Power       = 0f,
            TargetType  = TargetType.Self,
            HealAmount  = 8,
        },
    ];

    private static readonly Dictionary<string, SpellDefinition> _byId =
        _definitions.ToDictionary(s => s.Id);

    public IReadOnlyList<SpellDefinition> All => _definitions;
    public SpellDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
