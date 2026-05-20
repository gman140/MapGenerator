using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemoryCompanionDefinitionProvider : ICompanionDefinitionProvider
{
    private static readonly List<CompanionDefinition> _all =
    [
        new()
        {
            Id = "slimeling", Name = "Slimeling", Emoji = "🟢",
            Description = "A cheerful blob of animated slime. Not threatening, but surprisingly loyal.",
            ElementType = DamageType.Nature, BaseAttack = 4,
            Moves =
            [
                new() { Id = "glob_bonk",      Name = "Glob Bonk",      Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Nature, Power = 1.0f },
                new() { Id = "dribble_splash", Name = "Dribble Splash", Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Nature, Power = 0.85f },
                new() { Id = "spore_puff",     Name = "Spore Puff",     Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Nature, Power = 0.75f, HitsAll = true },
                new() { Id = "acid_drool",     Name = "Acid Drool",     Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Nature, Power = 1.2f },
                new() { Id = "slick_coat",     Name = "Slick Coat",     Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.Defense,    EffectValue = 2f, EffectTurns = 2 },
                new() { Id = "mend_goo",       Name = "Mend Goo",       Kind = CompanionMoveKind.PlayerBuff, EffectStat = null,                    EffectValue = 8f },
                new() { Id = "gunk_wrap",      Name = "Gunk Wrap",      Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,    EffectValue = -3f, EffectTurns = 2 },
                new() { Id = "ooze_drench",    Name = "Ooze Drench",    Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,  EffectValue = -3f, EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "emberhatch", Name = "Emberhatch", Emoji = "🔥",
            Description = "A fierce hatchling wreathed in fire. Its enthusiasm exceeds its caution.",
            ElementType = DamageType.Fire, BaseAttack = 5,
            Moves =
            [
                new() { Id = "scratch",        Name = "Scratch",        Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Slashing, Power = 0.8f },
                new() { Id = "ember_blob",     Name = "Ember Blob",     Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Fire,     Power = 1.0f },
                new() { Id = "cinder_flurry",  Name = "Cinder Flurry",  Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Fire,     Power = 0.75f, HitsAll = true },
                new() { Id = "hot_bite",       Name = "Hot Bite",       Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Fire,     Power = 1.15f },
                new() { Id = "yowl",           Name = "Yowl",           Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.Attack,     EffectValue = 4f, EffectTurns = 2 },
                new() { Id = "hype_dash",      Name = "Hype Dash",      Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.DodgeChance, EffectValue = 0.12f, EffectTurns = 2 },
                new() { Id = "char",           Name = "Char",           Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense, EffectValue = -3f, EffectTurns = 2 },
                new() { Id = "choke_smoke",    Name = "Choke Smoke",    Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,  EffectValue = -3f, EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "frostling", Name = "Frostling", Emoji = "❄️",
            Description = "A delicate creature of living ice. It prefers cold places and cool company.",
            ElementType = DamageType.Frost, BaseAttack = 4,
            Moves =
            [
                new() { Id = "frost_peck",     Name = "Frost Peck",     Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Frost, Power = 0.9f },
                new() { Id = "shiver_bite",    Name = "Shiver Bite",    Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Frost, Power = 1.0f },
                new() { Id = "hail_spit",      Name = "Hail Spit",      Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Frost, Power = 0.75f, HitsAll = true },
                new() { Id = "ice_spike",      Name = "Ice Spike",      Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Frost, Power = 1.2f },
                new() { Id = "frost_shell",    Name = "Frost Shell",    Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.Defense, EffectValue = 3f, EffectTurns = 3 },
                new() { Id = "ice_mend",       Name = "Ice Mend",       Kind = CompanionMoveKind.PlayerBuff, EffectStat = null,                 EffectValue = 7f },
                new() { Id = "sluggify",       Name = "Sluggify",       Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,  EffectValue = -3f, EffectTurns = 2 },
                new() { Id = "brittle_rub",    Name = "Brittle Rub",    Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense, EffectValue = -3f, EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "sparkling", Name = "Sparkling", Emoji = "⚡",
            Description = "A crackling ball of storm energy. It is very excitable and will not sit still.",
            ElementType = DamageType.Storm, BaseAttack = 5,
            Moves =
            [
                new() { Id = "nip_shock",      Name = "Nip Shock",      Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Storm, Power = 0.8f },
                new() { Id = "crackle_bite",   Name = "Crackle Bite",   Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Storm, Power = 1.0f },
                new() { Id = "pop_burst",      Name = "Pop Burst",      Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Storm, Power = 0.75f, HitsAll = true },
                new() { Id = "thundersnap",    Name = "Thundersnap",    Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Storm, Power = 1.2f },
                new() { Id = "amp_up",         Name = "Amp Up",         Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.Attack,      EffectValue = 4f,    EffectTurns = 2 },
                new() { Id = "jitterstep",     Name = "Jitterstep",     Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.DodgeChance, EffectValue = 0.12f, EffectTurns = 2 },
                new() { Id = "short_circuit",  Name = "Short Circuit",  Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense, EffectValue = -4f, EffectTurns = 2 },
                new() { Id = "zap_daze",       Name = "Zap Daze",       Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,  EffectValue = -3f, EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "shadelurk", Name = "Shadelurk", Emoji = "🌑",
            Description = "A creature born from shed darkness. It follows at the edge of your shadow, barely visible.",
            ElementType = DamageType.Dark, BaseAttack = 4,
            Moves =
            [
                new() { Id = "nip",            Name = "Nip",            Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Dark, Power = 0.9f },
                new() { Id = "shadow_lunge",   Name = "Shadow Lunge",   Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Dark, Power = 1.0f },
                new() { Id = "dusk_scatter",   Name = "Dusk Scatter",   Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Dark, Power = 0.75f, HitsAll = true },
                new() { Id = "void_fang",      Name = "Void Fang",      Kind = CompanionMoveKind.Attack,     DamageType = DamageType.Dark, Power = 1.2f },
                new() { Id = "shade_slip",     Name = "Shade Slip",     Kind = CompanionMoveKind.PlayerBuff, EffectStat = ModifierStat.DodgeChance, EffectValue = 0.15f, EffectTurns = 2 },
                new() { Id = "void_drink",     Name = "Void Drink",     Kind = CompanionMoveKind.PlayerBuff, EffectStat = null,                     EffectValue = 8f },
                new() { Id = "rattle",         Name = "Rattle",         Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,  EffectValue = -3f, EffectTurns = 2 },
                new() { Id = "peel_apart",     Name = "Peel Apart",     Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense, EffectValue = -4f, EffectTurns = 2 },
            ],
        },
    ];

    private static readonly Dictionary<string, CompanionDefinition> _byId =
        _all.ToDictionary(d => d.Id);

    public CompanionDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;

    public CompanionMove? GetMove(string companionDefId, string moveId)
    {
        if (!_byId.TryGetValue(companionDefId, out var def)) return null;
        return def.Moves.FirstOrDefault(m => m.Id == moveId);
    }

    public IReadOnlyList<CompanionDefinition> GetAll() => _all;
}
