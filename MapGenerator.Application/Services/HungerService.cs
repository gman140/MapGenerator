using MapGenerator.Domain.Enums;

namespace MapGenerator.Application.Services;

public static class HungerService
{
    public static HungerBand GetBand(double satiety) => satiety switch
    {
        >= 90 => HungerBand.Stuffed,
        >= 55 => HungerBand.Satiated,
        >= 30 => HungerBand.Peckish,
        >= 10 => HungerBand.Hungry,
        _     => HungerBand.Starving,
    };

    public static double GetCooldownMultiplier(double satiety) => satiety switch
    {
        >= 90 => 1.1,
        >= 55 => 1.0,
        >= 30 => 1.2,
        >= 10 => 1.5,
        _     => 2.0,
    };

    public static bool CanLayEgg(double satiety) => satiety > 10;

    // Drain per tile moved — scales with how hard the terrain is to cross.
    public static double DrainForMovement(long cooldownMs) =>
        0.25 + cooldownMs / 2000.0;
}
