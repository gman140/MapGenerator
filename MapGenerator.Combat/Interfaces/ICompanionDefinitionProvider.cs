using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Interfaces;

public interface ICompanionDefinitionProvider
{
    CompanionDefinition? GetById(string id);
    CompanionMove? GetMove(string companionDefId, string moveId);
    IReadOnlyList<CompanionDefinition> GetAll();
}
