using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class EggExplosionService
{
    private static readonly string[] Openers =
    [
        "You see an egg glow red hot and detonate.",
        "You hear a crack and turn just in time to see an egg expand and explode.",
        "There are a lot of eggs here. You step on one. It screams and explodes.",
        "A soft hiss draws your attention to one egg in particular. It answers the question of whether you should be worried with a bang.",
        "One egg has been watching you since you arrived. It makes its move and erupts violently.",
        "You brush past an egg. It takes this personally and detonates.",
        "Something catches your eye — an egg, vibrating at an alarming frequency. Then it resolves the vibration with explosive violence.",
        "The largest egg here shudders once. You have no further warning as it self destructs.",
        "An egg near your feet begins to glow a deep, furious orange. It follows through like a grenade would.",
        "A faint hiss. You look down. The egg looks up. It explodes.",
        "One egg among the many has clearly had enough. It acts on this and explodes.",
        "You hear a sound like a kettle building pressure coming from an egg. Then the pressure furiously finds an exit.",
    ];

    private static readonly string[] Consequences =
    [
        "Stars fill your vision. You cannot move.",
        "The world tilts sideways. You are on your knees, ears ringing.",
        "Smoke, heat, silence. Everything is spinning.",
        "You are hurled backward. For a long moment you just stare at the sky.",
        "A concussive wave knocks the air from you. You forget, briefly, your own name.",
        "White light. Then ringing. Then the slow, humiliating process of trying to stand up.",
        "The ground is oddly comfortable. You stay there for a while.",
        "Then nothing. Then everything at once. You lie very still.",
        "You are briefly airborne. It is not pleasant.",
        "Your vision narrows to a pinpoint. You are entirely, helplessly still.",
        "You are, briefly, on fire. Then just on the ground.",
        "Then warmth. Then the intimate embrace of the earth beneath you.",
    ];

    private static readonly BiomeType[] AllBiomes = Enum.GetValues<BiomeType>();
    private static readonly double[] RingChance = [1.0, 0.80, 0.55, 0.25];

    public (bool exploded, string message, List<(int Q, int R, BiomeType NewBiome)> biomeChanges)
        TryExplode(Player player, HexTile tile)
    {
        if (tile.EggCount <= 0)
            return (false, string.Empty, []);

        bool hit = false;
        for (int i = 0; i < tile.EggCount; i++)
        {
            if (Random.Shared.NextDouble() < 0.01)
            {
                hit = true;
                break;
            }
        }

        if (!hit) return (false, string.Empty, []);

        tile.EggCount--;
        player.EggsDestroyed++;
        player.StunnedUntil = DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeMilliseconds();

        var opener      = Openers[Random.Shared.Next(Openers.Length)];
        var consequence = Consequences[Random.Shared.Next(Consequences.Length)];
        var message     = $"{opener} {consequence}";
        var biomeChanges = GenerateBlastZone(tile.Q, tile.R);
        return (true, message, biomeChanges);
    }

    private static List<(int Q, int R, BiomeType)> GenerateBlastZone(int centerQ, int centerR)
    {
        var blastBiome = AllBiomes[Random.Shared.Next(AllBiomes.Length)];
        var tiles = new List<(int Q, int R, BiomeType)> { (centerQ, centerR, blastBiome) };

        for (int dq = -3; dq <= 3; dq++)
        {
            for (int dr = Math.Max(-3, -dq - 3); dr <= Math.Min(3, -dq + 3); dr++)
            {
                if (dq == 0 && dr == 0) continue;
                int dist = Math.Max(Math.Abs(dq), Math.Max(Math.Abs(dr), Math.Abs(dq + dr)));
                if (Random.Shared.NextDouble() < RingChance[dist])
                    tiles.Add((centerQ + dq, centerR + dr, blastBiome));
            }
        }

        return tiles;
    }
}
