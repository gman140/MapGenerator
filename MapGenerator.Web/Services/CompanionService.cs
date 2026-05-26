using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Web.Services;

public class CompanionService(ICompanionRepository repo)
{
    private readonly ICompanionRepository _repo = repo;

    // Use an Elemental Core item to add a new element type. Returns error or null.
    public async Task<string?> UseElementalCoreAsync(
        PlayerCompanion companion, DamageType newElement)
    {
        if (companion.ElementTypes.Count >= 2)
            return $"{companion.Nickname} already has two element types.";
        if (companion.ElementTypes.Contains(newElement))
            return $"{companion.Nickname} is already a {newElement} type.";

        companion.ElementTypes.Add(newElement);
        companion.BonusStatPoints += 3;
        await _repo.SaveAsync(companion);
        return null;
    }

    // Use an Evolution Stone to advance the companion's tier. Returns error or null.
    public async Task<string?> UseEvolutionStoneAsync(PlayerCompanion companion)
    {
        if (companion.Tier == CompanionTier.T3)
            return $"{companion.Nickname} is already at the highest tier.";

        var nextTier = companion.Tier + 1;
        companion.SpritePixels = TierRegistry.ExpandSprite(
            companion.SpritePixels, companion.Tier, nextTier);
        companion.Tier = nextTier;
        companion.BonusStatPoints += 5;
        await _repo.SaveAsync(companion);
        return null;
    }

    // Allocate one stat point. Returns error or null.
    public async Task<string?> AllocateStatAsync(PlayerCompanion companion, string stat)
    {
        if (companion.AvailableStatPoints <= 0)
            return "No stat points available.";

        string[] valid = ["ATK", "VIT", "DEF", "SPD", "FOC", "RES"];
        if (!valid.Contains(stat))
            return $"Unknown stat: {stat}";

        companion.AllocatedStats.TryGetValue(stat, out int current);
        companion.AllocatedStats[stat] = current + 1;
        await _repo.SaveAsync(companion);
        return null;
    }

    // Gather bonus — may give a small item and return flavor text or null
    public (string? bonusText, string? itemId) RollGatherBonus(
        PlayerCompanion companion, Random rng)
    {
        if (rng.NextDouble() >= 0.25) return (null, null);

        var tDef   = TemperamentRegistry.Get(companion.Temperament);
        string msg = tDef.GatherFlavor.Replace("{name}", companion.Nickname);
        return (msg, PickItem(rng, tDef.GatherItemPool));
    }

    private static string PickItem(Random rng, string[] pool) =>
        pool[rng.Next(pool.Length)];
}
