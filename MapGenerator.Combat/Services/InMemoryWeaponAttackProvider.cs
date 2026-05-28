using MapGenerator.Combat.Interfaces;
using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Services;

public class InMemoryWeaponAttackProvider : IWeaponAttackProvider
{
    private static readonly WeaponAttackDefinition[] _definitions =
    [
        // ── Built-in (all weapons) ────────────────────────────────────────────
        new()
        {
            Id          = "light_attack",
            Name        = "Light Attack",
            Description = "A quick, reliable strike.",
            StaminaCost = 2,
            Power       = 1.0f,
        },
        new()
        {
            Id          = "heavy_attack",
            Name        = "Heavy Strike",
            Description = "A powerful blow that deals 50% more damage.",
            StaminaCost = 4,
            Power       = 1.5f,
        },

        // ── Flint Knife ───────────────────────────────────────────────────────
        new()
        {
            Id          = "flintknife_bleed",
            Name        = "Bleed Out",
            Description = "A precise jab that opens a seeping wound.",
            StaminaCost = 3,
            Power       = 0.9f,
            Effects     = [new(CompanionStatus.Poisoned, 0.70f, 3)],
        },

        // ── Wood Club ─────────────────────────────────────────────────────────
        new()
        {
            Id          = "woodclub_concuss",
            Name        = "Concussive Blow",
            Description = "A heavy swing aimed at the head. May stun the target.",
            StaminaCost = 3,
            Power       = 1.0f,
            Effects     = [new(CompanionStatus.Paralyzed, 0.55f, 1)],
        },

        // ── Iron Dagger ───────────────────────────────────────────────────────
        new()
        {
            Id          = "irondagger_precise",
            Name        = "Precise Strike",
            Description = "A calculated thrust that finds gaps in the target's guard.",
            StaminaCost = 3,
            Power       = 1.1f,
            Effects     = [new(CompanionStatus.Exposed, 0.65f, 2)],
        },

        // ── Iron Sword ────────────────────────────────────────────────────────
        new()
        {
            Id          = "ironsword_rend",
            Name        = "Rend",
            Description = "A tearing slash that shreds the target's defenses.",
            StaminaCost = 3,
            Power       = 0.9f,
            Effects     = [new(CompanionStatus.Shattered, 0.80f, 2)],
        },
        new()
        {
            Id          = "ironsword_cleave",
            Name        = "Cleave",
            Description = "A wide arc that strikes all enemies at once.",
            StaminaCost = 5,
            Power       = 0.75f,
            HitsAll     = true,
        },

        // ── Iron Mace ─────────────────────────────────────────────────────────
        new()
        {
            Id          = "ironmace_pulverize",
            Name        = "Pulverize",
            Description = "A devastating blow that shatters the target's armor.",
            StaminaCost = 5,
            Power       = 1.3f,
            Effects     = [new(CompanionStatus.Shattered, 0.70f, 2)],
        },

        // ── Ember Staff ───────────────────────────────────────────────────────
        new()
        {
            Id          = "emberstaff_ignite",
            Name        = "Ignite",
            Description = "Channel fire through the staff, searing the target.",
            StaminaCost = 3,
            Power       = 1.1f,
            UsesMagic   = true,
            Effects     = [new(CompanionStatus.Burned, 0.65f, 3)],
        },

        // ── Frost Staff ───────────────────────────────────────────────────────
        new()
        {
            Id          = "froststaff_deepfreeze",
            Name        = "Deep Freeze",
            Description = "A piercing blast of cold that slows the target significantly.",
            StaminaCost = 3,
            Power       = 1.0f,
            UsesMagic   = true,
            Effects     = [new(CompanionStatus.Chilled, 0.80f, 2)],
        },

        // ── Storm Staff ───────────────────────────────────────────────────────
        new()
        {
            Id          = "stormstaff_discharge",
            Name        = "Discharge",
            Description = "An electric surge that may lock the target in place.",
            StaminaCost = 3,
            Power       = 1.0f,
            UsesMagic   = true,
            Effects     = [new(CompanionStatus.Paralyzed, 0.50f, 1)],
        },

        // ── Vine Staff ────────────────────────────────────────────────────────
        new()
        {
            Id          = "vinestaff_entangle",
            Name        = "Entangle",
            Description = "Roots seize the target, weakening and exposing it.",
            StaminaCost = 4,
            Power       = 0.8f,
            UsesMagic   = true,
            Effects     =
            [
                new(CompanionStatus.Weakened, 0.80f, 2),
                new(CompanionStatus.Exposed,  0.50f, 2),
            ],
        },

        // ── Shadow Staff ──────────────────────────────────────────────────────
        new()
        {
            Id          = "shadowstaff_hex",
            Name        = "Hex",
            Description = "A dark whisper that curses the target's accuracy.",
            StaminaCost = 3,
            Power       = 0.9f,
            UsesMagic   = true,
            Effects     = [new(CompanionStatus.Cursed, 0.90f, 3)],
        },

        // ── Quartz Fang (boss) ────────────────────────────────────────────────
        new()
        {
            Id          = "quartzfang_shatterpoint",
            Name        = "Shatter Point",
            Description = "Strike a resonant frequency through the target, cracking its defenses and jolting its nerves.",
            StaminaCost = 3,
            Power       = 0.9f,
            Effects     =
            [
                new(CompanionStatus.Shattered, 0.70f, 2),
                new(CompanionStatus.Paralyzed, 0.40f, 1),
            ],
        },
        new()
        {
            Id          = "quartzfang_overcharge",
            Name        = "Overcharge",
            Description = "Release all stored lightning at once, striking every enemy.",
            StaminaCost = 5,
            Power       = 0.7f,
            UsesMagic   = true,
            HitsAll     = true,
            Effects     = [new(CompanionStatus.Paralyzed, 0.40f, 1)],
        },

        // ── Warden's Brand (boss) ─────────────────────────────────────────────
        new()
        {
            Id             = "wardensbrand_drain",
            Name           = "Drain",
            Description    = "Draw life force directly from the target into yourself.",
            StaminaCost    = 4,
            Power          = 1.0f,
            LifestealRatio = 0.40f,
        },
        new()
        {
            Id          = "wardensbrand_curseedge",
            Name        = "Curse Edge",
            Description = "A cut that carries ancient malice, weakening and hexing the target.",
            StaminaCost = 4,
            Power       = 0.8f,
            Effects     =
            [
                new(CompanionStatus.Cursed,   0.80f, 3),
                new(CompanionStatus.Weakened, 0.80f, 2),
            ],
        },

        // ── Thornwhip (boss) ──────────────────────────────────────────────────
        new()
        {
            Id          = "thornwhip_lash",
            Name        = "Lash",
            Description = "A wide crack of the whip that catches every enemy and leaves wounds weeping poison.",
            StaminaCost = 4,
            Power       = 0.75f,
            HitsAll     = true,
            Effects     = [new(CompanionStatus.Poisoned, 0.70f, 3)],
        },
        new()
        {
            Id          = "thornwhip_overgrowth",
            Name        = "Overgrowth",
            Description = "Vines erupt and seize the target, leaving it exposed and weakened.",
            StaminaCost = 3,
            Power       = 0.9f,
            Effects     =
            [
                new(CompanionStatus.Exposed,  0.75f, 2),
                new(CompanionStatus.Weakened, 0.60f, 2),
            ],
        },

        // ── Glacial Maul (boss) ───────────────────────────────────────────────
        new()
        {
            Id          = "glacialmaul_frostcrush",
            Name        = "Frostcrush",
            Description = "A titanic blow infused with glacial cold. Deals massive damage and almost certainly chills the target.",
            StaminaCost = 4,
            Power       = 1.4f,
            Effects     = [new(CompanionStatus.Chilled, 0.90f, 3)],
        },
        new()
        {
            Id          = "glacialmaul_permafroslam",
            Name        = "Permafrost Slam",
            Description = "Smash the ground, sending a shockwave of ice that chills all enemies.",
            StaminaCost = 6,
            Power       = 1.0f,
            HitsAll     = true,
            Effects     = [new(CompanionStatus.Chilled, 0.70f, 2)],
        },
    ];

    private static readonly Dictionary<string, WeaponAttackDefinition> _byId =
        _definitions.ToDictionary(a => a.Id);

    public IReadOnlyList<WeaponAttackDefinition> All => _definitions;
    public WeaponAttackDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
