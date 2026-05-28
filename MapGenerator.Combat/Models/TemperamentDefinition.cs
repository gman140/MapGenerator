using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class TemperamentDefinition
{
    public CompanionTemperament Id { get; init; }

    // Display
    public string Hint { get; init; } = "";

    // Stat bonuses applied to base stats in battle
    public int AtkBonus { get; init; }
    public int DefBonus { get; init; }
    public int VitBonus { get; init; }
    public int SpdBonus { get; init; }
    public int FocBonus { get; init; }
    public int ResBonus { get; init; }

    // Battle AI thresholds
    public float HealThreshold { get; init; } = 0.28f;
    public float HealChance    { get; init; } = 0.65f;
    public float DebuffChance  { get; init; } = 0.40f;
    public float BuffChance    { get; init; } = 0.25f;
    public float AttackBias    { get; init; } = 0.75f;

    // Gather bonus — {name} is replaced with the companion's nickname at use time
    public string   GatherFlavor   { get; init; } = "";
    public string[] GatherItemPool { get; init; } = [];
}

public static class TemperamentRegistry
{
    private static readonly Dictionary<CompanionTemperament, TemperamentDefinition> _all = new()
    {
        [CompanionTemperament.Bold] = new()
        {
            Id = CompanionTemperament.Bold,
            Hint = "+ATK, -DEF",
            AtkBonus = 2, DefBonus = -1,
            HealThreshold = 0.28f, HealChance = 0.65f, DebuffChance = 0.40f, BuffChance = 0.25f, AttackBias = 0.82f,
            GatherFlavor   = "{name} charges ahead and flushes out something that was hiding nearby.",
            GatherItemPool = ["Flint", "Amber", "Quartz", "Stone"],
        },
        [CompanionTemperament.Timid] = new()
        {
            Id = CompanionTemperament.Timid,
            Hint = "+DEF, -ATK",
            DefBonus = 2, AtkBonus = -1,
            HealThreshold = 0.45f, HealChance = 0.75f, DebuffChance = 0.40f, BuffChance = 0.25f, AttackBias = 0.60f,
            GatherFlavor   = "{name} hesitantly pokes around the edges and uncovers something tucked away.",
            GatherItemPool = ["Herbs", "PaleMushroom", "Moss", "Reed"],
        },
        [CompanionTemperament.Reckless] = new()
        {
            Id = CompanionTemperament.Reckless,
            Hint = "+ATK, +SPD, -DEF",
            AtkBonus = 3, DefBonus = -2, SpdBonus = 1,
            HealThreshold = 0.15f, HealChance = 0.65f, DebuffChance = 0.20f, BuffChance = 0.10f, AttackBias = 0.92f,
            GatherFlavor   = "{name} tears through the undergrowth and comes back ruffled but triumphant.",
            GatherItemPool = ["CrackedOrb", "HollowStone", "Amber", "DeepOre"],
        },
        [CompanionTemperament.Careful] = new()
        {
            Id = CompanionTemperament.Careful,
            Hint = "+DEF, +RES, -SPD",
            DefBonus = 2, ResBonus = 1, SpdBonus = -1, AtkBonus = -1,
            HealThreshold = 0.45f, HealChance = 0.80f, DebuffChance = 0.45f, BuffChance = 0.40f, AttackBias = 0.65f,
            GatherFlavor   = "{name} methodically sniffs every corner and finds something overlooked.",
            GatherItemPool = ["Quartz", "Amber", "MossGem", "RiverGlass"],
        },
        [CompanionTemperament.Cunning] = new()
        {
            Id = CompanionTemperament.Cunning,
            Hint = "+FOC, +SPD, -VIT",
            FocBonus = 2, SpdBonus = 1, VitBonus = -1,
            HealThreshold = 0.28f, HealChance = 0.65f, DebuffChance = 0.55f, BuffChance = 0.25f, AttackBias = 0.75f,
            GatherFlavor   = "{name} outwits a skittish creature lurking nearby and claims its small hoard.",
            GatherItemPool = ["TidalCoin", "TarnishedRing", "Amber", "Quartz"],
        },
        [CompanionTemperament.Stoic] = new()
        {
            Id = CompanionTemperament.Stoic,
            Hint = "+RES, +DEF, -FOC",
            ResBonus = 2, DefBonus = 1, FocBonus = -1,
            HealThreshold = 0.28f, HealChance = 0.70f, DebuffChance = 0.40f, BuffChance = 0.35f, AttackBias = 0.75f,
            GatherFlavor   = "{name} sits perfectly still until a curious creature approaches and drops something.",
            GatherItemPool = ["HollowStone", "CrackedOrb", "Stone", "Flint"],
        },
        [CompanionTemperament.Playful] = new()
        {
            Id = CompanionTemperament.Playful,
            Hint = "+SPD, +FOC, -ATK",
            SpdBonus = 2, FocBonus = 1, AtkBonus = -1,
            HealThreshold = 0.28f, HealChance = 0.65f, DebuffChance = 0.40f, BuffChance = 0.25f, AttackBias = 0.75f,
            GatherFlavor   = "{name} bounds through the area and accidentally uncovers something in the process.",
            GatherItemPool = ["CrowFeather", "Reed", "Herbs", "PaleMushroom"],
        },
        [CompanionTemperament.Fierce] = new()
        {
            Id = CompanionTemperament.Fierce,
            Hint = "+ATK×2, -RES, -DEF",
            AtkBonus = 4, DefBonus = -2, ResBonus = -1,
            HealThreshold = 0.12f, HealChance = 0.65f, DebuffChance = 0.15f, BuffChance = 0.10f, AttackBias = 0.90f,
            GatherFlavor   = "{name} asserts dominance over the local fauna. They leave something behind in their haste.",
            GatherItemPool = ["Amber", "HollowStone", "BoneFragment", "Quartz"],
        },
    };

    public static TemperamentDefinition Get(CompanionTemperament t) => _all[t];
}
