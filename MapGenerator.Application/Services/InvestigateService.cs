using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InvestigateService
{
    private readonly ITileNoteRepository _noteRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly IFeatureDefinitionProvider _featureProvider;
    private readonly IBiomeDefinitionProvider _biomeProvider;
    private readonly MapGeneratorService _mapCache;
    private readonly SettlementCacheService _settlementCache;

    public InvestigateService(
        ITileNoteRepository noteRepo,
        IPlayerTileVisitRepository visitRepo,
        IFeatureDefinitionProvider featureProvider,
        IBiomeDefinitionProvider biomeProvider,
        MapGeneratorService mapCache,
        SettlementCacheService settlementCache)
    {
        _noteRepo        = noteRepo;
        _visitRepo       = visitRepo;
        _featureProvider = featureProvider;
        _biomeProvider   = biomeProvider;
        _mapCache        = mapCache;
        _settlementCache = settlementCache;
    }

    public async Task<(string flavorText, List<TileNote> notes)> InvestigateAsync(Player player)
    {
        var tile       = _mapCache.GetCachedTile(player.Q, player.R);
        var visitCount = await _visitRepo.CountVisitsAsync(player.Id, player.Q, player.R);
        var neighbors  = GetNeighborTiles(player.Q, player.R);
        var flavor     = tile != null
            ? BuildFlavorText(tile.Biome, tile.FeatureId, player.Q, player.R, visitCount, neighbors)
            : "You look around carefully. There is not much to see. You look again. Still nothing. You are thorough and it has not helped.";

        var (sName, sRole, sType) = _settlementCache.GetSettlementInfo(player.Q, player.R);
        if (sName != null && sRole.HasValue && sType.HasValue)
        {
            var settlementNote = GetSettlementNote(sName, sRole.Value, sType.Value, player.Q, player.R);
            if (!string.IsNullOrEmpty(settlementNote))
                flavor = flavor + " " + settlementNote;
        }

        var notes = await _noteRepo.GetNotesForTileAsync(player.Q, player.R);
        return (flavor, notes);
    }

    private List<HexTile> GetNeighborTiles(int q, int r)
    {
        (int dq, int dr)[] dirs = [(1, 0), (-1, 0), (0, 1), (0, -1), (1, -1), (-1, 1)];
        var result = new List<HexTile>(6);
        foreach (var (dq, dr) in dirs)
        {
            var t = _mapCache.GetCachedTile(q + dq, r + dr);
            if (t != null) result.Add(t);
        }
        return result;
    }

    // ── Core assembly ────────────────────────────────────────────────────────

    private string BuildFlavorText(BiomeType biome, string? featureId, int q, int r, int visitCount, List<HexTile> neighbors)
    {
        // Rare event: ~4% per tile per day — completely overrides normal text
        var rareHash = Math.Abs(q * 31 + r * 37 + DateTime.UtcNow.DayOfYear * 7919);
        if (rareHash % 25 == 0)
            return RareEvents[rareHash % RareEvents.Length];

        int hashA = Math.Abs(q * 11 + r * 17);
        int hashB = Math.Abs(q * 7  + r * 23);

        string body;
        var def = _featureProvider.GetById(featureId);
        if (def != null)
        {
            var prefix   = GetTimePrefix(def.Category, GetTimeOfDay(), q, r);
            var bodyText = $"{def.PartA[hashA % def.PartA.Length]} {def.PartB[hashB % def.PartB.Length]}";
            body = string.IsNullOrEmpty(prefix) ? bodyText : $"{prefix} {bodyText}";
        }
        else
        {
            var biomeDef = _biomeProvider.GetByType(biome);
            var partA = biomeDef?.InvestigatePartA is { Length: > 0 } a ? a : ["You look around carefully."];
            var partB = biomeDef?.InvestigatePartB is { Length: > 0 } b ? b : ["There is not much to see."];
            body = $"{partA[hashA % partA.Length]} {partB[hashB % partB.Length]}";
        }

        var visitIntro   = GetVisitIntro(visitCount, q, r);
        var neighborNote = GetNeighborInfluence(biome, neighbors, q, r);

        var sb = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(visitIntro)) sb.Append(visitIntro).Append(' ');
        sb.Append(body);
        if (!string.IsNullOrEmpty(neighborNote)) sb.Append(' ').Append(neighborNote);
        return sb.ToString();
    }

    // ── Time of day ───────────────────────────────────────────────────────────

    private enum TimeOfDay { Dawn, Morning, Afternoon, Dusk, Night }

    private static TimeOfDay GetTimeOfDay()
    {
        int h = DateTime.Now.Hour;
        if (h is >= 4  and < 7)  return TimeOfDay.Dawn;
        if (h is >= 7  and < 12) return TimeOfDay.Morning;
        if (h is >= 12 and < 17) return TimeOfDay.Afternoon;
        if (h is >= 17 and < 20) return TimeOfDay.Dusk;
        return TimeOfDay.Night;
    }

    private static string GetTimePrefix(FeatureCategory cat, TimeOfDay time, int q, int r)
    {
        int idx = Math.Abs(q * 13 + r * 29);
        string[] options = (cat, time) switch
        {
            (FeatureCategory.Nature, TimeOfDay.Dawn) =>
            [
                "In the first light, the natural world is resuming a conversation you interrupted.",
                "Dawn here arrives quietly, the way things do when they are not performing for anyone.",
                "The first birds sound surprised, as they always do, as though night is a trick that keeps working.",
                "Something lifts here at dawn that you hadn't noticed was pressing down.",
                "The world is reassembling itself at the edges of visibility.",
                "Everything here has been doing something all night. You have interrupted it.",
                "The trees hold the dark a little longer than the sky does.",
                "Dawn arrives as if it remembers this place fondly.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Morning) =>
            [
                "The morning has arrived and brought everything along with it.",
                "The morning is doing what it always does — making things look manageable.",
                "Morning light here is generous in a way afternoon light forgets to be.",
                "In the full morning, the natural world is operating at its most straightforward.",
                "The light here is complete now. Everything is available for inspection.",
                "Morning in a natural place is a kind of consensus.",
                "You are not the first thing awake here, but you are probably the most distracted.",
                "The full morning has settled in and found everything acceptable.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Afternoon) =>
            [
                "The afternoon has settled in and is not going anywhere soon.",
                "The afternoon slows everything down. This is not an accident.",
                "The insects are at peak confidence now. This is their best hour.",
                "Afternoon here has the quality of a held note — long, sustained, certain of itself.",
                "Midday heat has a way of making everything feel decided.",
                "Almost nothing is moving here that doesn't have to.",
                "Everything here has made its peace with the afternoon.",
                "The afternoon light is honest and unforgiving and everywhere.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Dusk) =>
            [
                "The light has gone golden and tentative, the way it gets when it's leaving.",
                "The last light finds all the best angles before it goes.",
                "At dusk the natural world begins its other shift.",
                "Something passes through the air at dusk here that doesn't have a name yet.",
                "The day is folding itself away. The natural world is helping.",
                "The light is going and taking its particular opinions about color with it.",
                "At dusk, the birds are making their final announcements.",
                "Dusk here has a way of making the ordinary feel significant.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Night) =>
            [
                "In the dark, the living world stops performing and becomes itself.",
                "The night here is full of things you cannot identify but feel moving.",
                "In the dark, the natural world has a different agreement with you.",
                "Night changes the terms here. You are no longer the obvious one.",
                "The sounds here at night are not alarming. They are simply not yours.",
                "You are less significant here at night. This is not an insult.",
                "Something is moving at the edge of what you can hear.",
                "The night here is occupied. You are passing through it.",
            ],

            (FeatureCategory.Ruins, TimeOfDay.Dawn) =>
            [
                "In the early light, the ruins look almost whole.",
                "Dawn makes ruins optimistic. This is one of dawn's few cruelties.",
                "At first light the old stones hold their shadows differently — as if remembering.",
                "The ruins in dawn light have stopped being ruins for a moment. The moment does not last.",
                "The early light touches the highest stones first. They hold it.",
                "The first light here moves across old walls like a slow question.",
                "At dawn the ruins are neither old nor young. They are just here.",
                "Dawn does something to ruins that nothing else does. It makes them look possible.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Morning) =>
            [
                "The morning light makes the old stones look considered, almost purposeful.",
                "Morning finds what's left and illuminates it with no particular mercy.",
                "In the morning the ruins are at their most archaeological. Things have happened here. Clearly.",
                "Shadows shorten in the morning. The ruins look most like themselves at this hour.",
                "Under full morning light, nothing about this place is softened.",
                "Morning here illuminates without editorializing.",
                "The ruined walls in morning light cast shadows that fall exactly where they always have.",
                "The morning light is thorough. It does not leave any corner of the ruin to imagination.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Afternoon) =>
            [
                "In full daylight the ruin is just a ruin. This is, somehow, still affecting.",
                "The afternoon light is flat and honest. It does not do the ruins any favors. It does them justice.",
                "Heat shimmers off the old stones in the afternoon. History becomes a physical sensation.",
                "Midday: the shadows shrink and the ruins are fully visible. Everything that happened here is fully visible.",
                "The afternoon sun sits on the stones heavily, the way time does.",
                "In full light, the ruin is complete — in its own fashion.",
                "The stones are warm to the touch. They have been warm since morning.",
                "You are standing in the full afternoon visibility of something that ended.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Dusk) =>
            [
                "The last light retreats from here slowly, as if it has been here before and is reluctant.",
                "At dusk the shadows lengthen and the ruins gain back what the centuries took.",
                "The dusk light turns the stonework warm. It feels like a condolence.",
                "Something about dusk returns this place to itself.",
                "The long shadows at dusk could belong to the walls they once were.",
                "Dusk returns some dignity to the ruin that daylight was less interested in.",
                "The warm light at dusk makes old stone look like intention.",
                "At dusk, the boundary between what this was and what it is becomes negotiable.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Night) =>
            [
                "The ruins at night are patient in a way they are not during the day.",
                "At night the ruins are honest about what they are and what they aren't.",
                "The dark gives the ruins a kind of authority the daylight argues with.",
                "Ruins at night stop pretending to be anything. You appreciate the honesty.",
                "Whatever happened here, the night doesn't judge it.",
                "The ruins at night are listening in a way they don't during the day.",
                "In the dark, the shape of this place is suggestion more than fact.",
                "Night here makes the ruins feel inhabited again. Not comfortably.",
            ],

            (FeatureCategory.Water, TimeOfDay.Dawn) =>
            [
                "The water holds the early quiet like it doesn't want to give it back.",
                "At dawn the water is still and silver and not yet asked to be anything.",
                "The first light finds the water and the water holds it carefully.",
                "Dawn here begins at the water and moves outward from it, slowly.",
                "The water has been awake all night and shows no sign of it.",
                "The water at dawn is not yet asking anything of you.",
                "First light settles on the water the way it settles nowhere else.",
                "Mist is possible here at dawn. Today, it is.",
            ],
            (FeatureCategory.Water, TimeOfDay.Morning) =>
            [
                "The morning light catches the water as if it meant to.",
                "The water in morning light is extravagantly itself.",
                "Morning finds the water in good form, as it usually does.",
                "The water in the morning is working hard and doesn't mind being watched.",
                "The water in morning light refuses to be ignored.",
                "Morning and water have a long-standing arrangement. You are witnessing it.",
                "Everything near the water has an opinion about the morning.",
                "The water today is bright and specific and uncomplicated.",
            ],
            (FeatureCategory.Water, TimeOfDay.Afternoon) =>
            [
                "The afternoon heat makes everything water-related feel like a decision.",
                "The afternoon light pushes straight down and the water takes it personally.",
                "Midday and the water is bright and competitive about it.",
                "The water in afternoon light is entirely unreserved. Everything is visible. Nothing is subtle.",
                "The glare off the water in the afternoon is a whole argument.",
                "The water at midday has stopped being subtle about it.",
                "Heat and water in the afternoon reach a kind of understanding. You are beside the point.",
                "The afternoon makes the water look like it means business.",
            ],
            (FeatureCategory.Water, TimeOfDay.Dusk) =>
            [
                "The water holds the last colors longer than the sky does.",
                "At dusk the water collects all the colors the sky is giving away.",
                "The water in dusk light goes through several opinions before settling on a final one.",
                "Dusk here starts at the water. Everything else follows.",
                "The reflection at dusk is more vivid than the source.",
                "At dusk, the water is doing something the sky already finished.",
                "You could watch the water at dusk for longer than you have. You consider it.",
                "The water holds the last light longer than anything else here.",
            ],
            (FeatureCategory.Water, TimeOfDay.Night) =>
            [
                "Water at night is a dark mirror and you are standing in front of it.",
                "The water at night is not the same water as during the day. You feel this.",
                "At night the water reflects only what it wants to. It is selective.",
                "Night water moves differently — slower, more purposeful, less interested in being watched.",
                "The water moves in the dark with more confidence than it has any right to.",
                "You can hear the water. You cannot entirely predict it.",
                "The dark surface of the water is only showing you part of what it knows.",
                "Night water has made a different arrangement with the world.",
            ],

            (FeatureCategory.Desert, TimeOfDay.Dawn) =>
            [
                "The desert is cold at dawn in a way the afternoon will not remember.",
                "Dawn in the desert is a brief negotiation between cold and heat. The heat will win.",
                "The desert at dawn is cooperative. This does not last.",
                "The first light here is horizontal and sharp. It finds everything.",
                "In the cold of desert dawn, the heat feels like a rumor.",
                "Dawn shadows in the desert are long and blue and do not last.",
                "The first hour here is the one hour the desert offers freely. Use it.",
                "The desert at dawn looks like an apology that hasn't decided whether it's sincere.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Morning) =>
            [
                "The morning light is honest here. There is nothing soft to filter it.",
                "Morning in the desert is decisive. So is everything here, eventually.",
                "The heat is already beginning. The morning does not ease into it.",
                "Morning light on sand looks like nothing else has ever looked.",
                "Morning here operates with a clarity that will not be available later.",
                "The morning is still offering a reasonable arrangement. The afternoon will not.",
                "In the morning, the desert is just a place. This will change.",
                "The shadow you cast in morning desert light is long and pointed and certain of direction.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Afternoon) =>
            [
                "The afternoon here is the main event. Everything else is prologue.",
                "The full midday desert is an argument that does not acknowledge counterpoints.",
                "In the afternoon, heat and light become the same thing and you are caught between them.",
                "Afternoon in the desert has a quality of being several moments at once, all of them hot.",
                "Everything here has made the same calculation about shade and arrived at the same conclusion.",
                "The afternoon here is not hostile. It is simply absolute.",
                "The heat at full afternoon is not a feature of this place. It is this place.",
                "Midday in this place is not an invitation to linger. You are lingering anyway.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Dusk) =>
            [
                "The desert at dusk turns every color at once, briefly and without apology.",
                "The heat lifts at dusk and takes something with it that you had been carrying.",
                "Dusk in the desert is the best hour. The desert knows this and is briefly generous.",
                "At dusk the desert's colors escape before the night takes them.",
                "The colors here at dusk are excessive and brief and entirely serious.",
                "The temperature drop at dusk is startling, as if the desert remembered manners.",
                "The long shadows at dusk return the landscape to three dimensions.",
                "Dusk in the desert feels like being thanked for staying this long.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Night) =>
            [
                "The desert at night is a different place — cooler, quieter, more itself.",
                "Night returns the desert to its other nature. Colder, larger, more honest.",
                "The stars are extraordinary here. The desert has never interfered with them.",
                "At night the desert is quiet in a way that takes effort to stand inside.",
                "The desert at night is enormous in a way the day obscures.",
                "The cold here at night arrives decisively, as everything does in this place.",
                "The silence here at night is not empty. It is the desert thinking.",
                "Night in the desert has no interest in making you comfortable. It has other concerns.",
            ],

            (FeatureCategory.Cold, TimeOfDay.Dawn) =>
            [
                "The cold here did not soften overnight.",
                "Dawn here is a suggestion of warmth. Nothing more.",
                "The cold at dawn takes the first light and does not return it.",
                "Morning takes longer to arrive here. The cold negotiates for every minute.",
                "Breath is visible here even now.",
                "The cold at dawn has a sharpness that will not soften much as the day continues.",
                "Dawn here is something you earn by having survived the night in it.",
                "The first light here arrives over frozen ground and is split in many directions.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Morning) =>
            [
                "Morning light enters this place carefully.",
                "The morning is bright here but offers no apology for the cold.",
                "Light without warmth. This is the morning here.",
                "Morning arrives but announces very little. The cold remains the primary condition.",
                "Your footsteps are the loudest thing in this cold morning.",
                "Morning brings light here but not warmth. These are separate services.",
                "Everything still here is either frozen, patient, or both.",
                "The cold here is a morning companion you did not choose.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Afternoon) =>
            [
                "Even afternoon warmth does not reach this place all the way.",
                "The sun is doing its best here. The cold is not impressed.",
                "The warmest part of the day, and still.",
                "Afternoon light here is bright and specific and ultimately insufficient.",
                "You are in the warmer part of the day and it is still this.",
                "The afternoon light here is white and cold and does not apologize.",
                "This is as warm as it gets today. You have taken note.",
                "Midday cold is a particular kind of cold. More honest than morning cold. Less patient than night.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Dusk) =>
            [
                "The cold deepens at dusk as if it has been waiting for this specific hour.",
                "The light leaves quickly here. The cold does not leave with it.",
                "Dusk arrives early in cold places and stays later than it should.",
                "Something about dusk makes the cold feel intentional.",
                "The dusk here arrives with reinforcements.",
                "Dusk here is brief — the cold shortens it on both ends.",
                "As the light goes, the cold becomes the primary sense.",
                "The light retreats quickly in cold places. The cold stays behind.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Night) =>
            [
                "At night, the cold here is the fundamental condition.",
                "Night and cold are indistinguishable here. They have reached an agreement.",
                "The night here is cold in a way that means it has always been and will always be.",
                "The dark and the cold here are not separate things.",
                "Everything alive here is somewhere else, or it has made arrangements.",
                "The cold at night here is not weather. It is the condition of the world.",
                "Night in this place is cold in a way that reminds you of the larger facts.",
                "You can be here, and you can be cold, and you can understand these as the same thing.",
            ],

            _ => [string.Empty],
        };
        return options[idx % options.Length];
    }

    // ── Visit context ─────────────────────────────────────────────────────────

    private static string GetVisitIntro(int visitCount, int q, int r)
    {
        int idx = Math.Abs(q * 17 + r * 41);
        string[] options = visitCount switch
        {
            1 => [
                "You have not stood here before.",
                "This place is entirely new to you.",
                "First impressions take hold immediately.",
                "You are somewhere you have never been.",
                "The air here feels unmapped to you.",
                "Nothing about this place is expected.",
                "You take in the details without reference points.",
                "No memory of this place precedes you.",
                "Every detail here is unfiltered by expectation.",
                "You arrive without assumptions. That won't last.",
                "You are a stranger here.",
                "This is as unknown as it will ever be.",
                "The unknown has a particular texture to it.",
                "You look at everything twice.",
                "You haven't decided what to think of it yet.",
                "Your eyes move differently in a new place.",
                "A new place demands your full attention.",
                "This is the beginning of knowing this place.",
                "You have no history here yet.",
                "Something about new ground sharpens you.",
                "You read the terrain like a page you've never opened.",
                "No one has told you what to expect here.",
                "You catalog it quietly, the way travelers do.",
            ],
            2 => [
                "You came back. That means something.",
                "Second time around, you look more carefully.",
                "A place becomes real on the second visit.",
                "Something drew you back. You're not sure what.",
                "You remember it better than you expected.",
                "The first visit was for looking. This one is for seeing.",
                "Coming back is how you decide a place is worth knowing.",
                "You know just enough about this place to want to know more.",
                "The second visit confirms what the first one suggested.",
                "You return with questions the first visit raised.",
                "It looks the same. You're not sure you do.",
                "Your memory of the first visit sits beside the reality of this one.",
                "You're still a stranger here, but a less complete one.",
            ],
            3 => [
                "Three times now. A place starts to mean something at three.",
                "Third visit. The map in your head is filling in.",
                "You're beginning to understand why you keep returning.",
                "Three times is enough to have an opinion.",
                "The third time is when coincidence becomes habit.",
                "Patterns are forming in your memory of here.",
                "Something about this place keeps bringing you back.",
                "You recognize the shape of it now.",
                "Three visits. You're invested now whether you meant to be or not.",
                "The third time settles something.",
                "You know the approach now. That's a small thing that isn't.",
                "You've crossed some threshold here without marking it.",
                "Three times in, you start noticing what changes.",
            ],
            <= 9 => [
                "The place settles around you like a half-remembered dream.",
                "You've been here before. Something is different, or you are.",
                "You know this ground. Not well, but enough.",
                "A return visit shows you what the first one didn't.",
                "There's a comfort here you hadn't anticipated.",
                "Familiar, but only just.",
                "You've stopped treating it like an unknown.",
                "The novelty is fading. Something else is taking its place.",
                "Familiar ground. You scan it out of habit now.",
                "This place has become part of your mental map.",
                "You notice things you overlooked before.",
                "You've been here enough times to have opinions about it.",
                "The surprise is gone. That isn't necessarily a loss.",
                "You move through it differently than you did at first.",
                "Each return layers something new over the old.",
            ],
            15 => [
                "Fifteen times. You could walk this place in the dark.",
                "You've been here so often it shows up in your dreams.",
                "The fifteenth visit. There's something ceremonial about that.",
                "You know which ground is soft, which footing is uncertain.",
                "At fifteen visits, a place starts to belong to you.",
                "Few places earn this much of your attention.",
                "You notice only changes now — everything else is expected.",
                "This has become a place you measure other places against.",
            ],
            _ => [
                "You know this place the way you know your own hands.",
                "This has become ordinary in the best possible way.",
                "Old ground. You walk it without thinking.",
                "The place doesn't surprise you anymore. You find you don't mind.",
                "There's no discovery left here, only depth.",
                "You've stopped counting how many times you've been here.",
                "Comfort lives in repetition. This is that.",
                "You know the light here at different hours.",
            ],
        };
        return options[idx % options.Length];
    }

    // ── Adjacent tile influence ───────────────────────────────────────────────

    private string GetNeighborInfluence(BiomeType selfBiome, List<HexTile> neighbors, int q, int r)
    {
        var best = neighbors
            .Where(n => n.Biome != selfBiome)
            .Select(n => (tile: n, def: _biomeProvider.GetByType(n.Biome)))
            .Where(x => x.def != null && x.def.NeighborPriority > 0 && x.def.NeighborText.Length > 0)
            .OrderByDescending(x => x.def!.NeighborPriority)
            .FirstOrDefault();

        if (best.def == null) return string.Empty;

        int idx = Math.Abs(q * 19 + r * 43);
        return best.def.NeighborText[idx % best.def.NeighborText.Length];
    }

    // ── Settlement context ────────────────────────────────────────────────────

    private static string GetSettlementNote(string name, SettlementTileRole role, SettlementType type, int q, int r)
    {
        int idx = Math.Abs(q * 53 + r * 61);
        string typeName = type switch
        {
            SettlementType.City    => "city",
            SettlementType.Town    => "town",
            SettlementType.Village => "village",
            _                      => "hamlet",
        };

        string[] options = role switch
        {
            SettlementTileRole.Center =>
            [
                $"You are standing at the heart of {name}. The main hall here carries the particular weight of a building that has become the place people look toward.",
                $"This is the center of {name}, a {typeName}. The building before you has been where decisions are made, or at least announced, for long enough that it looks settled into that purpose.",
                $"The central hall of {name} stands here. It is not the largest thing you have seen, but it holds itself as if it were.",
                $"You are at the center of {name}. A {typeName}'s main hall has a way of making you feel briefly official just by standing in front of it.",
            ],
            SettlementTileRole.Residential =>
            [
                $"The houses of {name} are close together here, as houses in a {typeName} tend to be. Lives are arranged behind these walls in ways you can only guess at.",
                $"You are among the homes of {name}. The {typeName}'s residents have built close to one another — the way people do when they have decided a place is worth staying in.",
                $"Residential {name}. The houses sit occupied and used. That much is clear from how they settle into the ground.",
                $"These are the homes that make {name} a {typeName} rather than just a name on a map.",
            ],
            SettlementTileRole.Market =>
            [
                $"The market quarter of {name} occupies this space. Something has been bought or sold here so many times that the ground seems to remember it.",
                $"Commerce happens here, in the market of {name}. The {typeName} trades in whatever it needs to trade in. The market gives the impression it always has.",
                $"You are in the trading heart of {name}. The smell of transactions, past and present, hangs in the air without being unpleasant about it.",
                $"The market of {name} has been here long enough to have opinions. You are standing in them.",
            ],
            SettlementTileRole.Farm =>
            [
                $"The farms that feed {name} extend here. The {typeName}'s survival is arranged in rows and takes no particular interest in being picturesque about it.",
                $"Agricultural land belonging to {name}. Everything a {typeName} eats has to come from somewhere. This is where it comes from.",
                $"These are the fields of {name}. They are not romantic. They are practical and they are working, which is the same thing in a different register.",
                $"The farmland at the edge of {name}. The {typeName} depends on this ground more than the {typeName} center tends to acknowledge.",
            ],
            SettlementTileRole.Guard =>
            [
                $"A watchtower of {name} marks this ground — the {typeName}'s formal acknowledgment that it has things worth watching for.",
                $"The guard post of {name}. Someone has always stood here, or somewhere near enough, for long enough that watching has become part of what the {typeName} is.",
                $"You are at a defensive position belonging to {name}. The {typeName} has decided this corner is worth protecting and has committed the architecture to prove it.",
                $"The garrison tower of {name}. Someone here is always watching the horizon. The job exists because the horizon is always there.",
            ],
            SettlementTileRole.Inn =>
            [
                $"The inn of {name} stands here — the {typeName}'s acknowledgment that people arrive and sometimes need somewhere to be before they move on.",
                $"You are at the inn that {name} keeps for travelers. Whatever else the {typeName} is, it has decided to be the kind of place that expects visitors.",
                $"The inn of {name} is where the {typeName} and the outside world negotiate their ongoing relationship. The terms seem reasonable.",
                $"The inn of {name} — which is another way of saying the {typeName} has a settled theory about hospitality and has built it in wood and stone.",
            ],
            SettlementTileRole.Mill =>
            [
                $"The mill of {name} works here, converting one thing steadily into something more useful, without comment or interruption.",
                $"A mill belonging to {name} occupies this space. Grain enters. Flour leaves. The {typeName} eats. This is the unglamorous arithmetic of how settlements persist.",
                $"The mill of {name}. Something is always grinding here, or was, or will be. The mechanism does not observe days off.",
                $"You are at {name}'s mill. The {typeName} processes what it grows at this point, which is the part of a settlement's survival that gets the least credit.",
            ],
            _ => [],
        };

        if (options.Length == 0) return string.Empty;
        return options[idx % options.Length];
    }

    // ── Rare events ───────────────────────────────────────────────────────────

    private static readonly string[] RareEvents =
    [
        "There is a duck here. It is standing on a small sign that reads 'NO.' You did not see where the sign came from. The duck has been here longer than you. You leave.",
        "You find a perfectly wrapped gift with your name on it. You open it. Inside: a smaller gift. Inside that: a note reading 'not yet.' You put it all back carefully.",
        "A very small, very old woman appears from behind something. She says something is coming. You ask what. She says 'Tuesday.' It is, in fact, Tuesday. She nods and walks away.",
        "A door stands in the open air with no walls around it. The door is locked. You knock. Something knocks back from the other side, once, with great deliberateness. You do not knock again.",
        "Someone has drawn a detailed map of this exact location in the dirt, including a small X where you are standing. You step off the X. The map updates.",
        "The air smells briefly of excellent bread. Then it's gone. There is no bread. You look for the bread for longer than you'd like to admit.",
        "You find a note in a bottle. Inside: instructions. The instructions are for a different bottle.",
        "A very large tortoise moves through this area with enormous confidence. It knows where it is going. You follow it for a while. It arrives at nothing in particular and seems satisfied. You feel envious.",
        "You hear a bell toll three times from somewhere far away. You count carefully. It tolls a fourth time, slightly apologetically.",
        "An extremely official-looking sign has been posted here. It says this area is 'Under New Management.' There is no further information.",
        "A crow lands in front of you and drops something at your feet: a chess piece. The rook. It leaves with the air of someone who has completed an errand. You keep the rook.",
        "Everything is fine here. The light is good. The air is pleasant. Nothing is wrong. This is somehow the most unsettling thing you've encountered all day.",
        "You find a perfectly ordinary rock. After considerable inspection, it is an excellent rock. You feel genuinely proud of it. You put it down. You have complicated feelings about this.",
        "The wind speaks. You don't catch what it said. It says it again, more slowly. You still don't catch it. The wind gives up. This is worse than if it hadn't tried.",
        "A very small fire is burning, unsupported, about two feet off the ground. It has been burning for some time. You agree to leave it to its business.",
        "You find a coin. Heads. You pick it up. Heads. You flip it. Heads. Again. Heads. You put the coin back down and walk away without flipping it again.",
        "You meet yourself here, briefly, coming from the other direction. You both stop. You both continue. You don't discuss it.",
        "Something has organized the rocks in this area into neat piles while you were looking at something else. You were looking at the ground. You looked up. The piles are there.",
        "A small creature runs past carrying what appears to be a single noodle. It has places to be. It does not slow down to explain.",
        "You find a boot. Inside: a note reading 'not this boot.' You consider looking for the other one. You decide against it. You think about it for the rest of the day.",
        "A sign reading 'WET PAINT' has been posted on something that is not painted. You touch it anyway. Your hand is now wet. The paint is invisible.",
        "Someone has installed a doorbell here with no door attached. You press it. It plays a full song. When it ends, nothing happens. You feel you should have prepared remarks.",
        "The light falls here in a way that makes everything look briefly like a painting. You stand in it for a moment. It passes. You're glad you noticed.",
        "You find a small cairn: three stones, carefully balanced. Someone built it and walked away. You add a fourth stone. This feels correct.",
        "You count the shadows in this area. There is one more than there should be. You count again. Same number. You stop counting.",
        "You get the strong feeling this place has been waiting for you. Not in a welcoming way. More like the way a question waits for an answer.",
        "Your footprints leading into this area are facing the wrong direction. You check your feet. Your feet are fine.",
        "A very old calendar is nailed to a tree. The month is correct. The year is not. You do not recognize the year.",
        "You find a pair of glasses in the dirt. Perfect condition. You try them on. Everything looks exactly the same. You place them back carefully, as if someone will return for them.",
        "A second moon is briefly visible. You look for someone to confirm this. You are alone. It does not reappear. You decide not to mention it.",
        "You find a small notebook. Every page is filled with the word 'almost.' You put it back.",
        "You sneeze. Something in the far distance falls over.",
        "There is a table here with two chairs. One is pulled out as if recently vacated. The table is set. The food is untouched. It is not cold.",
        "The echo here returns slightly earlier than it should. You test this several times. The results are consistent. This is worse than if they hadn't been.",
        "The air here smells like rain that hasn't happened yet. You decide to take it as a good sign. It feels like one.",
        "A butterfly lands on your hand and stays for an unreasonable amount of time. Eventually it leaves. You watch it go longer than you meant to.",
        "A crow stands on a rock watching you. When you move left, it moves left. When you move right, it moves right. When you stop, it stops. It seems very interested in how this will end.",
        "You find a small fire burning in a neat firepit. The fire is recent. There is no one here. There are two cups near the fire. One has been used. The other is clean and waiting.",
    ];

}
