using MapGenerator.Domain.Enums;

namespace MapGenerator.Combat.Models;

public enum MoveEffectTarget { Self, Enemy }

public class MoveEffect
{
    public CompanionStatus  Status   { get; init; }
    public MoveEffectTarget Target   { get; init; }
    public float            Chance   { get; init; } = 1.0f;
    public int              Duration { get; init; }
}
