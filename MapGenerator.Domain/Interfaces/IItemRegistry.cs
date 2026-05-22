using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IItemRegistry
{
    ResourceDefinition? FindMaterial(string id);
    EquipmentDefinition? FindEquipment(string id);
    ConsumableDefinition? FindConsumable(string id);

    /// <summary>Returns the display name for any item ID regardless of type.</summary>
    string? GetName(string id);
}
