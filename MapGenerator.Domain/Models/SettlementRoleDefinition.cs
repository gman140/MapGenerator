using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class SettlementRoleDefinition
{
    public SettlementTileRole Role { get; init; }
    public string Name { get; init; } = string.Empty;
    public string MapIcon { get; init; } = string.Empty;
}
