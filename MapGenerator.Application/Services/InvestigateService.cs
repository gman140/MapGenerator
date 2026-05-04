using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InvestigateService
{
    private readonly ITileNoteRepository _noteRepo;
    private readonly MapGeneratorService _mapCache;

    public InvestigateService(ITileNoteRepository noteRepo, MapGeneratorService mapCache)
    {
        _noteRepo = noteRepo;
        _mapCache = mapCache;
    }

    public async Task<(string flavorText, List<TileNote> notes)> InvestigateAsync(Player player)
    {
        var tile = _mapCache.GetCachedTile(player.Q, player.R);
        var flavor = tile != null
            ? BuildFlavorText(tile.Biome, tile.Feature, player.Q, player.R)
            : "You look around carefully. There is not much to see. You look again. Still nothing. You are thorough and it has not helped.";
        var notes = await _noteRepo.GetNotesForTileAsync(player.Q, player.R);
        return (flavor, notes);
    }

    // ── Core assembly ────────────────────────────────────────────────────────

    private static string BuildFlavorText(BiomeType biome, TileFeature feature, int q, int r)
    {
        // Rare event: ~4% per tile per day — completely overrides normal text
        var rareHash = Math.Abs(q * 31 + r * 37 + DateTime.UtcNow.DayOfYear * 7919);
        if (rareHash % 25 == 0)
            return RareEvents[rareHash % RareEvents.Length];

        int hashA = Math.Abs(q * 11 + r * 17);
        int hashB = Math.Abs(q * 7  + r * 23);

        if (feature != TileFeature.None)
        {
            var (partA, partB) = GetFeatureParts(feature);
            var prefix = GetTimePrefix(GetFeatureCategory(feature), GetTimeOfDay(), q, r);
            var body   = $"{partA[hashA % partA.Length]} {partB[hashB % partB.Length]}";
            return string.IsNullOrEmpty(prefix) ? body : $"{prefix} {body}";
        }
        else
        {
            var (partA, partB) = GetBiomeParts(biome);
            return $"{partA[hashA % partA.Length]} {partB[hashB % partB.Length]}";
        }
    }

    // ── Time of day ───────────────────────────────────────────────────────────

    private enum TimeOfDay { Dawn, Morning, Afternoon, Dusk, Night }
    private enum FeatureCategory { Nature, Ruins, Water, Desert, Cold }

    private static TimeOfDay GetTimeOfDay()
    {
        int h = DateTime.Now.Hour;
        if (h is >= 4  and < 7)  return TimeOfDay.Dawn;
        if (h is >= 7  and < 12) return TimeOfDay.Morning;
        if (h is >= 12 and < 17) return TimeOfDay.Afternoon;
        if (h is >= 17 and < 20) return TimeOfDay.Dusk;
        return TimeOfDay.Night;
    }

    private static FeatureCategory GetFeatureCategory(TileFeature f) => f switch
    {
        TileFeature.SpookyWoods   or TileFeature.MushroomGrove  or TileFeature.AncientGlade or
        TileFeature.FernDell      or TileFeature.BramblePatch   or TileFeature.MushroomRing or
        TileFeature.TropicalGrove or TileFeature.WildOrchard    => FeatureCategory.Nature,

        TileFeature.BurnedRuins   or TileFeature.AncientShrine  or TileFeature.RuinedTower  or
        TileFeature.AbandonedFarm or TileFeature.ForgottenGrave or TileFeature.CrumbledFortress or
        TileFeature.StoneCircle   or TileFeature.LonelyWell     or TileFeature.WitchsCottage => FeatureCategory.Ruins,

        TileFeature.HotSpring or TileFeature.TidePools or TileFeature.ReedBeds => FeatureCategory.Water,

        TileFeature.SaltFlats  or TileFeature.OasisGrove or
        TileFeature.DeadForest or TileFeature.QuickSand  => FeatureCategory.Desert,

        _ => FeatureCategory.Cold,
    };

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

    // ── Feature text (PartA = observation, PartB = event/reaction) ────────────

    private static (string[] partA, string[] partB) GetFeatureParts(TileFeature feature) => feature switch
    {
        TileFeature.SpookyWoods => (
            [
                "The trees lean inward and their branches touch above your head, blocking the sky.",
                "Something laughs, very softly, from a direction you cannot determine.",
                "Your shadow is behaving incorrectly. You can't identify how, exactly.",
                "The birds stopped singing when you entered. You only just noticed.",
            ], [
                "You stay longer than you should, trying to name what is wrong. You leave without naming it.",
                "You call out. It laughs again. You do not call out a second time.",
                "You watch it for a long moment. It watches back with what appears to be mild amusement.",
                "You find a small, careful arrangement of stones nearby. A warning or a welcome. Impossible to say.",
            ]
        ),
        TileFeature.MushroomGrove => (
            [
                "The mushrooms here are enormous — the sitting kind — and they glow a gentle blue.",
                "Spores drift through the air like slow snow, catching the light.",
                "The largest mushroom has a small door carved into it. There is a brass knocker.",
                "One mushroom is significantly larger than the others and appears to be in charge.",
            ], [
                "You sit on one. It is extremely comfortable. It hums. You hum back.",
                "One lands on your hand and you understand something important. Then it dissolves. The understanding dissolves with it.",
                "You knock. Nothing answers. The knocker is warm.",
                "You eat a small one. It tastes of strawberries and old regret. The regret is pleasant.",
            ]
        ),
        TileFeature.AncientGlade => (
            [
                "A shaft of light falls through the canopy at an angle that no sun position explains.",
                "At the heart of the glade stands a tree wider than a house, older than most things with names.",
                "A deer stands at the far edge, watching you with eyes that are a color deer eyes should not be.",
                "The grass here is implausibly soft — the kind of soft that takes centuries.",
            ], [
                "You stand in it. It is warm in a way that has nothing to do with heat.",
                "Carved into its bark are thousands of names — some in alphabets you don't recognize, some in no alphabet at all.",
                "When you blink it is closer. When you blink again it is gone.",
                "You lie down in it for what feels like a moment. The sun has moved considerably when you stand.",
            ]
        ),
        TileFeature.FernDell => (
            [
                "The ferns grow so tall here that the sky is only a green suggestion overhead.",
                "At the center of the dell, partially buried, is a perfect glass marble with a very small storm inside it.",
                "A path winds between the fronds — too regular to be accidental.",
                "Something small and very quick moves between the fronds just ahead of you.",
            ], [
                "Something watches you from within. You catch a glimpse: it is smiling about something.",
                "The storm is real and very angry. You put the marble back. It continues.",
                "You follow it a short distance. It leads somewhere you cannot see and turns with great confidence.",
                "You push through to the other side. More ferns. These ones feel older. You cannot explain this.",
            ]
        ),
        TileFeature.BramblePatch => (
            [
                "The thorns catch at your clothes and hold with great personal conviction.",
                "Something shines at the heart of the bramble tangle, catching the light.",
                "The brambles have formed a natural wall around a small clearing with exactly one wooden chair in it.",
                "A single perfect rose grows at the center of all this thorn and difficulty.",
            ], [
                "You fight through and find: a brass button engraved with a ship. The nearest sea is very far away.",
                "You stand looking at it for a long time before deciding not to want it.",
                "The chair faces away from you. You consider walking around it. You decide not to.",
                "It has no smell. It has, instead, an expression.",
            ]
        ),
        TileFeature.MushroomRing => (
            [
                "The ring of mushrooms is perfect. Too perfect. Mathematically perfect.",
                "A voice says your name clearly, from no specific direction.",
                "The air inside the ring is perfectly still, even as the wind moves outside it.",
                "You feel an irresistible urge to dance upon seeing the ring.",
            ], [
                "You step inside. The world goes very quiet. Not silent — quiet. You stay longer than intended.",
                "You spin around. There is no one. The mushrooms do not blink. You note this carefully.",
                "You stand in the center and feel every stone equidistant behind you. They are doing something. You cannot tell what.",
                "You dance. Briefly. Just a little. The mushrooms seem satisfied. You feel satisfied and confused about feeling satisfied.",
            ]
        ),
        TileFeature.TropicalGrove => (
            [
                "The palms cast shade of a quality you haven't encountered before — dense and slightly golden.",
                "A parrot lands on your shoulder with the confidence of something that has decided to be there.",
                "Bright fruit hangs heavy on every branch, ripe beyond what should be possible this season.",
                "A hammock is strung between two palms, well-knotted and worn smooth by use.",
            ], [
                "You sit in the shade for a while. Something passes overhead that isn't a bird. You do not look up.",
                "It whispers something in a language you don't speak. It seems frustrated. You both manage.",
                "You eat one. It tastes exactly like what you needed. When you reach for another, both are gone.",
                "You lie in it. The waves sound close. When you open your eyes, the light has changed completely.",
            ]
        ),
        TileFeature.WildOrchard => (
            [
                "The fruit trees have had years to develop their own opinions and have done so enthusiastically.",
                "A rope swing hangs from the largest tree, well-knotted and worn smooth by use.",
                "An old stone wall circles the orchard, half-collapsed, with a gate that still swings.",
                "A beehive fills a hollow trunk near the center, enormous and humming with serious business.",
            ], [
                "The apples are small, lopsided, and taste better than any apple has a right to. You eat three.",
                "You swing on it once. You feel briefly embarrassed. You swing again. You stop feeling embarrassed.",
                "Someone carved 'HELP YOURSELF' on the gatepost. You help yourself. It feels genuinely fine.",
                "One bee lands on your hand and examines you at length before flying away. You have been cleared.",
            ]
        ),
        TileFeature.BurnedRuins => (
            [
                "One chimney still stands amid everything that fell, looking vaguely proud of the distinction.",
                "The walls are black with old fire. Behind a fallen beam, a patch of painted plaster survives.",
                "You find a door handle in the ash — ornate, heavy, belonging to something that mattered.",
                "Something moves in the ruins ahead, pausing when you pause.",
            ], [
                "At its base, a hearthstone still bears the scorch marks of a thousand ordinary evenings.",
                "Blue flowers. Very cheerful blue flowers. You wish you hadn't seen them.",
                "You set it back down carefully, exactly where you found it.",
                "You wait it out. It waits longer. You decide it lives here now, and you respect that.",
            ]
        ),
        TileFeature.AncientShrine => (
            [
                "The offerings here span lifetimes: coins, a clay figure, a shoe, a lock of hair.",
                "A candle burns at the altar. It was lit recently. The area is empty.",
                "The inscriptions are in a language you don't know but feel you almost understand.",
                "Your knees bring you down before you've decided to kneel.",
            ], [
                "You feel that arriving with nothing is also a kind of offering. You arrived with nothing.",
                "You search carefully. There is no one. The candle continues anyway.",
                "You trace them with one finger. You feel briefly obligated to something unnamed.",
                "When you stand, something feels different. Not better or worse. Settled.",
            ]
        ),
        TileFeature.RuinedTower => (
            [
                "At the very top of the rubble, impossibly balanced, a stone gargoyle still watches the horizon.",
                "The staircase still winds up the remaining wall, ending at a floor that is no longer there.",
                "Carved above the threshold in letters two feet tall: a warning in an unfamiliar language.",
                "The floor inside was once a mosaic. You piece together what you can of its subject.",
            ], [
                "One eye is missing. The remaining one is extremely alert. It has seen things.",
                "You climb it anyway. At the top you can see very far. Someone is watching from the trees. They don't wave.",
                "You understand it anyway, somehow. It says something was kept here. It says they did their best.",
                "It is a map of somewhere. One tile is missing at the center. You look for it. It isn't here.",
            ]
        ),
        TileFeature.AbandonedFarm => (
            [
                "The table inside is set as if someone left mid-meal, fully intending to return.",
                "A scarecrow stands in the overgrown field, clothes in better shape than anything else here.",
                "In the barn the stalls are empty but the hay is stacked — optimistically.",
                "You find a journal on the windowsill, left open to its last entry.",
            ], [
                "The candle burned down over years, not hours. The chairs still face each other.",
                "It has no face. The hat it wears has strong opinions about you.",
                "The hay has mostly turned to dust. Someone planned to come back.",
                "It is about the weather. Written by someone who expected to write the next one.",
            ]
        ),
        TileFeature.ForgottenGrave => (
            [
                "A cairn of carefully chosen stones, stacked by someone sad and patient and thorough.",
                "The grave marker has fallen face-down in the grass.",
                "Wildflowers have grown so thick over the mound it looks like the ground itself is blooming.",
                "The grave is very small.",
            ], [
                "You add a stone. It feels correct in a way you cannot explain.",
                "You consider turning it over. You decide against it. Some privacy belongs to strangers too.",
                "You stand here for a moment. It seems important that someone stood here.",
                "You stay longer than you intended. You stay until it seems right to leave.",
            ]
        ),
        TileFeature.CrumbledFortress => (
            [
                "The great hall is open to the sky now, birch trees growing up through the old flagstones.",
                "The ramparts — what remains of them — look out over everything this place once defended.",
                "The portcullis has rusted in its raised position, permanently open.",
                "In the armory, one sword has rusted to its stand, patient and immovable.",
            ], [
                "In thirty years of leaves, somewhere underfoot, a goblet waits.",
                "The land is fine. The land was always going to be fine.",
                "Halfway down the gatehouse someone has posted a sign: 'CLOSED.' It is old. It is serious.",
                "You try to pull the sword free. It says no. Not with a sound — with a quality of no.",
            ]
        ),
        TileFeature.StoneCircle => (
            [
                "The stones are older than any name you know, and they are aware of this.",
                "One stone leans slightly. In a crevice against it: a coin, a button, a folded blank piece of paper.",
                "The center of the circle feels like the inside of a held breath.",
                "At a specific moment of the year, the shadows here align to point somewhere. You feel this.",
            ], [
                "You walk the circle. On the third pass you feel briefly seen. On the fourth pass, it has moved on.",
                "You think about the blank piece of paper for an unreasonable amount of time.",
                "You stand in the center. You feel every stone equally behind you. They are doing something you cannot perceive.",
                "You don't know where they point. You feel very strongly that you are meant to.",
            ]
        ),
        TileFeature.LonelyWell => (
            [
                "A stone-rimmed well, older than the nearest building, standing without context.",
                "The bucket hangs on its rope, perfectly preserved and waiting.",
                "The well is dry. You lower yourself in to be sure.",
                "Someone has painted a small, kind face on the stone rim.",
            ], [
                "You drop a coin. A long time passes. Then: a sound from below — a word, maybe. You drop another. Nothing.",
                "You pull it up. It is full of cold, clear water. The well has been dry for thirty years.",
                "At the bottom: an iron box. Empty. Smelling faintly of something sweet.",
                "It is looking at you. You feel better about the well, specifically.",
            ]
        ),
        TileFeature.WitchsCottage => (
            [
                "The smoke from the chimney smells of lavender and burnt hair and something that has no name.",
                "The garden grows things you don't have names for. Some of them appear to have yours.",
                "A cat sits on the windowsill, watching you with the calm authority of ownership.",
                "You knock on the door.",
            ], [
                "The smell follows you for an hour afterward. You decide not to investigate why.",
                "One of them reaches toward you. 'Manners,' you say. It withdraws. You feel you've done well.",
                "It has made a decision about you. You are not certain of the verdict.",
                "Inside: silence, then movement, then silence. The door stays closed. A hand draws the curtain.",
            ]
        ),
        TileFeature.HotSpring => (
            [
                "The water is warm and perfectly clear and smells faintly of minerals and something peaceful.",
                "Steam rises in slow columns that the air seems reluctant to disturb.",
                "The rocks around the spring are coated in orange and yellow minerals, bright as stained glass.",
                "The sound the water makes entering the pool is exactly the sound you needed to hear.",
            ], [
                "You put your hand in. Every muscle you own relaxes simultaneously.",
                "In the steam, briefly: your face. It looks considerably more rested. It winks.",
                "It is beautiful and slightly alarming. You appreciate both things simultaneously.",
                "You sit at the edge with your feet in the water. Something has been lifted. You don't ask what.",
            ]
        ),
        TileFeature.TidePools => (
            [
                "The pools are full of small, improbable creatures committed entirely to their improbable lives.",
                "A hermit crab has established a home in a porcelain teacup and decorated it thoroughly.",
                "Something in the farthest pool glows a soft, cold green.",
                "You crouch at the largest pool and something immediately crouches back.",
            ], [
                "You examine them for a while. One examines you back with equivalent seriousness.",
                "You approve. Moving on, you feel oddly invested in the crab's future.",
                "When you wade toward it, it moves to the next pool. It reaches the sea and is gone.",
                "It is a crab. It raises one claw. You raise one finger. You are here for eleven minutes.",
            ]
        ),
        TileFeature.ReedBeds => (
            [
                "The reeds grow taller than your head, turning the world into corridors of green.",
                "Your reflection in the still water watches you with more attention than reflections usually manage.",
                "Something large moves through the reeds ahead of you, parting them without a sound.",
                "A red-winged blackbird addresses you from a swaying reed with considerable urgency.",
            ], [
                "Something small watches from within the fronds. You feel its specific interest.",
                "When you stop, it stops a half-second late. When you walk, it follows.",
                "The reeds close back where it passed. You stand still for a long time before continuing.",
                "You cannot help with whatever it is. You nod sympathetically. It seems to accept this.",
            ]
        ),
        TileFeature.SaltFlats => (
            [
                "The silence here is not an absence of sound but a presence of something old and dry and patient.",
                "A single wooden post stands at the center of the flats, attached to nothing, explaining nothing.",
                "The heat haze reshapes the horizon continuously: a city, a forest, a face.",
                "The cracks in the salt form a pattern too deliberate to be geological.",
            ], [
                "It is completely untroubled by your visit.",
                "It belongs to reasons that are no longer available for consultation.",
                "You walk toward the face. It stays the same distance away. You walk for a while anyway.",
                "You try to memorize it. You carry only part of it away. Not the part you wanted.",
            ]
        ),
        TileFeature.OasisGrove => (
            [
                "The water appears suddenly between the palms, cold and clear and entirely real.",
                "The shade here has a quality of shade you don't encounter most places.",
                "An ibis stands at the water's edge with the stillness of something that has been here a long time.",
                "Coins cover the bottom of the pool — every traveler who found this place left one.",
            ], [
                "You drink from it with your hands like an animal and feel absolutely no shame about this.",
                "You sit in it and something passes overhead that isn't a bird. You don't look up.",
                "It was standing here before you arrived. It will be standing here when you leave.",
                "You add yours. It feels correct in a way that needs no explaining.",
            ]
        ),
        TileFeature.DeadForest => (
            [
                "The bleached trees stand like columns of testimony to something thorough and final.",
                "A crow sits on every tenth snag, watching you in shifts as you move through.",
                "One fallen tree caught another in falling and they lean together, holding each other up.",
                "The light here is harsh and unfiltered. Nothing soft remains to catch it.",
            ], [
                "Even the wind moves around them. The silence is of the complete variety.",
                "One follows at a consistent distance. You stop. It stops. This continues for a while.",
                "Lichens have grown over them both. The lichens are flourishing and seem delighted.",
                "You see yourself clearly in it. You squint. You feel it might be good for you.",
            ]
        ),
        TileFeature.QuickSand => (
            [
                "The ground here looks perfectly solid.",
                "A sign reads CAUTION.",
                "You test the ground ahead with a stick.",
                "The mud here has the color and texture of confidence.",
            ], [
                "It is not solid. You sort this out with effort and approximately no dignity.",
                "The sign is halfway in. It has read CAUTIO for some time.",
                "The stick disagrees strongly with the ground. You take a different path. The stick does not return.",
                "You commit to a step. The mud commits to disagreeing. You negotiate for a while.",
            ]
        ),
        TileFeature.CaveEntrance => (
            [
                "The cave breathes. One long, slow breath in, one out — cool, mineral-smelling.",
                "The walls of the entrance are covered in pressed handprints, hundreds of them, every size.",
                "You call into the cave.",
                "Something lives in the dark beyond the entrance. You can hear it from here.",
            ], [
                "You breathe with it for a while. It doesn't notice. You notice.",
                "You press your hand against one that is roughly your size. It fits.",
                "Your voice comes back different — not an echo, but a version that has been somewhere.",
                "It breathes slow and rhythmic. It doesn't move toward you. It doesn't need to.",
            ]
        ),
        TileFeature.FrozenShrine => (
            [
                "The altar is sealed in ice so clear the inscriptions are perfectly readable.",
                "The offerings inside the ice are perfectly preserved — flowers, tokens, a small meal.",
                "The ice is lit from within, blue-white, with no visible source.",
                "The cold radiates from the shrine itself, not from the surrounding air.",
            ], [
                "You read them. You wish you hadn't. You read them again.",
                "Someone placed these with no expectation that they would last. They lasted.",
                "A shape moves through its depths — large, deliberate, indifferent. Gone before you can describe it.",
                "You touch it briefly. Something very old and cold is not asleep. You pull your hand back. It lets go.",
            ]
        ),
        TileFeature.IcyCavern => (
            [
                "The ice is every shade of blue, from pale surface to something close to black at depth.",
                "Trapped in a column of ice: a green leaf, perfect, from a tree that no longer grows here.",
                "Your footsteps arrive after you, the acoustics delivering them slightly late.",
                "The cavern opens into a vast blue chamber and you stop moving.",
            ], [
                "You stand in it and feel the word 'cathedral' without knowing why. The ice breathes back when you do.",
                "You stand and look at it for a long time. Nearby, a small fish, frozen mid-turn.",
                "When you stop, they continue two more steps, then stop. You wait. They do not resume.",
                "It is beautiful in a way that makes other beautiful things seem slightly embarrassed.",
            ]
        ),
        _ => (
            ["You examine the area carefully."],
            ["You find it interesting, in your own way."]
        ),
    };

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
