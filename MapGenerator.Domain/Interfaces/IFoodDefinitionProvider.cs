using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IFoodDefinitionProvider
{
    IReadOnlyList<FoodDefinition> All { get; }
    FoodDefinition? GetById(string resourceId);
}
