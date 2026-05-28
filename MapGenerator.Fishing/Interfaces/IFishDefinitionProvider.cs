using MapGenerator.Fishing.Models;

namespace MapGenerator.Fishing.Interfaces;

public interface IFishDefinitionProvider
{
    IReadOnlyList<FishDefinition> All { get; }
    FishDefinition? GetById(string id);
    IReadOnlyList<FishDefinition> GetPool(string[] fishIds, int poleTier);
}
