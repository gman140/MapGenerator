namespace MapGenerator.Combat.Services;

public enum HitOutcome { Miss, NearMiss, Hit }

public static class CombatMath
{
    // Three-tier hit resolution used by both PvE and PvP.
    //   attackerSpd / targetSpd  — effective speeds (after status modifiers)
    //   power                    — move power; heavy (≥1.15) adds miss, fast (≤0.8) reduces it
    //   accuracyMod              — positive = more accurate (equipment, Steady); negative = less (Cursed)
    public static HitOutcome RollHit(
        int attackerSpd, int targetSpd, float power, float accuracyMod, Random rng)
    {
        float spdAdj   = Math.Clamp((attackerSpd - targetSpd) * 0.015f, -0.12f, 0.12f);
        float powerAdj = power >= 1.15f ? 0.05f : power <= 0.8f ? -0.04f : 0f;
        float miss     = Math.Clamp(0.10f - spdAdj - accuracyMod + powerAdj, 0.02f, 0.35f);
        float near     = Math.Clamp(0.20f - spdAdj * 0.5f,                    0.04f, 0.40f);

        double roll = rng.NextDouble();
        if (roll < miss)        return HitOutcome.Miss;
        if (roll < miss + near) return HitOutcome.NearMiss;
        return HitOutcome.Hit;
    }

    public static bool RollCrit(float critChance, Random rng) =>
        critChance > 0f && rng.NextDouble() < critChance;

    // Compute the flat contribution of a single status multiplier applied to a base stat.
    // pct > 0 = buff; pct < 0 = debuff. min is the minimum absolute-value magnitude.
    public static float StatusStatEffect(float baseStat, float pct, int min)
    {
        if (pct == 0f) return 0f;
        float raw = baseStat * pct;
        return pct > 0f
            ? Math.Max(raw, min)
            : Math.Min(raw, -min);
    }
}
