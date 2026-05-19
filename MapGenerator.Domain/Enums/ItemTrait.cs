namespace MapGenerator.Domain.Enums;

[Flags]
public enum ItemTrait
{
    None             = 0,
    Resource         = 1 << 0,
    Equipment        = 1 << 1,
    CombatConsumable = 1 << 2,
    Loot             = 1 << 3,
}
