using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public static class BiomeProperties
{
    public static long CooldownMs(BiomeType biome) => biome switch
    {
        BiomeType.Ocean     =>    0,
        BiomeType.Shallows  => 1200,
        BiomeType.Beach     =>  400,
        BiomeType.River     => 1500,
        BiomeType.Swamp     => 3000,
        BiomeType.Marsh     => 2000,
        BiomeType.Grassland =>  400,
        BiomeType.Plains    =>  400,
        BiomeType.Savanna   =>  500,
        BiomeType.Forest    =>  700,
        BiomeType.Jungle    => 1200,
        BiomeType.Desert    =>  900,
        BiomeType.Mountain  => 2000,
        BiomeType.Tundra    => 1800,
        BiomeType.Snow      => 2500,
        BiomeType.Glacier   => 3000,
        BiomeType.Volcano   =>    0,
        BiomeType.Lake      =>    0,
        _                   =>  400,
    };

    public static (byte R, byte G, byte B) MinimapRgb(BiomeType biome) => biome switch
    {
        BiomeType.Ocean     => ( 28,  78, 140),
        BiomeType.Lake      => ( 58, 110, 165),
        BiomeType.Shallows  => ( 58, 138, 170),
        BiomeType.Beach     => (210, 195, 140),
        BiomeType.River     => ( 64, 120, 190),
        BiomeType.Swamp     => ( 60,  80,  55),
        BiomeType.Marsh     => ( 85, 104,  72),
        BiomeType.Grassland => (110, 175,  65),
        BiomeType.Plains    => (185, 175,  90),
        BiomeType.Savanna   => (200, 184,  80),
        BiomeType.Forest    => ( 40,  90,  40),
        BiomeType.Jungle    => ( 26,  92,  26),
        BiomeType.Desert    => (210, 175, 100),
        BiomeType.Mountain  => (130, 125, 120),
        BiomeType.Tundra    => (142, 168, 152),
        BiomeType.Snow      => (230, 235, 240),
        BiomeType.Glacier   => (200, 220, 232),
        BiomeType.Volcano   => ( 90,  40,  24),
        _                   => (100, 100, 100),
    };
}
