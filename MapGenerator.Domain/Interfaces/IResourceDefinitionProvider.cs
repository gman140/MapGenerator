using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IResourceDefinitionProvider
{
    IReadOnlyList<ResourceDefinition> All { get; }
    ResourceDefinition? GetById(string? id);
}
