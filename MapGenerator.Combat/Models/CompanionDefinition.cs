using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public class CompanionDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DamageType ElementType { get; init; }
    public int BaseAttack { get; init; }
    public string Emoji { get; init; } = "🐾";
    public string[] DefaultSprite { get; init; } = [];
    public List<CompanionMove> Moves { get; init; } = [];
}
