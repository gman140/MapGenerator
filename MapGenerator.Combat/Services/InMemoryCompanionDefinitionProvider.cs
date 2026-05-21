using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemoryCompanionDefinitionProvider : ICompanionDefinitionProvider
{
    // ── Sprite helper ────────────────────────────────────────────────────────
    // rows = 256-char string (16 rows × 16 cols). '.' = transparent, other chars map via pal.
    private static string[] Spr(Dictionary<char, string> pal, string rows)
    {
        var result = new string[256];
        for (int i = 0; i < 256 && i < rows.Length; i++)
        {
            char c = rows[i];
            result[i] = c != '.' && pal.TryGetValue(c, out var col) ? col : "";
        }
        return result;
    }

    // ── Default sprites ──────────────────────────────────────────────────────

    // Slimeling — round green blob with dot eyes
    private static readonly string[] _slimelingSprite = Spr(
        new() { ['A'] = "#78ae79ff", ['B'] = "#95cb84ff", ['C'] = "#5d9074ff", ['D'] = "#426d62ff", ['E'] = "#b9ba57ff", ['F'] = "#292b3aff", ['G'] = "#bcde94ff", ['H'] = "#f9ebd8ff", ['I'] = "#d6c5b8ff", ['J'] = "#924e64ff", ['K'] = "#f2a198ff", ['L'] = "#cf6e84ff", ['M'] = "#444f54ff" },
        "................" +
        "........AB..CA.." +
        "......BBBBACABB." +
        ".BB..CDEBDBAADDD" +
        ".ADAAAACDABBABD." +
        ".ABBBBADAAFBBB.." +
        "ACBBGBAAAAABHB.." +
        "DAABGAAAAAACHHH." +
        ".CAAAAACAAHHHHH." +
        ".CAAAAACCAIHJJI." +
        ".DCACCCCCCIIKL.." +
        ".CCCDDCCCCCCII.." +
        ".MCDDDDCCDDCD..." +
        "..MM...MCM.DM..." +
        "................" +
        "................"
    );

    // Emberhatch — orange fire creature with yellow flame crest
    private static readonly string[] _emberHatchSprite = Spr(
        new() { ['A'] = "#673452ff", ['B'] = "#e64863ff", ['C'] = "#b0335eff", ['D'] = "#f2bc74ff", ['E'] = "#ce8b51ff", ['F'] = "#ac4a52ff", ['G'] = "#bf6c52ff", ['H'] = "#4f5474ff", ['I'] = "#f3ffe1ff" },
        "................" +
        ".......AA......." +
        "......ABBA......" +
        "......CCBC......" +
        ".....ACDBCA....." +
        ".....ADDDEA....." +
        "....ACDBBECA...." +
        "....AACABCAA...." +
        "...AFGGEAAGFA..." +
        "..AFDDDEEEGEFA.." +
        ".AFDAHDDDDHAEFA." +
        ".AGDDIDDDDIDEGA." +
        ".AGEDDDDDDDDEGA." +
        "..AFGGGGGGGGFA.." +
        "...AAAAAAAAAA..." +
        "................"
    );

    // Frostling — pale blue crystalline creature with icy crystal tip
    private static readonly string[] _frostlingSprite = Spr(
        new() { ['A'] = "#5abfca8c", ['B'] = "#3a4f2cff", ['C'] = "#5abfcaff", ['D'] = "#d4dcf2ff", ['E'] = "#92c7cdff", ['F'] = "#537e60ff" },
        "................" +
        "................" +
        "................" +
        "................" +
        "................" +
        ".....A.........." +
        "...A......BCB..." +
        "......A...DBDB.." +
        ".........ECDBEBB" +
        ".EBECEEECDCEDEF." +
        "FCBBBBBBBBDE...." +
        "EFFDDDDDDDBE...." +
        "..ECDEEEECEF...." +
        ".BFFFF..FECB...." +
        ".BB.BB....BBB..." +
        "................"
    );

    // Sparkling — bright yellow crackling energy ball with electric sparks
    private static readonly string[] _sparklingSprite = Spr(
        new() { ['A'] = "#ffe400be", ['B'] = "#ffe872ff", ['C'] = "#f3c73fff", ['D'] = "#dec6bfff", ['E'] = "#ffffffff", ['F'] = "#fff6b6ff", ['G'] = "#d18d0eff", ['H'] = "#a7744aff", ['I'] = "#2d2929ff", ['J'] = "#7c5c44ff" },
        ".A..A.A..BBCCC.." +
        "..A.A...CBCDEED." +
        "A.A...BBCCEDEED." +
        ".A.A.BFFBCEDDED." +
        "....CFFBBCEEDED." +
        ".A.BFCBBCGCCDDG." +
        ".A.BFBBCCCGGGG.." +
        "A.BFFBCGCG......" +
        "..BFBBCCG......." +
        "..BFBCCCG......." +
        ".CCCBGGG........" +
        ".BFBBCCG........" +
        ".BFBBCCCGG......" +
        "..BFBBCGCCG....." +
        "HHHBBBBBCGGIIHH." +
        "IHHJJJJIIHJIIIHH"
    );

    // Shadelurk — dark purple shadow creature with glowing eyes and wispy wings
    private static readonly string[] _shadelurkSprite = Spr(
        new() { ['A'] = "#978ed9ee", ['B'] = "#beb0f3ee", ['C'] = "#815bd7ee", ['D'] = "#472cb3ee", ['E'] = "#c4c9d9ee", ['F'] = "#444444ee", ['G'] = "#7e4854ee", ['H'] = "#ffffffee", ['I'] = "#262626ee", ['J'] = "#d992a1ee", ['K'] = "#ac6d7aee", ['L'] = "#372680ee" },
        "......ABBBBB...." +
        "....AAAABBBBBB.." +
        "...AAAAABBBBBBB." +
        "...AAAACCCCCCCB." +
        "..AAACDDDDDDDCCB" +
        "..ACDDEEFGGGGFEC" +
        ".DCDCEHHIJIFHHHE" +
        ".DDCHHHHIKIIKGHE" +
        ".DDDCBEHIKJKJGHE" +
        ".DCCDDCCEIIFFEEC" +
        "DDLCCDDDDDDDCCC." +
        "DDLCCCCCCCCAAAB." +
        ".DLLCCCCAAAAAB.." +
        ".LL...CCCCCA...." +
        ".LL............." +
        ".L.............."
    );

    // ── Companion definitions ────────────────────────────────────────────────

    private static readonly List<CompanionDefinition> _all =
    [
        new()
        {
            Id = "slimeling", Name = "Slimeling", Emoji = "🟢",
            Description = "A cheerful blob of animated slime. Not threatening, but surprisingly loyal.",
            ElementType = DamageType.Nature, BaseAttack = 4,
            DefaultSprite = _slimelingSprite,
            Moves =
            [
                new() { Id = "glob_bonk",      Name = "Glob Bonk",      Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 1.0f },
                new() { Id = "dribble_splash", Name = "Dribble Splash", Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 0.85f },
                new() { Id = "spore_puff",     Name = "Spore Puff",     Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 0.75f, HitsAll = true },
                new() { Id = "acid_drool",     Name = "Acid Drool",     Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 1.2f },
                new() { Id = "slick_coat",     Name = "Slick Coat",     Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Defense,    EffectValue = 2f,    EffectTurns = 2 },
                new() { Id = "mend_goo",       Name = "Mend Goo",       Kind = CompanionMoveKind.PlayerBuff,  EffectStat = null,                    EffectValue = 8f },
                new() { Id = "gunk_wrap",      Name = "Gunk Wrap",      Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2 },
                new() { Id = "ooze_drench",    Name = "Ooze Drench",    Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,   EffectValue = -3f,   EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "emberhatch", Name = "Emberhatch", Emoji = "🔥",
            Description = "A fierce hatchling wreathed in fire. Its enthusiasm exceeds its caution.",
            ElementType = DamageType.Fire, BaseAttack = 5,
            DefaultSprite = _emberHatchSprite,
            Moves =
            [
                new() { Id = "scratch",       Name = "Scratch",       Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Slashing, Power = 0.8f },
                new() { Id = "ember_blob",    Name = "Ember Blob",    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Fire,     Power = 1.0f },
                new() { Id = "cinder_flurry", Name = "Cinder Flurry", Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Fire,     Power = 0.75f, HitsAll = true },
                new() { Id = "hot_bite",      Name = "Hot Bite",      Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Fire,     Power = 1.15f },
                new() { Id = "yowl",          Name = "Yowl",          Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Attack,      EffectValue = 4f,    EffectTurns = 2 },
                new() { Id = "hype_dash",     Name = "Hype Dash",     Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.DodgeChance, EffectValue = 0.12f, EffectTurns = 2 },
                new() { Id = "char",          Name = "Char",          Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -3f,   EffectTurns = 2 },
                new() { Id = "choke_smoke",   Name = "Choke Smoke",   Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "frostling", Name = "Frostling", Emoji = "❄️",
            Description = "A delicate creature of living ice. It prefers cold places and cool company.",
            ElementType = DamageType.Frost, BaseAttack = 4,
            DefaultSprite = _frostlingSprite,
            Moves =
            [
                new() { Id = "frost_peck",   Name = "Frost Peck",   Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 0.9f },
                new() { Id = "shiver_bite",  Name = "Shiver Bite",  Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 1.0f },
                new() { Id = "hail_spit",    Name = "Hail Spit",    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 0.75f, HitsAll = true },
                new() { Id = "ice_spike",    Name = "Ice Spike",    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 1.2f },
                new() { Id = "frost_shell",  Name = "Frost Shell",  Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Defense,    EffectValue = 3f,    EffectTurns = 3 },
                new() { Id = "ice_mend",     Name = "Ice Mend",     Kind = CompanionMoveKind.PlayerBuff,  EffectStat = null,                    EffectValue = 7f },
                new() { Id = "sluggify",     Name = "Sluggify",     Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2 },
                new() { Id = "brittle_rub",  Name = "Brittle Rub",  Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,   EffectValue = -3f,   EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "sparkling", Name = "Sparkling", Emoji = "⚡",
            Description = "A crackling ball of storm energy. It is very excitable and will not sit still.",
            ElementType = DamageType.Storm, BaseAttack = 5,
            DefaultSprite = _sparklingSprite,
            Moves =
            [
                new() { Id = "nip_shock",     Name = "Nip Shock",     Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 0.8f },
                new() { Id = "crackle_bite",  Name = "Crackle Bite",  Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 1.0f },
                new() { Id = "pop_burst",     Name = "Pop Burst",     Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 0.75f, HitsAll = true },
                new() { Id = "thundersnap",   Name = "Thundersnap",   Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 1.2f },
                new() { Id = "amp_up",        Name = "Amp Up",        Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Attack,      EffectValue = 4f,    EffectTurns = 2 },
                new() { Id = "jitterstep",    Name = "Jitterstep",    Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.DodgeChance, EffectValue = 0.12f, EffectTurns = 2 },
                new() { Id = "short_circuit", Name = "Short Circuit", Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -4f,   EffectTurns = 2 },
                new() { Id = "zap_daze",      Name = "Zap Daze",      Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2 },
            ],
        },
        new()
        {
            Id = "shadelurk", Name = "Shadelurk", Emoji = "🌑",
            Description = "A creature born from shed darkness. It follows at the edge of your shadow, barely visible.",
            ElementType = DamageType.Dark, BaseAttack = 4,
            DefaultSprite = _shadelurkSprite,
            Moves =
            [
                new() { Id = "nip",          Name = "Nip",          Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 0.9f },
                new() { Id = "shadow_lunge", Name = "Shadow Lunge", Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 1.0f },
                new() { Id = "dusk_scatter", Name = "Dusk Scatter", Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 0.75f, HitsAll = true },
                new() { Id = "void_fang",    Name = "Void Fang",    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 1.2f },
                new() { Id = "shade_slip",   Name = "Shade Slip",   Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.DodgeChance, EffectValue = 0.15f, EffectTurns = 2 },
                new() { Id = "void_drink",   Name = "Void Drink",   Kind = CompanionMoveKind.PlayerBuff,  EffectStat = null,                     EffectValue = 8f },
                new() { Id = "rattle",       Name = "Rattle",       Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2 },
                new() { Id = "peel_apart",   Name = "Peel Apart",   Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,   EffectValue = -4f,   EffectTurns = 2 },
            ],
        }
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
