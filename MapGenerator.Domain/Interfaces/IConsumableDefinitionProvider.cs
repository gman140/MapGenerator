using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IConsumableDefinitionProvider
{
    IReadOnlyList<ConsumableDefinition> All { get; }
    ConsumableDefinition? GetById(string id);
}
