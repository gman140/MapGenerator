using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class CompanionDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<DamageType> ElementTypes { get; init; } = [];
    public int BaseAttack { get; init; }
    public string[] DefaultSprite { get; init; } = [];

    // Base stats — contribute to derived battle values alongside allocation and temperament
    public int BaseVitality { get; init; } = 5;  // HP pool
    public int BaseDefense  { get; init; } = 2;  // flat damage reduction
    public int BaseSpeed    { get; init; } = 5;  // turn order
    public int BaseFocus    { get; init; } = 2;  // crit chance bonus
    public int BaseResist   { get; init; } = 1;  // type-weakness dampening
}
