using MapGenerator.Domain.Models;

namespace MapGenerator.Combat.Interfaces;

public interface IWeaponAttackProvider
{
    IReadOnlyList<WeaponAttackDefinition> All { get; }
    WeaponAttackDefinition? GetById(string id);
}
