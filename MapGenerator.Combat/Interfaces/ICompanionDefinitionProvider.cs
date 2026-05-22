using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Interfaces;

public interface ICompanionDefinitionProvider
{
    CompanionDefinition? GetById(string id);
    IReadOnlyList<CompanionDefinition> GetAll();
}
