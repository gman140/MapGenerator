using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Models;
using MapGenerator.Combat.Services;
using MapGenerator.Application.Services;
using MapGenerator.Domain.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.CombatSim;

internal static class Program
{
    private const int    FightsPerEnemy  = 20;   // more fights since armor/hat are random per fight
    private const int    MaxTurns        = 150;
    private const int    Seed            = 42;
    private const string PrimarySpell    = "firebolt";
    private const int    PrimarySpellCost = 3;

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int seed = args.Length > 0 && int.TryParse(args[0], out int s) ? s : Seed;

        var enemyProvider      = new InMemoryEnemyDefinitionProvider();
        var affixProvider      = new InMemoryEnemyAffixProvider();
        var equipmentProvider  = new InMemoryEquipmentDefinitionProvider();
        var consumableProvider = new InMemoryConsumableDefinitionProvider();
        var spellProvider      = new InMemorySpellDefinitionProvider();
        var companionProvider     = new InMemoryCompanionDefinitionProvider();
        var companionMoveProvider = new InMemoryCompanionMoveProvider();
        var spawner               = new EnemySpawner(enemyProvider, affixProvider);
        var engine                = new CombatEngine(
            enemyProvider, equipmentProvider, consumableProvider,
            spellProvider, companionProvider, companionMoveProvider, spawner);

        var allEnemies = enemyProvider.All;
        var allEquip   = equipmentProvider.All;
        var weapons    = allEquip.Where(e => e.EquipmentSlot == "Weapon").ToList();
        var armors     = allEquip.Where(e => e.EquipmentSlot == "Armor").ToList();
        var hats       = allEquip.Where(e => e.EquipmentSlot == "Hat").ToList();
        var players    = GeneratePlayers(new Random(seed));

        int totalBattles = weapons.Count * Enum.GetValues<Strategy>().Length
                           * players.Count * allEnemies.Count * FightsPerEnemy;

        Console.WriteLine($"Running {weapons.Count} weapons × {Enum.GetValues<Strategy>().Length} strategies × " +
                          $"{players.Count} players × {allEnemies.Count} enemies × {FightsPerEnemy} fights = {totalBattles:N0} battles");
        Console.WriteLine();

        // sims[weaponId][strategy] = FightStats[playerIndex][enemyIndex]
        var sims = new Dictionary<string, Dictionary<Strategy, FightStats[][]>>();

        foreach (var weapon in weapons)
        {
            sims[weapon.Id] = [];
            foreach (var strat in Enum.GetValues<Strategy>())
            {
                Console.Write($"  {WeaponAbbr(weapon),-5}  {weapon.Name,-20}  {StratLabel(strat),-30}");
                sims[weapon.Id][strat] = RunSim(
                    engine, players, allEnemies, armors, hats,
                    new Random(seed), strat, weapon);
                Console.WriteLine();
            }
        }
        Console.WriteLine();

        PrintReport(players, allEnemies, weapons, armors, hats, sims, seed);
    }

    // ── Simulation ────────────────────────────────────────────────────────────

    private static FightStats[][] RunSim(
        CombatEngine engine, List<Player> players,
        IReadOnlyList<EnemyDefinition> enemies,
        List<EquipmentDefinition> armors,
        List<EquipmentDefinition> hats,
        Random rng, Strategy strategy, EquipmentDefinition weapon)
    {
        int W = players.Count, E = enemies.Count;
        var results = new FightStats[W][];
        for (int i = 0; i < W; i++) results[i] = new FightStats[E];

        for (int pi = 0; pi < W; pi++)
        {
            for (int ei = 0; ei < E; ei++)
            {
                int wins = 0, totalTurns = 0;
                double totalHpFrac = 0;
                for (int f = 0; f < FightsPerEnemy; f++)
                {
                    var armor = armors[rng.Next(armors.Count)];
                    var hat   = hats[rng.Next(hats.Count)];
                    var (won, turns, hpFrac) = RunFight(engine, players[pi], enemies[ei], rng, strategy, weapon, armor, hat);
                    if (won) { wins++; totalHpFrac += hpFrac; }
                    totalTurns += turns;
                }
                results[pi][ei] = new FightStats(wins, FightsPerEnemy,
                    totalTurns / (double)FightsPerEnemy,
                    wins > 0 ? totalHpFrac / wins : 0);
            }
            Console.Write(".");
        }
        return results;
    }

    private static (bool won, int turns, double hpFrac) RunFight(
        CombatEngine engine, Player player, EnemyDefinition def,
        Random rng, Strategy strategy,
        EquipmentDefinition weapon, EquipmentDefinition armor, EquipmentDefinition hat)
    {
        var session = BuildSession(player, def, rng, weapon, armor, hat);
        int turns   = 0;
        while (!engine.IsFinished(session) && turns < MaxTurns)
        {
            engine.ProcessTurn(session, player, PickAction(session, strategy));
            turns++;
        }
        bool   won    = session.Phase == CombatPhase.Victory;
        double hpFrac = (double)session.PlayerHp / session.PlayerMaxHp;
        return (won, turns, hpFrac);
    }

    // ── AI strategies ─────────────────────────────────────────────────────────

    private static CombatAction PickAction(CombatSession session, Strategy strategy)
    {
        var target = session.Enemies.FirstOrDefault(e => e.CurrentHp > 0 && !e.HasFled);
        string? tid = target?.InstanceId;
        return strategy switch
        {
            Strategy.AlwaysAttack => new CombatAction { Type = CombatActionType.Attack,     TargetEnemyId = tid },
            Strategy.AttackHeavy  => PickAttackHeavy(session, tid),
            Strategy.MagicHeavy   => PickMagicHeavy(session, tid),
            _                     => new CombatAction { Type = CombatActionType.Attack,     TargetEnemyId = tid },
        };
    }

    private static CombatAction PickAttackHeavy(CombatSession session, string? tid) =>
        session.PlayerStamina >= 4
            ? new CombatAction { Type = CombatActionType.HeavyAttack, TargetEnemyId = tid }
            : new CombatAction { Type = CombatActionType.Defend };

    private static CombatAction PickMagicHeavy(CombatSession session, string? tid) =>
        session.PlayerMana >= PrimarySpellCost
            ? new CombatAction { Type = CombatActionType.Spell, SpellId = PrimarySpell, TargetEnemyId = tid }
            : new CombatAction { Type = CombatActionType.Refocus };

    // ── Session / player builders ─────────────────────────────────────────────

    private static List<Player> GeneratePlayers(Random rng)
    {
        string[] statKeys = ["MaxHp", "BaseAttack", "BaseDefense", "BaseMagic", "BaseDodgeChance"];
        var players = new List<Player>();
        for (int level = 1; level <= 20; level++)
        {
            var p = new Player
            {
                Id = $"sim-L{level}", Level = level,
                MaxHp = 30, BaseAttack = 10, BaseDefense = 5,
                BaseDodgeChance = 0.10f, MaxStamina = 10, MaxMana = 10, BaseMagic = 8,
            };
            for (int pt = 0; pt < (level - 1) * 3; pt++)
                switch (statKeys[rng.Next(statKeys.Length)])
                {
                    case "MaxHp":           p.MaxHp           += 2;     break;
                    case "BaseAttack":      p.BaseAttack++;              break;
                    case "BaseDefense":     p.BaseDefense++;             break;
                    case "BaseMagic":       p.BaseMagic++;               break;
                    case "BaseDodgeChance": p.BaseDodgeChance += 0.02f; break;
                }
            players.Add(p);
        }
        return players;
    }

    private static CombatSession BuildSession(
        Player player, EnemyDefinition def, Random rng,
        EquipmentDefinition weapon, EquipmentDefinition armor, EquipmentDefinition hat)
    {
        int hp    = (int)(def.BaseHp * (0.90 + rng.NextDouble() * 0.20));
        var enemy = new Enemy
        {
            InstanceId   = Guid.NewGuid().ToString("N"),
            DefinitionId = def.Id, Name = def.Name,
            CurrentHp = hp, MaxHp = hp,
            Attack    = def.BaseAttack, Defense = def.BaseDefense,
            ActionTable = [.. def.ActionTable],
        };

        var session = new CombatSession
        {
            Id = Guid.NewGuid().ToString("N"), PlayerId = player.Id,
            PlayerHp = player.MaxHp, PlayerMaxHp = player.MaxHp,
            PlayerStamina = 4, PlayerMaxStamina = player.MaxStamina,
            PlayerBaseAttack = player.BaseAttack, PlayerBaseDefense = player.BaseDefense,
            PlayerBaseDodgeChance = player.BaseDodgeChance,
            PlayerMana = 2, PlayerMaxMana = player.MaxMana, PlayerBaseMagic = player.BaseMagic,
            Enemies = [enemy], Phase = CombatPhase.PlayerTurn, ContextLabel = "Sim",
        };

        session.PlayerWeaponDamageType = weapon.WeaponDamageType;

        // Apply all affixes from weapon, armor, and hat as session modifiers
        foreach (var piece in new[] { weapon, armor, hat })
        foreach (var affix in piece.Affixes)
        {
            session.ActiveModifiers.Add(new CombatModifier
            {
                Id     = $"Equip:{piece.Id}:{affix.Stat}",
                Stat   = affix.Stat,
                Value  = affix.Value,
                Source = "Equipment",
            });
        }

        return session;
    }

    // ── Report ────────────────────────────────────────────────────────────────

    private static void PrintReport(
        List<Player> players,
        IReadOnlyList<EnemyDefinition> enemies,
        List<EquipmentDefinition> weapons,
        List<EquipmentDefinition> armors,
        List<EquipmentDefinition> hats,
        Dictionary<string, Dictionary<Strategy, FightStats[][]>> sims,
        int seed)
    {
        int W = players.Count, E = enemies.Count, Wep = weapons.Count;
        string[] wAbbr = weapons.Select(WeaponAbbr).ToArray();

        Sep();
        Console.WriteLine("  COMBAT BALANCE SIMULATION");
        Console.WriteLine($"  {W} players (L1–L{W})  ×  {E} enemies  ×  {Wep} weapons  ×  {FightsPerEnemy} fights  ×  3 strategies");
        Console.WriteLine($"  Seed: {seed}");
        Console.WriteLine($"  Strategies:  Basic = always Attack  |  Heavy = HeavyAttack/Defend  |  Magic = {PrimarySpell}/Refocus");
        Console.WriteLine($"  Armor: random per fight ({string.Join(", ", armors.Select(a => a.Name))})");
        Console.WriteLine($"  Hat:   random per fight ({string.Join(", ", hats.Select(h => h.Name))})");
        Sep();

        // ── Weapon table ──────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("WEAPONS");
        Console.WriteLine($"  {"Abbr",-5}  {"Name",-20}  {"DmgType",-12}  {"Affixes"}");
        Console.WriteLine($"  {"----",-5}  {"----",-20}  {"-------",-12}  {"-------"}");
        foreach (var w in weapons)
        {
            string affixStr = string.Join(", ", w.Affixes.Select(a => $"{a.Stat}+{a.Value}"));
            Console.WriteLine($"  {WeaponAbbr(w),-5}  {w.Name,-20}  {w.WeaponDamageType,-12}  {affixStr}");
        }

        // ── Player stats ──────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("PLAYER STATS");
        Console.WriteLine("  Lvl   HP   Atk  Def  Mag  Dodge  Pts");
        Console.WriteLine("  ---  ---  ---  ---  ---  -----  ---");
        foreach (var p in players)
            Console.WriteLine($"  L{p.Level,2}  {p.MaxHp,3}  {p.BaseAttack,3}  {p.BaseDefense,3}  {p.BaseMagic,3}  {p.BaseDodgeChance*100,4:F0}%  {(p.Level-1)*3,3}");

        // ── Win rate matrix: Enemy × Weapon (averaged across all levels + strategies) ──
        Console.WriteLine();
        Console.WriteLine("OVERALL WIN RATES  (avg all levels & strategies)");
        PrintEnemyWeaponGrid(enemies, weapons, wAbbr,
            (wi, ei) => AverageAllStrategies(sims, weapons[wi].Id, W, ei));

        // ── Per-strategy breakdown ────────────────────────────────────────────
        foreach (var strat in Enum.GetValues<Strategy>())
        {
            Console.WriteLine();
            Console.WriteLine($"STRATEGY: {StratLabel(strat).ToUpper()}");
            PrintEnemyWeaponGrid(enemies, weapons, wAbbr,
                (wi, ei) => sims[weapons[wi].Id][strat].Sum(row => row[ei].Wins)
                            / (double)(W * FightsPerEnemy));
        }

        // ── Best weapon per enemy ─────────────────────────────────────────────
        Console.WriteLine();
        Sep();
        Console.WriteLine("  BEST WEAPON PER ENEMY  (highest overall win rate)");
        Console.WriteLine($"  {"Enemy",-26}  {"Best Weapon",-22}  {"Win%",5}  {"Worst Weapon",-22}  {"Win%",5}");
        Console.WriteLine($"  {new string('-',26)}  {new string('-',22)}  {"----",5}  {new string('-',22)}  {"----",5}");

        for (int ei = 0; ei < E; ei++)
        {
            double BestOfStrats(int wi)
            {
                double max = 0;
                foreach (var st in Enum.GetValues<Strategy>())
                {
                    double r = sims[weapons[wi].Id][st].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy);
                    if (r > max) max = r;
                }
                return max;
            }

            int bestWi  = Enumerable.Range(0, Wep).MaxBy(BestOfStrats);
            int worstWi = Enumerable.Range(0, Wep).MinBy(BestOfStrats);

            Console.WriteLine(
                $"  {enemies[ei].Name,-26}  {weapons[bestWi].Name,-22}  {BestOfStrats(bestWi)*100,4:F0}%  " +
                $"{weapons[worstWi].Name,-22}  {BestOfStrats(worstWi)*100,4:F0}%");
        }

        // ── Balance flags ─────────────────────────────────────────────────────
        Console.WriteLine();
        Sep();
        Console.WriteLine("  BALANCE FLAGS");
        Console.WriteLine();
        var flags = new List<string>();

        for (int ei = 0; ei < E; ei++)
        {
            // Best win rate across all weapons × strategies
            double bestAny = 0;
            double bestL20 = 0;
            foreach (var w in weapons)
            foreach (var st in Enum.GetValues<Strategy>())
            {
                double overall = sims[w.Id][st].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy);
                double l20     = sims[w.Id][st][W - 1][ei].WinRate;
                if (overall > bestAny) bestAny = overall;
                if (l20 > bestL20) bestL20 = l20;
            }

            if (bestAny < 0.02)
                flags.Add($"  [UNBEATABLE]       {enemies[ei].Name} — < 2% win rate across all weapons & strategies.");
            else if (bestL20 < 0.40)
                flags.Add($"  [HARD AT L20]      {enemies[ei].Name} — best L20 win rate is only {bestL20*100:F0}%.");

            // Trivial: any physical weapon beats it 100% at L1 with basic attack
            bool trivial = weapons.Any(w =>
                sims[w.Id][Strategy.AlwaysAttack][0][ei].WinRate >= 1.0);
            if (trivial)
                flags.Add($"  [TRIVIAL]          {enemies[ei].Name} — 100% at L1 with basic attack and some weapon.");

            // Magic-dominant: best magic weapon >> best physical weapon (overall)
            double bestMagic = weapons
                .Where(w => w.Affixes.Any(a => a.Stat == ModifierStat.Magic))
                .Select(w => sims[w.Id][Strategy.MagicHeavy].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy))
                .DefaultIfEmpty(0).Max();
            double bestPhysical = weapons
                .Where(w => w.Affixes.All(a => a.Stat != ModifierStat.Magic))
                .Select(w => sims[w.Id][Strategy.AlwaysAttack].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy))
                .DefaultIfEmpty(0).Max();
            if (bestMagic - bestPhysical >= 0.30)
                flags.Add($"  [MAGIC PREFERRED]  {enemies[ei].Name} — best magic {bestMagic*100:F0}% vs best physical {bestPhysical*100:F0}%.");

            // Physical-resistant: no physical weapon breaks 50% overall
            bool physResist = weapons
                .Where(w => w.Affixes.All(a => a.Stat != ModifierStat.Magic))
                .All(w => sims[w.Id][Strategy.AlwaysAttack].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy) < 0.50);
            if (physResist && bestAny >= 0.10)
                flags.Add($"  [PHYS RESISTANT]   {enemies[ei].Name} — no physical weapon exceeds 50% win rate.");

            // Damage-type sensitive: best damage type >> worst damage type by 40%+
            var weaponRates = weapons.Select(w =>
                AverageAllStrategies(sims, w.Id, W, ei)).ToArray();
            double spread = weaponRates.Max() - weaponRates.Min();
            if (spread >= 0.40 && bestAny >= 0.30)
                flags.Add($"  [TYPE SENSITIVE]   {enemies[ei].Name} — {spread*100:F0}% spread between best and worst weapon type.");
        }

        if (flags.Count == 0)
            Console.WriteLine("  No obvious balance issues detected.");
        else
            foreach (var f in flags) Console.WriteLine(f);

        Console.WriteLine();
        Sep();
    }

    // ── Grid printer ──────────────────────────────────────────────────────────

    private static void PrintEnemyWeaponGrid(
        IReadOnlyList<EnemyDefinition> enemies,
        List<EquipmentDefinition> weapons,
        string[] wAbbr,
        Func<int, int, double> rateGetter)
    {
        Console.Write($"  {"Enemy",-26}");
        for (int wi = 0; wi < weapons.Count; wi++) Console.Write($" {wAbbr[wi],5}");
        Console.WriteLine();
        Console.Write($"  {new string('-', 26)}");
        for (int wi = 0; wi < weapons.Count; wi++) Console.Write($" {"-----",5}");
        Console.WriteLine();

        for (int ei = 0; ei < enemies.Count; ei++)
        {
            Console.Write($"  {enemies[ei].Name,-26}");
            for (int wi = 0; wi < weapons.Count; wi++)
            {
                double rate = rateGetter(wi, ei);
                Console.Write($" {rate*100,4:F0}%");
            }
            Console.WriteLine();
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static double AverageAllStrategies(
        Dictionary<string, Dictionary<Strategy, FightStats[][]>> sims,
        string weaponId, int W, int ei)
    {
        double total = 0;
        int count = 0;
        foreach (var st in Enum.GetValues<Strategy>())
        {
            total += sims[weaponId][st].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy);
            count++;
        }
        return total / count;
    }

    private static void Sep() =>
        Console.WriteLine("=".PadRight(90, '='));

    private static string StratLabel(Strategy s) => s switch
    {
        Strategy.AlwaysAttack => "Basic  (always Attack)",
        Strategy.AttackHeavy  => "Heavy  (HeavyAttack / Defend)",
        Strategy.MagicHeavy   => "Magic  (Firebolt / Refocus)",
        _                     => s.ToString(),
    };

    private static string WeaponAbbr(EquipmentDefinition w) => w.Id switch
    {
        "FlintKnife"  => "Flnt",
        "WoodClub"    => "Club",
        "IronDagger"  => "Dggr",
        "IronSword"   => "Swrd",
        "IronMace"    => "Mace",
        "EmberStaff"  => "Embr",
        "FrostStaff"  => "Frst",
        "StormStaff"  => "Strm",
        "VineStaff"   => "Vine",
        "ShadowStaff" => "Shdw",
        _             => w.Id[..Math.Min(4, w.Id.Length)],
    };
}

// ── Types ─────────────────────────────────────────────────────────────────────

internal enum Strategy { AlwaysAttack, AttackHeavy, MagicHeavy }

internal record FightStats(int Wins, int Total, double AvgTurns, double AvgHpFracOnWin)
{
    public double WinRate => (double)Wins / Total;
}
