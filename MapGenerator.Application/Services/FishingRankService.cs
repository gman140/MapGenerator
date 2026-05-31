namespace MapGenerator.Application.Services;

public static class FishingRankService
{
    private static readonly int[]    Thresholds = [0, 50, 150, 350, 700, 1200, 2000, 3200, 5000, 8000];
    private static readonly string[] Names      = ["Novice", "River Hand", "Tidewatcher", "Shore Expert",
                                                    "Deep Caster", "Salt Fisher", "Seasoned Mariner",
                                                    "Master Angler", "Grand Fisher", "Legendary"];
    public const int MaxRank = 10;

    public static string GetRankNameForRank(int rank) => Names[Math.Clamp(rank, 1, MaxRank) - 1];

    // ── Rank from accumulated XP ──────────────────────────────────────────────

    public static int GetRank(int xp)
    {
        for (int i = Thresholds.Length - 1; i >= 0; i--)
            if (xp >= Thresholds[i]) return i + 1;
        return 1;
    }

    public static string GetRankName(int xp) => Names[GetRank(xp) - 1];

    // XP already earned within the current rank band
    public static int GetXpInRank(int xp)
    {
        int rank = GetRank(xp);
        return xp - Thresholds[rank - 1];
    }

    // XP needed to complete the current rank band (0 at max rank)
    public static int GetXpRangeForRank(int xp)
    {
        int rank = GetRank(xp);
        if (rank >= MaxRank) return 0;
        return Thresholds[rank] - Thresholds[rank - 1];
    }

    // ── XP per catch ──────────────────────────────────────────────────────────

    public static int GetCatchXp(float rarityWeight, bool isChainMaterial)
    {
        if (isChainMaterial) return 25;
        return rarityWeight switch
        {
            >= 2.0f => 2,
            >= 1.0f => 4,
            >= 0.5f => 8,
            _       => 15,
        };
    }

    // ── Rank bonuses (all multiplicative, applied on top of pole/lure/companion/food) ─

    public static double GetStrikeMultiplier(int rank) => rank switch
    {
        >= 10 => 1.20,
        >= 8  => 1.15,
        >= 2  => 1.08,
        _     => 1.00,
    };

    public static double GetWaitMultiplier(int rank) => rank switch
    {
        >= 10 => 0.80,
        >= 8  => 0.85,
        >= 5  => 0.90,
        _     => 1.00,
    };

    public static double GetRarityMultiplier(int rank) => rank switch
    {
        >= 10 => 1.20,
        >= 7  => 1.15,
        >= 4  => 1.08,
        _     => 1.00,
    };

    public static double GetTensionMultiplier(int rank) => rank switch
    {
        >= 10 => 0.88,
        >= 9  => 0.90,
        >= 6  => 0.92,
        _     => 1.00,
    };

    public static double GetChainBoost(int rank) => rank switch
    {
        >= 10 => 1.50,
        >= 9  => 1.40,
        >= 7  => 1.20,
        _     => 1.00,
    };

    // Multiplier applied to byproduct drop chances
    public static double GetDropRateMultiplier(int rank) => rank >= 3 ? 1.50 : 1.00;

    // ── Human-readable bonus description for a specific rank threshold ────────

    public static string? GetUnlockDescription(int rank) => rank switch
    {
        2  => "+8% strike window",
        3  => "Byproduct drops ×1.5",
        4  => "+8% rare fish chance",
        5  => "−10% bite wait time",
        6  => "−8% tension drain",
        7  => "+15% rare fish; chain materials +20%",
        8  => "+15% strike window total; −15% wait total",
        9  => "Chain materials +40% total; −10% tension total",
        10 => "+20% strike, −20% wait, −12% tension, +20% rare, +50% chain",
        _  => null,
    };
}
