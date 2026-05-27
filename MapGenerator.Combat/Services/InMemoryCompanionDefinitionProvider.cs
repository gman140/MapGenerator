using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemoryCompanionDefinitionProvider : ICompanionDefinitionProvider
{
    // ── Sprite helpers ───────────────────────────────────────────────────────
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

    private static string[] Spr20(Dictionary<char, string> pal, string rows)
    {
        var result = new string[400];
        for (int i = 0; i < 400 && i < rows.Length; i++)
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
        new() { ['A'] = "#1b141eff", ['B'] = "#759da9ff", ['C'] = "#fdf5f1ff", ['D'] = "#b8d8d1ff", ['E'] = "#eb8a06ff", ['F'] = "#be5340ff" },
        "...A.AAAAAA....." +
        "..AAAAAAAAAAA..." +
        "...AABCCDADCBA.." +
        "..AAACCACCCACA.." +
        "..AAACCACCCACA.." +
        "..AAACCCEEECCA.." +
        "..AAACCCFACCCA.." +
        ".AAAABCCCCCCBAA." +
        "ABDCAAADCCCDAABA" +
        "ADCCDADCCCCCDABA" +
        "AADCBACCCCCCCAAA" +
        ".AAAAADCCCCEEEA." +
        "..AAAABDCCEEFFA." +
        "..AFAAAAAFEFFA.." +
        "..AFFFAAAAAAA..." +
        "...AAAA........."
    );

    private static readonly string[] _sparklingSprite = Spr(
        new() { ['A'] = "#38aa91ff", ['B'] = "#7becbfff", ['C'] = "#fdf5f1ff" },
        "...AA.....AA...." +
        "..ABBAAAAABBA..." +
        ".ABCCCBBBCCCBA.." +
        ".ABBCCCCCCCBBBA." +
        ".ABBBBBBBBBBBBA." +
        ".ABBBBCCCCBBBBBA" +
        "ABBBCCCCCCCCBBBA" +
        "ABBBCCCCCCCCBBBA" +
        "ABBCCCCCCCACCBBA" +
        "ABBCCCCACCACCBBA" +
        "ABBCCCCACCACCBBA" +
        ".ABCCCCACCCCCBA." +
        ".ABBCCCCCCCCBBA." +
        "..ABBBCCCCBBBA.." +
        "...AABBBBBBAA..." +
        ".....AAAAAA....."
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

    // ── Evolved 20×20 sprites ────────────────────────────────────────────────

    // Nature — larger slimeling with leaf crown and defined face
    private static readonly string[] _slimelingSprite20 = Spr20(
        new() { ['A'] = "#1b141eff", ['B'] = "#68b229ff", ['C'] = "#ea6d9dff", ['D'] = "#af407fff", ['E'] = "#75224aff", ['F'] = "#fdf5f1ff", ['G'] = "#ffb2b2ff", ['H'] = "#cde042ff" },
        "...........AAA......" +
        "...........ABBA....." +
        "............ABA....." +
        "...........ABA......" +
        "...AAA.....ABA......" +
        "..ABBBAAAAABA......." +
        "..ABAABBBBBBA......." +
        "..AA..AACCDCA......." +
        ".......ACCCCA......." +
        ".....AAACCCCAAA....." +
        "....ADCCDDDDCCDA...." +
        "...ADCDDCCCCDDCDA..." +
        "..ADDDCCCCECCCDDDA.." +
        "..ADCCCCCCCCCCCCDA.." +
        "..ADCCCDAACAADCCDA.." +
        "..ADCCAAAACAAAACDA.." +
        "..ACACDAFDCDFADACA.." +
        "..ACACCCCCCCCCCACA.." +
        ".ACCADCCCGGCCCDACCA." +
        "ACCDHCCDAAAADCHCDCCA"
    );

    // Fire — winged emberhatch with fuller flame body
    private static readonly string[] _emberHatchSprite20 = Spr20(
        new() { ['A'] = "#1b141eff", ['B'] = "#eb8a06ff", ['C'] = "#ffc95cff", ['D'] = "#b1415cff", ['E'] = "#eb7171ff", ['F'] = "#b8d8d1ff", ['G'] = "#788ddeff", ['H'] = "#fdf5f1ff", ['I'] = "#5458c0ff", ['J'] = "#be5340ff", ['K'] = "#526a98ff", ['L'] = "#759da9ff" },
        ".......AAAAA.AAA...." +
        "......ABCCCBADEEA..." +
        ".....ABCCCCCCEEEAA.." +
        "....AACAAACCCCDDCBA." +
        "....ADCFAGACCCCCCCBA" +
        "....ADCHIABCCBJCJBCA" +
        "...AAACCCCCCCCCCCCCA" +
        "...ADBCCCCCCCCCCCCCA" +
        "...ADCCCCJFHFJJJJJJA" +
        ".AAAACCCCCBHBCCCCCA." +
        "ACCABCCCCCBBJAAAAA.." +
        "ACCCBBCCCBKKKJA....." +
        ".ACCBBCCCCJLLLJA...." +
        ".ABCCBCCCCCJKKJJA..." +
        "..ABCBCBBCBHLJJFA..." +
        "...ABBBCJHJJJFAA...." +
        "....ABBBJJBBBJA....." +
        ".....AABBBAAJBA....." +
        ".....ABCCA.ABBBA...." +
        ".....ABHBHAABBJFA..."
    );

    // Frost — elaborate ice crystal creature with crystal arms
    private static readonly string[] _frostlingSprite20 = Spr20(
        new() { ['A'] = "#293a49ff", ['B'] = "#15161aff", ['C'] = "#3b4f62ff", ['D'] = "#64669fff", ['E'] = "#546c6aff", ['F'] = "#698f80ff", ['G'] = "#8c0099ff", ['H'] = "#e0bcc5ff" },
        "...AA..............." +
        "...BCAA............." +
        "..BBDDCA............" +
        "...BAEDCA..........." +
        "....BAADCA.........." +
        "....BAAADDA........." +
        "...BAACCACC........." +
        "...BCCAAACCA........" +
        "....BBAAACCAAA......" +
        ".....BAACACBCFAA...." +
        ".....BACAAABCCFFA..." +
        "....BAACAAAABCEFBA.." +
        "....BAAACAABBEFBGFA." +
        ".....BABACBEBEFFFEB." +
        ".....BBBBAABFBEEHB.." +
        "......BCCBABCEBBB..." +
        ".....AFCCCBBEEFB...." +
        "....ACEEEEEFFCB....." +
        "...ABAAFFFCCB......." +
        "...B.BBBBBB........."
    );

    // Storm — energetic sparkling with defined lightning body
    private static readonly string[] _sparklingSprite20 = Spr20(
        new() { ['A'] = "#e0b571ff", ['B'] = "#c79546ff", ['C'] = "#e0cca7ff", ['D'] = "#a8631cff", ['E'] = "#c1acabff", ['F'] = "#e0e0e0ff", ['G'] = "#a7744aff", ['H'] = "#2d2929ff", ['I'] = "#7c5c44ff" },
        ".......AAABAA......." +
        ".....AACCABAAAB....." +
        "...AABCAAABAABBBB..." +
        "..ACAABAAAABBBBBBB.." +
        ".ACCAABDBBDBBBEFBB.." +
        ".ACAABBBDDDBBEFFEBD." +
        ".BBAABBD...DBFFEFFD." +
        ".AABBDD.....DEEFFFE." +
        ".ACABBD.....DDFEFE.." +
        ".ACABBD......DDEE..." +
        "..ACBBD............." +
        "..ACABBD............" +
        "...AABDBD..........." +
        "...BBABBD..........." +
        "....AAABBD.........." +
        "....ACAABBD........." +
        ".....ACABBB........." +
        ".GG..AAABDD........." +
        "GGGGGAABBBDHH.GG...." +
        "GGGIIIIHHGGIHHHGGI.."
    );

    // Dark — large cloaked shadelurk with flowing shadow tendrils
    private static readonly string[] _shadelurkSprite20 = Spr20(
        new() { ['A'] = "#1b141eff", ['B'] = "#ccc1beff", ['C'] = "#fdf5f1ff", ['D'] = "#918692ff", ['E'] = "#e25322ff", ['F'] = "#ffffffff" },
        "...........AAAAA...." +
        ".........AABCCCCAA.." +
        "........ADBBBCCCCBA." +
        ".......ABBBBBCCCCCBA" +
        ".......ABBBDBCCCCCCA" +
        ".......ABBDDAACCCCCA" +
        ".......ADBDAAAABBCBA" +
        "........ABCAAEACAAAA" +
        ".......ADABCAACDEAA." +
        ".......AADBBCCBDAAA." +
        ".....AADADBCCCCCCCA." +
        "...AACBCCAAADCDCBA.." +
        "..ABCAABCCDCAABCA..." +
        ".ACAA..ADADA..AAAA.." +
        ".ABA..ACDBDCA..ADA.." +
        "ACCCA.ABCACBA.ABBBA." +
        "ACCDAADAA.AADAABBDA." +
        "ACBA.ACA...ABAABDA.." +
        ".AA..ABFA..ADBAAA..." +
        "......AAA...AAA....."
    );

    // ── Companion definitions ────────────────────────────────────────────────

    private static readonly List<CompanionDefinition> _all =
    [
        new()
        {
            Id = "slimeling", Name = "Slimeling",
            Description = "A cheerful blob of animated slime. Not threatening, but surprisingly loyal.",
            ElementTypes = [DamageType.Nature], BaseAttack = 4,
            DefaultSprite = _slimelingSprite, DefaultSpriteEvolved = _slimelingSprite20,
            BaseVitality = 7, BaseDefense = 2, BaseSpeed = 4, BaseFocus = 2, BaseResist = 2,
        },
        new()
        {
            Id = "emberhatch", Name = "Emberhatch",
            Description = "A fierce hatchling wreathed in fire. Its enthusiasm exceeds its caution.",
            ElementTypes = [DamageType.Fire], BaseAttack = 5,
            DefaultSprite = _emberHatchSprite, DefaultSpriteEvolved = _emberHatchSprite20,
            BaseVitality = 5, BaseDefense = 1, BaseSpeed = 6, BaseFocus = 3, BaseResist = 1,
        },
        new()
        {
            Id = "frostling", Name = "Frostling",
            Description = "A delicate creature of living ice. It prefers cold places and cool company.",
            ElementTypes = [DamageType.Frost], BaseAttack = 4,
            DefaultSprite = _frostlingSprite, DefaultSpriteEvolved = _frostlingSprite20,
            BaseVitality = 6, BaseDefense = 4, BaseSpeed = 3, BaseFocus = 2, BaseResist = 3,
        },
        new()
        {
            Id = "sparkling", Name = "Sparkling",
            Description = "A crackling ball of storm energy. It is very excitable and will not sit still.",
            ElementTypes = [DamageType.Storm], BaseAttack = 5,
            DefaultSprite = _sparklingSprite, DefaultSpriteEvolved = _sparklingSprite20,
            BaseVitality = 4, BaseDefense = 1, BaseSpeed = 8, BaseFocus = 4, BaseResist = 1,
        },
        new()
        {
            Id = "shadelurk", Name = "Shadelurk",
            Description = "A creature born from shed darkness. It follows at the edge of your shadow, barely visible.",
            ElementTypes = [DamageType.Dark], BaseAttack = 4,
            DefaultSprite = _shadelurkSprite, DefaultSpriteEvolved = _shadelurkSprite20,
            BaseVitality = 5, BaseDefense = 3, BaseSpeed = 5, BaseFocus = 5, BaseResist = 2,
        }
    ];

    private static readonly Dictionary<string, CompanionDefinition> _byId =
        _all.ToDictionary(d => d.Id);

    public CompanionDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;

    public IReadOnlyList<CompanionDefinition> GetAll() => _all;
}
