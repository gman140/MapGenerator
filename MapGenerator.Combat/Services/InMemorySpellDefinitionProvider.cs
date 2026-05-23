using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemorySpellDefinitionProvider : ISpellDefinitionProvider
{
    private static readonly SpellDefinition[] _definitions =
    [
        // ── Fire ─────────────────────────────────────────────────────────────
        new()
        {
            Id          = "firebolt",
            Name        = "Firebolt",
            Description = "A bolt of searing flame. Burns the target over time.",
            DamageType  = DamageType.Fire,
            ManaCost    = 3,
            Power       = 1.4f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.Burn, Chance = 0.30f, Value = 0.04f, Turns = 3 }],
        },
        new()
        {
            Id          = "scorch",
            Name        = "Scorch",
            Description = "An intense column of flame. Deals heavy damage and leaves the target burning for several turns.",
            DamageType  = DamageType.Fire,
            ManaCost    = 5,
            Power       = 2.0f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.Burn, Chance = 0.80f, Value = 0.06f, Turns = 4 }],
        },

        // ── Frost ────────────────────────────────────────────────────────────
        new()
        {
            Id          = "frost_ray",
            Name        = "Frost Ray",
            Description = "A freezing beam that chills the target, reducing its stamina recovery.",
            DamageType  = DamageType.Frost,
            ManaCost    = 3,
            Power       = 1.2f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.StaminaRegen, Chance = 1.0f, Value = -2f, Turns = 2 }],
        },
        new()
        {
            Id          = "glacial_spike",
            Name        = "Glacial Spike",
            Description = "A lance of absolute cold. Hits hard and may freeze the target in place.",
            DamageType  = DamageType.Frost,
            ManaCost    = 5,
            Power       = 1.6f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.Stun, Chance = 0.60f, Value = 0f, Turns = 2 }],
        },

        // ── Storm ────────────────────────────────────────────────────────────
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
            Id          = "chain_lightning",
            Name        = "Chain Lightning",
            Description = "Lightning that leaps between all enemies. Cheaper than Thunder, and the shock may leave them briefly stunned.",
            DamageType  = DamageType.Storm,
            ManaCost    = 4,
            Power       = 0.8f,
            TargetType  = TargetType.AllEnemies,
            OnHits      = [new() { StatusType = ModifierStat.Stun, Chance = 0.30f, Value = 0f, Turns = 1 }],
        },

        // ── Nature ───────────────────────────────────────────────────────────
        new()
        {
            Id          = "thorn_whip",
            Name        = "Thorn Whip",
            Description = "A lash of barbed vines. Deals solid damage and coats the target in festering poison.",
            DamageType  = DamageType.Nature,
            ManaCost    = 4,
            Power       = 1.3f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.Venom, Chance = 0.70f, Value = 0.04f, Turns = 4 }],
        },
        new()
        {
            Id          = "entangle",
            Name        = "Entangle",
            Description = "Roots erupt from the ground, seizing the target. Deals little damage but guarantees a moment of paralysis.",
            DamageType  = DamageType.Nature,
            ManaCost    = 3,
            Power       = 0.4f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.Stun, Chance = 1.0f, Value = 0f, Turns = 1 }],
        },

        // ── Dark ─────────────────────────────────────────────────────────────
        new()
        {
            Id          = "shadow_bolt",
            Name        = "Shadow Bolt",
            Description = "A dense shard of void energy. Deals strong dark damage and may inflict wasting disease.",
            DamageType  = DamageType.Dark,
            ManaCost    = 3,
            Power       = 1.5f,
            TargetType  = TargetType.Single,
            OnHits      = [new() { StatusType = ModifierStat.Disease, Chance = 0.40f, Value = 0.04f, Turns = 3 }],
        },
        new()
        {
            Id          = "hex",
            Name        = "Hex",
            Description = "A whispered curse that deals no direct damage. Guarantees the target is both diseased and poisoned.",
            DamageType  = null,
            ManaCost    = 2,
            Power       = 0f,
            TargetType  = TargetType.Single,
            OnHits      =
            [
                new() { StatusType = ModifierStat.Disease, Chance = 1.0f, Value = 0.03f, Turns = 3 },
                new() { StatusType = ModifierStat.Venom,   Chance = 1.0f, Value = 0.03f, Turns = 3 },
            ],
        },

        // ── Self ─────────────────────────────────────────────────────────────
        new()
        {
            Id          = "mend",
            Name        = "Mend",
            Description = "Channel restorative energy to heal your wounds.",
            ManaCost    = 2,
            TargetType  = TargetType.Self,
            HealAmount  = 8,
        },
        new()
        {
            Id          = "iron_skin",
            Name        = "Iron Skin",
            Description = "Harden your body against incoming blows. Boosts Defense and Resistance for several turns.",
            ManaCost    = 3,
            TargetType  = TargetType.Self,
            SelfBuffs   =
            [
                new(ModifierStat.Defense,    6f, 3),
                new(ModifierStat.Resistance, 3f, 3),
            ],
        },
        new()
        {
            Id          = "battle_trance",
            Name        = "Battle Trance",
            Description = "Enter a focused combat state. Boosts both physical Attack and Magic for several turns.",
            ManaCost    = 4,
            TargetType  = TargetType.Self,
            SelfBuffs   =
            [
                new(ModifierStat.Attack, 8f, 3),
                new(ModifierStat.Magic,  5f, 3),
            ],
        },

        // ── Ultimate ─────────────────────────────────────────────────────────
        new()
        {
            Id          = "soul_rend",
            Name        = "Soul Rend",
            Description = "You reach into the essence of every enemy and tear. Deals massive dark damage to all foes and leaves each one diseased and poisoned.",
            DamageType  = DamageType.Dark,
            ManaCost    = 10,
            Power       = 1.8f,
            TargetType  = TargetType.AllEnemies,
            OnHits      =
            [
                new() { StatusType = ModifierStat.Disease, Chance = 1.0f, Value = 0.05f, Turns = 4 },
                new() { StatusType = ModifierStat.Venom,   Chance = 1.0f, Value = 0.04f, Turns = 3 },
            ],
        },
    ];

    private static readonly Dictionary<string, SpellDefinition> _byId =
        _definitions.ToDictionary(s => s.Id);

    public IReadOnlyList<SpellDefinition> All => _definitions;
    public SpellDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
