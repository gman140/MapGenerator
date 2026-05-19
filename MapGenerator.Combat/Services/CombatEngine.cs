using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;
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

    public CombatTurnResult ProcessTurn(CombatSession session, Player player, CombatAction action)
    {
        var events = new List<CombatEvent>();

        int cost = StaminaCosts.GetValueOrDefault(action.Type, 0);
        if (session.PlayerStamina < cost)
        {
            string msg = "Not enough stamina for that.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return new CombatTurnResult { Session = session, Events = events };
        }

        session.TurnNumber++;
        session.PlayerStamina -= cost;
        bool defendedThisTurn = action.Type == CombatActionType.Defend;

        // Step 2: Player action — emit stamina drop first, then the action
        if (cost > 0)
            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.PlayerUpdate,
                PlayerStamina = session.PlayerStamina,
                DelayMs = 120,
            });

        switch (action.Type)
        {
            case CombatActionType.Attack:
                ExecutePlayerAttack(session, action.TargetEnemyId, isHeavy: false, events);
                break;
            case CombatActionType.HeavyAttack:
                ExecutePlayerAttack(session, action.TargetEnemyId, isHeavy: true, events);
                break;
            case CombatActionType.Defend:
                session.PlayerDefending = true;
                string defendMsg = "You plant your feet and raise your guard. Incoming damage will be halved this turn.";
                session.Log.Add(defendMsg);
                events.Add(new CombatEvent
                {
                    Kind = CombatEventKind.PlayerUpdate,
                    Log = defendMsg,
                    PlayerDefending = true,
                    DelayMs = 200,
                });
                break;
            case CombatActionType.Dodge:
                session.PlayerDodging = true;
                string dodgeMsg = "You shift into a ready stance, prepared to slip aside.";
                session.Log.Add(dodgeMsg);
                events.Add(new CombatEvent
                {
                    Kind = CombatEventKind.PlayerUpdate,
                    Log = dodgeMsg,
                    PlayerDodging = true,
                    DelayMs = 200,
                });
                break;
            case CombatActionType.UseItem:
                ExecutePlayerUseItem(session, player, action.ItemId, events);
                break;
            case CombatActionType.Flee:
                if (ExecutePlayerFlee(session, player, events))
                    return new CombatTurnResult { Session = session, Events = events };
                break;
        }

        // Step 3: Check victory after player action
        if (IsAllDeadOrFled(session))
        {
            session.Phase = CombatPhase.Victory;
            string victoryMsg = "The battle is over. You stand amid the quiet.";
            session.Log.Add(victoryMsg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = victoryMsg, DelayMs = 300 });
            return new CombatTurnResult { Session = session, Events = events };
        }

        // Step 4: Enemy turns — add a beat before the enemy phase
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, DelayMs = 350 });

        foreach (var enemy in session.Enemies.Where(e => e.CurrentHp > 0 && !e.HasFled).ToList())
        {
            var def = _enemyProvider.GetById(enemy.DefinitionId);

            // Step 4a: Check enemy flee
            if (def != null
                && (float)enemy.CurrentHp / enemy.MaxHp < def.FleeHpThreshold
                && _rng.NextDouble() < def.FleeChance)
            {
                enemy.HasFled = true;
                string fleeText = def.FleeTexts.Count > 0
                    ? PickRandom(def.FleeTexts)
                    : $"The {enemy.Name} flees!";
                session.Log.Add(fleeText);
                events.Add(new CombatEvent
                {
                    Kind = CombatEventKind.EnemyFled,
                    EnemyInstanceId = enemy.InstanceId,
                    Log = fleeText,
                });
                continue;
            }

            // Step 4b: Enemy action
            ExecuteEnemyAction(session, player, enemy, def, events);
        }

        // All enemies fled during their own turns → victory
        if (IsAllDeadOrFled(session))
        {
            session.Phase = CombatPhase.Victory;
            string allFledMsg = "The last of them disappears into the dark. The fight is over.";
            session.Log.Add(allFledMsg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = allFledMsg, DelayMs = 300 });
            return new CombatTurnResult { Session = session, Events = events };
        }

        // Step 5: Check defeat
        if (session.PlayerHp <= 0)
        {
            session.PlayerHp = 0;
            session.Phase    = CombatPhase.Defeat;
            string defeatMsg = "You collapse. The darkness takes you.";
            session.Log.Add(defeatMsg);
            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.PlayerUpdate,
                Log = defeatMsg,
                PlayerHp = 0,
                DelayMs = 400,
            });
            return new CombatTurnResult { Session = session, Events = events };
        }

        // Step 6: Recover stamina
        int gain = 3 + (defendedThisTurn ? 2 : 0);
        session.PlayerStamina = Math.Min(session.PlayerMaxStamina, session.PlayerStamina + gain);
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.PlayerUpdate,
            PlayerStamina = session.PlayerStamina,
            DelayMs = 150,
        });

        // Step 7: Tick modifiers
        TickModifiers(session);

        // Step 8: Clear turn flags
        session.PlayerDefending = false;
        session.PlayerDodging   = false;
        foreach (var e in session.Enemies) e.IsDefending = false;
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.PlayerUpdate,
            PlayerDefending = false,
            PlayerDodging   = false,
        });

        return new CombatTurnResult { Session = session, Events = events };
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

    private void ExecutePlayerAttack(CombatSession session, string? targetId, bool isHeavy, List<CombatEvent> events)
    {
        var target = GetTargetEnemy(session, targetId);
        if (target == null)
        {
            string noTarget = "No valid target.";
            session.Log.Add(noTarget);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = noTarget, DelayMs = 200 });
            return;
        }

        int effAtk = EffectivePlayerAttack(session);
        int effDef = EffectiveEnemyDefense(target);
        double variance = 0.85 + _rng.NextDouble() * 0.30;
        double mult = isHeavy ? 1.5 : 1.0;
        int damage = (int)Math.Max(1, (effAtk - effDef) * mult * variance);

        target.CurrentHp = Math.Max(0, target.CurrentHp - damage);

        string verb = isHeavy ? "drive a heavy blow into" : "strike";
        bool killed = target.CurrentHp == 0;
        string fell = killed ? $" The {target.Name} falls." : string.Empty;
        string log = $"You {verb} the {target.Name} for {damage} damage.{fell}";
        session.Log.Add(log);

        // Shake + HP update (simultaneous)
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.ShakeEnemy,
            EnemyInstanceId = target.InstanceId,
            Log = log,
            EnemyHp = target.CurrentHp,
            DelayMs = 380,
        });

        if (killed)
            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.EnemyDied,
                EnemyInstanceId = target.InstanceId,
            });
    }

    // ── Player use item ──────────────────────────────────────────────────────

    private void ExecutePlayerUseItem(CombatSession session, Player player, string? itemId, List<CombatEvent> events)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            string msg = "No item selected.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return;
        }

        int qty = player.Inventory.GetValueOrDefault(itemId)
                + player.CraftedItems.GetValueOrDefault(itemId);
        if (qty <= 0)
        {
            string msg = "You don't have that item.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return;
        }

        // Check resource-based combat consumables first
        var resDef = _resourceProvider.GetById(itemId);
        if (resDef != null && resDef.Traits.HasFlag(ItemTrait.CombatConsumable))
        {
            ConsumeItem(player, itemId);
            var evt = new CombatEvent { Kind = CombatEventKind.PlayerUpdate, DelayMs = 300 };
            var parts = new List<string>();

            if (resDef.CombatHpRestore > 0)
            {
                int before = session.PlayerHp;
                session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + resDef.CombatHpRestore);
                int healed = session.PlayerHp - before;
                evt.PlayerHp = session.PlayerHp;
                parts.Add($"restored {healed} HP");
            }
            if (resDef.CombatStaminaRestore > 0)
            {
                session.PlayerStamina = Math.Min(session.PlayerMaxStamina, session.PlayerStamina + resDef.CombatStaminaRestore);
                evt.PlayerStamina = session.PlayerStamina;
                parts.Add($"recovered {resDef.CombatStaminaRestore} stamina");
            }
            if (resDef.CombatBuffStat.HasValue && resDef.CombatBuffTurns > 0)
            {
                session.ActiveModifiers.Add(new CombatModifier
                {
                    Id             = $"Item:{itemId}",
                    Stat           = resDef.CombatBuffStat.Value,
                    Value          = resDef.CombatBuffValue,
                    TurnsRemaining = resDef.CombatBuffTurns,
                    Source         = $"Item:{itemId}",
                });
                parts.Add(resDef.CombatBuffLabel ?? $"+{resDef.CombatBuffValue} for {resDef.CombatBuffTurns}t");
            }

            string msg = $"You use {resDef.Name}. " + (parts.Count > 0 ? string.Join(", ", parts) + "." : "Nothing happened.");
            session.Log.Add(msg);
            evt.Log = msg;
            events.Add(evt);
            return;
        }

        // Fall back to food items
        var food = _foodProvider.GetById(itemId);
        if (food != null)
        {
            int heal = Math.Max(1, (int)Math.Ceiling(food.SatietyRestore / 2.0));
            int before = session.PlayerHp;
            session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + heal);
            int healed = session.PlayerHp - before;
            player.Satiety = Math.Min(100, player.Satiety + food.SatietyRestore);
            ConsumeItem(player, itemId);

            string msg = $"You eat the {food.Name}. You recover {healed} HP. ({session.PlayerHp}/{session.PlayerMaxHp})";
            session.Log.Add(msg);
            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.PlayerUpdate,
                Log = msg,
                PlayerHp = session.PlayerHp,
                DelayMs = 300,
            });
            return;
        }

        string cantUse = "You can't use that here.";
        session.Log.Add(cantUse);
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = cantUse, DelayMs = 200 });
    }

    // ── Player flee ──────────────────────────────────────────────────────────

    private bool ExecutePlayerFlee(CombatSession session, Player player, List<CombatEvent> events)
    {
        const float BaseFleeChance = 0.60f;
        if (_rng.NextDouble() < BaseFleeChance)
        {
            session.Phase      = CombatPhase.Victory;
            session.PlayerFled = true;
            string msg = "You seize an opening and sprint away. You escape.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 300 });
            return true;
        }
        string failMsg = "You attempt to flee but can't find an opening. The enemies press in.";
        session.Log.Add(failMsg);
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = failMsg, DelayMs = 300 });
        return false;
    }

    // ── Enemy action ─────────────────────────────────────────────────────────

    private void ExecuteEnemyAction(CombatSession session, Player player, Enemy enemy, EnemyDefinition? def, List<CombatEvent> events)
    {
        var actionType = PickWeightedAction(enemy.ActionTable);

        switch (actionType)
        {
            case EnemyActionType.Attack:
                ExecuteEnemyAttack(session, enemy, def, isHeavy: false, events);
                break;
            case EnemyActionType.HeavyAttack:
                ExecuteEnemyAttack(session, enemy, def, isHeavy: true, events);
                break;
            case EnemyActionType.Defend:
                ExecuteEnemyDefend(session, enemy, def, events);
                break;
            case EnemyActionType.Buff:
                ExecuteEnemyBuff(session, enemy, def, events);
                break;
            case EnemyActionType.Regenerate:
                ExecuteEnemyRegen(session, enemy, def, events);
                break;
        }
    }

    private void ExecuteEnemyAttack(CombatSession session, Enemy enemy, EnemyDefinition? def, bool isHeavy, List<CombatEvent> events)
    {
        int effAtk = EffectiveEnemyAttack(enemy);
        int effDef = EffectivePlayerDefense(session);
        double variance = 0.85 + _rng.NextDouble() * 0.30;
        double mult = isHeavy ? 1.5 : 1.0;

        float extraMult = 1.0f + enemy.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.DamageMultiplier)
            .Sum(m => m.Value);
        enemy.ActiveModifiers.RemoveAll(m => m.Stat == ModifierStat.DamageMultiplier);

        int damage = (int)Math.Max(1, (effAtk - effDef) * mult * variance * extraMult);
        if (session.PlayerDefending) damage = (int)Math.Max(1, damage * 0.5);

        // Dodge roll
        if (session.PlayerDodging)
        {
            float dodge = EffectivePlayerDodgeChance(session);
            double roll = _rng.NextDouble();
            if (roll < dodge)
            {
                string dodgeMsg = "You sidestep the blow completely!";
                session.Log.Add(dodgeMsg);
                events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = dodgeMsg, DelayMs = 300 });
                return;
            }
            if (roll < dodge * 2)
            {
                damage = (int)Math.Max(1, damage * 0.5);
                string partialMsg = "You partially deflect the attack...";
                session.Log.Add(partialMsg);
                events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = partialMsg, DelayMs = 200 });
            }
            else
            {
                string failMsg = "Your dodge fails.";
                session.Log.Add(failMsg);
                events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = failMsg, DelayMs = 150 });
            }
        }

        session.PlayerHp = Math.Max(0, session.PlayerHp - damage);

        List<string>? pool = isHeavy ? def?.HeavyAttackTexts : def?.AttackTexts;
        string flavorText = pool?.Count > 0
            ? PickRandom(pool)
            : $"The {enemy.Name} attacks you.";
        string log = $"{flavorText} [{damage} damage, {session.PlayerHp}/{session.PlayerMaxHp} HP]";
        session.Log.Add(log);

        // Announce the attack with a pause, then shake + HP drop
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = log, DelayMs = 380 });
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.ShakePlayer,
            PlayerHp = session.PlayerHp,
            DelayMs = 350,
        });
    }

    private void ExecuteEnemyDefend(CombatSession session, Enemy enemy, EnemyDefinition? def, List<CombatEvent> events)
    {
        enemy.IsDefending = true;

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
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = text, DelayMs = 300 });
    }

    private void ExecuteEnemyBuff(CombatSession session, Enemy enemy, EnemyDefinition? def, List<CombatEvent> events)
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
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = text, DelayMs = 300 });
    }

    private void ExecuteEnemyRegen(CombatSession session, Enemy enemy, EnemyDefinition? def, List<CombatEvent> events)
    {
        int amount = def?.RegenerateAmount ?? 5;
        int before = enemy.CurrentHp;
        enemy.CurrentHp = Math.Min(enemy.MaxHp, enemy.CurrentHp + amount);
        int healed = enemy.CurrentHp - before;

        string text = def?.RegenerateTexts.Count > 0
            ? PickRandom(def.RegenerateTexts)
            : $"The {enemy.Name} regenerates {healed} HP.";
        session.Log.Add(text);
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.EnemyUpdate,
            EnemyInstanceId = enemy.InstanceId,
            Log = text,
            EnemyHp = enemy.CurrentHp,
            DelayMs = 300,
        });
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
