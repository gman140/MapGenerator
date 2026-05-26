using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class TierDefinition
{
    public CompanionTier Id          { get; init; }
    public string        DisplayName { get; init; } = "";
    public int           SpriteSize  { get; init; }
    public int           GridCols    { get; init; }
    public int           MaxMoves    { get; init; }
}

public static class TierRegistry
{
    private static readonly Dictionary<CompanionTier, TierDefinition> _all = new()
    {
        [CompanionTier.T1] = new() { Id = CompanionTier.T1, DisplayName = "Base",    SpriteSize = 256, GridCols = 16, MaxMoves = 4 },
        [CompanionTier.T2] = new() { Id = CompanionTier.T2, DisplayName = "Evolved", SpriteSize = 400, GridCols = 20, MaxMoves = 5 },
        [CompanionTier.T3] = new() { Id = CompanionTier.T3, DisplayName = "Apex",    SpriteSize = 576, GridCols = 24, MaxMoves = 6 },
    };

    public static TierDefinition Get(CompanionTier tier) => _all[tier];

    public static string[] ExpandSprite(string[] pixels, CompanionTier fromTier, CompanionTier toTier)
    {
        var from = Get(fromTier);
        var to   = Get(toTier);
        if (from.GridCols >= to.GridCols) return pixels;

        var result = new string[to.SpriteSize];
        int offset = (to.GridCols - from.GridCols) / 2;

        for (int r = 0; r < from.GridCols; r++)
            for (int c = 0; c < from.GridCols; c++)
            {
                int srcIdx = r * from.GridCols + c;
                if (srcIdx >= pixels.Length) continue;
                result[(r + offset) * to.GridCols + (c + offset)] = pixels[srcIdx];
            }

        return result;
    }
}
