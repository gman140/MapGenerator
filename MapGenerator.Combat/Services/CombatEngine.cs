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
    private readonly IEquipmentDefinitionProvider _equipmentProvider;
    private readonly IConsumableDefinitionProvider _consumableProvider;
    private readonly ISpellDefinitionProvider _spellProvider;
    private readonly ICompanionDefinitionProvider _companionProvider;
    private readonly ICompanionMoveProvider _companionMoveProvider;
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
        [CombatActionType.Spell]       = 0,  // mana cost handled separately
        [CombatActionType.Refocus]     = 0,
    };

    public CombatEngine(
        IEnemyDefinitionProvider enemyProvider,
        IEquipmentDefinitionProvider equipmentProvider,
        IConsumableDefinitionProvider consumableProvider,
        ISpellDefinitionProvider spellProvider,
        ICompanionDefinitionProvider companionProvider,
        ICompanionMoveProvider companionMoveProvider,
        EnemySpawner spawner)
    {
        _enemyProvider      = enemyProvider;
        _equipmentProvider  = equipmentProvider;
        _consumableProvider = consumableProvider;
        _spellProvider      = spellProvider;
        _companionProvider  = companionProvider;
        _companionMoveProvider = companionMoveProvider;
        _spawner            = spawner;
    }

    // ── StartCombat ──────────────────────────────────────────────────────────

    public CombatSession StartCombat(CombatStartContext context)
    {
        var player  = context.Player;
        var enemies = _spawner.SpawnEnemies(context, _rng);

        var session = new CombatSession
        {
            Id                    = Guid.NewGuid().ToString("N"),
            PlayerId              = player.Id,
            PlayerHp              = player.MaxHp,
            PlayerMaxHp           = player.MaxHp,
            PlayerStamina         = 4,
            PlayerMaxStamina      = player.MaxStamina,
            PlayerBaseAttack      = player.BaseAttack,
            PlayerBaseDefense     = player.BaseDefense,
            PlayerBaseResistance  = player.BaseResistance,
            PlayerBaseDodgeChance = player.BaseDodgeChance,
            PlayerMana            = 2,
            PlayerMaxMana         = player.MaxMana,
            PlayerBaseMagic       = player.BaseMagic,
            Enemies               = enemies,
            Phase                 = CombatPhase.PlayerTurn,
            ContextLabel          = BuildContextLabel(context),
        };

        ApplyEquipmentModifiers(session, player);

        if (player.EquippedWeaponId != null)
        {
            var weaponDef = _equipmentProvider.GetById(player.EquippedWeaponId);
            if (weaponDef != null)
                session.PlayerWeaponDamageType = weaponDef.WeaponDamageType;
        }

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
                events.Add(new CombatEvent { Kind = CombatEventKind.PlayerBounce });
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
                events.Add(new CombatEvent { Kind = CombatEventKind.PlayerBounce });
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
            case CombatActionType.Spell:
                ExecutePlayerSpell(session, action.SpellId, action.TargetEnemyId, events);
                break;
            case CombatActionType.Refocus:
                ExecuteRefocus(session, events);
                break;
        }

        // Check victory after player action
        if (IsAllDeadOrFled(session))
        {
            session.Phase = CombatPhase.Victory;
            string victoryMsg = "The battle is over. You stand amid the quiet.";
            session.Log.Add(victoryMsg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = victoryMsg, DelayMs = 300 });
            return new CombatTurnResult { Session = session, Events = events };
        }

        // Companion turn (invulnerable; acts every turn if present)
        if (session.CompanionPresent && session.CompanionMoveIds.Count > 0)
        {
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, DelayMs = 200 });
            ExecuteCompanionAction(session, events);

            if (IsAllDeadOrFled(session))
            {
                session.Phase = CombatPhase.Victory;
                string cVictory = "The last enemy falls. You and your companion stand victorious.";
                session.Log.Add(cVictory);
                events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = cVictory, DelayMs = 300 });
                return new CombatTurnResult { Session = session, Events = events };
            }
        }

        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, DelayMs = 350 });

        foreach (var enemy in session.Enemies.Where(e => e.CurrentHp > 0 && !e.HasFled).ToList())
        {
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
                events.Add(new CombatEvent
                {
                    Kind = CombatEventKind.EnemyFled,
                    EnemyInstanceId = enemy.InstanceId,
                    Log = fleeText,
                });
                continue;
            }

            ExecuteEnemyAction(session, player, enemy, def, events);
        }

        if (IsAllDeadOrFled(session))
        {
            session.Phase = CombatPhase.Victory;
            string allFledMsg = "The last of them disappears into the dark. The fight is over.";
            session.Log.Add(allFledMsg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = allFledMsg, DelayMs = 300 });
            return new CombatTurnResult { Session = session, Events = events };
        }

        // Check defeat
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

        // Recover stamina (base 2, +2 if defended, plus any StaminaRegen modifiers)
        int staminaRegenBonus = (int)session.ActiveModifiers.Where(m => m.Stat == ModifierStat.StaminaRegen).Sum(m => m.Value);
        int gain = 2 + (defendedThisTurn ? 2 : 0) + staminaRegenBonus;
        session.PlayerStamina = Math.Min(session.PlayerMaxStamina, session.PlayerStamina + gain);

        // Recover mana (+1/turn, plus any ManaRegen modifiers)
        int manaRegenBonus = (int)session.ActiveModifiers.Where(m => m.Stat == ModifierStat.ManaRegen).Sum(m => m.Value);
        session.PlayerMana = Math.Min(session.PlayerMaxMana, session.PlayerMana + 1 + manaRegenBonus);

        // Passive HP regen from hat/equipment
        int hpRegen = (int)session.ActiveModifiers.Where(m => m.Stat == ModifierStat.HpRegen).Sum(m => m.Value);
        if (hpRegen > 0 && session.PlayerHp > 0 && session.PlayerHp < session.PlayerMaxHp)
            session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + hpRegen);

        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.PlayerUpdate,
            PlayerStamina = session.PlayerStamina,
            PlayerMana = session.PlayerMana,
            PlayerHp = session.PlayerHp,
            DelayMs = 150,
        });

        // Tick modifiers (DoT modifiers deal damage here)
        TickModifiers(session, events);

        // Check defeat from DoT
        if (session.PlayerHp <= 0)
        {
            session.PlayerHp = 0;
            session.Phase    = CombatPhase.Defeat;
            string dotDefeat = "The affliction consumes you. You fall.";
            session.Log.Add(dotDefeat);
            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.PlayerUpdate,
                Log = dotDefeat,
                PlayerHp = 0,
                DelayMs = 400,
            });
            return new CombatTurnResult { Session = session, Events = events };
        }

        // Clear turn flags
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

        int totalXp = 0;
        foreach (var enemy in session.Enemies.Where(e => e.CurrentHp <= 0))
        {
            defeated[enemy.DefinitionId] = defeated.GetValueOrDefault(enemy.DefinitionId) + 1;

            var def = _enemyProvider.GetById(enemy.DefinitionId);
            if (def == null) continue;
            totalXp += def.BaseXp;
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
        // Fled enemies still drop loot but give no XP
        foreach (var enemy in session.Enemies.Where(e => e.HasFled && e.CurrentHp > 0))
        {
            defeated[enemy.DefinitionId] = defeated.GetValueOrDefault(enemy.DefinitionId) + 1;
            var def = _enemyProvider.GetById(enemy.DefinitionId);
            if (def == null) continue;
            foreach (var entry in def.LootTable)
            {
                if (_rng.NextDouble() < entry.Chance)
                {
                    int qty = entry.MinQty == entry.MaxQty ? entry.MinQty : _rng.Next(entry.MinQty, entry.MaxQty + 1);
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
            Victory          = session.Phase == CombatPhase.Victory,
            Fled             = fled,
            PlayerDied       = playerDied,
            HpRemaining      = session.PlayerHp,
            StaminaRemaining = session.PlayerStamina,
            ManaRemaining    = session.PlayerMana,
            XpGained         = playerDied ? 0 : totalXp,
            LootGained       = loot,
            EnemiesDefeated  = defeated,
            SummaryMessage   = summary,
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
        int rawDamage = (int)Math.Max(1, (effAtk - effDef) * mult * variance);

        var def = _enemyProvider.GetById(target.DefinitionId);
        var (typeModifier, typeLog) = GetTypeEffectiveness(session.PlayerWeaponDamageType, def);
        int damage = (int)Math.Max(1, rawDamage * typeModifier);

        target.CurrentHp = Math.Max(0, target.CurrentHp - damage);

        string verb = isHeavy ? "drive a heavy blow into" : "strike";
        bool killed = target.CurrentHp == 0;
        string fell = killed ? $" The {target.Name} falls." : string.Empty;
        string typeNote = typeLog != null ? $" {typeLog}" : string.Empty;
        string log = $"You {verb} the {target.Name} for {damage} damage.{typeNote}{fell}";
        session.Log.Add(log);

        events.Add(new CombatEvent { Kind = CombatEventKind.PlayerAttack });
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

        var consumable = _consumableProvider.GetById(itemId);
        if (consumable != null && consumable.UsableInCombat)
        {
            ConsumeItem(player, itemId);
            var evt = new CombatEvent { Kind = CombatEventKind.PlayerUpdate, DelayMs = 300 };
            var parts = new List<string>();

            if (consumable.CombatHpRestore > 0)
            {
                float healMult = 1f + session.ActiveModifiers.Where(m => m.Stat == ModifierStat.HealBonus).Sum(m => m.Value);
                int rawHeal = (int)(consumable.CombatHpRestore * healMult);
                int before = session.PlayerHp;
                session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + rawHeal);
                int healed = session.PlayerHp - before;
                evt.PlayerHp = session.PlayerHp;
                parts.Add($"restored {healed} HP");
            }
            if (consumable.CombatStaminaRestore > 0)
            {
                session.PlayerStamina = Math.Min(session.PlayerMaxStamina, session.PlayerStamina + consumable.CombatStaminaRestore);
                evt.PlayerStamina = session.PlayerStamina;
                parts.Add($"recovered {consumable.CombatStaminaRestore} stamina");
            }
            if (consumable.CombatBuffStat.HasValue && consumable.CombatBuffTurns > 0)
            {
                session.ActiveModifiers.Add(new CombatModifier
                {
                    Id             = $"Item:{itemId}",
                    Stat           = consumable.CombatBuffStat.Value,
                    Value          = consumable.CombatBuffValue,
                    TurnsRemaining = consumable.CombatBuffTurns,
                    Source         = $"Item:{itemId}",
                });
                parts.Add(consumable.CombatBuffLabel ?? $"+{consumable.CombatBuffValue} for {consumable.CombatBuffTurns}t");
            }
            if (consumable.ClearsStatuses?.Length > 0)
            {
                int cleared = session.ActiveModifiers.RemoveAll(m => consumable.ClearsStatuses.Contains(m.Stat));
                if (cleared > 0)
                    parts.Add($"cured {cleared} affliction(s)");
            }

            string msg = $"You use {consumable.Name}. " + (parts.Count > 0 ? string.Join(", ", parts) + "." : "Nothing happened.");
            session.Log.Add(msg);
            evt.Log = msg;
            events.Add(evt);
            return;
        }

        if (consumable != null)
        {
            int heal = Math.Max(1, (int)Math.Ceiling(consumable.SatietyRestore / 2.0));
            int before = session.PlayerHp;
            session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + heal);
            int healed = session.PlayerHp - before;
            player.Satiety = Math.Min(100, player.Satiety + consumable.SatietyRestore);
            ConsumeItem(player, itemId);

            string msg = $"You eat the {consumable.Name}. You recover {healed} HP. ({session.PlayerHp}/{session.PlayerMaxHp})";
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

    // ── Player spell ─────────────────────────────────────────────────────────

    private void ExecutePlayerSpell(CombatSession session, string? spellId, string? targetId, List<CombatEvent> events)
    {
        if (string.IsNullOrEmpty(spellId))
        {
            string msg = "No spell selected.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return;
        }

        var spell = _spellProvider.GetById(spellId);
        if (spell == null)
        {
            string msg = "Unknown spell.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return;
        }

        if (session.PlayerMana < spell.ManaCost)
        {
            string msg = $"Not enough mana to cast {spell.Name}. ({session.PlayerMana}/{spell.ManaCost} mana)";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return;
        }

        session.PlayerMana -= spell.ManaCost;
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.PlayerManaUpdate,
            PlayerMana = session.PlayerMana,
            DelayMs = 80,
        });

        // Self-heal spell
        if (spell.TargetType == TargetType.Self)
        {
            float healMult = 1f + session.ActiveModifiers.Where(m => m.Stat == ModifierStat.HealBonus).Sum(m => m.Value);
            int heal = (int)((spell.HealAmount + EffectivePlayerMagic(session) / 2) * healMult);
            int before = session.PlayerHp;
            session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + heal);
            int healed = session.PlayerHp - before;
            string msg = $"You cast {spell.Name}. You recover {healed} HP. ({session.PlayerHp}/{session.PlayerMaxHp})";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.PlayerBounce });
            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.PlayerUpdate,
                Log = msg,
                PlayerHp = session.PlayerHp,
                DelayMs = 350,
            });
            return;
        }

        // Damage spells
        var targets = spell.TargetType == TargetType.AllEnemies
            ? session.Enemies.Where(e => e.CurrentHp > 0 && !e.HasFled).ToList()
            : [GetTargetEnemy(session, targetId)!];
        targets = targets.Where(t => t != null).ToList();

        if (targets.Count == 0)
        {
            string msg = "No valid target.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 200 });
            return;
        }

        string castMsg = spell.TargetType == TargetType.AllEnemies
            ? $"You cast {spell.Name}, striking all enemies!"
            : $"You cast {spell.Name}!";
        session.Log.Add(castMsg);
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = castMsg, DelayMs = 250 });
        events.Add(new CombatEvent { Kind = CombatEventKind.PlayerAttack });

        foreach (var target in targets)
        {
            var enemyDef = _enemyProvider.GetById(target.DefinitionId);
            double variance = 0.85 + _rng.NextDouble() * 0.30;
            int rawDamage = (int)Math.Max(1, EffectivePlayerMagic(session) * spell.Power * variance);

            int damage = spell.SecondaryDamageType.HasValue
                ? ApplySplitDamage(rawDamage, spell.DamageType!.Value, spell.SecondaryDamageType, spell.SecondaryRatio, enemyDef)
                : spell.DamageType.HasValue
                    ? (int)Math.Max(1, rawDamage * GetTypeEffectiveness(spell.DamageType.Value, enemyDef).modifier)
                    : rawDamage;

            var (_, typeLog) = spell.DamageType.HasValue
                ? GetTypeEffectiveness(spell.DamageType.Value, enemyDef)
                : (1f, null);

            target.CurrentHp = Math.Max(0, target.CurrentHp - damage);
            bool killed = target.CurrentHp == 0;
            string typeNote = typeLog != null ? $" {typeLog}" : string.Empty;
            string fell = killed ? $" The {target.Name} falls." : string.Empty;
            string log = $"It strikes the {target.Name} for {damage} damage.{typeNote}{fell}";
            session.Log.Add(log);

            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.ShakeEnemy,
                EnemyInstanceId = target.InstanceId,
                Log = log,
                EnemyHp = target.CurrentHp,
                DelayMs = 320,
            });

            if (killed)
                events.Add(new CombatEvent { Kind = CombatEventKind.EnemyDied, EnemyInstanceId = target.InstanceId });

            if (!killed && spell.OnHit != null && _rng.NextDouble() < spell.OnHit.Chance)
                ApplyOrRefreshEnemyStatus(target, spell.OnHit, session, events);
        }
    }

    // ── Refocus ───────────────────────────────────────────────────────────────

    private static void ExecuteRefocus(CombatSession session, List<CombatEvent> events)
    {
        int gained = Math.Min(3, session.PlayerMaxMana - session.PlayerMana);
        session.PlayerMana += gained;
        string msg = $"You take a steadying breath and focus your energy. +{gained} mana. ({session.PlayerMana}/{session.PlayerMaxMana})";
        session.Log.Add(msg);
        events.Add(new CombatEvent { Kind = CombatEventKind.PlayerBounce });
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.PlayerManaUpdate,
            Log = msg,
            PlayerMana = session.PlayerMana,
            DelayMs = 300,
        });
    }

    // ── Companion action ──────────────────────────────────────────────────────

    private void ExecuteCompanionAction(CombatSession session, List<CombatEvent> events)
    {
        if (session.CompanionDefinitionId == null || session.CompanionMoveIds.Count == 0) return;

        string moveId = session.CompanionMoveIds[_rng.Next(session.CompanionMoveIds.Count)];
        var move = _companionMoveProvider.GetById(moveId);
        if (move == null) return;

        string companionName = string.IsNullOrEmpty(session.CompanionName) ? "Your companion" : session.CompanionName;

        switch (move.Kind)
        {
            case CompanionMoveKind.PlayerBuff:
                ExecuteCompanionBuff(session, move, companionName, events);
                return;
            case CompanionMoveKind.EnemyDebuff:
                ExecuteCompanionDebuff(session, move, companionName, events);
                return;
        }

        // Attack
        string header = move.HitsAll
            ? $"{companionName} uses {move.Name} on all enemies!"
            : $"{companionName} uses {move.Name}!";
        session.Log.Add(header);
        events.Add(new CombatEvent { Kind = CombatEventKind.CompanionAction, Log = header, DelayMs = 250 });

        var targets = move.HitsAll
            ? session.Enemies.Where(e => e.CurrentHp > 0 && !e.HasFled).ToList()
            : [GetTargetEnemy(session, null)!];
        targets = targets.Where(t => t != null).ToList();

        foreach (var target in targets)
        {
            var def = _enemyProvider.GetById(target.DefinitionId);
            double variance = 0.85 + _rng.NextDouble() * 0.30;
            int rawDamage = (int)Math.Max(1, session.CompanionBaseAttack * move.Power * variance);
            var (typeMod, typeLog) = GetTypeEffectiveness(move.DamageType, def);
            int damage = (int)Math.Max(1, rawDamage * typeMod);

            target.CurrentHp = Math.Max(0, target.CurrentHp - damage);
            bool killed = target.CurrentHp == 0;
            string typeNote = typeLog != null ? $" {typeLog}" : string.Empty;
            string fell = killed ? $" The {target.Name} falls." : string.Empty;
            string hitLog = $"It strikes the {target.Name} for {damage} damage.{typeNote}{fell}";
            session.Log.Add(hitLog);

            events.Add(new CombatEvent
            {
                Kind = CombatEventKind.ShakeEnemy,
                EnemyInstanceId = target.InstanceId,
                Log = hitLog,
                EnemyHp = target.CurrentHp,
                DelayMs = 320,
            });

            if (killed)
                events.Add(new CombatEvent { Kind = CombatEventKind.EnemyDied, EnemyInstanceId = target.InstanceId });
        }
    }

    private void ExecuteCompanionBuff(CombatSession session, CompanionMove move, string companionName, List<CombatEvent> events)
    {
        events.Add(new CombatEvent { Kind = CombatEventKind.CompanionBounce });

        if (move.EffectStat == null)
        {
            // Heal HP
            int before = session.PlayerHp;
            session.PlayerHp = Math.Min(session.PlayerMaxHp, session.PlayerHp + (int)move.EffectValue);
            int healed = session.PlayerHp - before;
            string msg = $"{companionName} uses {move.Name}! You recover {healed} HP. ({session.PlayerHp}/{session.PlayerMaxHp})";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.PlayerUpdate, Log = msg, PlayerHp = session.PlayerHp, DelayMs = 400 });
        }
        else
        {
            session.ActiveModifiers.Add(new CombatModifier
            {
                Id             = $"Companion:{move.Id}:{session.TurnNumber}",
                Stat           = move.EffectStat.Value,
                Value          = move.EffectValue,
                TurnsRemaining = move.EffectTurns,
                Source         = "Companion",
            });
            string statName = move.EffectStat.Value switch
            {
                ModifierStat.Attack      => "attack",
                ModifierStat.Defense     => "defense",
                ModifierStat.DodgeChance => "dodge",
                _                        => move.EffectStat.Value.ToString().ToLower(),
            };
            string msg = $"{companionName} uses {move.Name}! Your {statName} increases for {move.EffectTurns} turns.";
            session.Log.Add(msg);
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = msg, DelayMs = 400 });
        }
    }

    private void ExecuteCompanionDebuff(CombatSession session, CompanionMove move, string companionName, List<CombatEvent> events)
    {
        var target = GetTargetEnemy(session, null);
        if (target == null) return;

        events.Add(new CombatEvent { Kind = CombatEventKind.CompanionBounce });

        target.ActiveModifiers.Add(new CombatModifier
        {
            Id             = $"Companion:Debuff:{move.Id}:{session.TurnNumber}",
            Stat           = move.EffectStat!.Value,
            Value          = move.EffectValue,
            TurnsRemaining = move.EffectTurns,
            Source         = "Companion",
        });
        string statName = move.EffectStat.Value switch
        {
            ModifierStat.Attack  => "attack",
            ModifierStat.Defense => "defense",
            _                    => move.EffectStat.Value.ToString().ToLower(),
        };
        string msg = $"{companionName} uses {move.Name}! The {target.Name}'s {statName} is reduced for {move.EffectTurns} turns.";
        session.Log.Add(msg);
        events.Add(new CombatEvent
        {
            Kind            = CombatEventKind.EnemyUpdate,
            EnemyInstanceId = target.InstanceId,
            Log             = msg,
            EnemyHp         = target.CurrentHp,
            DelayMs         = 400,
        });
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
            case EnemyActionType.DoubleStrike:
                ExecuteEnemyDoubleStrike(session, enemy, def, events);
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

    private void ExecuteEnemyAttack(CombatSession session, Enemy enemy, EnemyDefinition? def, bool isHeavy, List<CombatEvent> events, double extraMult = 1.0)
    {
        int effAtk = EffectiveEnemyAttack(enemy);
        DamageType atkType = def?.AttackDamageType ?? DamageType.Bludgeoning;
        int effDef = EnergyDamageTypes.Contains(atkType)
            ? EffectivePlayerResistance(session)
            : EffectivePlayerDefense(session);
        double variance = 0.85 + _rng.NextDouble() * 0.30;
        double mult = isHeavy ? 1.5 : 1.0;

        float buffMult = 1.0f + enemy.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.DamageMultiplier)
            .Sum(m => m.Value);
        enemy.ActiveModifiers.RemoveAll(m => m.Stat == ModifierStat.DamageMultiplier);

        int damage = (int)Math.Max(1, (effAtk - effDef) * mult * extraMult * variance * buffMult);
        if (session.PlayerDefending) damage = (int)Math.Max(1, damage * 0.5);

        // Dodge roll — skipped for Spectral enemies
        if (session.PlayerDodging && !enemy.ActionsCantBeDodged)
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

        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = log, DelayMs = 380 });
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.ShakePlayer,
            EnemyInstanceId = enemy.InstanceId,
            PlayerHp = session.PlayerHp,
            DelayMs = 350,
        });

        // On-hit status proc
        if (enemy.OnHitEffect != null && _rng.NextDouble() < enemy.OnHitEffect.Chance)
            ApplyOrRefreshStatus(session, enemy.OnHitEffect, events);
    }

    private void ExecuteEnemyDoubleStrike(CombatSession session, Enemy enemy, EnemyDefinition? def, List<CombatEvent> events)
    {
        string intro = $"The {enemy.Name} strikes twice in rapid succession!";
        session.Log.Add(intro);
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = intro, DelayMs = 300 });

        ExecuteEnemyAttack(session, enemy, def, isHeavy: false, events, extraMult: 0.65);
        if (session.PlayerHp > 0)
        {
            events.Add(new CombatEvent { Kind = CombatEventKind.Pause, DelayMs = 200 });
            ExecuteEnemyAttack(session, enemy, def, isHeavy: false, events, extraMult: 0.65);
        }
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
        events.Add(new CombatEvent { Kind = CombatEventKind.EnemyBounce, EnemyInstanceId = enemy.InstanceId });
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = text, DelayMs = 450 });
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
        events.Add(new CombatEvent { Kind = CombatEventKind.EnemyBounce, EnemyInstanceId = enemy.InstanceId });
        events.Add(new CombatEvent { Kind = CombatEventKind.Pause, Log = text, DelayMs = 450 });
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
        events.Add(new CombatEvent { Kind = CombatEventKind.EnemyBounce, EnemyInstanceId = enemy.InstanceId });
        events.Add(new CombatEvent
        {
            Kind = CombatEventKind.EnemyUpdate,
            EnemyInstanceId = enemy.InstanceId,
            Log = text,
            EnemyHp = enemy.CurrentHp,
            DelayMs = 450,
        });
    }

    // ── Player status effects ────────────────────────────────────────────────

    private static void ApplyOrRefreshStatus(CombatSession session, OnHitEffect effect, List<CombatEvent> events)
    {
        var existing = session.ActiveModifiers.FirstOrDefault(m => m.Stat == effect.StatusType);
        if (existing != null)
        {
            existing.TurnsRemaining = Math.Max(existing.TurnsRemaining ?? 0, effect.Turns);
            return;
        }

        session.ActiveModifiers.Add(new CombatModifier
        {
            Id             = $"Status:{effect.StatusType}",
            Stat           = effect.StatusType,
            Value          = effect.Value,
            TurnsRemaining = effect.Turns,
            Source         = $"Status:{effect.StatusType}",
        });

        string label = effect.StatusType switch
        {
            ModifierStat.Disease      => "Disease",
            ModifierStat.Burn         => "Burn",
            ModifierStat.Venom        => "Venom",
            ModifierStat.StaminaDrain => "a Curse",
            _                         => effect.StatusType.ToString(),
        };
        string msg = $"You are afflicted with {label}!";
        session.Log.Add(msg);
        events.Add(new CombatEvent { Kind = CombatEventKind.StatusApplied, Log = msg, DelayMs = 250 });
    }

    private static void ApplyOrRefreshEnemyStatus(Enemy target, OnHitEffect effect, CombatSession session, List<CombatEvent> events)
    {
        var existing = target.ActiveModifiers.FirstOrDefault(m => m.Stat == effect.StatusType);
        if (existing != null)
        {
            existing.TurnsRemaining = Math.Max(existing.TurnsRemaining ?? 0, effect.Turns);
            return;
        }

        target.ActiveModifiers.Add(new CombatModifier
        {
            Id             = $"Status:{effect.StatusType}",
            Stat           = effect.StatusType,
            Value          = effect.Value,
            TurnsRemaining = effect.Turns,
            Source         = $"Status:{effect.StatusType}",
        });

        string label = effect.StatusType switch
        {
            ModifierStat.Burn         => "Burn",
            ModifierStat.Venom        => "Venom",
            _                         => effect.StatusType.ToString(),
        };
        string msg = $"The {target.Name} is afflicted with {label}!";
        session.Log.Add(msg);
        events.Add(new CombatEvent { Kind = CombatEventKind.StatusApplied, Log = msg, DelayMs = 250 });
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
        var def = _equipmentProvider.GetById(itemId);
        if (def == null) return;

        foreach (var affix in def.Affixes)
            session.ActiveModifiers.Add(new CombatModifier
            {
                Id     = $"Equip:{affix.Stat}:{itemId}",
                Stat   = affix.Stat,
                Value  = affix.Value,
                Source = $"Equipment:{itemId}",
            });
    }

    // ── Effective stat helpers ────────────────────────────────────────────────

    private static readonly HashSet<DamageType> EnergyDamageTypes =
    [
        DamageType.Fire, DamageType.Frost, DamageType.Storm, DamageType.Nature, DamageType.Dark,
    ];

    private static int EffectivePlayerAttack(CombatSession s) =>
        (int)(s.PlayerBaseAttack + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value));

    private static int EffectivePlayerDefense(CombatSession s) =>
        (int)(s.PlayerBaseDefense + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Defense).Sum(m => m.Value));

    private static int EffectivePlayerResistance(CombatSession s) =>
        (int)(s.PlayerBaseResistance + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Resistance).Sum(m => m.Value));

    private static float EffectivePlayerDodgeChance(CombatSession s) =>
        s.PlayerBaseDodgeChance + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.DodgeChance).Sum(m => m.Value);

    private static int EffectivePlayerMagic(CombatSession s) =>
        (int)(s.PlayerBaseMagic + s.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Magic).Sum(m => m.Value));

    private static int EffectiveEnemyAttack(Enemy e) =>
        (int)(e.Attack + e.ActiveModifiers
            .Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value));

    private static int EffectiveEnemyDefense(Enemy e)
    {
        int def = e.Defense;
        if (e.IsDefending) def = (int)(def * 2.5);
        return def;
    }

    // ── Modifier tick ─────────────────────────────────────────────────────────

    private static void TickModifiers(CombatSession session, List<CombatEvent> events)
    {
        // Apply DoT damage before decrementing so the turn they expire still deals damage
        for (int i = session.ActiveModifiers.Count - 1; i >= 0; i--)
        {
            var mod = session.ActiveModifiers[i];
            if (mod.Stat is ModifierStat.Disease or ModifierStat.Burn or ModifierStat.Venom)
            {
                int dmg = (int)Math.Max(1, session.PlayerMaxHp * mod.Value);
                session.PlayerHp = Math.Max(0, session.PlayerHp - dmg);
                string msg = $"[{mod.Stat}] deals {dmg} damage. ({session.PlayerHp}/{session.PlayerMaxHp} HP)";
                session.Log.Add(msg);
                events.Add(new CombatEvent { Kind = CombatEventKind.StatusTick, Log = msg, PlayerHp = session.PlayerHp, DelayMs = 280 });
            }
            else if (mod.Stat == ModifierStat.StaminaDrain)
            {
                int drained = (int)Math.Min(session.PlayerStamina, mod.Value);
                session.PlayerStamina = Math.Max(0, session.PlayerStamina - drained);
                string msg = $"[Curse] drains {drained} stamina. ({session.PlayerStamina}/{session.PlayerMaxStamina})";
                session.Log.Add(msg);
                events.Add(new CombatEvent { Kind = CombatEventKind.StatusTick, Log = msg, PlayerStamina = session.PlayerStamina, DelayMs = 280 });
            }
        }

        // Apply DoT damage to enemies
        foreach (var enemy in session.Enemies.Where(e => e.CurrentHp > 0 && !e.HasFled))
        {
            for (int i = enemy.ActiveModifiers.Count - 1; i >= 0; i--)
            {
                var mod = enemy.ActiveModifiers[i];
                if (mod.Stat is ModifierStat.Burn or ModifierStat.Venom)
                {
                    int dmg = (int)Math.Max(1, mod.Value);
                    enemy.CurrentHp = Math.Max(0, enemy.CurrentHp - dmg);
                    string msg = $"[{mod.Stat}] burns the {enemy.Name} for {dmg} damage. ({enemy.CurrentHp} HP)";
                    session.Log.Add(msg);
                    events.Add(new CombatEvent
                    {
                        Kind = CombatEventKind.EnemyUpdate,
                        EnemyInstanceId = enemy.InstanceId,
                        EnemyHp = enemy.CurrentHp,
                        Log = msg,
                        DelayMs = 280,
                    });
                    if (enemy.CurrentHp == 0)
                        events.Add(new CombatEvent { Kind = CombatEventKind.EnemyDied, EnemyInstanceId = enemy.InstanceId });
                }
            }
        }

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

    // ── Type effectiveness ────────────────────────────────────────────────────

    private static (float modifier, string? log) GetTypeEffectiveness(DamageType attackType, EnemyDefinition? def)
    {
        if (def == null) return (1f, null);

        if (def.Weaknesses.Contains(attackType))
        {
            string text = def.WeaknessText.TryGetValue(attackType, out string? custom) && custom != null
                ? custom
                : "Super effective!";
            return (1.5f, text);
        }

        if (def.Resistances.Contains(attackType))
        {
            string text = def.ResistanceText.TryGetValue(attackType, out string? custom) && custom != null
                ? custom
                : "Not very effective…";
            return (0.5f, text);
        }

        return (1f, null);
    }

    // For split-damage attacks (primary + secondary type, each portion scaled separately)
    private static int ApplySplitDamage(int total, DamageType primary, DamageType? secondary, float secondaryRatio, EnemyDefinition? def)
    {
        if (secondary == null || secondaryRatio <= 0f)
        {
            var (mod, _) = GetTypeEffectiveness(primary, def);
            return (int)Math.Max(1, total * mod);
        }

        float primaryRatio  = 1f - secondaryRatio;
        var (pMod, _) = GetTypeEffectiveness(primary, def);
        var (sMod, _) = GetTypeEffectiveness(secondary.Value, def);
        return (int)Math.Max(1, total * primaryRatio * pMod + total * secondaryRatio * sMod);
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
