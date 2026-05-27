using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
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
        if (c2.EffSpd > c1.EffSpd) (c1, c2) = (c2, c1);

        for (int round = 1; round <= 30; round++)
        {
            Emit(null, null, BattleEventKind.RoundStart, $"── Round {round} ──");
            TakeTurn(c1, c2, Emit);
            if (c2.Hp <= 0) break;
            TickMods(c1); TickMods(c2);
            TickStatus(c1, Emit); TickStatus(c2, Emit);
            if (c2.Hp <= 0) break;
            events.Add(new BattleEvent(null, null, BattleEventKind.TurnBreak, "",
                challenger.Hp, challenger.MaxHp, accepter.Hp, accepter.MaxHp));
            TakeTurn(c2, c1, Emit);
            if (c1.Hp <= 0) break;
            TickMods(c1); TickMods(c2);
            TickStatus(c1, Emit); TickStatus(c2, Emit);
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

        // Stun (move-lock from Stun move)
        if (attacker.StunnedTurns > 0)
        {
            attacker.StunnedTurns--;
            emit(attacker.OwnerId, null, BattleEventKind.Stun, $"  {attacker.Name} is stunned and can't move!");
            return;
        }

        // Status: DoT and skip effects
        var statusDef = StatusRegistry.Get(attacker.Status);
        if (statusDef.DoTMultiplier > 0)
        {
            int tickDmg = (int)Math.Max(1, attacker.MaxHp * statusDef.DoTMultiplier);
            attacker.Hp = Math.Max(0, attacker.Hp - tickDmg);
            emit(null, attacker.OwnerId, BattleEventKind.Status,
                $"  {attacker.Name} is {statusDef.DisplayName.ToLower()}! -{tickDmg} HP | {attacker.Hp}/{attacker.MaxHp} HP");
            if (attacker.Hp <= 0) return;
        }
        if (statusDef.SkipChance > 0 && _rng.NextDouble() < statusDef.SkipChance)
        {
            emit(attacker.OwnerId, null, BattleEventKind.Status,
                $"  {attacker.Name} is {statusDef.DisplayName.ToLower()} and can't move!");
            return;
        }

        var move = ChooseMove(attacker, defender);
        bool crit = _rng.NextDouble() < attacker.EffectiveFocus;

        switch (move.Kind)
        {
            case CompanionMoveKind.Attack:
            case CompanionMoveKind.StatusAttack:
            {
                float eff = GetTypeMultiplier(move.DamageType, defender);
                float raw = attacker.EffectiveAttack * move.Power * (float)(0.85 + _rng.NextDouble() * 0.30);
                float dmg = Math.Max(1f, raw * StatusRegistry.Get(attacker.Status).AttackMultiplier - Math.Max(0f, defender.EffectiveDefense) * 0.5f) * (crit ? 1.5f : 1f) * eff;
                int damage = (int)Math.Round(dmg);

                if (defender.EffectiveDodge > 0 && _rng.NextDouble() < defender.EffectiveDodge)
                {
                    emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Dodge,
                        $"  {attacker.Name} → {move.Name} | {defender.Name} dodges!");
                    break;
                }

                defender.Hp = Math.Max(0, defender.Hp - damage);
                string tags = (crit ? " [CRIT]" : "") + (eff > 1f ? " [Super effective!]" : eff < 1f ? " [Resisted]" : "");

                // Try inflicting status
                if (move.Kind == CompanionMoveKind.StatusAttack && move.InflictStatus != CompanionStatus.None
                    && defender.Status == CompanionStatus.None && _rng.NextDouble() < move.StatusChance)
                {
                    defender.Status      = move.InflictStatus;
                    defender.StatusTurns = move.StatusDuration;
                    tags += $" [{move.InflictStatus}!]";
                }

                emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Attack,
                    $"  {attacker.Name} → {move.Name} | {damage} dmg{tags} | {defender.Name}: {defender.Hp}/{defender.MaxHp} HP");

                // Rally on low-HP crit
                if (crit && defender.Hp > 0 && defender.Hp < defender.MaxHp * 0.35f && _rng.NextDouble() < 0.20)
                {
                    defender.Mods.Add(new Mod(ModifierStat.Attack, 3f, 2));
                    emit(defender.OwnerId, null, BattleEventKind.Rally,
                        $"  {defender.Name} rallies! (+3 ATK for 2 rounds)");
                }
                break;
            }
            case CompanionMoveKind.PlayerBuff:
            {
                if (move.EffectStat == null)
                {
                    int heal = (int)Math.Round(move.EffectValue * (crit ? 1.5f : 1f));
                    attacker.Hp = Math.Min(attacker.MaxHp, attacker.Hp + heal);
                    emit(attacker.OwnerId, null, BattleEventKind.Heal,
                        $"  {attacker.Name} → {move.Name} | +{heal} HP | {attacker.Hp}/{attacker.MaxHp} HP");
                }
                else
                {
                    attacker.Mods.Add(new Mod(move.EffectStat.Value, move.EffectValue, move.EffectTurns));
                    emit(attacker.OwnerId, null, BattleEventKind.Buff,
                        $"  {attacker.Name} → {move.Name} | {FormatStat(move.EffectStat.Value)} {move.EffectValue:+#.#;-#.#;0} for {move.EffectTurns} rounds");
                }
                break;
            }
            case CompanionMoveKind.EnemyDebuff when move.EffectStat.HasValue:
            {
                defender.Mods.Add(new Mod(move.EffectStat.Value, move.EffectValue, move.EffectTurns));
                emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Debuff,
                    $"  {attacker.Name} → {move.Name} | {defender.Name}'s {FormatStat(move.EffectStat.Value)} {move.EffectValue:+#.#;-#.#;0} for {move.EffectTurns} rounds");
                break;
            }
        }
        attacker.TurnCount++;
    }

    private CompanionMove ChooseMove(PvpCompanion attacker, PvpCompanion defender)
    {
        if (attacker.Moves.Count == 0)
            return new CompanionMove { Id = "fallback", Name = "Tackle", Kind = CompanionMoveKind.Attack, Power = 0.8f };

        var tDef = TemperamentRegistry.Get(attacker.Temperament);

        if (attacker.Hp < attacker.MaxHp * tDef.HealThreshold)
        {
            var heal = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.PlayerBuff && m.EffectStat == null);
            if (heal != null && _rng.NextDouble() < tDef.HealChance) return heal;
        }

        if (defender.Status == CompanionStatus.None && attacker.TurnCount <= 2 && _rng.NextDouble() < 0.50)
        {
            var status = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.StatusAttack);
            if (status != null) return status;
        }

        if (attacker.TurnCount <= 1 && _rng.NextDouble() < tDef.DebuffChance)
        {
            var debuff = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.EnemyDebuff);
            if (debuff != null) return debuff;
        }

        if (attacker.TurnCount <= 2 && _rng.NextDouble() < tDef.BuffChance
            && !attacker.Mods.Any(m => m.Stat == ModifierStat.Attack && m.Value > 0))
        {
            var buff = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.PlayerBuff && m.EffectStat != null);
            if (buff != null) return buff;
        }

        var attacks = attacker.Moves
            .Where(m => m.Kind is CompanionMoveKind.Attack or CompanionMoveKind.StatusAttack)
            .ToList();
        if (attacks.Count > 0 && _rng.NextDouble() < tDef.AttackBias)
            return attacks.MaxBy(m => m.Power)!;

        return attacker.Moves[_rng.Next(attacker.Moves.Count)];
    }

    private static void TickStatus(PvpCompanion c, Action<string?, string?, BattleEventKind, string> emit)
    {
        if (c.Status == CompanionStatus.None) return;
        c.StatusTurns--;
        if (c.StatusTurns <= 0)
        {
            emit(null, c.OwnerId, BattleEventKind.Status, $"  {c.Name} recovered from {StatusRegistry.Get(c.Status).DisplayName}.");
            c.Status = CompanionStatus.None;
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
        // Resist reduces deviation from neutral
        return 1f + (raw - 1f) * (1f - defender.EffectiveResist);
    }

    private static void TickMods(PvpCompanion c) => c.Mods.RemoveAll(m => { m.Turns--; return m.Turns <= 0; });

    private void PruneExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var k in _challenges.Where(kv => now > kv.Value.ExpiresAt).Select(kv => kv.Key).ToList())
            _challenges.Remove(k);
    }

    private static string FormatStat(ModifierStat stat) => stat switch
    {
        ModifierStat.Attack      => "ATK",
        ModifierStat.Defense     => "DEF",
        ModifierStat.DodgeChance => "DODGE",
        ModifierStat.Resistance  => "RES",
        _ => stat.ToString(),
    };

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
        public int EffSpd { get; }
        public int TurnCount { get; set; }
        public int StunnedTurns { get; set; }
        public CompanionStatus Status { get; set; } = CompanionStatus.None;
        public int StatusTurns { get; set; }
        public List<Mod> Mods { get; } = [];

        private readonly float _baseAtk;
        private readonly float _baseDef;
        private readonly float _effFoc;
        private readonly float _effRes;

        public float EffectiveAttack  => _baseAtk + Mods.Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value);
        public float EffectiveDefense => _baseDef  + Mods.Where(m => m.Stat == ModifierStat.Defense).Sum(m => m.Value);
        public float EffectiveFocus   => _effFoc;
        public float EffectiveResist  => _effRes;
        public float EffectiveDodge   => Math.Clamp(Mods.Where(m => m.Stat == ModifierStat.DodgeChance).Sum(m => m.Value), 0f, 0.75f);

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

            MaxHp = 10 + Math.Max(0, baseVit + tDef.VitBonus + fDef.VitBonus) * 5;
            Hp    = MaxHp;

            EffSpd   = baseSpd + tDef.SpdBonus + fDef.SpdBonus;
            _baseAtk = Math.Max(0f, baseAttack + tDef.AtkBonus + fDef.AtkBonus);
            _baseDef = Math.Max(0f, baseDef + tDef.DefBonus + fDef.DefBonus);
            _effFoc  = Math.Clamp(0.05f + (baseFoc + tDef.FocBonus + fDef.FocBonus) * 0.025f, 0.05f, 0.45f);
            _effRes  = Math.Clamp((baseRes + tDef.ResBonus + fDef.ResBonus) * 0.06f, 0f, 0.55f);
        }
    }
}
