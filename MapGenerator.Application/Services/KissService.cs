using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class KissService
{
    private static readonly TimeSpan KissCooldown = TimeSpan.FromMinutes(1);

    private readonly IPlayerRepository _playerRepo;

    public KissService(IPlayerRepository playerRepo)
    {
        _playerRepo = playerRepo;
    }

    public async Task<(bool success, string kisserMsg, string kisseeMsg, string observerMsg)> KissAsync(
        Player kisser, IReadOnlySet<Permission> permissions, string targetId, string targetName)
    {
        if (!permissions.Contains(Permission.IgnoreCooldowns) && kisser.LastKissedAt.HasValue)
        {
            var remaining = KissCooldown - (DateTime.UtcNow - kisser.LastKissedAt.Value);
            if (remaining > TimeSpan.Zero)
                return (false, $"You need {remaining.TotalSeconds:F0}s before doing that again.", "", "");
        }

        int idx = Random.Shared.Next(KisserMessages.Length);
        string kisserMsg   = string.Format(KisserMessages[idx],   targetName);
        string kisseeMsg   = string.Format(KisseeMessages[idx % KisseeMessages.Length],  kisser.Username);
        string observerMsg = string.Format(ObserverMessages[idx % ObserverMessages.Length], kisser.Username, targetName);

        kisser.LastKissedAt = DateTime.UtcNow;
        await _playerRepo.UpdateAsync(kisser);

        return (true, kisserMsg, kisseeMsg, observerMsg);
    }

    private static readonly string[] KisserMessages =
    [
        "You kiss {0}. It goes about as well as these things ever do.",
        "You plant a kiss on {0}. The kiss lands. The implications are unclear.",
        "You kiss {0} with the confidence of someone who has not thought this through.",
        "You lean in and kiss {0}. There is a moment of shared awareness. Then it passes.",
        "You kiss {0}. Boldly. Perhaps too boldly. That is a question for later.",
        "You kiss {0}. The moment is brief, warm, and immediately complicated.",
        "You kiss {0}. You do not regret it. That judgment may be premature.",
        "You go ahead and kiss {0}. The decision was made before you finished making it.",
        "You kiss {0}. This was always going to happen. You feel that now.",
        "You kiss {0}. One of you is surprised. You decide it was them.",
        "You kiss {0}. The kiss is decisive. The aftermath is less so.",
        "You kiss {0}. Something has changed. You are not yet sure what.",
    ];

    private static readonly string[] KisseeMessages =
    [
        "{0} has kissed you. You were not prepared. Few things about {0} are preparable for.",
        "{0} just kissed you. You are processing this. Give it a moment.",
        "You have been kissed by {0}. Take a moment. Several moments. You have earned them.",
        "{0} kissed you. You didn't see that coming. Arguably you should have.",
        "{0} has kissed you. The situation has developed.",
        "You have been kissed by {0}. The world continues turning, somewhat confusingly.",
        "{0} kissed you. You have thoughts about this. You will have more of them later.",
        "{0} just kissed you. You are briefly unsure of the date, the time, and your location.",
        "You were just kissed by {0}. Your position on this is still forming.",
        "{0} kissed you. Your composure, formerly reliable, has taken the rest of the day off.",
        "{0} kissed you. You did not stop them. This is also information.",
        "{0} kissed you. The appropriate response is still being decided by parts of you that haven't spoken yet.",
    ];

    private static readonly string[] ObserverMessages =
    [
        "{0} just kissed {1}. You witnessed this. You have complicated feelings about it.",
        "{0} kissed {1}. The two of them briefly became a situation. You are adjacent to it.",
        "Something happened between {0} and {1}. You caught all of it. You pretend you didn't.",
        "{0} and {1} just kissed. You were right here. You are now unsure where to look.",
        "{0} kissed {1}. You are trying to remember if you knew this was coming.",
        "{0} kissed {1}. The air here has changed. You are responsible for none of it.",
        "You watched {0} kiss {1}. You did not intend to. You could not look away.",
        "{0} kissed {1} in front of you. You are a witness to something. You are processing what.",
        "{0} kissed {1}. You were here for the whole thing. They seem unaware of this.",
        "{0} and {1}: a development. You are trying to figure out what this means for you. Nothing, probably.",
        "{0} kissed {1} without any apparent hesitation. You admire this and also find it alarming.",
        "{0} kissed {1}. You want it noted that you were here. You are noting it yourself.",
    ];
}
