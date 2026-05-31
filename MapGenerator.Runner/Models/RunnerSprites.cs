namespace MapGenerator.Runner.Models;

/// <summary>
/// Built-in 16×16 pixel art for runner enemies.
/// Each array is 256 color strings (row-major). Empty string = transparent.
/// </summary>
public static class RunnerSprites
{
    // ── Warrior ────────────────────────────────────────────────────────────────
    // Red-armored melee fighter, side profile facing left.
    // Head/helmet on left; sword tip at far left (cols 0-1); body cols 2-9.
    public static readonly string[] Warrior = Build(
    [
        "________________",  // row  0
        "___HHHH_________",  // row  1  helmet top
        "__HHHHHH________",  // row  2  helmet
        "__HFFFHH________",  // row  3  face
        "__HFXFHH________",  // row  4  eye
        "__HFFFH_________",  // row  5  lower face
        "___HHH__________",  // row  6  chin guard
        "SS_RRRRRRr______",  // row  7  sword blade + shoulders
        "G_RRRRRRRRr_____",  // row  8  sword guard + chest
        "__rRRRRRRr______",  // row  9  body
        "__rRRRRRr_______",  // row 10  lower body
        "___DRRRd________",  // row 11  belt
        "___RR_RR________",  // row 12  thighs
        "___RR_RR________",  // row 13  shins
        "___DD_DD________",  // row 14  boots
        "___dD_Dd________",  // row 15  boot toes
    ],
    new()
    {
        ['_'] = "",
        ['H'] = "#888899",   // steel helmet
        ['F'] = "#ffddcc",   // face skin
        ['X'] = "#222233",   // eye
        ['R'] = "#cc3333",   // red armor
        ['r'] = "#991111",   // dark red shadow
        ['S'] = "#ddddee",   // sword blade silver
        ['G'] = "#eecc44",   // sword guard gold
        ['D'] = "#553322",   // dark boots / belt
        ['d'] = "#331100",   // darker toe / toe shadow
    });

    // ── Archer ─────────────────────────────────────────────────────────────────
    // Green-hooded ranged fighter, side profile facing left.
    // Head/hood on left; bow on far left (col 0); body cols 2-9.
    public static readonly string[] Archer = Build(
    [
        "________________",  // row  0
        "___VVVV_________",  // row  1  hood top
        "__VVVVVV________",  // row  2  hood
        "__VFFFVV________",  // row  3  face
        "__VFXFVV________",  // row  4  eye
        "__VFFFV_________",  // row  5  lower face
        "___VVV__________",  // row  6  chin
        "W__LLLLLLl______",  // row  7  bow top + shoulders
        "Ww_LLLLLLLl_____",  // row  8  bow mid + chest
        "W__lLLLLLl______",  // row  9  bow + body
        "W__lLLLLl_______",  // row 10  bow tip + lower body
        "_W__LLl_________",  // row 11  bow bottom + waist
        "___LL_LL________",  // row 12  thighs
        "___LL_LL________",  // row 13  shins
        "___BB_BB________",  // row 14  boots
        "___bB_Bb________",  // row 15  boot toes
    ],
    new()
    {
        ['_'] = "",
        ['V'] = "#3a6622",   // dark green hood
        ['F'] = "#ffddcc",   // face skin
        ['X'] = "#222233",   // eye
        ['W'] = "#9b6328",   // bow wood (brown)
        ['w'] = "#c48040",   // bow wood highlight
        ['L'] = "#8b6644",   // leather armor
        ['l'] = "#664433",   // leather shadow
        ['B'] = "#443322",   // boots
        ['b'] = "#221100",   // boot toe shadow
    });

    private static string[] Build(string[] rows, Dictionary<char, string> palette)
    {
        var pixels = new string[256];
        int i = 0;
        foreach (var row in rows)
            foreach (char c in row)
                pixels[i++] = palette.TryGetValue(c, out var col) ? col : "";
        return pixels;
    }
}
