using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Services;

public class InMemoryCompanionMoveProvider : ICompanionMoveProvider
{
    private static readonly IReadOnlyList<CompanionMove> _all =
    [
        // ── Universal physical attacks ────────────────────────────────────────
        new() { Id = "thump",        Name = "Thump",        Description = "A solid, bone-rattling strike.",                                                  Kind = CompanionMoveKind.Attack, DamageType = DamageType.Bludgeoning, Power = 0.9f },
        new() { Id = "piercing_jab", Name = "Piercing Jab", Description = "A quick, focused thrust that finds the gap in armor.",                            Kind = CompanionMoveKind.Attack, DamageType = DamageType.Piercing,    Power = 0.9f },
        new() { Id = "claw_slash",   Name = "Claw Slash",   Description = "A sweeping slash with sharpened claws.",                                          Kind = CompanionMoveKind.Attack, DamageType = DamageType.Slashing,    Power = 0.9f },

        // ── Universal buff / debuff ───────────────────────────────────────────
        new() { Id = "rally_cry",  Name = "Rally Cry",  Description = "A rallying call that sharpens your focus and lifts your attack.",                     Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Attack, EffectValue = 3f,  EffectTurns = 2 },
        new() { Id = "growl",      Name = "Growl",      Description = "An intimidating growl that rattles the enemy and weakens their strikes.",             Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack, EffectValue = -3f, EffectTurns = 2 },

        // ── Slimeling (Nature) ───────────────────────────────────────────────
        new() { Id = "glob_bonk",      Name = "Glob Bonk",      Description = "A satisfying, gelatinous impact.",                                                Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 1.0f,  EligibleTypes = [DamageType.Nature] },
        new() { Id = "dribble_splash", Name = "Dribble Splash", Description = "Flings a glob of slime. Sticks on contact.",                                     Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 0.85f, EligibleTypes = [DamageType.Nature] },
        new() { Id = "spore_puff",     Name = "Spore Puff",     Description = "Releases a cloud of toxic spores that hits all enemies.",                         Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 0.75f, HitsAll = true, EligibleTypes = [DamageType.Nature] },
        new() { Id = "acid_drool",     Name = "Acid Drool",     Description = "Corrosive liquid poured directly onto the target.",                               Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Nature, Power = 1.2f,  EligibleTypes = [DamageType.Nature] },
        new() { Id = "slick_coat",     Name = "Slick Coat",     Description = "Coats you in a layer of protective goo, temporarily bolstering your defense.",    Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Defense,    EffectValue = 2f,    EffectTurns = 2, EligibleTypes = [DamageType.Nature] },
        new() { Id = "mend_goo",       Name = "Mend Goo",       Description = "Applies a warm, restorative slime that heals a small amount of HP.",              Kind = CompanionMoveKind.PlayerBuff,  EffectStat = null,                    EffectValue = 8f,                     EligibleTypes = [DamageType.Nature] },
        new() { Id = "gunk_wrap",      Name = "Gunk Wrap",      Description = "Entangles the enemy in thick slime, impairing their attack.",                     Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Nature] },
        new() { Id = "ooze_drench",    Name = "Ooze Drench",    Description = "Coats the enemy in dissolving goo, weakening their defenses.",                    Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Nature] },

        // ── Emberhatch (Fire) ────────────────────────────────────────────────
        new() { Id = "scratch",       Name = "Scratch",       Description = "A fast swipe with burning claws.",                                                  Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Slashing, Power = 0.8f,  EligibleTypes = [DamageType.Fire] },
        new() { Id = "ember_blob",    Name = "Ember Blob",    Description = "Launches a burning mass of cinders.",                                               Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Fire,     Power = 1.0f,  EligibleTypes = [DamageType.Fire] },
        new() { Id = "cinder_flurry", Name = "Cinder Flurry", Description = "A flurry of sparks that singes all enemies.",                                      Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Fire,     Power = 0.75f, HitsAll = true, EligibleTypes = [DamageType.Fire] },
        new() { Id = "hot_bite",      Name = "Hot Bite",      Description = "Bites with jaws that run much hotter than they should.",                            Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Fire,     Power = 1.15f, EligibleTypes = [DamageType.Fire] },
        new() { Id = "yowl",          Name = "Yowl",          Description = "A piercing cry that sharpens your fighting instincts.",                              Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Attack,      EffectValue = 4f,    EffectTurns = 2, EligibleTypes = [DamageType.Fire] },
        new() { Id = "hype_dash",     Name = "Hype Dash",     Description = "Bursts forward with startling speed, improving your evasion.",                      Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.DodgeChance, EffectValue = 0.12f, EffectTurns = 2, EligibleTypes = [DamageType.Fire] },
        new() { Id = "char",          Name = "Char",          Description = "Scorches the target's armor, reducing their defense.",                              Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Fire] },
        new() { Id = "choke_smoke",   Name = "Choke Smoke",   Description = "Fills the air with acrid smoke that impairs the enemy's attack.",                   Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Fire] },

        // ── Frostling (Frost) ────────────────────────────────────────────────
        new() { Id = "frost_peck",   Name = "Frost Peck",   Description = "A sharp peck that carries a bone-deep chill.",                                       Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 0.9f,  EligibleTypes = [DamageType.Frost] },
        new() { Id = "shiver_bite",  Name = "Shiver Bite",  Description = "Clamps down hard. The cold does the rest.",                                          Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 1.0f,  EligibleTypes = [DamageType.Frost] },
        new() { Id = "hail_spit",    Name = "Hail Spit",    Description = "Spits small pellets of sleet that pelt all enemies.",                                Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 0.75f, HitsAll = true, EligibleTypes = [DamageType.Frost] },
        new() { Id = "ice_spike",    Name = "Ice Spike",    Description = "Drives a crystallized spike of ice into the target.",                                Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Frost, Power = 1.2f,  EligibleTypes = [DamageType.Frost] },
        new() { Id = "frost_shell",  Name = "Frost Shell",  Description = "Encases you in a protective layer of ice, bolstering your defenses.",                Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Defense,    EffectValue = 3f,    EffectTurns = 3, EligibleTypes = [DamageType.Frost] },
        new() { Id = "ice_mend",     Name = "Ice Mend",     Description = "Draws on winter's stillness to restore a small amount of your HP.",                  Kind = CompanionMoveKind.PlayerBuff,  EffectStat = null,                    EffectValue = 7f,                     EligibleTypes = [DamageType.Frost] },
        new() { Id = "sluggify",     Name = "Sluggify",     Description = "Flash-freezes the enemy's reflexes, significantly reducing their attack.",           Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Frost] },
        new() { Id = "brittle_rub",  Name = "Brittle Rub",  Description = "Coats the enemy in ice crystals that shatter their armor.",                         Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Frost] },

        // ── Sparkling (Storm) ────────────────────────────────────────────────
        new() { Id = "nip_shock",     Name = "Nip Shock",     Description = "A quick nip that discharges a jolt of static.",                                    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 0.8f,  EligibleTypes = [DamageType.Storm] },
        new() { Id = "crackle_bite",  Name = "Crackle Bite",  Description = "A solid bite crackling with electrical energy.",                                    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 1.0f,  EligibleTypes = [DamageType.Storm] },
        new() { Id = "pop_burst",     Name = "Pop Burst",     Description = "An explosive discharge that shocks all enemies.",                                   Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 0.75f, HitsAll = true, EligibleTypes = [DamageType.Storm] },
        new() { Id = "thundersnap",   Name = "Thundersnap",   Description = "A powerful snap that releases a focused thunderclap.",                              Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Storm, Power = 1.2f,  EligibleTypes = [DamageType.Storm] },
        new() { Id = "amp_up",        Name = "Amp Up",        Description = "Channels electric energy into you, surging your attack.",                          Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.Attack,      EffectValue = 4f,    EffectTurns = 2, EligibleTypes = [DamageType.Storm] },
        new() { Id = "jitterstep",    Name = "Jitterstep",    Description = "Zaps your reflexes into overdrive, improving evasion.",                            Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.DodgeChance, EffectValue = 0.12f, EffectTurns = 2, EligibleTypes = [DamageType.Storm] },
        new() { Id = "short_circuit", Name = "Short Circuit", Description = "Overloads the enemy's defenses with a surge of current.",                         Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -4f,   EffectTurns = 2, EligibleTypes = [DamageType.Storm] },
        new() { Id = "zap_daze",      Name = "Zap Daze",      Description = "A stunning shock that scrambles the enemy's attack instincts.",                    Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Storm] },

        // ── Shadelurk (Dark) ─────────────────────────────────────────────────
        new() { Id = "nip",          Name = "Nip",          Description = "A quick bite from the darkness.",                                                    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 0.9f,  EligibleTypes = [DamageType.Dark] },
        new() { Id = "shadow_lunge", Name = "Shadow Lunge", Description = "Lunges from the shadows with dark momentum.",                                        Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 1.0f,  EligibleTypes = [DamageType.Dark] },
        new() { Id = "dusk_scatter", Name = "Dusk Scatter", Description = "Scatters a burst of shadow energy across all enemies.",                              Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 0.75f, HitsAll = true, EligibleTypes = [DamageType.Dark] },
        new() { Id = "void_fang",    Name = "Void Fang",    Description = "Bites deep with fangs saturated in void energy.",                                    Kind = CompanionMoveKind.Attack,      DamageType = DamageType.Dark, Power = 1.2f,  EligibleTypes = [DamageType.Dark] },
        new() { Id = "shade_slip",   Name = "Shade Slip",   Description = "Wraps you in shadow, making you harder to hit.",                                     Kind = CompanionMoveKind.PlayerBuff,  EffectStat = ModifierStat.DodgeChance, EffectValue = 0.15f, EffectTurns = 2, EligibleTypes = [DamageType.Dark] },
        new() { Id = "void_drink",   Name = "Void Drink",   Description = "Draws vitality from the void, restoring your HP.",                                   Kind = CompanionMoveKind.PlayerBuff,  EffectStat = null,                     EffectValue = 8f,                     EligibleTypes = [DamageType.Dark] },
        new() { Id = "rattle",       Name = "Rattle",       Description = "A terrifying rattle that unsettles the enemy's focus.",                              Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Attack,     EffectValue = -3f,   EffectTurns = 2, EligibleTypes = [DamageType.Dark] },
        new() { Id = "peel_apart",   Name = "Peel Apart",   Description = "Tears at the enemy's defenses with shadowy claws.",                                  Kind = CompanionMoveKind.EnemyDebuff, EffectStat = ModifierStat.Defense,    EffectValue = -4f,   EffectTurns = 2, EligibleTypes = [DamageType.Dark] },

        // ── Status attacks (one per element) ─────────────────────────────────
        new() { Id = "toxic_spore",    Name = "Toxic Spore",    Description = "Releases a cloud of virulent spores that infects the target with poison.",         Kind = CompanionMoveKind.StatusAttack, DamageType = DamageType.Nature, Power = 0.7f,  EligibleTypes = [DamageType.Nature], InflictStatus = CompanionStatus.Poisoned,  StatusChance = 0.55f, StatusDuration = 3 },
        new() { Id = "scorch",         Name = "Scorch",         Description = "A searing blast that scorches the target, leaving a burning wound.",               Kind = CompanionMoveKind.StatusAttack, DamageType = DamageType.Fire,   Power = 0.75f, EligibleTypes = [DamageType.Fire],   InflictStatus = CompanionStatus.Burned,   StatusChance = 0.55f, StatusDuration = 3 },
        new() { Id = "flash_freeze",   Name = "Flash Freeze",   Description = "An instant freeze that slows the target's movements to a crawl.",                  Kind = CompanionMoveKind.StatusAttack, DamageType = DamageType.Frost,  Power = 0.7f,  EligibleTypes = [DamageType.Frost],  InflictStatus = CompanionStatus.Chilled,  StatusChance = 0.60f, StatusDuration = 2 },
        new() { Id = "shock_pulse",    Name = "Shock Pulse",    Description = "Delivers a paralyzing jolt that can lock up the target mid-battle.",                Kind = CompanionMoveKind.StatusAttack, DamageType = DamageType.Storm,  Power = 0.65f, EligibleTypes = [DamageType.Storm],  InflictStatus = CompanionStatus.Paralyzed,StatusChance = 0.50f, StatusDuration = 2 },
        new() { Id = "hexing_gaze",    Name = "Hexing Gaze",    Description = "A withering glare that saps the target's strength and will to fight.",              Kind = CompanionMoveKind.StatusAttack, DamageType = DamageType.Dark,   Power = 0.65f, EligibleTypes = [DamageType.Dark],   InflictStatus = CompanionStatus.Cursed,   StatusChance = 0.50f, StatusDuration = 3 },

        // ── Universal status attack ───────────────────────────────────────────
        new() { Id = "venomous_bite",  Name = "Venomous Bite",  Description = "A calculated bite that drives venom deep into the wound.",                         Kind = CompanionMoveKind.StatusAttack, DamageType = DamageType.Piercing, Power = 0.8f, InflictStatus = CompanionStatus.Poisoned, StatusChance = 0.45f, StatusDuration = 3 },
    ];

    private static readonly Dictionary<string, CompanionMove> _byId =
        _all.ToDictionary(m => m.Id);

    public IReadOnlyList<CompanionMove> All => _all;

    public CompanionMove? GetById(string id) =>
        _byId.TryGetValue(id, out var move) ? move : null;

    public IReadOnlyList<CompanionMove> GetEligibleFor(IEnumerable<DamageType> elementTypes)
    {
        var types = elementTypes.ToHashSet();
        return _all.Where(m => m.EligibleTypes.Count == 0 || m.EligibleTypes.Any(t => types.Contains(t))).ToList();
    }
}
