using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Combat.Services;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Web.Services;

// ── Stats bundle passed in a challenge and to SimulateBattle ────────────────

public record CompanionBattleStats(
    int BaseAttack,
    int BaseVitality,
    int BaseDefense,
    int BaseSpeed,
    int BaseFocus,
    int BaseResist,
    CompanionTemperament Temperament,
    CompanionForm Form,
    IReadOnlyList<DamageType> ElementTypes,
    List<string> MoveIds);

// ── Challenge & result records ───────────────────────────────────────────────

public record CompanionBattleChallenge(
    string ChallengerId,
    string ChallengerName,
    string TargetId,
    string ChallengerCompanionName,
    string[] ChallengerCompanionSprite,
    CompanionBattleStats ChallengerStats,
    DateTimeOffset ExpiresAt);

public enum BattleEventKind { Intro, RoundStart, TurnBreak, Attack, Heal, Buff, Debuff, Dodge, Stun, Status, Rally, Outcome }

public record BattleEvent(
    string? ActorId,   // who is acting (null for round headers / outcome draws)
    string? TargetId,  // who is targeted (null for self-actions)
    BattleEventKind Kind,
    string Text,
    int ChallengerHp, int ChallengerMaxHp,
    int AccepterHp,   int AccepterMaxHp);

public record CompanionBattleResult(
    string ChallengerId,
    string ChallengerName,
    string ChallengerCompanionName,
    string[] ChallengerCompanionSprite,
    string AccepterId,
    string AccepterName,
    string AccepterCompanionName,
    string[] AccepterCompanionSprite,
    string? WinnerId,
    string? WinnerName,
    string? WinningCompanionName,
    List<string> Log,
    List<BattleEvent> Events);

// ── Service ──────────────────────────────────────────────────────────────────

public class CompanionBattleService
{
    private readonly ICompanionMoveProvider _moveProvider;
    private readonly Lock _lock = new();
    private readonly Dictionary<string, CompanionBattleChallenge> _challenges = []; // keyed by targetId
    private readonly Random _rng = new();

    public CompanionBattleService(ICompanionMoveProvider moveProvider) => _moveProvider = moveProvider;

    public (bool ok, string? error) TryCreateChallenge(CompanionBattleChallenge challenge)
    {
        lock (_lock)
        {
            PruneExpired();
            if (_challenges.TryGetValue(challenge.TargetId, out var existing))
            {
                return existing.ChallengerId == challenge.ChallengerId
                    ? (false, "You already challenged this player.")
                    : (false, "That player already has a pending challenge.");
            }
            _challenges[challenge.TargetId] = challenge;
        }
        return (true, null);
    }

    public CompanionBattleChallenge? GetPendingChallenge(string targetId)
    {
        lock (_lock)
        {
            if (!_challenges.TryGetValue(targetId, out var ch)) return null;
            if (DateTimeOffset.UtcNow > ch.ExpiresAt) { _challenges.Remove(targetId); return null; }
            return ch;
        }
    }

    public CompanionBattleChallenge? TakeChallenge(string targetId)
    {
        lock (_lock)
        {
            if (!_challenges.TryGetValue(targetId, out var ch)) return null;
            _challenges.Remove(targetId);
            return DateTimeOffset.UtcNow > ch.ExpiresAt ? null : ch;
        }
    }

    public CompanionBattleResult SimulateBattle(
        CompanionBattleChallenge challenge,
        string accepterId, string accepterName,
        string accepterCompanionName, string[] accepterSprite,
        CompanionBattleStats accepterStats)
    {
        var log    = new List<string>();
        var events = new List<BattleEvent>();

        var challenger = Build(challenge.ChallengerId, challenge.ChallengerName,
            challenge.ChallengerCompanionName, challenge.ChallengerStats);
        var accepter = Build(accepterId, accepterName, accepterCompanionName, accepterStats);

        void Emit(string? actorId, string? targetId, BattleEventKind kind, string text)
        {
            log.Add(text);
            events.Add(new BattleEvent(actorId, targetId, kind, text,
                challenger.Hp, challenger.MaxHp, accepter.Hp, accepter.MaxHp));
        }

        Emit(null, null, BattleEventKind.Intro,
            $"⚔ {challenger.PlayerName}'s {challenger.Name} vs {accepter.PlayerName}'s {accepter.Name}!");

        // Higher Speed goes first; challenger wins ties
        var c1 = challenger;
        var c2 = accepter;
        if (c2.BaseSpd > c1.BaseSpd) (c1, c2) = (c2, c1);

        for (int round = 1; round <= 30; round++)
        {
            Emit(null, null, BattleEventKind.RoundStart, $"── Round {round} ──");
            TakeTurn(c1, c2, Emit);
            if (c2.Hp <= 0) break;
            TickMods(c1); TickMods(c2);
            TickStatuses(c1, Emit); TickStatuses(c2, Emit);
            if (c2.Hp <= 0) break;
            events.Add(new BattleEvent(null, null, BattleEventKind.TurnBreak, "",
                challenger.Hp, challenger.MaxHp, accepter.Hp, accepter.MaxHp));
            TakeTurn(c2, c1, Emit);
            if (c1.Hp <= 0) break;
            TickMods(c1); TickMods(c2);
            TickStatuses(c1, Emit); TickStatuses(c2, Emit);
            if (c1.Hp <= 0) break;
        }

        string? winnerId = null, winnerName = null, winningCompanionName = null;
        if (challenger.Hp > 0 && accepter.Hp <= 0)
        {
            (winnerId, winnerName, winningCompanionName) = (challenger.OwnerId, challenger.PlayerName, challenger.Name);
            Emit(challenger.OwnerId, null, BattleEventKind.Outcome, $"🏆 {challenger.Name} wins for {challenger.PlayerName}!");
        }
        else if (accepter.Hp > 0 && challenger.Hp <= 0)
        {
            (winnerId, winnerName, winningCompanionName) = (accepter.OwnerId, accepter.PlayerName, accepter.Name);
            Emit(accepter.OwnerId, null, BattleEventKind.Outcome, $"🏆 {accepter.Name} wins for {accepter.PlayerName}!");
        }
        else
        {
            Emit(null, null, BattleEventKind.Outcome, "⚖ Both companions are exhausted. It's a draw!");
        }

        return new CompanionBattleResult(
            challenge.ChallengerId, challenge.ChallengerName,
            challenge.ChallengerCompanionName, challenge.ChallengerCompanionSprite,
            accepterId, accepterName, accepterCompanionName, accepterSprite,
            winnerId, winnerName, winningCompanionName, log, events);
    }

    // ── Private helpers ─────────────────────────────────────────────────────

    private PvpCompanion Build(string ownerId, string playerName, string name, CompanionBattleStats s)
    {
        var moves = s.MoveIds.Select(id => _moveProvider.GetById(id)).OfType<CompanionMove>().ToList();
        return new PvpCompanion(ownerId, playerName, name,
            s.BaseAttack, s.BaseVitality, s.BaseDefense, s.BaseSpeed, s.BaseFocus, s.BaseResist,
            s.Temperament, s.Form, s.ElementTypes, moves);
    }

    private void TakeTurn(PvpCompanion attacker, PvpCompanion defender,
        Action<string?, string?, BattleEventKind, string> emit)
    {
        if (attacker.Hp <= 0) return;

        // Stun/Paralysis skip chance from active statuses
        foreach (var (status, _) in attacker.ActiveStatuses)
        {
            var def = StatusRegistry.Get(status);
            if (def.SkipChance > 0 && _rng.NextDouble() < def.SkipChance)
            {
                emit(attacker.OwnerId, null, BattleEventKind.Stun,
                    $"  {attacker.Name} is {def.DisplayName.ToLower()} and can't move!");
                return;
            }
        }

        // DoT tick at turn start
        foreach (var (status, _) in attacker.ActiveStatuses.ToList())
        {
            var def = StatusRegistry.Get(status);
            if (def.DoTMultiplier <= 0) continue;
            int tickDmg = (int)Math.Max(1, attacker.MaxHp * def.DoTMultiplier);
            attacker.Hp = Math.Max(0, attacker.Hp - tickDmg);
            emit(null, attacker.OwnerId, BattleEventKind.Status,
                $"  {attacker.Name} takes {tickDmg} {def.DisplayName.ToLower()} damage! {attacker.Hp}/{attacker.MaxHp} HP");
            if (attacker.Hp <= 0) return;
        }

        var move = ChooseMove(attacker, defender);
        bool crit = CombatMath.RollCrit(attacker.EffectiveFocus, _rng);

        // Damage component
        if (move.HasDamage)
        {
            float eff = GetTypeMultiplier(move.DamageType, defender);
            float raw = attacker.EffectiveAttack * move.Power * (float)(0.85 + _rng.NextDouble() * 0.30);

            var outcome = CombatMath.RollHit(
                attacker.EffectiveSpeed, defender.EffectiveSpeed,
                move.Power, -attacker.MissChanceTotal, _rng);

            if (outcome == HitOutcome.Miss)
            {
                emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Dodge,
                    $"  {attacker.Name} → {move.Name} | {defender.Name} evades!");
            }
            else
            {
                float nearMult = outcome == HitOutcome.NearMiss ? 0.5f : 1f;
                float dmg = Math.Max(1f,
                    raw * nearMult * eff - Math.Max(0f, defender.EffectiveDefense) * 0.5f)
                    * (crit ? 1.5f : 1f);
                int damage = (int)Math.Round(dmg);

                defender.Hp = Math.Max(0, defender.Hp - damage);
                string critTag = crit ? " [CRIT]" : "";
                string effTag  = eff > 1f ? " [Super effective!]" : eff < 1f ? " [Resisted]" : "";
                string nearTag = outcome == HitOutcome.NearMiss ? " [Glancing]" : "";

                emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Attack,
                    $"  {attacker.Name} → {move.Name} | {damage} dmg{critTag}{nearTag}{effTag} | {defender.Name}: {defender.Hp}/{defender.MaxHp} HP");

                // Rally on low-HP crit
                if (crit && defender.Hp > 0 && defender.Hp < defender.MaxHp * 0.35f && _rng.NextDouble() < 0.20)
                {
                    attacker.Mods.Add(new Mod(ModifierStat.Attack, 3f, 2));
                    emit(attacker.OwnerId, null, BattleEventKind.Rally,
                        $"  {attacker.Name} rallies! (+3 ATK for 2 rounds)");
                }
            }
        }

        // Heal component
        if (move.HasHeal)
        {
            int heal = (int)Math.Round(move.HealAmount * (crit ? 1.5f : 1f));
            attacker.Hp = Math.Min(attacker.MaxHp, attacker.Hp + heal);
            emit(attacker.OwnerId, null, BattleEventKind.Heal,
                $"  {attacker.Name} → {move.Name} | +{heal} HP | {attacker.Hp}/{attacker.MaxHp} HP");
        }

        // Status effects component
        foreach (var effect in move.Effects)
        {
            if (_rng.NextDouble() >= effect.Chance) continue;

            if (effect.Target == MoveEffectTarget.Self)
            {
                ApplyOrRefreshStatus(attacker.ActiveStatuses, effect.Status, effect.Duration);
                var def = StatusRegistry.Get(effect.Status);
                // Also apply Mod-based stat changes for immediate buff display
                ApplyStatusAsMod(attacker.Mods, attacker, def, effect.Duration);
                emit(attacker.OwnerId, null, BattleEventKind.Buff,
                    $"  {attacker.Name} → {move.Name} | {def.DisplayName}! ({effect.Duration} turns)");
            }
            else
            {
                ApplyOrRefreshStatus(defender.ActiveStatuses, effect.Status, effect.Duration);
                var def = StatusRegistry.Get(effect.Status);
                emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Debuff,
                    $"  {attacker.Name} → {move.Name} | {defender.Name} is {def.DisplayName}! ({effect.Duration} turns)");
            }
        }

        attacker.TurnCount++;
    }

    private static void ApplyOrRefreshStatus(List<(CompanionStatus Status, int Turns)> list, CompanionStatus status, int duration)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Status == status)
            {
                list[i] = (status, Math.Max(list[i].Turns, duration));
                return;
            }
        }
        list.Add((status, duration));
    }

    private static void ApplyStatusAsMod(List<Mod> mods, PvpCompanion c, StatusDefinition def, int turns)
    {
        // Stat-modifier statuses are computed dynamically from ActiveStatuses, but for non-zero
        // Mods entries we still surface them for display in the emit text. We don't duplicate
        // the computation — EffectiveAttack/Defense already fold ActiveStatuses in.
        // Only MissChance needs a Mod entry since nothing else tracks it as a Mod.
        _ = c; _ = turns; _ = def; // statuses computed dynamically; no separate Mod needed
    }

    private CompanionMove ChooseMove(PvpCompanion attacker, PvpCompanion defender)
    {
        if (attacker.Moves.Count == 0)
            return new CompanionMove { Id = "fallback", Name = "Tackle", Power = 0.8f };

        var tDef = TemperamentRegistry.Get(attacker.Temperament);

        // Prefer healing when low HP
        if (attacker.Hp < attacker.MaxHp * tDef.HealThreshold)
        {
            var heal = attacker.Moves.FirstOrDefault(m => m.HasHeal && !m.HasDamage);
            if (heal != null && _rng.NextDouble() < tDef.HealChance) return heal;
        }

        // Early turns: try a status-inflicting attack if defender isn't already loaded with statuses
        if (defender.ActiveStatuses.Count < 2 && attacker.TurnCount <= 2 && _rng.NextDouble() < 0.50)
        {
            var statusAtk = attacker.Moves.FirstOrDefault(m => m.HasDamage && m.HasEnemyEffect);
            if (statusAtk != null) return statusAtk;
        }

        // Early turns: try a pure debuff
        if (attacker.TurnCount <= 1 && _rng.NextDouble() < tDef.DebuffChance)
        {
            var debuff = attacker.Moves.FirstOrDefault(m => m.HasEnemyEffect && !m.HasDamage);
            if (debuff != null) return debuff;
        }

        // Early turns: try a self-buff (if no attack buff active)
        if (attacker.TurnCount <= 2 && _rng.NextDouble() < tDef.BuffChance
            && !attacker.Mods.Any(m => m.Stat == ModifierStat.Attack && m.Value > 0)
            && !attacker.ActiveStatuses.Any(s => StatusRegistry.Get(s.Status).AtkPct > 0))
        {
            var buff = attacker.Moves.FirstOrDefault(m => m.HasSelfBuff && !m.HasDamage && !m.HasHeal);
            if (buff != null) return buff;
        }

        // Default: prefer highest-power attack
        var attacks = attacker.Moves.Where(m => m.HasDamage).ToList();
        if (attacks.Count > 0 && _rng.NextDouble() < tDef.AttackBias)
            return attacks.MaxBy(m => m.Power)!;

        return attacker.Moves[_rng.Next(attacker.Moves.Count)];
    }

    private static void TickStatuses(PvpCompanion c, Action<string?, string?, BattleEventKind, string> emit)
    {
        for (int i = c.ActiveStatuses.Count - 1; i >= 0; i--)
        {
            var (status, turns) = c.ActiveStatuses[i];
            int newTurns = turns - 1;
            if (newTurns <= 0)
            {
                c.ActiveStatuses.RemoveAt(i);
                emit(null, c.OwnerId, BattleEventKind.Status,
                    $"  {c.Name} recovered from {StatusRegistry.Get(status).DisplayName}.");
            }
            else
            {
                c.ActiveStatuses[i] = (status, newTurns);
            }
        }
    }

    private float GetTypeMultiplier(DamageType attackType, PvpCompanion defender)
    {
        float raw = 1f;
        foreach (var dt in defender.ElementTypes)
        {
            if (_weakTo.TryGetValue(dt, out var beats) && beats == attackType) raw *= 1.3f;
            else if (_weakTo.TryGetValue(attackType, out var losesTo) && losesTo == dt) raw *= 0.75f;
        }
        return 1f + (raw - 1f) * (1f - defender.EffectiveResist);
    }

    private static void TickMods(PvpCompanion c) => c.Mods.RemoveAll(m => { m.Turns--; return m.Turns <= 0; });

    private void PruneExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var k in _challenges.Where(kv => now > kv.Value.ExpiresAt).Select(kv => kv.Key).ToList())
            _challenges.Remove(k);
    }

    // Fire > Nature > Dark > Storm > Frost > Fire
    private static readonly Dictionary<DamageType, DamageType> _weakTo = new()
    {
        [DamageType.Nature] = DamageType.Fire,
        [DamageType.Dark]   = DamageType.Nature,
        [DamageType.Storm]  = DamageType.Dark,
        [DamageType.Frost]  = DamageType.Storm,
        [DamageType.Fire]   = DamageType.Frost,
    };

    private sealed class Mod(ModifierStat stat, float value, int turns)
    {
        public ModifierStat Stat  { get; } = stat;
        public float        Value { get; } = value;
        public int          Turns { get; set; } = turns;
    }

    private sealed class PvpCompanion
    {
        public string OwnerId { get; }
        public string PlayerName { get; }
        public string Name { get; }
        public IReadOnlyList<DamageType> ElementTypes { get; }
        public IReadOnlyList<CompanionMove> Moves { get; }
        public CompanionTemperament Temperament { get; }
        public int MaxHp { get; }
        public int Hp { get; set; }
        public int BaseSpd { get; }
        public int TurnCount { get; set; }
        public List<(CompanionStatus Status, int Turns)> ActiveStatuses { get; } = [];
        public List<Mod> Mods { get; } = [];

        private readonly float _baseAtk;
        private readonly float _baseDef;
        private readonly float _effFoc;
        private readonly float _effRes;

        public float EffectiveAttack =>
            _baseAtk
            + Mods.Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value)
            + ActiveStatuses.Sum(s =>
                CombatMath.StatusStatEffect(_baseAtk, StatusRegistry.Get(s.Status).AtkPct, StatusRegistry.Get(s.Status).AtkMin));

        public float EffectiveDefense =>
            _baseDef
            + Mods.Where(m => m.Stat == ModifierStat.Defense).Sum(m => m.Value)
            + ActiveStatuses.Sum(s =>
                CombatMath.StatusStatEffect(_baseDef, StatusRegistry.Get(s.Status).DefPct, StatusRegistry.Get(s.Status).DefMin));

        public int EffectiveSpeed =>
            (int)(BaseSpd
            + Mods.Where(m => m.Stat == ModifierStat.Speed).Sum(m => m.Value)
            + ActiveStatuses.Sum(s =>
                CombatMath.StatusStatEffect(BaseSpd, StatusRegistry.Get(s.Status).SpdPct, StatusRegistry.Get(s.Status).SpdMin)));

        public float MissChanceTotal =>
            ActiveStatuses.Sum(s => StatusRegistry.Get(s.Status).MissChanceAdd);

        public float EffectiveFocus   => _effFoc;
        public float EffectiveResist  => _effRes;

        public PvpCompanion(
            string ownerId, string playerName, string name,
            int baseAttack, int baseVit, int baseDef, int baseSpd, int baseFoc, int baseRes,
            CompanionTemperament temperament, CompanionForm form,
            IReadOnlyList<DamageType> elementTypes, IReadOnlyList<CompanionMove> moves)
        {
            OwnerId      = ownerId;
            PlayerName   = playerName;
            Name         = name;
            ElementTypes = elementTypes;
            Moves        = moves;
            Temperament  = temperament;

            var tDef = TemperamentRegistry.Get(temperament);
            var fDef = FormRegistry.Get(form);

            MaxHp    = 10 + Math.Max(0, baseVit + tDef.VitBonus + fDef.VitBonus) * 5;
            Hp       = MaxHp;
            BaseSpd  = baseSpd + tDef.SpdBonus + fDef.SpdBonus;
            _baseAtk = Math.Max(0f, baseAttack + tDef.AtkBonus + fDef.AtkBonus);
            _baseDef = Math.Max(0f, baseDef + tDef.DefBonus + fDef.DefBonus);
            _effFoc  = Math.Clamp(0.05f + (baseFoc + tDef.FocBonus + fDef.FocBonus) * 0.025f, 0.05f, 0.45f);
            _effRes  = Math.Clamp((baseRes + tDef.ResBonus + fDef.ResBonus) * 0.06f, 0f, 0.55f);
        }
    }
}
