using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class EggService
{
    private static readonly TimeSpan EggCooldown = TimeSpan.FromMinutes(1);

    private readonly IMapRepository _mapRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly MapGeneratorService _mapCache;

    public EggService(IMapRepository mapRepo, IPlayerRepository playerRepo, MapGeneratorService mapCache)
    {
        _mapRepo = mapRepo;
        _playerRepo = playerRepo;
        _mapCache = mapCache;
    }

    public async Task<(bool success, string message, int eggCount)> LayEggAsync(
        Player player, IReadOnlySet<Permission> permissions)
    {
        if (!permissions.Contains(Permission.IgnoreCooldowns) && player.LastEggLaidAt.HasValue)
        {
            var remaining = EggCooldown - (DateTime.UtcNow - player.LastEggLaidAt.Value);
            if (remaining > TimeSpan.Zero)
                return (false, $"You need to rest {remaining.TotalSeconds:F0}s before laying another egg.", 0);
        }

        var newCount = await _mapRepo.IncrementEggCountAsync(player.Q, player.R);
        _mapCache.UpdateCachedEggCount(player.Q, player.R, newCount);
        player.LastEggLaidAt = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        return (true, GetEggDescriptor(newCount), newCount);
    }

    public static string GetEggDescriptor(int count) => count switch
    {
        1  => "A single, lonely egg sits here.",
        2  => "Two eggs. A modest collection.",
        3  => "Three eggs. Someone is committed.",
        4  => "Four eggs. This is getting weird.",
        5  => "Five eggs. What are you doing.",
        6  => "Six eggs. The ground is running out of room.",
        7  => "Seven eggs. A sacred number, maybe?",
        8  => "Eight eggs. An entire octet of eggs.",
        9  => "Nine eggs. Approaching dangerous levels.",
        10 => "Ten eggs. You have a problem.",
        11 => "Eleven eggs. The eggs are aware of each other now.",
        12 => "Twelve eggs. A full dozen. The universe is deeply unsettled.",
        13 => "Thirteen eggs. An unlucky number for everyone except the eggs.",
        14 => "Fourteen eggs. Some of them are making eye contact.",
        15 => "Fifteen eggs. Local wildlife has begun to worship this spot.",
        16 => "Sixteen eggs. This violates at least three ordinances.",
        17 => "Seventeen eggs. A prime number of eggs, if you believe in omens.",
        18 => "Eighteen eggs. The ground has accepted its fate.",
        19 => "Nineteen eggs. You are one egg from a very round number and somehow that makes it worse.",
        20 => "Twenty eggs. A round number. The eggs have organized.",
        21 => "Twenty-one eggs. Blackjack. The house always loses.",
        22 => "Twenty-two eggs. They are arranged in a pattern you don't fully understand.",
        23 => "Twenty-three eggs. Scientists have been called. They are not okay.",
        24 => "Twenty-four eggs. Two dozen. This is a commercial operation now.",
        25 => "Twenty-five eggs. A quarter century of eggs. Someone will write a thesis.",
        26 => "Twenty-six eggs. One for every letter of the alphabet. Coincidence? The eggs say no.",
        27 => "Twenty-seven eggs. The pile has begun to hum softly.",
        28 => "Twenty-eight eggs. The eggs have started leaving notes.",
        29 => "Twenty-nine eggs. You are so close to thirty and the eggs know it.",
        30 => "Thirty eggs. A milestone. The eggs have elected a spokesperson.",
        31 => "Thirty-one eggs. The spokesperson has issued a statement. It was just the word 'more.'",
        32 => "Thirty-two eggs. A suspicious power of two. The eggs are computing something.",
        33 => "Thirty-three eggs. The pattern is becoming clearer. You wish it weren't.",
        34 => "Thirty-four eggs. Neighboring tiles have begun to look nervous.",
        35 => "Thirty-five eggs. A committee has formed to address the egg situation. The committee is eggs.",
        36 => "Thirty-six eggs. Six by six. The eggs have achieved geometry.",
        37 => "Thirty-seven eggs. Statistically the most commonly guessed egg count. Somehow that's worse.",
        38 => "Thirty-eight eggs. The eggs have drafted a constitution.",
        39 => "Thirty-nine eggs. One short of forty. The eggs are being theatrical about it.",
        40 => "Forty eggs. The eggs have declared this land sovereign territory.",
        41 => "Forty-one eggs. A cartographer has arrived to map the egg cluster. She has not returned.",
        42 => "Forty-two eggs. The answer to life, the universe, and apparently this.",
        43 => "Forty-three eggs. The eggs are no longer reacting to sudden movements.",
        44 => "Forty-four eggs. A palindrome of eggs. They are looking at themselves in a mirror.",
        45 => "Forty-five eggs. Local legends are forming. Children are told not to come here at night.",
        46 => "Forty-six eggs. The eggs have begun construction on something. No one knows what.",
        47 => "Forty-seven eggs. A prime number of eggs. The eggs have rejected divisibility.",
        48 => "Forty-eight eggs. Four dozen. An industrial quantity. Regulators are involved.",
        49 => "Forty-nine eggs. Seven squared. The eggs have achieved a higher mathematical form.",
        50 => "Fifty eggs. Half a hundred. The eggs held a ceremony. You were not invited.",
        51 => "Fifty-one eggs. The eggs have voted. The motion passed unanimously.",
        52 => "Fifty-two eggs. One for every week of the year. The eggs are playing a long game.",
        53 => "Fifty-three eggs. A prime. The eggs will not be factored.",
        54 => "Fifty-four eggs. The egg spokesperson has written a memoir.",
        55 => "Fifty-five eggs. A Fibonacci number. The eggs are aware of this.",
        56 => "Fifty-six eggs. The eggs have begun to lobby local government.",
        57 => "Fifty-seven eggs. Heinz varieties. The eggs have diversified their brand.",
        58 => "Fifty-eight eggs. Two away from sixty. The eggs are milking the suspense.",
        59 => "Fifty-nine eggs. A prime. The eggs have refused to comment further.",
        60 => "Sixty eggs. Five dozen. The eggs have incorporated as a limited liability company.",
        61 => "Sixty-one eggs. The company's first quarterly report was just an egg.",
        62 => "Sixty-two eggs. Structural engineers have been consulted. They left crying.",
        63 => "Sixty-three eggs. The eggs have started a newsletter. It is very well written.",
        64 => "Sixty-four eggs. Eight by eight. The eggs have arranged themselves as a chessboard. You are a pawn.",
        65 => "Sixty-five eggs. Retirement age for eggs. The oldest ones look tired.",
        66 => "Sixty-six eggs. Route 66. The eggs are going somewhere and you cannot stop them.",
        67 => "Sixty-seven eggs. A prime. The eggs have established a religion. Attendance is mandatory.",
        68 => "Sixty-eight eggs. One short of sixty-nine. The eggs find this hilarious.",
        69 => "Sixty-nine eggs. The eggs are immature about this. So are you.",
        70 => "Seventy eggs. The eggs have filed a noise complaint against the wind.",
        71 => "Seventy-one eggs. A prime. The eggs have transcended your understanding of prime.",
        72 => "Seventy-two eggs. Six dozen. The eggs have achieved bulk pricing.",
        73 => "Seventy-three eggs. Sheldon Cooper's favorite number. The eggs are smug about this.",
        74 => "Seventy-four eggs. The eggs have started a podcast. It is extremely long.",
        75 => "Seventy-five eggs. Three quarters of a hundred. The eggs are approaching something. You feel it.",
        76 => "Seventy-six eggs. The eggs have commissioned a statue of themselves.",
        77 => "Seventy-seven eggs. Lucky sevens, doubled. The eggs have won something. You have lost it.",
        78 => "Seventy-eight eggs. The eggs have hired legal counsel.",
        79 => "Seventy-nine eggs. A prime. The eggs' lawyer is also an egg.",
        80 => "Eighty eggs. The eggs have annexed the surrounding tiles diplomatically.",
        81 => "Eighty-one eggs. Nine squared. The eggs have achieved a perfect square. They are very proud.",
        82 => "Eighty-two eggs. The eggs have begun terraforming.",
        83 => "Eighty-three eggs. A prime. The eggs have stopped returning your calls.",
        84 => "Eighty-four eggs. Seven dozen. The eggs have stopped acknowledging your existence.",
        85 => "Eighty-five eggs. The egg civilization has entered its golden age.",
        86 => "Eighty-six eggs. To eighty-six something means to remove it. The eggs know this. They are looking at you.",
        87 => "Eighty-seven eggs. The eggs have developed a written language. It is unreadable.",
        88 => "Eighty-eight eggs. Two figure-eights. The eggs have achieved infinity, twice.",
        89 => "Eighty-nine eggs. A prime. A Fibonacci number. The eggs are flexing.",
        90 => "Ninety eggs. The eggs have built a capital city. There is a flag.",
        91 => "Ninety-one eggs. The flag depicts an egg.",
        92 => "Ninety-two eggs. The egg nation has applied for UN membership.",
        93 => "Ninety-three eggs. The application was approved. The eggs gave a speech.",
        94 => "Ninety-four eggs. The speech was just the word 'egg' for forty minutes.",
        95 => "Ninety-five eggs. A standing ovation followed. You were not there.",
        96 => "Ninety-six eggs. Eight dozen. The eggs have an economy now. You cannot afford to live here.",
        97 => "Ninety-seven eggs. A prime. The egg GDP is higher than several real nations.",
        98 => "Ninety-eight eggs. Two away from one hundred. The eggs are preparing a celebration.",
        99 => "Ninety-nine eggs. One short. The eggs are savoring this moment. Let them have it.",
        100 => "One hundred eggs. A perfect century. The eggs have achieved what no egg has achieved before.",
        >= 101          => $"{count} eggs. You have achieved egg singularity.",
        _               => $"{count} eggs cluster here.",
    };
}
