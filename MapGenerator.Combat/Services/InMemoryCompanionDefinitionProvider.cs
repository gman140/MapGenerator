using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemoryCompanionDefinitionProvider : ICompanionDefinitionProvider
{
    // ── Sprite helper ────────────────────────────────────────────────────────
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
            Id = "slimeling", Name = "Slimeling",
            Description = "A cheerful blob of animated slime. Not threatening, but surprisingly loyal.",
            ElementTypes = [DamageType.Nature], BaseAttack = 4,
            DefaultSprite = _slimelingSprite,
            BaseVitality = 7, BaseDefense = 2, BaseSpeed = 4, BaseFocus = 2, BaseResist = 2,
        },
        new()
        {
            Id = "emberhatch", Name = "Emberhatch",
            Description = "A fierce hatchling wreathed in fire. Its enthusiasm exceeds its caution.",
            ElementTypes = [DamageType.Fire], BaseAttack = 5,
            DefaultSprite = _emberHatchSprite,
            BaseVitality = 5, BaseDefense = 1, BaseSpeed = 6, BaseFocus = 3, BaseResist = 1,
        },
        new()
        {
            Id = "frostling", Name = "Frostling",
            Description = "A delicate creature of living ice. It prefers cold places and cool company.",
            ElementTypes = [DamageType.Frost], BaseAttack = 4,
            DefaultSprite = _frostlingSprite,
            BaseVitality = 6, BaseDefense = 4, BaseSpeed = 3, BaseFocus = 2, BaseResist = 3,
        },
        new()
        {
            Id = "sparkling", Name = "Sparkling",
            Description = "A crackling ball of storm energy. It is very excitable and will not sit still.",
            ElementTypes = [DamageType.Storm], BaseAttack = 5,
            DefaultSprite = _sparklingSprite,
            BaseVitality = 4, BaseDefense = 1, BaseSpeed = 8, BaseFocus = 4, BaseResist = 1,
        },
        new()
        {
            Id = "shadelurk", Name = "Shadelurk",
            Description = "A creature born from shed darkness. It follows at the edge of your shadow, barely visible.",
            ElementTypes = [DamageType.Dark], BaseAttack = 4,
            DefaultSprite = _shadelurkSprite,
            BaseVitality = 5, BaseDefense = 3, BaseSpeed = 5, BaseFocus = 5, BaseResist = 2,
        }
    ];

    private static readonly Dictionary<string, CompanionDefinition> _byId =
        _all.ToDictionary(d => d.Id);

    public CompanionDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;

    public IReadOnlyList<CompanionDefinition> GetAll() => _all;
}
