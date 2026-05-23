using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Web.Services;

public record CompanionBattleChallenge(
    string ChallengerId,
    string ChallengerName,
    string TargetId,
    string ChallengerCompanionName,
    string[] ChallengerCompanionSprite,
    int ChallengerBaseAttack,
    IReadOnlyList<DamageType> ChallengerElementTypes,
    List<string> ChallengerMoveIds,
    DateTimeOffset ExpiresAt);

public enum BattleEventKind { Intro, RoundStart, TurnBreak, Attack, Heal, Buff, Debuff, Dodge, Stun, Rally, Outcome }

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

    // Returns the challenge for the given target without removing it (for UI display)
    public CompanionBattleChallenge? GetPendingChallenge(string targetId)
    {
        lock (_lock)
        {
            if (!_challenges.TryGetValue(targetId, out var ch)) return null;
            if (DateTimeOffset.UtcNow > ch.ExpiresAt) { _challenges.Remove(targetId); return null; }
            return ch;
        }
    }

    // Removes and returns the challenge (used when accepting or declining)
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
        int accepterBaseAttack, IReadOnlyList<DamageType> accepterElementTypes,
        List<string> accepterMoveIds)
    {
        var log    = new List<string>();
        var events = new List<BattleEvent>();

        // Keep stable references so HP values always map to the correct column
        var challenger = Build(challenge.ChallengerId, challenge.ChallengerName,
            challenge.ChallengerCompanionName, challenge.ChallengerBaseAttack,
            challenge.ChallengerElementTypes, challenge.ChallengerMoveIds);
        var accepter = Build(accepterId, accepterName, accepterCompanionName,
            accepterBaseAttack, accepterElementTypes, accepterMoveIds);

        void Emit(string? actorId, string? targetId, BattleEventKind kind, string text)
        {
            log.Add(text);
            events.Add(new BattleEvent(actorId, targetId, kind, text,
                challenger.Hp, challenger.MaxHp, accepter.Hp, accepter.MaxHp));
        }

        Emit(null, null, BattleEventKind.Intro,
            $"⚔ {challenger.PlayerName}'s {challenger.Name} vs {accepter.PlayerName}'s {accepter.Name}!");

        // Higher BaseAttack goes first; challenger wins ties
        var c1 = challenger;
        var c2 = accepter;
        if (c2.BaseAttack > c1.BaseAttack) (c1, c2) = (c2, c1);

        for (int round = 1; round <= 30; round++)
        {
            Emit(null, null, BattleEventKind.RoundStart, $"── Round {round} ──");
            TakeTurn(c1, c2, Emit);
            if (c2.Hp <= 0) break;
            TickMods(c1); TickMods(c2);
            events.Add(new BattleEvent(null, null, BattleEventKind.TurnBreak, "",
                challenger.Hp, challenger.MaxHp, accepter.Hp, accepter.MaxHp));
            TakeTurn(c2, c1, Emit);
            if (c1.Hp <= 0) break;
            TickMods(c1); TickMods(c2);
        }

        string? winnerId = null, winnerName = null, winningCompanionName = null;
        if (c1.Hp > 0 && c2.Hp <= 0)
        {
            (winnerId, winnerName, winningCompanionName) = (c1.OwnerId, c1.PlayerName, c1.Name);
            Emit(c1.OwnerId, null, BattleEventKind.Outcome, $"🏆 {c1.Name} wins for {c1.PlayerName}!");
        }
        else if (c2.Hp > 0 && c1.Hp <= 0)
        {
            (winnerId, winnerName, winningCompanionName) = (c2.OwnerId, c2.PlayerName, c2.Name);
            Emit(c2.OwnerId, null, BattleEventKind.Outcome, $"🏆 {c2.Name} wins for {c2.PlayerName}!");
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

    private PvpCompanion Build(string ownerId, string playerName, string name,
        int baseAttack, IReadOnlyList<DamageType> elements, List<string> moveIds)
    {
        var moves = moveIds.Select(id => _moveProvider.GetById(id)).OfType<CompanionMove>().ToList();
        return new PvpCompanion(ownerId, playerName, name, baseAttack, elements, moves);
    }

    private void TakeTurn(PvpCompanion attacker, PvpCompanion defender,
        Action<string?, string?, BattleEventKind, string> emit)
    {
        if (attacker.Hp <= 0) return;

        if (attacker.StunnedTurns > 0)
        {
            attacker.StunnedTurns--;
            emit(attacker.OwnerId, null, BattleEventKind.Stun, $"  {attacker.Name} is stunned and can't move!");
            return;
        }

        var move = ChooseMove(attacker, defender);
        bool crit = _rng.NextDouble() < 0.10;

        switch (move.Kind)
        {
            case CompanionMoveKind.Attack:
            {
                float eff = GetTypeMultiplier(move.DamageType, defender.ElementTypes);
                float raw = attacker.EffectiveAttack * move.Power * (float)(0.85 + _rng.NextDouble() * 0.30);
                float dmg = Math.Max(1f, raw - Math.Max(0f, defender.EffectiveDefense) * 0.5f) * (crit ? 1.5f : 1f) * eff;
                int damage = (int)Math.Round(dmg);

                if (defender.EffectiveDodge > 0 && _rng.NextDouble() < defender.EffectiveDodge)
                {
                    emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Dodge,
                        $"  {attacker.Name} → {move.Name} | {defender.Name} dodges!");
                    break;
                }

                defender.Hp = Math.Max(0, defender.Hp - damage);
                string tags = (crit ? " [CRIT]" : "") + (eff > 1f ? " [Super effective!]" : eff < 1f ? " [Resisted]" : "");
                emit(attacker.OwnerId, defender.OwnerId, BattleEventKind.Attack,
                    $"  {attacker.Name} → {move.Name} | {damage} dmg{tags} | {defender.Name}: {defender.Hp}/{defender.MaxHp} HP");

                // Rally: low-HP defender hit by crit gains a burst
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

        // Heal when critically low
        if (attacker.Hp < attacker.MaxHp * 0.28f)
        {
            var heal = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.PlayerBuff && m.EffectStat == null);
            if (heal != null && _rng.NextDouble() < 0.65) return heal;
        }

        // Open with a debuff occasionally
        if (attacker.TurnCount <= 1 && _rng.NextDouble() < 0.40)
        {
            var debuff = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.EnemyDebuff);
            if (debuff != null) return debuff;
        }

        // Buff early if not already buffed
        if (attacker.TurnCount <= 2 && _rng.NextDouble() < 0.25
            && !attacker.Mods.Any(m => m.Stat == ModifierStat.Attack && m.Value > 0))
        {
            var buff = attacker.Moves.FirstOrDefault(m => m.Kind == CompanionMoveKind.PlayerBuff && m.EffectStat != null);
            if (buff != null) return buff;
        }

        var attacks = attacker.Moves.Where(m => m.Kind == CompanionMoveKind.Attack).ToList();
        if (attacks.Count > 0 && _rng.NextDouble() < 0.75)
            return attacks.MaxBy(m => m.Power)!;

        return attacker.Moves[_rng.Next(attacker.Moves.Count)];
    }

    private static float GetTypeMultiplier(DamageType attackType, IReadOnlyList<DamageType> defenderTypes)
    {
        float mult = 1f;
        foreach (var dt in defenderTypes)
        {
            if (_weakTo.TryGetValue(dt, out var beats) && beats == attackType) mult *= 1.3f;
            else if (_weakTo.TryGetValue(attackType, out var losesTo) && losesTo == dt) mult *= 0.75f;
        }
        return mult;
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

    // Fire > Nature > Dark > Storm > Frost > Fire  (each key is weak to its value)
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

    private sealed class PvpCompanion(
        string ownerId, string playerName, string name,
        int baseAttack, IReadOnlyList<DamageType> elementTypes, IReadOnlyList<CompanionMove> moves)
    {
        public string                     OwnerId      { get; } = ownerId;
        public string                     PlayerName   { get; } = playerName;
        public string                     Name         { get; } = name;
        public int                        BaseAttack   { get; } = baseAttack;
        public IReadOnlyList<DamageType>  ElementTypes { get; } = elementTypes;
        public IReadOnlyList<CompanionMove> Moves      { get; } = moves;
        public int                        MaxHp        { get; } = 50 + baseAttack * 8;
        public int                        Hp           { get; set; } = 50 + baseAttack * 8;
        public int                        TurnCount    { get; set; }
        public int                        StunnedTurns { get; set; }
        public List<Mod>                  Mods         { get; } = [];

        public float EffectiveAttack  => BaseAttack * 4f + Mods.Where(m => m.Stat == ModifierStat.Attack).Sum(m => m.Value);
        public float EffectiveDefense => Mods.Where(m => m.Stat == ModifierStat.Defense).Sum(m => m.Value);
        public float EffectiveDodge   => Math.Clamp(Mods.Where(m => m.Stat == ModifierStat.DodgeChance).Sum(m => m.Value), 0f, 0.75f);
    }
}
