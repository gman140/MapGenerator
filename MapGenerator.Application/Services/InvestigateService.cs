using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InvestigateService
{
    private readonly ITileNoteRepository _noteRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;
    private readonly IFeatureDefinitionProvider _featureProvider;
    private readonly MapGeneratorService _mapCache;

    public InvestigateService(
        ITileNoteRepository noteRepo,
        IPlayerTileVisitRepository visitRepo,
        IFeatureDefinitionProvider featureProvider,
        MapGeneratorService mapCache)
    {
        _noteRepo        = noteRepo;
        _visitRepo       = visitRepo;
        _featureProvider = featureProvider;
        _mapCache        = mapCache;
    }

    public async Task<(string flavorText, List<TileNote> notes)> InvestigateAsync(Player player)
    {
        var tile       = _mapCache.GetCachedTile(player.Q, player.R);
        var visitCount = await _visitRepo.CountVisitsAsync(player.Id, player.Q, player.R);
        var neighbors  = GetNeighborTiles(player.Q, player.R);
        var flavor     = tile != null
            ? BuildFlavorText(tile.Biome, tile.FeatureId, player.Q, player.R, visitCount, neighbors)
            : "You look around carefully. There is not much to see. You look again. Still nothing. You are thorough and it has not helped.";
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
            var (partA, partB) = GetBiomeParts(biome);
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
            ],
            (FeatureCategory.Nature, TimeOfDay.Morning) =>
            [
                "The morning has arrived and brought everything along with it.",
                "The morning is doing what it always does — making things look manageable.",
                "Morning light here is generous in a way afternoon light forgets to be.",
                "In the full morning, the natural world is operating at its most straightforward.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Afternoon) =>
            [
                "The afternoon has settled in and is not going anywhere soon.",
                "The afternoon slows everything down. This is not an accident.",
                "The insects are at peak confidence now. This is their best hour.",
                "Afternoon here has the quality of a held note — long, sustained, certain of itself.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Dusk) =>
            [
                "The light has gone golden and tentative, the way it gets when it's leaving.",
                "The last light finds all the best angles before it goes.",
                "At dusk the natural world begins its other shift.",
                "Something passes through the air at dusk here that doesn't have a name yet.",
            ],
            (FeatureCategory.Nature, TimeOfDay.Night) =>
            [
                "In the dark, the living world stops performing and becomes itself.",
                "The night here is full of things you cannot identify but feel moving.",
                "In the dark, the natural world has a different agreement with you.",
                "Night changes the terms here. You are no longer the obvious one.",
            ],

            (FeatureCategory.Ruins, TimeOfDay.Dawn) =>
            [
                "In the early light, the ruins look almost whole.",
                "Dawn makes ruins optimistic. This is one of dawn's few cruelties.",
                "At first light the old stones hold their shadows differently — as if remembering.",
                "The ruins in dawn light have stopped being ruins for a moment. The moment does not last.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Morning) =>
            [
                "The morning light makes the old stones look considered, almost purposeful.",
                "Morning finds what's left and illuminates it with no particular mercy.",
                "In the morning the ruins are at their most archaeological. Things have happened here. Clearly.",
                "Shadows shorten in the morning. The ruins look most like themselves at this hour.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Afternoon) =>
            [
                "In full daylight the ruin is just a ruin. This is, somehow, still affecting.",
                "The afternoon light is flat and honest. It does not do the ruins any favors. It does them justice.",
                "Heat shimmers off the old stones in the afternoon. History becomes a physical sensation.",
                "Midday: the shadows shrink and the ruins are fully visible. Everything that happened here is fully visible.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Dusk) =>
            [
                "The last light retreats from here slowly, as if it has been here before and is reluctant.",
                "At dusk the shadows lengthen and the ruins gain back what the centuries took.",
                "The dusk light turns the stonework warm. It feels like a condolence.",
                "Something about dusk returns this place to itself.",
            ],
            (FeatureCategory.Ruins, TimeOfDay.Night) =>
            [
                "The ruins at night are patient in a way they are not during the day.",
                "At night the ruins are honest about what they are and what they aren't.",
                "The dark gives the ruins a kind of authority the daylight argues with.",
                "Ruins at night stop pretending to be anything. You appreciate the honesty.",
            ],

            (FeatureCategory.Water, TimeOfDay.Dawn) =>
            [
                "The water holds the early quiet like it doesn't want to give it back.",
                "At dawn the water is still and silver and not yet asked to be anything.",
                "The first light finds the water and the water holds it carefully.",
                "Dawn here begins at the water and moves outward from it, slowly.",
            ],
            (FeatureCategory.Water, TimeOfDay.Morning) =>
            [
                "The morning light catches the water as if it meant to.",
                "The water in morning light is extravagantly itself.",
                "Morning finds the water in good form, as it usually does.",
                "The water in the morning is working hard and doesn't mind being watched.",
            ],
            (FeatureCategory.Water, TimeOfDay.Afternoon) =>
            [
                "The afternoon heat makes everything water-related feel like a decision.",
                "The afternoon light pushes straight down and the water takes it personally.",
                "Midday and the water is bright and competitive about it.",
                "The water in afternoon light is entirely unreserved. Everything is visible. Nothing is subtle.",
            ],
            (FeatureCategory.Water, TimeOfDay.Dusk) =>
            [
                "The water holds the last colors longer than the sky does.",
                "At dusk the water collects all the colors the sky is giving away.",
                "The water in dusk light goes through several opinions before settling on a final one.",
                "Dusk here starts at the water. Everything else follows.",
            ],
            (FeatureCategory.Water, TimeOfDay.Night) =>
            [
                "Water at night is a dark mirror and you are standing in front of it.",
                "The water at night is not the same water as during the day. You feel this.",
                "At night the water reflects only what it wants to. It is selective.",
                "Night water moves differently — slower, more purposeful, less interested in being watched.",
            ],

            (FeatureCategory.Desert, TimeOfDay.Dawn) =>
            [
                "The desert is cold at dawn in a way the afternoon will not remember.",
                "Dawn in the desert is a brief negotiation between cold and heat. The heat will win.",
                "The desert at dawn is cooperative. This does not last.",
                "The first light here is horizontal and sharp. It finds everything.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Morning) =>
            [
                "The morning light is honest here. There is nothing soft to filter it.",
                "Morning in the desert is decisive. So is everything here, eventually.",
                "The heat is already beginning. The morning does not ease into it.",
                "Morning light on sand looks like nothing else has ever looked.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Afternoon) =>
            [
                "The afternoon here is the main event. Everything else is prologue.",
                "The full midday desert is an argument that does not acknowledge counterpoints.",
                "In the afternoon, heat and light become the same thing and you are caught between them.",
                "Afternoon in the desert has a quality of being several moments at once, all of them hot.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Dusk) =>
            [
                "The desert at dusk turns every color at once, briefly and without apology.",
                "The heat lifts at dusk and takes something with it that you had been carrying.",
                "Dusk in the desert is the best hour. The desert knows this and is briefly generous.",
                "At dusk the desert's colors escape before the night takes them.",
            ],
            (FeatureCategory.Desert, TimeOfDay.Night) =>
            [
                "The desert at night is a different place — cooler, quieter, more itself.",
                "Night returns the desert to its other nature. Colder, larger, more honest.",
                "The stars are extraordinary here. The desert has never interfered with them.",
                "At night the desert is quiet in a way that takes effort to stand inside.",
            ],

            (FeatureCategory.Cold, TimeOfDay.Dawn) =>
            [
                "The cold here did not soften overnight.",
                "Dawn here is a suggestion of warmth. Nothing more.",
                "The cold at dawn takes the first light and does not return it.",
                "Morning takes longer to arrive here. The cold negotiates for every minute.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Morning) =>
            [
                "Morning light enters this place carefully.",
                "The morning is bright here but offers no apology for the cold.",
                "Light without warmth. This is the morning here.",
                "Morning arrives but announces very little. The cold remains the primary condition.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Afternoon) =>
            [
                "Even afternoon warmth does not reach this place all the way.",
                "The sun is doing its best here. The cold is not impressed.",
                "The warmest part of the day, and still.",
                "Afternoon light here is bright and specific and ultimately insufficient.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Dusk) =>
            [
                "The cold deepens at dusk as if it has been waiting for this specific hour.",
                "The light leaves quickly here. The cold does not leave with it.",
                "Dusk arrives early in cold places and stays later than it should.",
                "Something about dusk makes the cold feel intentional.",
            ],
            (FeatureCategory.Cold, TimeOfDay.Night) =>
            [
                "At night, the cold here is the fundamental condition.",
                "Night and cold are indistinguishable here. They have reached an agreement.",
                "The night here is cold in a way that means it has always been and will always be.",
                "The dark and the cold here are not separate things.",
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
            ],
            <= 4 => [
                "The place settles around you like a half-remembered dream.",
                "You've been here before. Something is different, or you are.",
                "You know this ground. Not well, but enough.",
                "A return visit shows you what the first one didn't.",
            ],
            <= 9 => [
                "Familiar ground. You scan it out of habit now.",
                "This place has become part of your mental map.",
                "You notice things you overlooked before.",
                "You've been here enough times to have opinions about it.",
            ],
            _ => [
                "You know this place the way you know your own hands.",
                "This has become ordinary in the best possible way.",
                "Old ground. You walk it without thinking.",
                "The place doesn't surprise you anymore. You find you don't mind.",
            ],
        };
        return options[idx % options.Length];
    }

    // ── Adjacent tile influence ───────────────────────────────────────────────

    private static string GetNeighborInfluence(BiomeType selfBiome, List<HexTile> neighbors, int q, int r)
    {
        static int Score(BiomeType b) => b switch
        {
            BiomeType.Volcano  => 10,
            BiomeType.Ocean    => 8,
            BiomeType.Glacier  => 7,
            BiomeType.Swamp    => 6,
            BiomeType.Marsh    => 6,
            BiomeType.Desert   => 5,
            BiomeType.River    => 5,
            BiomeType.Snow     => 4,
            BiomeType.Tundra   => 4,
            BiomeType.Jungle   => 4,
            BiomeType.Mountain => 3,
            BiomeType.Forest   => 3,
            BiomeType.Beach    => 3,
            BiomeType.Shallows => 2,
            _ => 0,
        };

        var best = neighbors
            .Where(n => n.Biome != selfBiome && Score(n.Biome) > 0)
            .OrderByDescending(n => Score(n.Biome))
            .FirstOrDefault();

        if (best == null) return string.Empty;

        int idx = Math.Abs(q * 19 + r * 43);
        string[] options = best.Biome switch
        {
            BiomeType.Volcano => [
                "Sulfur drifts in from somewhere it shouldn't.",
                "The air on one side carries a warmth that has nothing to do with the sun.",
                "Ash has settled here in fine gray layers from whatever burns nearby.",
            ],
            BiomeType.Ocean => [
                "Salt air finds you from the direction of the water.",
                "The sea is close enough to smell, maybe to hear.",
                "Something maritime is in the air here — borrowed from wherever the water begins.",
            ],
            BiomeType.Glacier => [
                "One direction is noticeably colder than the others.",
                "Glacial air presses in from one side with quiet certainty.",
                "The cold here has been traveling over ice to reach you.",
            ],
            BiomeType.Swamp or BiomeType.Marsh => [
                "A wet, organic smell drifts in from somewhere low and still.",
                "The air carries a note of standing water from somewhere nearby.",
                "The edge of wetter ground is close enough to smell.",
            ],
            BiomeType.Desert => [
                "Fine grit finds your teeth from the direction of the open dry land.",
                "The air on one side is noticeably drier than the rest.",
                "The desert's edge is close enough to taste.",
            ],
            BiomeType.River => [
                "You can hear running water nearby if you listen for it.",
                "The sound of a river reaches here — intermittent, comfortable.",
                "Water is moving somewhere close. You can't see it, but the sound finds you.",
            ],
            BiomeType.Snow => [
                "The air carries a clean cold from whatever is white on the horizon.",
                "Snow is visible somewhere close, lending the air an edge.",
                "The cold from nearby elevation finds this place reliably.",
            ],
            BiomeType.Tundra => [
                "The wind from one direction has been traveling over open frozen ground.",
                "A spare, cold air reaches here from the tundra nearby.",
                "Something wide and cold is close. You feel its edge.",
            ],
            BiomeType.Jungle => [
                "Dense green presses in from one direction.",
                "The jungle is audible from here — layered, relentless.",
                "Jungle heat and humidity find this place from one side.",
            ],
            BiomeType.Mountain => [
                "The terrain rises sharply not far from here.",
                "Elevation is visible in one direction, patient and permanent.",
                "The shadow of higher ground passes through here at certain hours.",
            ],
            BiomeType.Forest => [
                "The treeline is close enough to feel — a change in light and temperature.",
                "Birdsong from the nearby forest finds this place.",
                "The smell of damp wood and leaf litter drifts over from the trees.",
            ],
            BiomeType.Beach => [
                "Surf sounds reach you on the right wind.",
                "The ground is sandier here, borrowed from wherever the shore begins.",
                "Something coastal is in the light here — reflected off water not far away.",
            ],
            BiomeType.Shallows => [
                "The air smells faintly of shallow water and wet sand.",
                "A coastal quality settles over the light here.",
                "Water is close — shallow, warm, audible when the wind shifts.",
            ],
            _ => [],
        };

        return options.Length == 0 ? string.Empty : options[idx % options.Length];
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
    ];

    // ── Biome text (no feature) ───────────────────────────────────────────────

    private static (string[] partA, string[] partB) GetBiomeParts(BiomeType biome) => biome switch
    {
        BiomeType.Ocean => (
            ["The waves are relentless and patient.", "The water goes dark very quickly.", "The sea is doing what the sea does, which is everything and nothing."],
            ["They have been doing this forever. They will do this forever after you. You are a brief event.", "You don't know what lives below. The water knows you don't know.", "You are here very briefly and it has no opinion about this."]
        ),
        BiomeType.Shallows => (
            ["The water is clear enough to see every pebble on the bottom.", "Sunlight bends through the water and makes patterns on the sandy floor.", "Small fish move in purposeful formations around your feet."],
            ["Small fish investigate your feet. One nudges deliberately. You have been studied.", "They almost look like writing. You squint. You're still not sure.", "One stops and faces you directly for an unreasonable amount of time."]
        ),
        BiomeType.Beach => (
            ["The sand holds the warmth of the day.", "The surf pulls at the sand with each wave, making the ground briefly liquid.", "The tide has left a line of shells and kelp and small mysterious things."],
            ["A crab assesses you from a respectful distance and decides you are probably not a threat. It is generous.", "You sink a little with each wave. You stay.", "You examine one of the small mysterious things. It examines you back with its many small eyes."]
        ),
        BiomeType.River => (
            ["The current is patient and consistent.", "The water is cold and fast and clear.", "The river bends away ahead of you with great purpose."],
            ["It does not care where you want to go — only where it's going. You consider letting it decide.", "Something silver passes beneath — too quick to be sure of.", "It knows where it is going and has known for a long time."]
        ),
        BiomeType.Swamp => (
            ["Something bubbles from the mud nearby — steady, purposeful.", "The water here is the color of strong tea.", "The air here is complicated."],
            ["You do not ask what it is reporting.", "It smells of things breaking down into other things. This is the natural order. You still find it unsettling.", "You breathe carefully through your nose and make a series of decisions."]
        ),
        BiomeType.Marsh => (
            ["The ground makes complicated promises about being solid.", "A heron stands ahead of you, still as a carved thing.", "The water and land here have come to an arrangement that benefits neither."],
            ["You test each step. The marsh tests back.", "It is watching for something with absolute focus. You stand still out of competitive instinct. It wins.", "You navigate it carefully and feel a modest but genuine pride at the other side."]
        ),
        BiomeType.Grassland => (
            ["The grass bends in a wave when the wind comes through.", "Crickets start and stop in shifts across the field.", "The smell here is green and warm and uncomplicated."],
            ["You are briefly part of the breath.", "You sit in the grass and listen to the negotiation.", "You stand in it for a while. It does not ask anything of you."]
        ),
        BiomeType.Plains => (
            ["The horizon is very far away and perfectly flat.", "The wind crosses the plains with nothing to stop it.", "The sky here is enormous in a way you don't notice until you look up."],
            ["You feel both significant and appropriately small. This is the correct ratio.", "It arrives at you carrying the smell of somewhere you haven't been yet.", "You look up. You stay looking up for a while."]
        ),
        BiomeType.Savanna => (
            ["The golden grass stretches in every direction.", "The dry grass rasps in the wind, a continuous sound like sand over stone.", "Something large stands very still in the distance, watching."],
            ["In the distance something stands very still watching you. When you look directly it moves.", "Somewhere a bird calls once. Nothing answers.", "When you look directly at it, it moves. When you look away, it stills again."]
        ),
        BiomeType.Forest => (
            ["Light filters through the canopy in shifting columns.", "A branch moves with no wind behind it.", "The forest is doing what forests do — being enormous and absorbed in its own concerns."],
            ["The forest is patient and absorbed in its own concerns. You are a brief parenthesis in something longer.", "Then another. Then the forest settles and is very still and you walk more carefully.", "You are a brief event here. The forest does not mind."]
        ),
        BiomeType.Jungle => (
            ["Everything here is growing with total commitment.", "Something watches you from the canopy with large yellow eyes.", "The undergrowth presses in from every direction."],
            ["The trees compete for the light overhead. The undergrowth competes for what's left. You are briefly disputed territory.", "You look up and lose it in the green. When you look down it has moved. When you look up it is where it started.", "You push through it carefully, borrowing space."]
        ),
        BiomeType.Desert => (
            ["The heat is physical. It presses against you like a hand.", "Nothing moves here that doesn't have to.", "The silence and the heat are the same weight."],
            ["The silence presses the same way. They weigh about the same.", "You move carefully, out of respect.", "You stand still for a moment and become briefly part of the landscape."]
        ),
        BiomeType.Mountain => (
            ["The rock underfoot is very old and very sure of itself.", "The wind at this elevation is a different thing than the wind below.", "The view from here is significant."],
            ["It makes no accommodations for you. You find your own way across it.", "Sharper, colder, more certain. It knows where it's been.", "You look at it for a while and feel correctly small."]
        ),
        BiomeType.Tundra => (
            ["The permafrost is under your boots, ancient and silent and unimpressed.", "The wind on the tundra has been traveling for a long time.", "The landscape here is spare and specific."],
            ["You are a warm soft thing walking briefly over something cold and very old.", "It still has somewhere to be. It passes through you on its way.", "It does not perform for visitors."]
        ),
        BiomeType.Snow => (
            ["Your breath makes small clouds that vanish quickly.", "The snow absorbs sound completely.", "The world here is white and still and very deliberate about it."],
            ["You name one of the clouds. This is the most important thing you'll do today.", "Each step falls into silence. You have the impression of a very large room that wants quiet.", "You move through it carefully, feeling like an intrusion."]
        ),
        BiomeType.Glacier => (
            ["The ice groans somewhere beneath your feet — a sound like the world settling.", "The ice at this depth is a blue that doesn't exist in most languages.", "The glacier is moving. Imperceptibly. Every moment."],
            ["The glacier is moving, imperceptibly, every moment. You are moving with it.", "It is a color that existed before anyone named colors.", "You are moving with it whether you mean to or not."]
        ),
        BiomeType.Volcano => (
            ["The ground radiates an uncomfortable heat.", "The air here smells of sulfur and something older.", "The rock underfoot is dark and sharp and recent by geological standards."],
            ["Everything here is communicating that you should not be here.", "You agree with the smell. You leave.", "You feel very strongly that this is not your biome."]
        ),
        _ => (
            ["You look around carefully."],
            ["There is not much to see. You look again to make sure."]
        ),
    };
}
