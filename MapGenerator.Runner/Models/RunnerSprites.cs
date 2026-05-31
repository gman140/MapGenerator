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
        "......ABC.......",
        ".....DAECC......",
        ".....DEEECC.....",
        ".....DAECCC.....",
        ".....DAABBC.....",
        ".....FGGFHGH....",
        ".....IGGHHG.....",
        ".....IFFHH......",
        "......IIGH......",
        ".....IGGIGH.....",
        "....GIGGGIGH....",
        "....GIGGGIGH....",
        "....GIGGGIGH....",
        "....GDAAAAIG....",
        ".JKLLLLLLLIGLLL.",
        ".....DB..AB....."
    ],
    new()
    {
        ['.'] = "",
        ['A'] = "#4d3a2dff", 
        ['B'] = "#584132ff", 
        ['C'] = "#644937ff", 
        ['D'] = "#3f2f25ff", 
        ['E'] = "#630009ff", 
        ['F'] = "#1b1917ff", 
        ['G'] = "#6e9b27ff", 
        ['H'] = "#81b037ff", 
        ['I'] = "#5d8321ff", 
        ['J'] = "#9b897dff", 
        ['K'] = "#8a7466ff", 
        ['L'] = "#504843ff"
    });

    // ── Archer ─────────────────────────────────────────────────────────────────
    // Green-hooded ranged fighter, side profile facing left.
    // Head/hood on left; bow on far left (col 0); body cols 2-9.
    public static readonly string[] Archer = Build(
    [
        "........AAAA....",
        ".......BBAAAA...",
        "......CBBBAABA..",
        ".....DEFDDDG....",
        "....CCCCCCCCC...",
        ".....HIIHIEJ....",
        ".....KIIKLEJ....",
        ".....HIILLJJ....",
        ".....EMMEJN.....",
        ".....EJJJL......",
        ".....EEJABO.....",
        "....CEEJAABO....",
        "...ILEEAAAIL....",
        "...HICBBAAIL....",
        ".....CBBBABO....",
        ".....CCBBAAAO..."
    ],
    new()
    {
        ['.'] = "",
        ['A'] = "#506287ff", 
        ['B'] = "#495774ff", 
        ['C'] = "#404a5fff", 
        ['D'] = "#2d332bff", 
        ['E'] = "#bdc0c6ff", 
        ['F'] = "#ccd2dcff", 
        ['G'] = "#444b43ff", 
        ['H'] = "#e5985aff", 
        ['I'] = "#f3ad75ff", 
        ['J'] = "#d3d6dbff", 
        ['K'] = "#473b33ff", 
        ['L'] = "#f9c194ff", 
        ['M'] = "#2f2a24ff", 
        ['N'] = "#ffd1acff", 
        ['O'] = "#536b9aff"
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
