using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface ISettlementRoleDefinitionProvider
{
    IReadOnlyList<SettlementRoleDefinition> All { get; }
    SettlementRoleDefinition? GetByRole(SettlementTileRole role);
}
