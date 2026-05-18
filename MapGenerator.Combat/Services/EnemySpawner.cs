using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Services;

public class EnemySpawner
{
    private readonly IEnemyDefinitionProvider _enemyProvider;

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

    // Possible enemies by biome — pools weighted toward difficulty appropriate for the terrain.
    // Peaceful biomes only spawn easy enemies; dangerous biomes allow harder ones.
    private static readonly Dictionary<string, string[]> BiomeEnemies = new(StringComparer.OrdinalIgnoreCase)
    {
        // Peaceful — easy enemies only
        ["Grassland"] = ["HexRabbit", "HexRabbit", "HexRabbit", "SentientEgg", "MushroomSprite"],
        ["Plains"]    = ["HexRabbit", "HexRabbit", "SentientEgg", "DustWraith"],
        ["Beach"]     = ["HexRabbit", "Slime", "Slime"],
        ["River"]     = ["BeaverSerpent", "Slime", "HexRabbit"],
        ["Glacier"]   = ["HexRabbit", "Slime"],
        ["Shallows"]  = ["Slime", "HexRabbit"],

        // Mixed — easy/medium
        ["Tundra"]    = ["HexRabbit", "BeaverSerpent", "Wolf"],
        ["Snow"]      = ["Wolf", "BeaverSerpent", "Bear"],
        ["Desert"]    = ["DustWraith", "DustWraith", "SentientEgg"],
        ["Savanna"]   = ["DustWraith", "Wolf", "SentientEgg"],

        // Dangerous — medium/hard
        ["Swamp"]     = ["Slime", "BeaverSerpent", "SentientEgg", "MushroomSprite"],
        ["Marsh"]     = ["Slime", "BeaverSerpent", "MushroomSprite", "SentientEgg"],
        ["Forest"]    = ["MushroomSprite", "HexRabbit", "BeaverSerpent", "Wolf", "SentientEgg"],
        ["Mountain"]  = ["Bear", "Wolf", "BeaverSerpent"],

        // Very dangerous — medium/hard, no easy enemies
        ["Jungle"]    = ["Wolf", "Bear", "MushroomSprite", "SentientEgg"],
        ["Volcano"]   = ["DustWraith", "Bear", "Wolf"],
    };

    // Possible enemies by dungeon theme
    private static readonly Dictionary<string, string[]> DungeonThemeEnemies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CrystalCavern"]  = ["Slime", "CaveTroll"],
        ["AncientTomb"]    = ["SentientEgg", "CaveTroll"],
        ["RootLabyrinth"]  = ["Slime", "Wolf"],
        ["FrozenVault"]    = ["Wolf", "Bear"],
    };

    public EnemySpawner(IEnemyDefinitionProvider enemyProvider)
    {
        _enemyProvider = enemyProvider;
    }

    public float GetEncounterRate(string biomeType, bool inDungeon) =>
        inDungeon ? 0.15f : BiomeEncounterRates.GetValueOrDefault(biomeType, 0f);

    public List<Enemy> SpawnEnemies(CombatStartContext context, Random rng)
    {
        var enemies = new List<Enemy>();

        // Boss room always spawns Cave Troll
        if (context.RoomType == "Boss")
        {
            var def = _enemyProvider.GetById("CaveTroll");
            if (def != null) enemies.Add(SpawnFromDef(def, rng));
            return enemies;
        }

        string[] pool = ResolvePool(context);
        if (pool.Length == 0) return enemies;

        int count = context.DungeonFloor.HasValue
            ? (rng.Next(2) == 0 ? 1 : Math.Min(2, context.DungeonFloor.Value))
            : 1;

        for (int i = 0; i < count; i++)
        {
            string id = pool[rng.Next(pool.Length)];
            var def = _enemyProvider.GetById(id);
            if (def != null) enemies.Add(SpawnFromDef(def, rng));
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

    private static Enemy SpawnFromDef(EnemyDefinition def, Random rng)
    {
        // Slight HP variance (+/- 10%) so fights feel different each time
        int hp = (int)(def.BaseHp * (0.90 + rng.NextDouble() * 0.20));

        return new Enemy
        {
            InstanceId   = Guid.NewGuid().ToString("N"),
            DefinitionId = def.Id,
            Name         = def.Name,
            CurrentHp    = hp,
            MaxHp        = hp,
            Attack       = def.BaseAttack,
            Defense      = def.BaseDefense,
            ActionTable  = [.. def.ActionTable],
        };
    }
}
