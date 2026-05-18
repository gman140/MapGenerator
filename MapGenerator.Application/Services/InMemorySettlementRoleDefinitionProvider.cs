using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemorySettlementRoleDefinitionProvider : ISettlementRoleDefinitionProvider
{
    private static readonly SettlementRoleDefinition[] _definitions =
    [
        new()
        {
            Role    = SettlementTileRole.Center,
            Name    = "Town Center",
            MapIcon = "<rect x='-8' y='-2' width='16' height='12' class='settle-center' rx='1'/><polygon points='-10,-2 0,-13 10,-2' class='settle-center-roof'/>",
        },
        new()
        {
            Role    = SettlementTileRole.Residential,
            Name    = "Residence",
            MapIcon = "<rect x='-5' y='0' width='10' height='8' class='settle-residential' rx='1'/><polygon points='-7,0 0,-9 7,0' class='settle-residential-roof'/>",
        },
        new()
        {
            Role    = SettlementTileRole.Market,
            Name    = "Market",
            MapIcon = "<rect x='-7' y='-1' width='14' height='8' class='settle-market' rx='1'/><line x1='-9' y1='-1' x2='9' y2='-1' class='settle-market-awning'/>",
        },
        new()
        {
            Role    = SettlementTileRole.Farm,
            Name    = "Farm",
            MapIcon = "<line x1='-9' y1='-2' x2='9' y2='-2' class='settle-farm'/><line x1='-9' y1='1' x2='9' y2='1' class='settle-farm'/><line x1='-9' y1='4' x2='9' y2='4' class='settle-farm'/>",
        },
        new()
        {
            Role    = SettlementTileRole.Guard,
            Name    = "Guard Post",
            MapIcon = "<rect x='-5' y='-12' width='10' height='15' class='settle-guard'/><rect x='-7' y='-15' width='14' height='4' class='settle-guard'/>",
        },
        new()
        {
            Role    = SettlementTileRole.Inn,
            Name    = "Inn",
            MapIcon = "<rect x='-7' y='-1' width='14' height='10' class='settle-inn' rx='1'/><polygon points='-9,-1 0,-11 9,-1' class='settle-inn-roof'/>",
        },
        new()
        {
            Role    = SettlementTileRole.Mill,
            Name    = "Mill",
            MapIcon = "<circle cx='0' cy='-3' r='6' class='settle-mill'/><line x1='0' y1='-10' x2='0' y2='4' class='settle-mill-arm'/><line x1='-7' y1='-3' x2='7' y2='-3' class='settle-mill-arm'/>",
        },
    ];

    private static readonly Dictionary<SettlementTileRole, SettlementRoleDefinition> _byRole =
        _definitions.ToDictionary(d => d.Role);

    public IReadOnlyList<SettlementRoleDefinition> All => _definitions;

    public SettlementRoleDefinition? GetByRole(SettlementTileRole role) =>
        _byRole.TryGetValue(role, out var def) ? def : null;
}
