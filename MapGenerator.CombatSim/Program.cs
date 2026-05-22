using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Models;
using MapGenerator.Combat.Services;
using MapGenerator.Application.Services;
using MapGenerator.Domain.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.CombatSim;

internal static class Program
{
    private const int    FightsPerEnemy  = 10;
    private const int    MaxTurns        = 150;
    private const int    Seed            = 42;
    private const string PrimarySpell    = "firebolt";
    private const int    PrimarySpellCost = 3;

    // Low tier:  WoodClub (Atk+5, Bludgeoning)  + LeatherArmor (Def+6)  + TravelHat (Dodge+5%)
    private static readonly EquipmentSet LowGear = new(
        AtkBonus: 5, WeaponDmgType: DamageType.Bludgeoning,
        DefBonus: 6, DodgeBonus: 0.05f);

    // High tier: IronSword (Atk+8, Slashing) + BearHideCloak (Def+10) + HoodedCowl (Dodge+10%)
    private static readonly EquipmentSet HighGear = new(
        AtkBonus: 8, WeaponDmgType: DamageType.Slashing,
        DefBonus: 10, DodgeBonus: 0.10f);

    // ── Entry ─────────────────────────────────────────────────────────────────

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
        var players    = GeneratePlayers(new Random(seed));

        // ── Run 9 simulations (3 strategies × 3 equipment tiers) ──────────────

        var sims = new Dictionary<(Strategy, EquipmentTier), FightStats[][]>();

        foreach (var strat in Enum.GetValues<Strategy>())
        foreach (var equip in Enum.GetValues<EquipmentTier>())
        {
            Console.Write($"  {StratLabel(strat) + " | " + TierLabel(equip),-36}");
            sims[(strat, equip)] = RunSim(engine, players, allEnemies, new Random(seed), strat, equip);
            Console.WriteLine();
        }
        Console.WriteLine();

        PrintReport(players, allEnemies, sims, seed);
    }

    // ── Simulation ────────────────────────────────────────────────────────────

    private static FightStats[][] RunSim(
        CombatEngine engine, List<Player> players,
        IReadOnlyList<EnemyDefinition> enemies,
        Random rng, Strategy strategy, EquipmentTier equip)
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
                    var (won, turns, hpFrac) = RunFight(engine, players[pi], enemies[ei], rng, strategy, equip);
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
        Random rng, Strategy strategy, EquipmentTier equip)
    {
        var session = BuildSession(player, def, rng, equip);
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
            Strategy.AlwaysAttack => new CombatAction { Type = CombatActionType.Attack,      TargetEnemyId = tid },
            Strategy.AttackHeavy  => PickAttackHeavy(session, tid),
            Strategy.MagicHeavy   => PickMagicHeavy(session, tid),
            _                     => new CombatAction { Type = CombatActionType.Attack,      TargetEnemyId = tid },
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
        Player player, EnemyDefinition def, Random rng, EquipmentTier equip)
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

        // Apply equipment as session modifiers (mirrors CombatEngine.ApplyEquipmentModifiers)
        var gear = equip switch
        {
            EquipmentTier.Low  => LowGear,
            EquipmentTier.High => HighGear,
            _                  => null,
        };
        if (gear != null)
        {
            session.PlayerWeaponDamageType = gear.WeaponDmgType;
            session.ActiveModifiers.Add(new CombatModifier { Id = "Equip:Atk",   Stat = ModifierStat.Attack,      Value = gear.AtkBonus,   Source = "Equipment" });
            session.ActiveModifiers.Add(new CombatModifier { Id = "Equip:Def",   Stat = ModifierStat.Defense,     Value = gear.DefBonus,   Source = "Equipment" });
            session.ActiveModifiers.Add(new CombatModifier { Id = "Equip:Dodge", Stat = ModifierStat.DodgeChance, Value = gear.DodgeBonus, Source = "Equipment" });
        }

        return session;
    }

    // ── Report ────────────────────────────────────────────────────────────────

    private static void PrintReport(
        List<Player> players,
        IReadOnlyList<EnemyDefinition> enemies,
        Dictionary<(Strategy, EquipmentTier), FightStats[][]> sims,
        int seed)
    {
        int W = players.Count, E = enemies.Count;
        string[] abbr = enemies.Select(e => CamelAbbrev(e.Name)).ToArray();

        Sep();
        Console.WriteLine("  COMBAT BALANCE SIMULATION");
        Console.WriteLine($"  {W} players (L1-L{W})  x  {E} enemies  x  {FightsPerEnemy} fights  x  9 combos  =  {W * E * FightsPerEnemy * 9:N0} battles");
        Console.WriteLine($"  Seed: {seed}");
        Console.WriteLine($"  Strategies:  Basic=always Attack  |  Heavy=HeavyAttack/Defend  |  Magic={PrimarySpell}/Refocus");
        Console.WriteLine($"  Equipment:   None  |  Low (WoodClub+LeatherArmor+TravelHat)  |  High (IronSword+BearHideCloak+HoodedCowl)");
        Sep();

        // ── Player stats ──────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("PLAYER STATS");
        Console.WriteLine("  Lvl   HP   Atk  Def  Mag  Dodge  Pts");
        Console.WriteLine("  ---  ---  ---  ---  ---  -----  ---");
        foreach (var p in players)
            Console.WriteLine($"  L{p.Level,2}  {p.MaxHp,3}  {p.BaseAttack,3}  {p.BaseDefense,3}  {p.BaseMagic,3}  {p.BaseDodgeChance*100,4:F0}%  {(p.Level-1)*3,3}");

        // ── Win-rate grids grouped by equipment tier ──────────────────────────
        foreach (var tier in Enum.GetValues<EquipmentTier>())
        {
            Console.WriteLine();
            Console.WriteLine($"══ {TierLabel(tier).ToUpper()} ══{new string('═', Math.Max(0, 50 - TierLabel(tier).Length))}");
            foreach (var strat in Enum.GetValues<Strategy>())
                PrintGrid(StratLabel(strat), abbr, players, sims[(strat, tier)]);
        }

        // ── 9-column summary matrix ───────────────────────────────────────────
        Console.WriteLine();
        Sep();
        Console.WriteLine("  9-COMBO SUMMARY  (overall win rate across all 20 levels)");
        Console.WriteLine("                                         Basic              Heavy              Magic");
        Console.WriteLine($"  {"Enemy",-22}  {"None",5} {"Low",5} {"High",5}   {"None",5} {"Low",5} {"High",5}   {"None",5} {"Low",5} {"High",5}");
        Console.WriteLine($"  {new string('-',22)}  {new string('-',5)} {new string('-',5)} {new string('-',5)}   {new string('-',5)} {new string('-',5)} {new string('-',5)}   {new string('-',5)} {new string('-',5)} {new string('-',5)}");

        for (int ei = 0; ei < E; ei++)
        {
            double Rate(Strategy st, EquipmentTier eq) =>
                sims[(st, eq)].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy);

            string Pct(Strategy st, EquipmentTier eq) => $"{Rate(st, eq)*100:F0}%";

            Console.WriteLine(
                $"  {enemies[ei].Name,-22}" +
                $"  {Pct(Strategy.AlwaysAttack, EquipmentTier.None),5}" +
                $" {Pct(Strategy.AlwaysAttack, EquipmentTier.Low),5}" +
                $" {Pct(Strategy.AlwaysAttack, EquipmentTier.High),5}" +
                $"   {Pct(Strategy.AttackHeavy, EquipmentTier.None),5}" +
                $" {Pct(Strategy.AttackHeavy, EquipmentTier.Low),5}" +
                $" {Pct(Strategy.AttackHeavy, EquipmentTier.High),5}" +
                $"   {Pct(Strategy.MagicHeavy, EquipmentTier.None),5}" +
                $" {Pct(Strategy.MagicHeavy, EquipmentTier.Low),5}" +
                $" {Pct(Strategy.MagicHeavy, EquipmentTier.High),5}");
        }

        // ── Equipment impact per strategy ─────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("EQUIPMENT LIFT  (High-tier vs No-equipment, per strategy)");
        Console.WriteLine($"  {"Enemy",-22}  {"Basic Δ",8}  {"Heavy Δ",8}  {"Magic Δ",8}  {"Best combo",12}");
        Console.WriteLine($"  {new string('-',22)}  {new string('-',8)}  {new string('-',8)}  {new string('-',8)}  {new string('-',12)}");

        for (int ei = 0; ei < E; ei++)
        {
            double Rate(Strategy st, EquipmentTier eq) =>
                sims[(st, eq)].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy);

            double bLift = Rate(Strategy.AlwaysAttack, EquipmentTier.High) - Rate(Strategy.AlwaysAttack, EquipmentTier.None);
            double hLift = Rate(Strategy.AttackHeavy,  EquipmentTier.High) - Rate(Strategy.AttackHeavy,  EquipmentTier.None);
            double mLift = Rate(Strategy.MagicHeavy,   EquipmentTier.High) - Rate(Strategy.MagicHeavy,   EquipmentTier.None);

            // Find best combo
            double bestRate = -1;
            string bestCombo = "";
            foreach (var st in Enum.GetValues<Strategy>())
            foreach (var eq in Enum.GetValues<EquipmentTier>())
            {
                double r = Rate(st, eq);
                if (r > bestRate) { bestRate = r; bestCombo = $"{StratShort(st)}+{TierShort(eq)}"; }
            }

            string Lift(double d) => d >= 0 ? $"+{d*100:F0}%" : $"{d*100:F0}%";
            Console.WriteLine(
                $"  {enemies[ei].Name,-22}  {Lift(bLift),8}  {Lift(hLift),8}  {Lift(mLift),8}  {bestCombo + $" ({bestRate*100:F0}%)",12}");
        }

        // ── Balance flags ─────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("BALANCE FLAGS");
        Console.WriteLine();
        var flags = new List<string>();

        for (int ei = 0; ei < E; ei++)
        {
            double Rate(Strategy st, EquipmentTier eq) =>
                sims[(st, eq)].Sum(row => row[ei].Wins) / (double)(W * FightsPerEnemy);

            double bestOverall = Enum.GetValues<Strategy>()
                .SelectMany(st => Enum.GetValues<EquipmentTier>().Select(eq => Rate(st, eq)))
                .Max();

            double bestAtL20 = Enum.GetValues<Strategy>()
                .SelectMany(st => Enum.GetValues<EquipmentTier>().Select(eq => sims[(st, eq)][W-1][ei].WinRate))
                .Max();

            if (bestOverall < 0.02)
                flags.Add($"  [UNBEATABLE]       {enemies[ei].Name} — < 2% win rate across all 9 combos.");
            else if (bestAtL20 < 0.40)
                flags.Add($"  [HARD AT L20]      {enemies[ei].Name} — best L20 win rate is only {bestAtL20*100:F0}%.");

            // Trivial if even the weakest combo beats it 100% at L1
            if (sims[(Strategy.AlwaysAttack, EquipmentTier.None)][0][ei].WinRate >= 1.0)
                flags.Add($"  [TRIVIAL]          {enemies[ei].Name} — 100% at L1 with no gear, basic attack.");

            // Magic-dependent even with high gear
            double mHigh  = Rate(Strategy.MagicHeavy,   EquipmentTier.High);
            double bHigh  = Rate(Strategy.AlwaysAttack,  EquipmentTier.High);
            if (mHigh - bHigh >= 0.30)
                flags.Add($"  [MAGIC DEPENDENT]  {enemies[ei].Name} — Magic+High is {mHigh*100:F0}% vs Basic+High {bHigh*100:F0}% (likely high physical defense).");

            // Equipment makes a huge difference vs physical but not magic
            double equipLiftBasic = Rate(Strategy.AlwaysAttack, EquipmentTier.High) - Rate(Strategy.AlwaysAttack, EquipmentTier.None);
            if (equipLiftBasic >= 0.25)
                flags.Add($"  [EQUIP SENSITIVE]  {enemies[ei].Name} — high-tier gear lifts Basic win rate by {equipLiftBasic*100:F0}%.");
        }

        if (flags.Count == 0)
            Console.WriteLine("  No obvious balance issues detected.");
        else
            foreach (var f in flags) Console.WriteLine(f);

        Console.WriteLine();
        Sep();
    }

    // ── Grid printer ──────────────────────────────────────────────────────────

    private static void PrintGrid(string title, string[] abbr, List<Player> players, FightStats[][] results)
    {
        int W = players.Count, E = abbr.Length;
        Console.WriteLine();
        Console.WriteLine($"  {title}");
        Console.Write  ("       ");
        foreach (var a in abbr) Console.Write($" {a,-5}");
        Console.WriteLine("  | Ovrl");
        Console.Write  ("  -----");
        foreach (var _ in abbr) Console.Write("------");
        Console.WriteLine("--+-----");
        for (int pi = 0; pi < W; pi++)
        {
            Console.Write($"  L{players[pi].Level,2}  ");
            int wins = 0;
            for (int ei = 0; ei < E; ei++)
            {
                var st = results[pi][ei];
                wins += st.Wins;
                Console.Write($" {st.WinRate*100,3:F0}% ");
            }
            Console.WriteLine($"  | {(double)wins/(E*FightsPerEnemy)*100:F0}%");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void Sep() =>
        Console.WriteLine("=============================================================================");

    private static string StratLabel(Strategy s) => s switch
    {
        Strategy.AlwaysAttack => "Basic  (always Attack)",
        Strategy.AttackHeavy  => "Heavy  (HeavyAttack / Defend)",
        Strategy.MagicHeavy   => "Magic  (Firebolt / Refocus)",
        _                     => s.ToString(),
    };

    private static string StratShort(Strategy s) => s switch
    {
        Strategy.AlwaysAttack => "Basic",
        Strategy.AttackHeavy  => "Heavy",
        Strategy.MagicHeavy   => "Magic",
        _                     => s.ToString(),
    };

    private static string TierLabel(EquipmentTier t) => t switch
    {
        EquipmentTier.None => "No Equipment",
        EquipmentTier.Low  => "Low Tier  (WoodClub + LeatherArmor + TravelHat)",
        EquipmentTier.High => "High Tier (IronSword + BearHideCloak + HoodedCowl)",
        _                  => t.ToString(),
    };

    private static string TierShort(EquipmentTier t) => t switch
    {
        EquipmentTier.None => "None",
        EquipmentTier.Low  => "Low",
        EquipmentTier.High => "High",
        _                  => t.ToString(),
    };

    private static string CamelAbbrev(string name)
    {
        var words = new List<string>();
        int start = 0;
        for (int i = 1; i < name.Length; i++)
            if (char.IsUpper(name[i])) { words.Add(name[start..i]); start = i; }
        words.Add(name[start..]);
        string abbr = words.Count == 1
            ? name[..Math.Min(5, name.Length)]
            : string.Concat(words.Select(w => w[..Math.Min(2, w.Length)]));
        return (abbr.Length > 5 ? abbr[..5] : abbr).PadRight(5);
    }
}

// ── Types ─────────────────────────────────────────────────────────────────────

internal enum Strategy     { AlwaysAttack, AttackHeavy, MagicHeavy }
internal enum EquipmentTier { None, Low, High }

internal record EquipmentSet(
    int AtkBonus, DamageType WeaponDmgType,
    int DefBonus, float DodgeBonus);

internal record FightStats(int Wins, int Total, double AvgTurns, double AvgHpFracOnWin)
{
    public double WinRate => (double)Wins / Total;
}
