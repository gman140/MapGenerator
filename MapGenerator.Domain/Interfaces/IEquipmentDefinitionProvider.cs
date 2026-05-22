using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IEquipmentDefinitionProvider
{
    IReadOnlyList<EquipmentDefinition> All { get; }
    EquipmentDefinition? GetById(string id);
}
