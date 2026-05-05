using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IBiomeDefinitionProvider
{
    IReadOnlyList<BiomeDefinition> All { get; }
    BiomeDefinition? GetByType(BiomeType type);
}
