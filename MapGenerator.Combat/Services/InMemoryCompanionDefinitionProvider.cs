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
            ElementType = DamageType.Nature, BaseAttack = 8,
            Moves =
            [
                new() { Id = "goo_glob",     Name = "Goo Glob",     DamageType = DamageType.Nature, Power = 1.0f },
                new() { Id = "root_wrap",    Name = "Root Wrap",    DamageType = DamageType.Nature, Power = 0.8f },
                new() { Id = "spore_cloud",  Name = "Spore Cloud",  DamageType = DamageType.Nature, Power = 0.7f, HitsAll = true },
                new() { Id = "acid_splash",  Name = "Acid Splash",  DamageType = DamageType.Nature, Power = 1.2f },
                new() { Id = "vine_whip",    Name = "Vine Whip",    DamageType = DamageType.Nature, Power = 1.0f },
                new() { Id = "natures_bite", Name = "Nature's Bite",DamageType = DamageType.Nature, Power = 1.3f },
            ],
        },
        new()
        {
            Id = "emberhatch", Name = "Emberhatch", Emoji = "🔥",
            Description = "A fierce hatchling wreathed in fire. Its enthusiasm exceeds its caution.",
            ElementType = DamageType.Fire, BaseAttack = 12,
            Moves =
            [
                new() { Id = "scratch",    Name = "Scratch",   DamageType = DamageType.Slashing, Power = 0.7f },
                new() { Id = "ember",      Name = "Ember",     DamageType = DamageType.Fire,     Power = 1.0f },
                new() { Id = "heat_wave",  Name = "Heat Wave", DamageType = DamageType.Fire,     Power = 0.8f, HitsAll = true },
                new() { Id = "ignite",     Name = "Ignite",    DamageType = DamageType.Fire,     Power = 1.2f },
                new() { Id = "fire_fang",  Name = "Fire Fang", DamageType = DamageType.Fire,     Power = 1.1f },
                new() { Id = "smolder",    Name = "Smolder",   DamageType = DamageType.Fire,     Power = 1.3f },
            ],
        },
        new()
        {
            Id = "frostling", Name = "Frostling", Emoji = "❄️",
            Description = "A delicate creature of living ice. It prefers cold places and cool company.",
            ElementType = DamageType.Frost, BaseAttack = 9,
            Moves =
            [
                new() { Id = "ice_shard",     Name = "Ice Shard",     DamageType = DamageType.Frost, Power = 0.9f },
                new() { Id = "frost_bite",    Name = "Frost Bite",    DamageType = DamageType.Frost, Power = 1.0f },
                new() { Id = "blizzard",      Name = "Blizzard",      DamageType = DamageType.Frost, Power = 0.7f, HitsAll = true },
                new() { Id = "chill_touch",   Name = "Chill Touch",   DamageType = DamageType.Frost, Power = 0.8f },
                new() { Id = "glacial_smash", Name = "Glacial Smash", DamageType = DamageType.Frost, Power = 1.3f },
                new() { Id = "arctic_breath", Name = "Arctic Breath", DamageType = DamageType.Frost, Power = 1.1f },
            ],
        },
        new()
        {
            Id = "sparkling", Name = "Sparkling", Emoji = "⚡",
            Description = "A crackling ball of storm energy. It is very excitable and will not sit still.",
            ElementType = DamageType.Storm, BaseAttack = 13,
            Moves =
            [
                new() { Id = "spark",           Name = "Spark",           DamageType = DamageType.Storm, Power = 0.7f },
                new() { Id = "thunderclap",     Name = "Thunderclap",     DamageType = DamageType.Storm, Power = 1.0f },
                new() { Id = "chain_lightning", Name = "Chain Lightning", DamageType = DamageType.Storm, Power = 0.8f, HitsAll = true },
                new() { Id = "static_shock",    Name = "Static Shock",    DamageType = DamageType.Storm, Power = 0.9f },
                new() { Id = "storm_surge",     Name = "Storm Surge",     DamageType = DamageType.Storm, Power = 1.2f },
                new() { Id = "arc_bolt",        Name = "Arc Bolt",        DamageType = DamageType.Storm, Power = 1.3f },
            ],
        },
        new()
        {
            Id = "shadelurk", Name = "Shadelurk", Emoji = "🌑",
            Description = "A creature born from shed darkness. It follows at the edge of your shadow, barely visible.",
            ElementType = DamageType.Dark, BaseAttack = 10,
            Moves =
            [
                new() { Id = "shadow_strike", Name = "Shadow Strike", DamageType = DamageType.Dark, Power = 1.0f },
                new() { Id = "umbral_slash",  Name = "Umbral Slash",  DamageType = DamageType.Dark, Power = 1.1f },
                new() { Id = "void_tap",      Name = "Void Tap",      DamageType = DamageType.Dark, Power = 0.8f },
                new() { Id = "dark_pulse",    Name = "Dark Pulse",    DamageType = DamageType.Dark, Power = 0.9f },
                new() { Id = "nightmare",     Name = "Nightmare",     DamageType = DamageType.Dark, Power = 1.3f },
                new() { Id = "dusk_fang",     Name = "Dusk Fang",     DamageType = DamageType.Dark, Power = 1.1f },
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
