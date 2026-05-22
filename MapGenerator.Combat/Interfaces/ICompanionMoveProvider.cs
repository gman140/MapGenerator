using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Interfaces;

public interface ICompanionMoveProvider
{
    IReadOnlyList<CompanionMove> All { get; }
    CompanionMove? GetById(string id);
    IReadOnlyList<CompanionMove> GetEligibleFor(IEnumerable<DamageType> elementTypes);
}
