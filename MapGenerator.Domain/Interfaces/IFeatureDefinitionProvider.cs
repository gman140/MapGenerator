using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IFeatureDefinitionProvider
{
    IReadOnlyList<TileFeatureDefinition> All { get; }
    TileFeatureDefinition? GetById(string? id);
}
