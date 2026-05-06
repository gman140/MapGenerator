using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IStructureDefinitionProvider
{
    IReadOnlyList<StructureDefinition> All { get; }
    StructureDefinition? GetByType(StructureType type);
}
