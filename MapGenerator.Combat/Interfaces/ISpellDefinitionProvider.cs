using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Interfaces;

public interface ISpellDefinitionProvider
{
    IReadOnlyList<SpellDefinition> All { get; }
    SpellDefinition? GetById(string id);
}
