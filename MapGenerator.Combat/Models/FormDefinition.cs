using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class FormDefinition
{
    public CompanionForm Id          { get; init; }
    public string        DisplayName { get; init; } = "";
    public string        Hint        { get; init; } = "";
    public int           AtkBonus    { get; init; }
    public int           DefBonus    { get; init; }
    public int           VitBonus    { get; init; }
    public int           SpdBonus    { get; init; }
    public int           FocBonus    { get; init; }
    public int           ResBonus    { get; init; }
    public int           GridCols    { get; init; }
    public int           SpriteSize  { get; init; }
    public int           MaxMoves    { get; init; }
}

public static class FormRegistry
{
    private static readonly Dictionary<CompanionForm, FormDefinition> _all = new()
    {
        [CompanionForm.None] = new()
        {
            Id = CompanionForm.None, DisplayName = "", Hint = "",
            GridCols = 16, SpriteSize = 256, MaxMoves = 4,
        },
        [CompanionForm.Fierce] = new()
        {
            Id = CompanionForm.Fierce, DisplayName = "Fierce", Hint = "+ATK×2, -DEF",
            AtkBonus = 4, DefBonus = -2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Nimble] = new()
        {
            Id = CompanionForm.Nimble, DisplayName = "Nimble", Hint = "+SPD, +FOC",
            SpdBonus = 3, FocBonus = 2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Sturdy] = new()
        {
            Id = CompanionForm.Sturdy, DisplayName = "Sturdy", Hint = "+DEF, +VIT",
            DefBonus = 3, VitBonus = 2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Arcane] = new()
        {
            Id = CompanionForm.Arcane, DisplayName = "Arcane", Hint = "+FOC×2, +RES",
            FocBonus = 3, ResBonus = 2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Savage] = new()
        {
            Id = CompanionForm.Savage, DisplayName = "Savage", Hint = "+ATK, +SPD, -DEF",
            AtkBonus = 3, SpdBonus = 2, DefBonus = -2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Guardian] = new()
        {
            Id = CompanionForm.Guardian, DisplayName = "Guardian", Hint = "+DEF, +VIT, +RES",
            DefBonus = 2, VitBonus = 2, ResBonus = 2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Brutal] = new()
        {
            Id = CompanionForm.Brutal, DisplayName = "Brutal", Hint = "+ATK×2, +VIT, -SPD",
            AtkBonus = 4, VitBonus = 2, SpdBonus = -2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
        [CompanionForm.Radiant] = new()
        {
            Id = CompanionForm.Radiant, DisplayName = "Radiant", Hint = "+RES×2, +DEF",
            ResBonus = 4, DefBonus = 2,
            GridCols = 20, SpriteSize = 400, MaxMoves = 5,
        },
    };

    public static FormDefinition Get(CompanionForm f) => _all[f];

    public static CompanionForm Roll(Random rng)
    {
        var choices = _all.Keys.Where(k => k != CompanionForm.None).ToArray();
        return choices[rng.Next(choices.Length)];
    }

    public static string[] ExpandSprite(string[] pixels, int fromCols, int toCols)
    {
        if (fromCols >= toCols) return pixels;
        var result = new string[toCols * toCols];
        int offset = (toCols - fromCols) / 2;
        for (int r = 0; r < fromCols; r++)
            for (int c = 0; c < fromCols; c++)
            {
                int srcIdx = r * fromCols + c;
                if (srcIdx >= pixels.Length) continue;
                result[(r + offset) * toCols + (c + offset)] = pixels[srcIdx];
            }
        return result;
    }
}
