using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class EquipmentDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string EquipmentSlot { get; init; } = string.Empty;  // "Weapon", "Armor", "Hat"
    public DamageType WeaponDamageType { get; init; } = DamageType.Bludgeoning;
    public IReadOnlyList<StatAffix> Affixes { get; init; } = [];
    public IReadOnlyList<string> WeaponAttackIds { get; init; } = [];

    // Lure-specific fishing effects (only used when EquipmentSlot == "Lure")
    public string? LureCategory { get; init; }          // "Freshwater", "Saltwater", "Deep", "Cold"
    public double LureCategoryBoost { get; init; } = 1.0;
    public double LureRarityBoost { get; init; } = 1.0;
}
