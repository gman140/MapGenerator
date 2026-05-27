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
        await _repo.SaveAsync(companion);
        return null;
    }

    // Use an Evolution Stone to evolve the companion and roll a random form. Returns error or null.
    public async Task<string?> UseEvolutionStoneAsync(PlayerCompanion companion)
    {
        if (companion.Form != CompanionForm.None)
            return $"{companion.Nickname} is already evolved.";

        var baseDef = FormRegistry.Get(CompanionForm.None);
        companion.Form         = FormRegistry.Roll(Random.Shared);
        companion.SpritePixels = FormRegistry.ExpandSprite(
            companion.SpritePixels, baseDef.GridCols, FormRegistry.Get(companion.Form).GridCols);
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
