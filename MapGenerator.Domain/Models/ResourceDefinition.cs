namespace MapGenerator.Domain.Models;

public class ResourceDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int MinQuantity { get; init; } = 1;
    public int MaxQuantity { get; init; } = 1;

    // Equipment fields — null/zero means not equippable
    public string? EquipmentSlot { get; init; }  // "Weapon", "Armor", "Hat"
    public int AttackBonus { get; init; }
    public int DefenseBonus { get; init; }
    public float DodgeChanceBonus { get; init; }
}
