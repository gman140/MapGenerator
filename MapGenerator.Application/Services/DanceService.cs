using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class DanceService
{
    private readonly IMapRepository _mapRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly MapGeneratorService _mapCache;

    public DanceService(IMapRepository mapRepo, IPlayerRepository playerRepo, MapGeneratorService mapCache)
    {
        _mapRepo    = mapRepo;
        _playerRepo = playerRepo;
        _mapCache   = mapCache;
    }

    private static readonly TimeSpan DanceCooldown = TimeSpan.FromMinutes(1);

    public async Task<(bool success, string message, bool eggDestroyed, int newEggCount)> DanceAsync(
        Player player, IReadOnlySet<Permission> permissions)
    {
        if (!permissions.Contains(Permission.IgnoreCooldowns) && player.LastDancedAt.HasValue)
        {
            var remaining = DanceCooldown - (DateTime.UtcNow - player.LastDancedAt.Value);
            if (remaining > TimeSpan.Zero)
                return (false, $"You need to catch your breath for {remaining.TotalSeconds:F0}s before dancing again.", false, 0);
        }

        var tile     = _mapCache.GetCachedTile(player.Q, player.R);
        bool hasEggs = tile != null && tile.EggCount > 0;

        string message    = DanceDescriptions[Random.Shared.Next(DanceDescriptions.Length)];
        int newEggCount   = tile?.EggCount ?? 0;
        bool eggDestroyed = false;

        if (hasEggs)
        {
            message += " " + EggEndings[Random.Shared.Next(EggEndings.Length)];
            int decremented = await _mapRepo.DecrementEggCountAsync(player.Q, player.R);
            if (decremented >= 0)
            {
                newEggCount  = decremented;
                eggDestroyed = true;
                _mapCache.UpdateCachedEggCount(player.Q, player.R, newEggCount);
                player.EggsDestroyed++;
            }
        }

        player.LastDancedAt = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(player);

        return (true, message, eggDestroyed, newEggCount);
    }

    public static string? GetTitle(int eggsDestroyed) => eggsDestroyed switch
    {
        0      => null,
        1      => "Accidental Omelettist",
        <= 5   => "Habitual Egg Menace",
        <= 15  => "Certified Yolk Killer",
        <= 30  => "Serial Scrambler",
        <= 60  => "Egg Executioner",
        <= 100 => "Architect of Yolk Tragedy",
        <= 200 => "Destroyer of Clutches",
        _      => "The Omelette That Walks",
    };

    private static readonly string[] DanceDescriptions =
    [
        "You dance with surprising confidence for someone who has clearly not practiced.",
        "You begin to dance. Nearby wildlife looks confused, then disinterested, then gone.",
        "Your movements could generously be described as dancing. You prefer this generous interpretation.",
        "You dance. A bird overhead changes course. The connection may be coincidental.",
        "You execute a series of moves that have never been attempted before, possibly for good reason.",
        "You dance with the conviction of someone who has misread the room several times before and not learned from it.",
        "The dance happens. It is neither graceful nor intentional, but it is committed.",
        "You sway. Call it dancing. You are.",
        "You dance like no one is watching, which is good, because you dance like no one should watch.",
        "Your footwork is creative. Your arms are their own decision entirely.",
        "You spin once, then slightly too many more times. You stop. The world continues to spin.",
        "You dance. The ground neither approves nor disapproves. It is simply ground.",
        "Your dancing has a quality to it that might be called 'free-form' by someone being kind.",
        "You dance. There is effort. There is intention. There is a concerning amount of elbows.",
        "You begin dancing and immediately commit to a decision you should have reconsidered.",
        "You dance the way you make most of your decisions: with confidence and without preparation.",
        "The dance is happening. It continues longer than it probably should.",
        "You find the rhythm immediately. You lose it almost as fast. You carry on without it.",
        "Your dancing is expressive. What it is expressing is not entirely clear, but it is expressive.",
        "You give the dance your full commitment. The dance is not sure what to do with this.",
        "You dance in a manner that suggests this seemed like a better idea before you started.",
        "You attempt a complex maneuver. It becomes a different, simpler maneuver. You land it. Barely.",
        "You dance as a personal statement. The statement is: 'I am dancing.'",
        "You sway with purpose. You stomp with intent. You pivot with what you choose to call grace.",
        "You dance until you are confident that you have danced, then you stop.",
        "Your dancing communicates a rich inner emotional landscape. The landscape is chaos.",
        "You move to a rhythm that exists primarily inside your own head. You follow it faithfully.",
        "You execute something that might be called a flourish. It was definitely something.",
        "The dance you perform is unique in the world. This is both true and concerning.",
        "You dance with the quiet authority of someone who has decided that dancing is happening and nothing will stop it.",
        "Several witnesses would describe your dancing differently. None of the descriptions would be flattering.",
        "You stumble into what becomes, to the charitable eye, a dance. You lean into it.",
        "Your interpretation of dance is original in the way that most original things are inadvertent.",
        "You perform moves that suggest a deep personal relationship with falling, interrupted at the last moment.",
        "The dance proceeds. It is unclear who is in charge, you or the dance. The dance seems comfortable either way.",
    ];

    private static readonly string[] EggEndings =
    [
        "Mid-spin, your heel finds an egg that had not anticipated being found this way. The egg does not survive the introduction.",
        "Your foot connects with an egg. The connection is decisive. So is the result.",
        "You step on an egg. It takes a moment to process. The egg had already processed everything it was going to.",
        "Your enthusiastic footwork encounters an egg. The egg loses.",
        "In the commotion, an egg is destroyed beneath your heel. You only notice afterward.",
        "You land on an egg with full commitment. The egg meets this commitment and raises it: nothing.",
        "As you pivot, your boot describes a short arc and at the end of that arc is an egg. Was an egg.",
        "An egg makes contact with your foot during a particularly ambitious move. This is the egg's last contact with anything.",
        "Your foot settles squarely on an egg. The egg, which had done nothing to deserve this, is gone.",
        "Mid-routine, you crush an egg. You are not proud of this. You are also not stopping.",
        "The dance claims a casualty: one egg. It didn't see you coming.",
        "You notice, only once it is too late, that one of your feet has committed an irreversible act upon an egg.",
        "One egg will not see tomorrow. Your footwork has made sure of that.",
        "An egg is destroyed beneath you. The others watch. This does not stop you.",
        "Your dancing is enthusiastic. The egg you have just crushed would agree, if it could.",
        "With a decisive stomp that was not meant for this purpose, you destroy an egg. You accept this.",
    ];
}
