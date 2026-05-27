using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryEquipmentDefinitionProvider : IEquipmentDefinitionProvider
{
    private static readonly EquipmentDefinition[] _definitions =
    [
        // ── Weapons — physical ────────────────────────────────────────────────
        new()
        {
            Id = "FlintKnife", Name = "Flint Knife", EquipmentSlot = "Weapon",
            Description    = "Flint knapped to a point and wrapped in fiber. Sharp enough to matter.",
            WeaponDamageType = DamageType.Piercing,
            Affixes        = [new(ModifierStat.Attack, 1), new(ModifierStat.Accuracy, 0.06f)],
        },
        new()
        {
            Id = "WoodClub", Name = "Wood Club", EquipmentSlot = "Weapon",
            Description    = "Heavy and unbalanced. Effective in the manner that blunt things are.",
            WeaponDamageType = DamageType.Bludgeoning,
            Affixes        = [new(ModifierStat.Attack, 2)],
        },
        new()
        {
            Id = "IronDagger", Name = "Iron Dagger", EquipmentSlot = "Weapon",
            Description    = "Short, direct, and faster than it looks.",
            WeaponDamageType = DamageType.Piercing,
            Affixes        = [new(ModifierStat.Attack, 2), new(ModifierStat.Accuracy, 0.04f)],
        },
        new()
        {
            Id = "IronSword", Name = "Iron Sword", EquipmentSlot = "Weapon",
            Description    = "A dull but reliable iron blade. It has seen better years but none worse.",
            WeaponDamageType = DamageType.Slashing,
            Affixes        = [new(ModifierStat.Attack, 4), new(ModifierStat.Accuracy, -0.04f)],
        },
        new()
        {
            Id = "IronMace", Name = "Iron Mace", EquipmentSlot = "Weapon",
            Description    = "A flanged iron head on a wrapped grip. It does not negotiate.",
            WeaponDamageType = DamageType.Bludgeoning,
            Affixes        = [new(ModifierStat.Attack, 5), new(ModifierStat.Accuracy, -0.07f)],
        },

        // ── Weapons — staves ──────────────────────────────────────────────────
        new()
        {
            Id = "EmberStaff", Name = "Ember Staff", EquipmentSlot = "Weapon",
            Description    = "Char-blackened wood with a coal core. Warm to the touch. Your spells carry more heat.",
            WeaponDamageType = DamageType.Fire,
            Affixes        = [new(ModifierStat.Attack, 1), new(ModifierStat.Magic, 1), new(ModifierStat.Accuracy, 0.03f)],
        },
        new()
        {
            Id = "FrostStaff", Name = "Frost Staff", EquipmentSlot = "Weapon",
            Description    = "A branch that never thawed. The air around it is still. Your spells cut colder.",
            WeaponDamageType = DamageType.Frost,
            Affixes        = [new(ModifierStat.Attack, 1), new(ModifierStat.Magic, 1), new(ModifierStat.Accuracy, 0.03f)],
        },
        new()
        {
            Id = "StormStaff", Name = "Storm Staff", EquipmentSlot = "Weapon",
            Description    = "Quartz-tipped and faintly humming. It remembers the lightning that made it.",
            WeaponDamageType = DamageType.Storm,
            Affixes        = [new(ModifierStat.Attack, 1), new(ModifierStat.Magic, 1), new(ModifierStat.Accuracy, 0.03f)],
        },
        new()
        {
            Id = "VineStaff", Name = "Vine Staff", EquipmentSlot = "Weapon",
            Description    = "Still growing, faintly. The bark is warm and the wood is alive. Your spells carry that.",
            WeaponDamageType = DamageType.Nature,
            Affixes        = [new(ModifierStat.Attack, 1), new(ModifierStat.Magic, 1), new(ModifierStat.Accuracy, 0.03f)],
        },
        new()
        {
            Id = "ShadowStaff", Name = "Shadow Staff", EquipmentSlot = "Weapon",
            Description    = "Wood that absorbed something it shouldn't have. Dark to the core. Your spells follow.",
            WeaponDamageType = DamageType.Dark,
            Affixes        = [new(ModifierStat.Attack, 1), new(ModifierStat.Magic, 1), new(ModifierStat.Accuracy, 0.03f)],
        },

        // ── Armor ─────────────────────────────────────────────────────────────
        new()
        {
            Id = "LeatherArmor", Name = "Leather Armor", EquipmentSlot = "Armor",
            Description = "Stitched from scraps. Better than nothing, which it slightly exceeds.",
            Affixes     = [new(ModifierStat.Defense, 6)],
        },
        new()
        {
            Id = "BearHideCloak", Name = "Bear Hide Cloak", EquipmentSlot = "Armor",
            Description = "Heavy and warm and smells of the bear it used to be. Excellent protection.",
            Affixes     = [new(ModifierStat.Defense, 10)],
        },
        new()
        {
            Id = "EchoMantle", Name = "Echo Mantle", EquipmentSlot = "Armor",
            Description = "Woven with quartz dust and something older. Physical blows still land. Other things don't.",
            Affixes     = [new(ModifierStat.Resistance, 6)],
        },

        // ── Hats ──────────────────────────────────────────────────────────────
        new()
        {
            Id = "TrailCap", Name = "Trail Cap", EquipmentSlot = "Hat",
            Description = "Light and close-fitting. Worn by people who intend to hit things many times.",
            Affixes     = [new(ModifierStat.StaminaRegen, 2)],
        },
        new()
        {
            Id = "MeditationCowl", Name = "Meditation Cowl", EquipmentSlot = "Hat",
            Description = "Deep-hooded and very quiet inside. The wearer finds their focus faster.",
            Affixes     = [new(ModifierStat.ManaRegen, 2)],
        },
        new()
        {
            Id = "MossHood", Name = "Moss Hood", EquipmentSlot = "Hat",
            Description = "Still damp. Still growing. Something about wearing living things helps the body remember what it's doing.",
            Affixes     = [new(ModifierStat.HpRegen, 1)],
        },
        new()
        {
            Id = "HealersWrap", Name = "Healer's Wrap", EquipmentSlot = "Hat",
            Description = "Herb-soaked linen wound tight around the head. Every remedy you apply works better than it should.",
            Affixes     = [new(ModifierStat.HealBonus, 0.20f)],
        },
    ];

    private static readonly Dictionary<string, EquipmentDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<EquipmentDefinition> All => _definitions;

    public EquipmentDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
