using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Services;

public class EnemySpawner
{
    private readonly IEnemyDefinitionProvider _enemyProvider;
    private readonly IEnemyAffixProvider _affixProvider;

    // Biome encounter rates (0..1)
    private static readonly Dictionary<string, float> BiomeEncounterRates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Ocean"]    = 0f,   ["Lake"]     = 0f,
        ["Glacier"]  = 0.005f, ["Shallows"] = 0.005f, ["Beach"] = 0.005f,
        ["Plains"]   = 0.01f, ["Grassland"] = 0.01f, ["Snow"] = 0.01f,
        ["Tundra"]   = 0.01f, ["River"]    = 0.01f,
        ["Desert"]   = 0.02f, ["Mountain"] = 0.02f,
        ["Savanna"]  = 0.025f, ["Volcano"] = 0.025f,
        ["Forest"]   = 0.04f, ["Marsh"]   = 0.04f,
        ["Swamp"]    = 0.045f,
        ["Jungle"]   = 0.05f,
    };

    // Biome affix chance (0..1) — peaceful biomes have no affixes
    private static readonly Dictionary<string, float> BiomeAffixChance = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Glacier"]   = 0f, ["Shallows"] = 0f, ["Beach"]    = 0f,
        ["Plains"]    = 0f, ["Grassland"] = 0f,
        ["River"]     = 0.05f,
        ["Tundra"]    = 0.10f, ["Snow"]  = 0.10f,
        ["Desert"]    = 0.15f, ["Savanna"] = 0.15f,
        ["Forest"]    = 0.20f, ["Mountain"] = 0.20f,
        ["Marsh"]     = 0.30f, ["Swamp"]   = 0.35f,
        ["Jungle"]    = 0.40f, ["Volcano"] = 0.45f,
    };

    // Affix pools per biome — thematically appropriate affixes
    private static readonly Dictionary<string, string[]> BiomeAffixPools = new(StringComparer.OrdinalIgnoreCase)
    {
        ["River"]    = ["venomous", "rabid"],
        ["Tundra"]   = ["rabid", "armored", "frenzied"],
        ["Snow"]     = ["rabid", "armored", "enraged"],
        ["Desert"]   = ["burning", "cursed", "spectral"],
        ["Savanna"]  = ["rabid", "enraged", "venomous"],
        ["Forest"]   = ["venomous", "rabid", "cursed", "frenzied"],
        ["Mountain"] = ["armored", "enraged", "frenzied"],
        ["Marsh"]    = ["venomous", "rabid", "cursed", "spectral"],
        ["Swamp"]    = ["venomous", "rabid", "cursed", "spectral", "frenzied"],
        ["Jungle"]   = ["venomous", "burning", "rabid", "cursed", "frenzied", "enraged"],
        ["Volcano"]  = ["burning", "enraged", "spectral", "armored", "frenzied"],
    };

    // All affixes available in dungeons
    private static readonly string[] DungeonAffixPool =
        ["rabid", "burning", "venomous", "armored", "spectral", "enraged", "cursed", "frenzied"];

    private const float DungeonAffixChance = 0.30f;

    // Possible enemies by biome
    private static readonly Dictionary<string, string[]> BiomeEnemies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Grassland"] = ["HexRabbit", "HexRabbit", "HexRabbit", "SentientEgg", "MushroomSprite", "StiltedMan", "CollapsedClown"],
        ["Plains"]    = ["HexRabbit", "HexRabbit", "SentientEgg", "DustWraith", "StiltedMan", "CollapsedClown"],
        ["Beach"]     = ["HexRabbit", "Slime", "Slime"],
        ["River"]     = ["BeaverSerpent", "Slime", "HexRabbit", "TwiceBornHeron", "MillGhost", "WaterloggedWorker"],
        ["Glacier"]   = ["HexRabbit", "Slime"],
        ["Shallows"]  = ["Slime", "HexRabbit"],
        ["Tundra"]    = ["HexRabbit", "BeaverSerpent", "Wolf", "TwiceBornHeron"],
        ["Snow"]      = ["Wolf", "BeaverSerpent", "Bear", "TwiceBornHeron"],
        ["Desert"]    = ["DustWraith", "DustWraith", "SentientEgg", "TombRobber"],
        ["Savanna"]   = ["DustWraith", "Wolf", "SentientEgg"],
        ["Swamp"]     = ["Slime", "BeaverSerpent", "SentientEgg", "MushroomSprite", "FermentedThing", "TwiceBornHeron", "SoggyButler", "DecomposedHound", "BogNoble"],
        ["Marsh"]     = ["Slime", "BeaverSerpent", "MushroomSprite", "SentientEgg", "TwiceBornHeron", "Witch", "WitchesBroom"],
        ["Forest"]    = ["MushroomSprite", "HexRabbit", "BeaverSerpent", "Wolf", "SentientEgg", "PaleLibrarian", "Witch", "WitchesBroom"],
        ["Mountain"]  = ["Bear", "Wolf", "BeaverSerpent", "StoneShepherd"],
        ["Jungle"]    = ["Wolf", "Bear", "MushroomSprite", "SentientEgg", "FermentedThing"],
        ["Volcano"]   = ["DustWraith", "Bear", "Wolf", "StoneShepherd"],
    };

    private static readonly Dictionary<string, string[]> DungeonThemeEnemies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CrystalCavern"]  = ["Slime", "StoneShepherd"],
        ["AncientTomb"]    = ["SentientEgg", "PaleLibrarian", "TombRobber", "SarcophagusGuard"],
        ["RootLabyrinth"]  = ["Slime", "Wolf", "MushroomSprite", "FermentedThing"],
        ["FrozenVault"]    = ["Wolf", "Bear", "TwiceBornHeron"],
        ["CovensHollow"]   = ["Witch", "WitchesBroom", "WitchesCoven", "WitchesOven"],
        ["BuriedCarnival"] = ["StiltedMan", "CollapsedClown", "FunhouseMirror"],
        ["DrownedEstate"]  = ["SoggyButler", "DecomposedHound", "BogNoble"],
        ["SunkenMill"]     = ["MillGhost", "WaterloggedWorker"],
    };

    // Per-theme boss pools — if a theme has an entry here, only those bosses spawn in that dungeon
    private static readonly Dictionary<string, string[]> ThemeBossPools = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CrystalCavern"]  = ["CaveTroll"],
        ["FrozenVault"]    = ["TheArrangement"],
        ["RootLabyrinth"]  = ["CoronatedRat"],
        ["AncientTomb"]    = ["TheUnwrapped"],
        ["CovensHollow"]   = ["WitchesMotherInLaw"],
        ["BuriedCarnival"] = ["TheRingmaster"],
        ["DrownedEstate"]  = ["TheDrowningLord"],
        ["SunkenMill"]     = ["TheFerryman"],
    };

    private static readonly string[] BossPool = ["CaveTroll", "TheArrangement", "CoronatedRat"];

    public EnemySpawner(IEnemyDefinitionProvider enemyProvider, IEnemyAffixProvider affixProvider)
    {
        _enemyProvider  = enemyProvider;
        _affixProvider  = affixProvider;
    }

    public float GetEncounterRate(string biomeType, bool inDungeon) =>
        inDungeon ? 0.15f : BiomeEncounterRates.GetValueOrDefault(biomeType, 0f);

    public static string[] GetLocations(string enemyId)
    {
        foreach (var (theme, pool) in ThemeBossPools)
            if (pool.Contains(enemyId))
                return [$"{theme} (Boss)"];

        if (BossPool.Contains(enemyId))
            return ["Dungeon Boss"];

        var locations = new List<string>();

        foreach (var (biome, pool) in BiomeEnemies)
            if (pool.Contains(enemyId))
                locations.Add(biome);

        foreach (var (theme, pool) in DungeonThemeEnemies)
            if (pool.Contains(enemyId))
                locations.Add($"{theme} (Dungeon)");

        return [.. locations];
    }

    public List<Enemy> SpawnEnemies(CombatStartContext context, Random rng)
    {
        var enemies = new List<Enemy>();

        if (context.RoomType == "Boss")
        {
            string[] bossPool = context.DungeonTheme != null
                && ThemeBossPools.TryGetValue(context.DungeonTheme, out var themedBossPool)
                ? themedBossPool : BossPool;
            string bossId = bossPool[rng.Next(bossPool.Length)];
            var def = _enemyProvider.GetById(bossId);
            if (def != null) enemies.Add(SpawnFromDef(def, rng, affixId: null));
            return enemies;
        }

        string[] pool = ResolvePool(context);
        if (pool.Length == 0) return enemies;

        int count = context.DungeonFloor.HasValue
            ? (rng.Next(2) == 0 ? 1 : Math.Min(2, context.DungeonFloor.Value))
            : 1;

        bool isDungeon = context.DungeonTheme != null;
        float affixChance = isDungeon
            ? DungeonAffixChance
            : BiomeAffixChance.GetValueOrDefault(context.BiomeType ?? string.Empty, 0f);
        string[] affixPool = isDungeon
            ? DungeonAffixPool
            : (BiomeAffixPools.GetValueOrDefault(context.BiomeType ?? string.Empty) ?? []);

        for (int i = 0; i < count; i++)
        {
            string id = pool[rng.Next(pool.Length)];
            var def = _enemyProvider.GetById(id);
            if (def == null) continue;

            string? affixId = null;
            if (affixPool.Length > 0 && rng.NextDouble() < affixChance)
                affixId = affixPool[rng.Next(affixPool.Length)];

            enemies.Add(SpawnFromDef(def, rng, affixId));
        }

        return enemies;
    }

    private static string[] ResolvePool(CombatStartContext context)
    {
        if (context.DungeonTheme != null
            && DungeonThemeEnemies.TryGetValue(context.DungeonTheme, out var dungeonPool))
            return dungeonPool;

        if (context.BiomeType != null
            && BiomeEnemies.TryGetValue(context.BiomeType, out var biomePool))
            return biomePool;

        return [];
    }

    private Enemy SpawnFromDef(EnemyDefinition def, Random rng, string? affixId)
    {
        int hp = (int)(def.BaseHp * (0.90 + rng.NextDouble() * 0.20));
        int atk = def.BaseAttack;
        int def2 = def.BaseDefense;

        var actionTable = new List<EnemyActionEntry>(def.ActionTable);
        string? affixLabel = null;
        OnHitEffect? onHit = null;
        bool cantBeDodged = false;

        if (affixId != null)
        {
            var affix = _affixProvider.GetById(affixId);
            if (affix != null)
            {
                affixLabel  = affix.Label;
                onHit       = affix.OnHit;
                cantBeDodged = affix.ActionsCantBeDodged;

                hp  = (int)(hp  * affix.HpMultiplier);
                atk = (int)(atk * affix.AttackMultiplier) + affix.AttackBonus;
                def2 += affix.DefenseBonus;

                actionTable.AddRange(affix.BonusActions);
            }
            else
            {
                affixId = null;
            }
        }

        string name = affixLabel != null ? $"{affixLabel} {def.Name}" : def.Name;

        return new Enemy
        {
            InstanceId          = Guid.NewGuid().ToString("N"),
            DefinitionId        = def.Id,
            Name                = name,
            CurrentHp           = hp,
            MaxHp               = hp,
            Attack              = atk,
            Defense             = def2,
            ActionTable         = actionTable,
            AffixId             = affixId,
            AffixLabel          = affixLabel,
            OnHitEffect         = onHit,
            ActionsCantBeDodged = cantBeDodged,
        };
    }
}
