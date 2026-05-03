using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class PermissionService
{
    private static readonly IReadOnlySet<Permission> AdminPermissions = new HashSet<Permission>
    {
        Permission.IgnoreMovementCooldown,
        Permission.MoveToAnyTile,
        Permission.IgnoreEggCooldown,
    };

    private static readonly IReadOnlySet<Permission> NoPermissions = new HashSet<Permission>();

    public IReadOnlySet<Permission> GetPermissions(Player player) =>
        player.IsAdmin ? AdminPermissions : NoPermissions;
}
