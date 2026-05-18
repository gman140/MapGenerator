using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Services;

public class CombatEngine : ICombatEngine
{
    private readonly IEnemyDefinitionProvider _enemyProvider;
    private readonly IResourceDefinitionProvider _resourceProvider;
    private readonly IFoodDefinitionProvider _foodProvider;
    private readonly EnemySpawner _spawner;
    private readonly Random _rng = new();

    private static readonly Dictionary<CombatActionType, int> StaminaCosts = new()
    {
        [CombatActionType.Attack]      = 2,
        [CombatActionType.HeavyAttack] = 4,
        [CombatActionType.Dodge]       = 2,
        [CombatActionType.Defend]      = 0,
        [CombatActionType.UseItem]     = 1,
        [CombatActionType.Flee]        = 0,
    };

    public CombatEngine(
        IEnemyDefinitionProvider enemyProvider,
        IResourceDefinitionProvider resourceProvider,
        IFoodDefinitionProvider foodProvider,
        EnemySpawner spawner)
    {
        _enemyProvider    = enemyProvider;
        _resourceProvider = resourceProvider;
        _foodProvider     = foodProvider;
        _spawner          = spawner;
    }

    // ── StartCombat ──────────────────────────────────────────────────────────

    public CombatSession StartCombat(CombatStartContext context)
    {
        var player  = context.Player;
        var enemies = _spawner.SpawnEnemies(context, _rng);

        var session = new CombatSession
        {
            Id                  = Guid.NewGuid().ToString("N"),
            PlayerId            = player.Id,
            PlayerHp            = player.MaxHp,
            PlayerMaxHp         = player.MaxHp,
            PlayerStamina       = 5,
            PlayerMaxStamina    = player.MaxStamina,
            PlayerBaseAttack    = player.BaseAttack,
            PlayerBaseDefense   = player.BaseDefense,
            PlayerBaseDodgeChance = player.BaseDodgeChance,
            Enemies             = enemies,
            Phase               = CombatPhase.PlayerTurn,
            ContextLabel        = BuildContextLabel(context),
        };

        ApplyEquipmentModifiers(session, player);

        foreach (var enemy in enemies)
        {
            var def = _enemyProvider.GetById(enemy.DefinitionId);
            if (def?.AppearTexts.Count > 0)
                session.Log.Add(PickRandom(def.AppearTexts));
        }

        return session;
    }

    // ── ProcessTurn ──────────────────────────────────────────────────────────

    public CombatSession ProcessTurn(CombatSession session, Player player, CombatAction action)
    {
        int cost = StaminaCosts.GetValueOrDefault(action.Type, 0);
        if (session.PlayerStamina < cost)
        {
            session.Log.Add("Not enough stamina for that.");
            return session;
        }

        session.TurnNumber++;
        session.PlayerStamina -= cost;
        bool defendedThisTurn = action.Type == CombatActionType.Defend;

        // Step 2: Player action
        switch (action.Type)
        {
            case CombatActionType.Attack:
                ExecutePlayerAttack(session, action.TargetEnemyId, isHeavy: false);
                break;
            case CombatActionType.HeavyAttack:
                ExecutePlayerAttack(session, action.TargetEnemyId, isHeavy: true);
                break;
            case CombatActionType.Defend:
                session.PlayerDefending = true;
                session.Log.Add("You plant your feet and raise your guard. Incoming damage will be halved this turn.");
                break;
            case CombatActionType.Dodge:
                session.PlayerDodging = true;
                session.Log.Add("You shift into a ready stance, prepared to slip aside.");
                break;
            case CombatActionType.UseItem:
                ExecutePlayerUseItem(session, player, action.ItemId);
                break;
            case CombatActionType.Flee:
                if (ExecutePlayerFlee(session, player))
                    return session;
                break;
        }

        // Step 3: Check victory
        if (IsAllDeadOrFled(session))
        {
            session.Phase = CombatPhase.Victory;
            session.Log.Add("The battle is over. You stand amid the quiet.");
            session.TurnBoundaryLogIndex = session.Log.Count;
            return session;
        }

        // Mark boundary between player phase and enemy phase for UI animation
        session.TurnBoundaryLogIndex = session.Log.Count;

        // Step 4: Enemy turns
        foreach (var enemy in session.Enemies.Where(e => e.CurrentHp > 0 && !e.HasFled))
        {
            // Step 4a: Check enemy flee
            var def = _enemyProvider.GetById(enemy.DefinitionId);
            if (def != null
                && (float)enemy.CurrentHp / enemy.MaxHp < def.FleeHpThreshold
                && _rng.NextDouble() < def.FleeChance)
            {
                enemy.HasFled = true;
                string fleeText = def.FleeTexts.Count > 0
                    ? PickRandom(def.FleeTexts)
                    : $"The {enemy.Name} flees!";
                session.Log.Add(fleeText);
                continue;
            }

            // Step 4b: Enemy action
            ExecuteEnemyAction(session, player, enemy, def);
        }

        // Step 4b post: all enemies fled during their own turns → victory
        if (IsAllDeadOrFled(session))
        {
            session.Phase = CombatPhase.Victory;
            session.Log.Add("The last of them disappears into the dark. The fight is over.");
            session.TurnBoundaryLogIndex = session.Log.Count;
            return session;
        }

        // Step 5: Check defeat
        if (session.PlayerHp <= 0)
        {
            session.PlayerHp = 0;
            session.Phase    = CombatPhase.Defeat;
            session.Log.Add("You collapse. The darkness takes you.");
            return session;
        }

        // Step 6: Recover stamina
        int gain = 3 + (defendedThisTurn ? 2 : 0);
        session.PlayerStamina = Math.Min(session.PlayerMaxStamina, session.PlayerStamina + gain);

        // Step 7: Tick modifiers
        TickModifiers(session);

        // Step 8: Clear turn flags
        session.PlayerDefending = false;
        session.PlayerDodging   = false;
        foreach (var e in session.Enemies) e.IsDefending = false;

        return session;
    }

    // ── IsFinished / Resolve ─────────────────────────────────────────────────

    public bool IsFinished(CombatSession session) =>
        session.Phase is CombatPhase.Victory or CombatPhase.Defeat;

    public CombatResult Resolve(CombatSession session)
    {
        bool playerDied = session.Phase == CombatPhase.Defeat;
        bool fled = session.PlayerFled;

        var loot = new Dictionary<string, int>();
        var defeated = new Dictionary<string, int>();

        // Dead and fled enemies both drop loot
        foreach (var enemy in session.Enemies.Where(e => e.CurrentHp <= 0 || e.HasFled))
        {
            defeated[enemy.DefinitionId] = defeated.GetValueOrDefault(enemy.DefinitionId) + 1;

            var def = _enemyProvider.GetById(enemy.DefinitionId);
            if (def == null) continue;
            foreach (var entry in def.LootTable)
            {
                if (_rng.NextDouble() < entry.Chance)
                {
                    int qty = entry.MinQty == entry.MaxQty
                        ? entry.MinQty
                        : _rng.Next(entry.MinQty, entry.MaxQty + 1);
                    loot[entry.ItemId] = loot.GetValueOrDefault(entry.ItemId) + qty;
                }
            }
        }

        string summary = playerDied
            ? "You have been defeated. Your journey ends here — for now."
            : fled
                ? "You escaped. The sounds of pursuit fade behind you."
                : loot.Count > 0
                    ? $"You gathered {loot.Count} item type(s) from the fallen."
                    : "The battle is won. The room falls silent.";

        return new CombatResult
        {
            Victory         = session.Phase == CombatPhase.Victory,
            Fled            = fled,
            PlayerDied      = playerDied,
            HpRemaining     = session.PlayerHp,
            StaminaRemaining = session.PlayerStamina,
            LootGained      = loot,
            EnemiesDefeated = defeated,
            SummaryMessage  = summary,
        };
    }

    // ── TryGenerateEncounter ─────────────────────────────────────────────────

    public CombatSession? TryGenerateEncounter(
        Player player, string biomeType, bool inDungeon, string? dungeonTheme, int? dungeonFloor)
    {
        float rate = _spawner.GetEncounterRate(biomeType, inDungeon);
        if (rate <= 0f || _rng.NextDouble() >= rate) return null;

        var context = new CombatStartContext
        {
            Player       = player,
            Trigger      = CombatTrigger.RandomEncounter,
            BiomeType    = biomeType,
            DungeonTheme = dungeonTheme,
            DungeonFloor = dungeonFloor,
        };

        var session = StartCombat(context);
        return session.Enemies.Count > 0 ? session : null;
    }

    // ── Player attack ────────────────────────────────────────────────────────

    private void ExecutePlayerAttack(CombatSession session, string? targetId, bool isHeavy)
    {
        var target = GetTargetEnemy(session, targetId);
        if (target == null) { session.Log.Add("No valid target."); return; }

        int effAtk = EffectivePlayerAttack(session);
        int effDef = EffectiveEnemyDefense(target);
        double variance = 0.85 + _rng.NextDouble() * 0.30;
        double mult = isHeavy ? 1.5 : 1.0;
        int damage = (int)Math.Max(1, (effAtk - effDef) * mult * variance);

        target.CurrentHp = Math.Max(0, target.CurrentHp - damage);

        string verb = isHeavy ? "drive a heavy blow into" : "strike";
        string fell = target.CurrentHp == 0 ? $" The {target.Name} falls." : string.Empty;
        session.Log.Add($"You {verb} the {target.Name} for {damage} damage.{fell}");
    }

    // ── Player use item ──────────────────────────────────────────────────────

    private void ExecutePlayerUseItem(CombatSession session, Player player, string? itemId)
    {
        if (string.IsNullOrEmpty(itemId)) { session.Log.Add("No item selected."); return; }

        int qty = player.Inventory.GetValueOrDefault(itemId)
                + player.CraftedItems.GetValueOrDefault(itemId);
        if (qty <= 0) { session.Log.Add("You don't have that item."); return; }

        var food = _foodProvider.GetById(itemId);
        if (food != null)
        {
            int heal = Math.Max(1, (int)Math.Ceiling(food.SatietyRestore / 2.0));
            int before = session.PlayerHp;
            session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + heal);
            int healed = session.PlayerHp - before;

            // Restore satiety on the player object too
            player.Satiety = Math.Min(100, player.Satiety + food.SatietyRestore);

            ConsumeItem(player, itemId);
            session.Log.Add($"You eat the {food.Name}. You recover {healed} HP. ({session.PlayerHp}/{session.PlayerMaxHp})");
            return;
        }

        session.Log.Add("You can't use that here.");
    }

    // ── Player flee ──────────────────────────────────────────────────────────

    private bool ExecutePlayerFlee(CombatSession session, Player player)
    {
        const float BaseFleeChance = 0.60f;
        if (_rng.NextDouble() < BaseFleeChance)
        {
            session.Phase     = CombatPhase.Victory;
            session.PlayerFled = true;
            session.Log.Add("You seize an opening and sprint away. You escape.");
            return true;
        }
        session.Log.Add("You attempt to flee but can't find an opening. The enemies press in.");
        return false;
    }

    // ── Enemy action ─────────────────────────────────────────────────────────

    private void ExecuteEnemyAction(CombatSession session, Player player, Enemy enemy, EnemyDefinition? def)
    {
        var actionType = PickWeightedAction(enemy.ActionTable);

        switch (actionType)
        {
            case EnemyActionType.Attack:
                ExecuteEnemyAttack(session, enemy, def, isHeavy: false);
                break;
            case EnemyActionType.HeavyAttack:
                ExecuteEnemyAttack(session, enemy, def, isHeavy: true);
                break;
            case EnemyActionType.Defend:
                ExecuteEnemyDefend(session, enemy, def);
                break;
            case EnemyActionType.Buff:
                ExecuteEnemyBuff(session, enemy, def);
                break;
            case EnemyActionType.Regenerate:
                ExecuteEnemyRegen(session, enemy, def);
                break;
        }
    }

    private void ExecuteEnemyAttack(CombatSession session, Enemy enemy, EnemyDefinition? def, bool isHeavy)
    {
        int effAtk = EffectiveEnemyAttack(enemy);
        int effDef = EffectivePlayerDefense(session);
        double variance = 0.85 + _rng.NextDouble() * 0.30;
        double mult = isHeavy ? 1.5 : 1.0;

        // DamageMultiplier from modifiers (Bear's Rear Up)
        float extraMult = 1.0f + enemy.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.DamageMultiplier)
            .Sum(m => m.Value);
        enemy.ActiveModifiers.RemoveAll(m => m.Stat == ModifierStat.DamageMultiplier);

        int damage = (int)Math.Max(1, (effAtk - effDef) * mult * variance * extraMult);

        // Defend halves damage
        if (session.PlayerDefending) damage = (int)Math.Max(1, damage * 0.5);

        // Dodge roll
        if (session.PlayerDodging)
        {
            float dodge = EffectivePlayerDodgeChance(session);
            double roll = _rng.NextDouble();
            if (roll < dodge)
            {
                session.Log.Add("You sidestep the blow completely!");
                return;
            }
            if (roll < dodge * 2)
            {
                damage = (int)Math.Max(1, damage * 0.5);
                session.Log.Add("You partially deflect the attack...");
            }
            else
            {
                session.Log.Add("Your dodge fails.");
            }
        }

        session.PlayerHp = Math.Max(0, session.PlayerHp - damage);

        List<string>? pool = isHeavy ? def?.HeavyAttackTexts : def?.AttackTexts;
        string flavorText = pool?.Count > 0
            ? PickRandom(pool)
            : $"The {enemy.Name} attacks you.";
        session.Log.Add($"{flavorText} [{damage} damage, {session.PlayerHp}/{session.PlayerMaxHp} HP]");
    }

    private void ExecuteEnemyDefend(CombatSession session, Enemy enemy, EnemyDefinition? def)
    {
        enemy.IsDefending = true;

        // Bear's Rear Up: also boost next attack
        if (def?.DefendDamageBonus > 0)
        {
            enemy.ActiveModifiers.Add(new CombatModifier
            {
                Id             = "RearUp",
                Stat           = ModifierStat.DamageMultiplier,
                Value          = def.DefendDamageBonus,
                TurnsRemaining = 1,
                Source         = "RearUp",
            });
        }

        string text = def?.DefendTexts.Count > 0
            ? PickRandom(def.DefendTexts)
            : $"The {enemy.Name} braces for your next blow.";
        session.Log.Add(text);
    }

    private void ExecuteEnemyBuff(CombatSession session, Enemy enemy, EnemyDefinition? def)
    {
        if (def?.BuffStat == null) return;

        enemy.ActiveModifiers.Add(new CombatModifier
        {
            Id             = "EnemyBuff",
            Stat           = def.BuffStat.Value,
            Value          = def.BuffValue,
            TurnsRemaining = def.BuffTurns,
            Source         = $"Buff:{def.Id}",
        });

        string text = def.BuffTexts.Count > 0
            ? PickRandom(def.BuffTexts)
            : $"The {enemy.Name} strengthens itself.";
        session.Log.Add(text);
    }

    private void ExecuteEnemyRegen(CombatSession session, Enemy enemy, EnemyDefinition? def)
    {
        int amount = def?.RegenerateAmount ?? 5;
        int before = enemy.CurrentHp;
        enemy.CurrentHp = Math.Min(enemy.MaxHp, enemy.CurrentHp + amount);
        int healed = enemy.CurrentHp - before;

        string text = def?.RegenerateTexts.Count > 0
            ? PickRandom(def.RegenerateTexts)
            : $"The {enemy.Name} regenerates {healed} HP.";
        session.Log.Add(text);
    }

    // ── Equipment modifiers ──────────────────────────────────────────────────

    private void ApplyEquipmentModifiers(CombatSession session, Player player)
    {
        TryApplyEquipmentItem(session, player.EquippedWeaponId);
        TryApplyEquipmentItem(session, player.EquippedArmorId);
        TryApplyEquipmentItem(session, player.EquippedHatId);
    }

    private void TryApplyEquipmentItem(CombatSession session, string? itemId)
    {
        if (itemId == null) return;
        var def = _resourceProvider.GetById(itemId);
        if (def == null) return;

        if (def.AttackBonus != 0)
            session.ActiveModifiers.Add(new CombatModifier
            {
                Id = $"Equip:Atk:{itemId}", Stat = ModifierStat.Attack,
                Value = def.AttackBonus, Source = $"Equipment:{itemId}",
            });
        if (def.DefenseBonus != 0)
            session.ActiveModifiers.Add(new CombatModifier
            {
                Id = $"Equip:Def:{itemId}", Stat = ModifierStat.Defense,
                Value = def.DefenseBonus, Source = $"Equipment:{itemId}",
            });
        if (def.DodgeChanceBonus != 0)
            session.ActiveModifiers.Add(new CombatModifier
            {
                Id = $"Equip:Dodge:{itemId}", Stat = ModifierStat.DodgeChance,
                Value = def.DodgeChanceBonus, Source = $"Equipment:{itemId}",
            });
    }

    // ── Effective stat helpers ────────────────────────────────────────────────

    private static int EffectivePlayerAttack(CombatSession s) =>
        (int)(s.PlayerBaseAttack + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value));

    private static int EffectivePlayerDefense(CombatSession s) =>
        (int)(s.PlayerBaseDefense + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Defense).Sum(m => m.Value));

    private static float EffectivePlayerDodgeChance(CombatSession s) =>
        s.PlayerBaseDodgeChance + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.DodgeChance).Sum(m => m.Value);

    private static int EffectiveEnemyAttack(Enemy e) =>
        (int)(e.Attack + e.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value));

    private static int EffectiveEnemyDefense(Enemy e)
    {
        int def = e.Defense;
        if (e.IsDefending) def = (int)(def * 2.5);  // defending roughly doubles effective defense
        return def;
    }

    // ── Modifier tick ─────────────────────────────────────────────────────────

    private static void TickModifiers(CombatSession session)
    {
        TickList(session.ActiveModifiers);
        foreach (var enemy in session.Enemies)
            TickList(enemy.ActiveModifiers);
    }

    private static void TickList(List<CombatModifier> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].TurnsRemaining == null) continue;
            list[i].TurnsRemaining--;
            if (list[i].TurnsRemaining <= 0)
                list.RemoveAt(i);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool IsAllDeadOrFled(CombatSession session) =>
        session.Enemies.All(e => e.CurrentHp <= 0 || e.HasFled);

    private static Enemy? GetTargetEnemy(CombatSession session, string? targetId)
    {
        if (targetId != null)
            return session.Enemies.FirstOrDefault(e => e.InstanceId == targetId && e.CurrentHp > 0 && !e.HasFled);
        return session.Enemies.FirstOrDefault(e => e.CurrentHp > 0 && !e.HasFled);
    }

    private static EnemyActionType PickWeightedAction(List<EnemyActionEntry> table)
    {
        int total = table.Sum(e => e.Weight);
        int roll = Random.Shared.Next(total);
        int cumulative = 0;
        foreach (var entry in table)
        {
            cumulative += entry.Weight;
            if (roll < cumulative) return entry.Action;
        }
        return table[0].Action;
    }

    private string PickRandom(List<string> list) =>
        list[_rng.Next(list.Count)];

    private static void ConsumeItem(Player player, string itemId)
    {
        if (player.Inventory.TryGetValue(itemId, out int qty) && qty > 0)
        {
            player.Inventory[itemId] = qty - 1;
            if (player.Inventory[itemId] <= 0) player.Inventory.Remove(itemId);
            return;
        }
        if (player.CraftedItems.TryGetValue(itemId, out int cqty) && cqty > 0)
        {
            player.CraftedItems[itemId] = cqty - 1;
            if (player.CraftedItems[itemId] <= 0) player.CraftedItems.Remove(itemId);
        }
    }

    private static string BuildContextLabel(CombatStartContext ctx)
    {
        if (ctx.DungeonTheme != null)
        {
            string theme = ctx.DungeonTheme switch
            {
                "CrystalCavern" => "Crystal Cavern",
                "AncientTomb"   => "Ancient Tomb",
                "RootLabyrinth" => "Root Labyrinth",
                "FrozenVault"   => "Frozen Vault",
                _               => ctx.DungeonTheme,
            };
            return ctx.DungeonFloor.HasValue ? $"Floor {ctx.DungeonFloor} — {theme}" : theme;
        }
        return ctx.BiomeType ?? "Unknown";
    }
}
