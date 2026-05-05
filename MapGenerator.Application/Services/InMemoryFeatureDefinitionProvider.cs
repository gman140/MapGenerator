using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryFeatureDefinitionProvider : IFeatureDefinitionProvider
{
    private readonly IReadOnlyList<TileFeatureDefinition> _all;
    private readonly Dictionary<string, TileFeatureDefinition> _byId;

    public InMemoryFeatureDefinitionProvider()
    {
        _all  = BuildDefinitions();
        _byId = _all.ToDictionary(d => d.Id);
    }

    public IReadOnlyList<TileFeatureDefinition> All => _all;

    public TileFeatureDefinition? GetById(string? id) =>
        id != null && _byId.TryGetValue(id, out var def) ? def : null;

    private static IReadOnlyList<TileFeatureDefinition> BuildDefinitions() =>
    [
        // ── Forest & nature ────────────────────────────────────────────────
        new()
        {
            Id           = "SpookyWoods",
            DisplayNames = ["Spooky Woods", "Eerie Thicket", "Haunted Grove", "Shadowed Hollows"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Swamp],
            Probability  = 0.040f,
            PartA =
            [
                "The trees lean inward and their branches touch above your head, blocking the sky.",
                "Something laughs, very softly, from a direction you cannot determine.",
                "Your shadow is behaving incorrectly. You can't identify how, exactly.",
                "The birds stopped singing when you entered. You only just noticed.",
            ],
            PartB =
            [
                "You stay longer than you should, trying to name what is wrong. You leave without naming it.",
                "You call out. It laughs again. You do not call out a second time.",
                "You watch it for a long moment. It watches back with what appears to be mild amusement.",
                "You find a small, careful arrangement of stones nearby. A warning or a welcome. Impossible to say.",
            ],
        },
        new()
        {
            Id           = "MushroomGrove",
            DisplayNames = ["Mushroom Grove", "Fungal Clearing", "Mycelium Dell", "Toadstool Hollow"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Swamp, BiomeType.Marsh],
            Probability  = 0.030f,
            PartA =
            [
                "The mushrooms here are enormous — the sitting kind — and they glow a gentle blue.",
                "Spores drift through the air like slow snow, catching the light.",
                "The largest mushroom has a small door carved into it. There is a brass knocker.",
                "One mushroom is significantly larger than the others and appears to be in charge.",
            ],
            PartB =
            [
                "You sit on one. It is extremely comfortable. It hums. You hum back.",
                "One lands on your hand and you understand something important. Then it dissolves. The understanding dissolves with it.",
                "You knock. Nothing answers. The knocker is warm.",
                "You eat a small one. It tastes of strawberries and old regret. The regret is pleasant.",
            ],
        },
        new()
        {
            Id           = "AncientGlade",
            DisplayNames = ["Ancient Glade", "Old Growth Clearing", "Elder Grove", "Forgotten Glade"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Jungle],
            Probability  = 0.025f,
            PartA =
            [
                "A shaft of light falls through the canopy at an angle that no sun position explains.",
                "At the heart of the glade stands a tree wider than a house, older than most things with names.",
                "A deer stands at the far edge, watching you with eyes that are a color deer eyes should not be.",
                "The grass here is implausibly soft — the kind of soft that takes centuries.",
            ],
            PartB =
            [
                "You stand in it. It is warm in a way that has nothing to do with heat.",
                "Carved into its bark are thousands of names — some in alphabets you don't recognize, some in no alphabet at all.",
                "When you blink it is closer. When you blink again it is gone.",
                "You lie down in it for what feels like a moment. The sun has moved considerably when you stand.",
            ],
        },
        new()
        {
            Id           = "FernDell",
            DisplayNames = ["Fern Dell", "Brackish Hollow", "Frond-Draped Nook", "Green Gully"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Jungle, BiomeType.Grassland],
            Probability  = 0.030f,
            PartA =
            [
                "The ferns grow so tall here that the sky is only a green suggestion overhead.",
                "At the center of the dell, partially buried, is a perfect glass marble with a very small storm inside it.",
                "A path winds between the fronds — too regular to be accidental.",
                "Something small and very quick moves between the fronds just ahead of you.",
            ],
            PartB =
            [
                "Something watches you from within. You catch a glimpse: it is smiling about something.",
                "The storm is real and very angry. You put the marble back. It continues.",
                "You follow it a short distance. It leads somewhere you cannot see and turns with great confidence.",
                "You push through to the other side. More ferns. These ones feel older. You cannot explain this.",
            ],
        },
        new()
        {
            Id           = "BramblePatch",
            DisplayNames = ["Brambly Thicket", "Thorned Tangle", "Bristled Hedge", "Snaring Brambles"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Grassland, BiomeType.Swamp],
            Probability  = 0.030f,
            PartA =
            [
                "The thorns catch at your clothes and hold with great personal conviction.",
                "Something shines at the heart of the bramble tangle, catching the light.",
                "The brambles have formed a natural wall around a small clearing with exactly one wooden chair in it.",
                "A single perfect rose grows at the center of all this thorn and difficulty.",
            ],
            PartB =
            [
                "You fight through and find: a brass button engraved with a ship. The nearest sea is very far away.",
                "You stand looking at it for a long time before deciding not to want it.",
                "The chair faces away from you. You consider walking around it. You decide not to.",
                "It has no smell. It has, instead, an expression.",
            ],
        },
        new()
        {
            Id           = "MushroomRing",
            DisplayNames = ["Mushroom Ring", "Fairy Ring", "Circle of Caps", "Fungal Crown"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Swamp, BiomeType.Marsh],
            Probability  = 0.025f,
            PartA =
            [
                "The ring of mushrooms is perfect. Too perfect. Mathematically perfect.",
                "A voice says your name clearly, from no specific direction.",
                "The air inside the ring is perfectly still, even as the wind moves outside it.",
                "You feel an irresistible urge to dance upon seeing the ring.",
            ],
            PartB =
            [
                "You step inside. The world goes very quiet. Not silent — quiet. You stay longer than intended.",
                "You spin around. There is no one. The mushrooms do not blink. You note this carefully.",
                "You stand in the center and feel every stone equidistant behind you. They are doing something. You cannot tell what.",
                "You dance. Briefly. Just a little. The mushrooms seem satisfied. You feel satisfied and confused about feeling satisfied.",
            ],
        },
        new()
        {
            Id           = "TropicalGrove",
            DisplayNames = ["Tropical Grove", "Palm-Fringed Shore", "Lush Bower", "Seaside Thicket"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Jungle, BiomeType.Beach],
            Probability  = 0.035f,
            PartA =
            [
                "The palms cast shade of a quality you haven't encountered before — dense and slightly golden.",
                "A parrot lands on your shoulder with the confidence of something that has decided to be there.",
                "Bright fruit hangs heavy on every branch, ripe beyond what should be possible this season.",
                "A hammock is strung between two palms, well-knotted and worn smooth by use.",
            ],
            PartB =
            [
                "You sit in the shade for a while. Something passes overhead that isn't a bird. You do not look up.",
                "It whispers something in a language you don't speak. It seems frustrated. You both manage.",
                "You eat one. It tastes exactly like what you needed. When you reach for another, both are gone.",
                "You lie in it. The waves sound close. When you open your eyes, the light has changed completely.",
            ],
        },
        new()
        {
            Id           = "WildOrchard",
            DisplayNames = ["Wild Orchard", "Gnarled Fruit Trees", "Tangled Grove", "Feral Garden"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains, BiomeType.Forest],
            Probability  = 0.025f,
            PartA =
            [
                "The fruit trees have had years to develop their own opinions and have done so enthusiastically.",
                "A rope swing hangs from the largest tree, well-knotted and worn smooth by use.",
                "An old stone wall circles the orchard, half-collapsed, with a gate that still swings.",
                "A beehive fills a hollow trunk near the center, enormous and humming with serious business.",
            ],
            PartB =
            [
                "The apples are small, lopsided, and taste better than any apple has a right to. You eat three.",
                "You swing on it once. You feel briefly embarrassed. You swing again. You stop feeling embarrassed.",
                "Someone carved 'HELP YOURSELF' on the gatepost. You help yourself. It feels genuinely fine.",
                "One bee lands on your hand and examines you at length before flying away. You have been cleared.",
            ],
        },

        // ── Ruins & man-made ───────────────────────────────────────────────
        new()
        {
            Id           = "BurnedRuins",
            DisplayNames = ["Burned Ruins", "Scorched Settlement", "Charred Remains", "Ash-Fallen Hamlet"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Grassland, BiomeType.Plains],
            Probability  = 0.020f,
            PartA =
            [
                "One chimney still stands amid everything that fell, looking vaguely proud of the distinction.",
                "The walls are black with old fire. Behind a fallen beam, a patch of painted plaster survives.",
                "You find a door handle in the ash — ornate, heavy, belonging to something that mattered.",
                "Something moves in the ruins ahead, pausing when you pause.",
            ],
            PartB =
            [
                "At its base, a hearthstone still bears the scorch marks of a thousand ordinary evenings.",
                "Blue flowers. Very cheerful blue flowers. You wish you hadn't seen them.",
                "You set it back down carefully, exactly where you found it.",
                "You wait it out. It waits longer. You decide it lives here now, and you respect that.",
            ],
        },
        new()
        {
            Id           = "AncientShrine",
            DisplayNames = ["Ancient Shrine", "Crumbled Altar", "Moss-Covered Shrine", "Weathered Offering Stone"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Mountain, BiomeType.Plains],
            Probability  = 0.015f,
            PartA =
            [
                "The offerings here span lifetimes: coins, a clay figure, a shoe, a lock of hair.",
                "A candle burns at the altar. It was lit recently. The area is empty.",
                "The inscriptions are in a language you don't know but feel you almost understand.",
                "Your knees bring you down before you've decided to kneel.",
            ],
            PartB =
            [
                "You feel that arriving with nothing is also a kind of offering. You arrived with nothing.",
                "You search carefully. There is no one. The candle continues anyway.",
                "You trace them with one finger. You feel briefly obligated to something unnamed.",
                "When you stand, something feels different. Not better or worse. Settled.",
            ],
        },
        new()
        {
            Id           = "RuinedTower",
            DisplayNames = ["Ruined Tower", "Crumbled Watchtower", "Collapsed Spire", "Broken Keep"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains, BiomeType.Mountain],
            Probability  = 0.015f,
            PartA =
            [
                "At the very top of the rubble, impossibly balanced, a stone gargoyle still watches the horizon.",
                "The staircase still winds up the remaining wall, ending at a floor that is no longer there.",
                "Carved above the threshold in letters two feet tall: a warning in an unfamiliar language.",
                "The floor inside was once a mosaic. You piece together what you can of its subject.",
            ],
            PartB =
            [
                "One eye is missing. The remaining one is extremely alert. It has seen things.",
                "You climb it anyway. At the top you can see very far. Someone is watching from the trees. They don't wave.",
                "You understand it anyway, somehow. It says something was kept here. It says they did their best.",
                "It is a map of somewhere. One tile is missing at the center. You look for it. It isn't here.",
            ],
        },
        new()
        {
            Id           = "AbandonedFarm",
            DisplayNames = ["Abandoned Farm", "Overgrown Homestead", "Derelict Farmstead", "Forgotten Fields"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains],
            Probability  = 0.025f,
            PartA =
            [
                "The table inside is set as if someone left mid-meal, fully intending to return.",
                "A scarecrow stands in the overgrown field, clothes in better shape than anything else here.",
                "In the barn the stalls are empty but the hay is stacked — optimistically.",
                "You find a journal on the windowsill, left open to its last entry.",
            ],
            PartB =
            [
                "The candle burned down over years, not hours. The chairs still face each other.",
                "It has no face. The hat it wears has strong opinions about you.",
                "The hay has mostly turned to dust. Someone planned to come back.",
                "It is about the weather. Written by someone who expected to write the next one.",
            ],
        },
        new()
        {
            Id           = "ForgottenGrave",
            DisplayNames = ["Forgotten Grave", "Unmarked Burial", "Moss-Grown Cairn", "Lonely Burial Mound"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains, BiomeType.Desert],
            Probability  = 0.020f,
            PartA =
            [
                "A cairn of carefully chosen stones, stacked by someone sad and patient and thorough.",
                "The grave marker has fallen face-down in the grass.",
                "Wildflowers have grown so thick over the mound it looks like the ground itself is blooming.",
                "The grave is very small.",
            ],
            PartB =
            [
                "You add a stone. It feels correct in a way you cannot explain.",
                "You consider turning it over. You decide against it. Some privacy belongs to strangers too.",
                "You stand here for a moment. It seems important that someone stood here.",
                "You stay longer than you intended. You stay until it seems right to leave.",
            ],
        },
        new()
        {
            Id           = "CrumbledFortress",
            DisplayNames = ["Crumbled Fortress", "Ruined Battlement", "Fallen Citadel", "Ancient Rampart"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Plains],
            Probability  = 0.010f,
            PartA =
            [
                "The great hall is open to the sky now, birch trees growing up through the old flagstones.",
                "The ramparts — what remains of them — look out over everything this place once defended.",
                "The portcullis has rusted in its raised position, permanently open.",
                "In the armory, one sword has rusted to its stand, patient and immovable.",
            ],
            PartB =
            [
                "In thirty years of leaves, somewhere underfoot, a goblet waits.",
                "The land is fine. The land was always going to be fine.",
                "Halfway down the gatehouse someone has posted a sign: 'CLOSED.' It is old. It is serious.",
                "You try to pull the sword free. It says no. Not with a sound — with a quality of no.",
            ],
        },
        new()
        {
            Id           = "StoneCircle",
            DisplayNames = ["Stone Circle", "Standing Stones", "Ancient Ring", "Megalith Ring"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Plains, BiomeType.Grassland, BiomeType.Mountain],
            Probability  = 0.010f,
            PartA =
            [
                "The stones are older than any name you know, and they are aware of this.",
                "One stone leans slightly. In a crevice against it: a coin, a button, a folded blank piece of paper.",
                "The center of the circle feels like the inside of a held breath.",
                "At a specific moment of the year, the shadows here align to point somewhere. You feel this.",
            ],
            PartB =
            [
                "You walk the circle. On the third pass you feel briefly seen. On the fourth pass, it has moved on.",
                "You think about the blank piece of paper for an unreasonable amount of time.",
                "You stand in the center. You feel every stone equally behind you. They are doing something you cannot perceive.",
                "You don't know where they point. You feel very strongly that you are meant to.",
            ],
        },
        new()
        {
            Id           = "LonelyWell",
            DisplayNames = ["Lonely Well", "Crumbling Well", "Stone-Rimmed Well", "Forgotten Well"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Plains, BiomeType.Desert, BiomeType.Grassland],
            Probability  = 0.015f,
            PartA =
            [
                "A stone-rimmed well, older than the nearest building, standing without context.",
                "The bucket hangs on its rope, perfectly preserved and waiting.",
                "The well is dry. You lower yourself in to be sure.",
                "Someone has painted a small, kind face on the stone rim.",
            ],
            PartB =
            [
                "You drop a coin. A long time passes. Then: a sound from below — a word, maybe. You drop another. Nothing.",
                "You pull it up. It is full of cold, clear water. The well has been dry for thirty years.",
                "At the bottom: an iron box. Empty. Smelling faintly of something sweet.",
                "It is looking at you. You feel better about the well, specifically.",
            ],
        },
        new()
        {
            Id           = "WitchsCottage",
            DisplayNames = ["Witch's Cottage", "Hermit's Hovel", "Crumbled Hut", "Smoke-Stained Den"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Swamp, BiomeType.Forest],
            Probability  = 0.010f,
            PartA =
            [
                "The smoke from the chimney smells of lavender and burnt hair and something that has no name.",
                "The garden grows things you don't have names for. Some of them appear to have yours.",
                "A cat sits on the windowsill, watching you with the calm authority of ownership.",
                "You knock on the door.",
            ],
            PartB =
            [
                "The smell follows you for an hour afterward. You decide not to investigate why.",
                "One of them reaches toward you. 'Manners,' you say. It withdraws. You feel you've done well.",
                "It has made a decision about you. You are not certain of the verdict.",
                "Inside: silence, then movement, then silence. The door stays closed. A hand draws the curtain.",
            ],
        },

        // ── Water & wet ────────────────────────────────────────────────────
        new()
        {
            Id           = "HotSpring",
            DisplayNames = ["Hot Spring", "Bubbling Vent", "Steaming Pool", "Geothermal Basin"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Snow, BiomeType.Tundra],
            Probability  = 0.020f,
            PartA =
            [
                "The water is warm and perfectly clear and smells faintly of minerals and something peaceful.",
                "Steam rises in slow columns that the air seems reluctant to disturb.",
                "The rocks around the spring are coated in orange and yellow minerals, bright as stained glass.",
                "The sound the water makes entering the pool is exactly the sound you needed to hear.",
            ],
            PartB =
            [
                "You put your hand in. Every muscle you own relaxes simultaneously.",
                "In the steam, briefly: your face. It looks considerably more rested. It winks.",
                "It is beautiful and slightly alarming. You appreciate both things simultaneously.",
                "You sit at the edge with your feet in the water. Something has been lifted. You don't ask what.",
            ],
        },
        new()
        {
            Id           = "TidePools",
            DisplayNames = ["Tide Pools", "Rocky Shallows", "Shell-Strewn Shore", "Barnacled Flats"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Beach, BiomeType.Shallows],
            Probability  = 0.040f,
            PartA =
            [
                "The pools are full of small, improbable creatures committed entirely to their improbable lives.",
                "A hermit crab has established a home in a porcelain teacup and decorated it thoroughly.",
                "Something in the farthest pool glows a soft, cold green.",
                "You crouch at the largest pool and something immediately crouches back.",
            ],
            PartB =
            [
                "You examine them for a while. One examines you back with equivalent seriousness.",
                "You approve. Moving on, you feel oddly invested in the crab's future.",
                "When you wade toward it, it moves to the next pool. It reaches the sea and is gone.",
                "It is a crab. It raises one claw. You raise one finger. You are here for eleven minutes.",
            ],
        },
        new()
        {
            Id           = "ReedBeds",
            DisplayNames = ["Reed Beds", "Cattail Marsh", "Papyrus Stand", "Waving Reeds"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Marsh, BiomeType.River],
            Probability  = 0.040f,
            PartA =
            [
                "The reeds grow taller than your head, turning the world into corridors of green.",
                "Your reflection in the still water watches you with more attention than reflections usually manage.",
                "Something large moves through the reeds ahead of you, parting them without a sound.",
                "A red-winged blackbird addresses you from a swaying reed with considerable urgency.",
            ],
            PartB =
            [
                "Something small watches from within the fronds. You feel its specific interest.",
                "When you stop, it stops a half-second late. When you walk, it follows.",
                "The reeds close back where it passed. You stand still for a long time before continuing.",
                "You cannot help with whatever it is. You nod sympathetically. It seems to accept this.",
            ],
        },

        // ── Desert & dry ───────────────────────────────────────────────────
        new()
        {
            Id           = "SaltFlats",
            DisplayNames = ["Salt Flats", "Bleached Expanse", "Cracked Saltpan", "White Wastes"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Beach],
            Probability  = 0.025f,
            PartA =
            [
                "The silence here is not an absence of sound but a presence of something old and dry and patient.",
                "A single wooden post stands at the center of the flats, attached to nothing, explaining nothing.",
                "The heat haze reshapes the horizon continuously: a city, a forest, a face.",
                "The cracks in the salt form a pattern too deliberate to be geological.",
            ],
            PartB =
            [
                "It is completely untroubled by your visit.",
                "It belongs to reasons that are no longer available for consultation.",
                "You walk toward the face. It stays the same distance away. You walk for a while anyway.",
                "You try to memorize it. You carry only part of it away. Not the part you wanted.",
            ],
        },
        new()
        {
            Id           = "OasisGrove",
            DisplayNames = ["Oasis Grove", "Desert Spring", "Palmed Hollow", "Hidden Waterhole"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert],
            Probability  = 0.020f,
            PartA =
            [
                "The water appears suddenly between the palms, cold and clear and entirely real.",
                "The shade here has a quality of shade you don't encounter most places.",
                "An ibis stands at the water's edge with the stillness of something that has been here a long time.",
                "Coins cover the bottom of the pool — every traveler who found this place left one.",
            ],
            PartB =
            [
                "You drink from it with your hands like an animal and feel absolutely no shame about this.",
                "You sit in it and something passes overhead that isn't a bird. You don't look up.",
                "It was standing here before you arrived. It will be standing here when you leave.",
                "You add yours. It feels correct in a way that needs no explaining.",
            ],
        },
        new()
        {
            Id           = "DeadForest",
            DisplayNames = ["Dead Forest", "Bleached Snags", "Skeletal Timbers", "Withered Copse"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Plains],
            Probability  = 0.020f,
            PartA =
            [
                "The bleached trees stand like columns of testimony to something thorough and final.",
                "A crow sits on every tenth snag, watching you in shifts as you move through.",
                "One fallen tree caught another in falling and they lean together, holding each other up.",
                "The light here is harsh and unfiltered. Nothing soft remains to catch it.",
            ],
            PartB =
            [
                "Even the wind moves around them. The silence is of the complete variety.",
                "One follows at a consistent distance. You stop. It stops. This continues for a while.",
                "Lichens have grown over them both. The lichens are flourishing and seem delighted.",
                "You see yourself clearly in it. You squint. You feel it might be good for you.",
            ],
        },
        new()
        {
            Id           = "QuickSand",
            DisplayNames = ["Quicksand", "Treacherous Silt", "Sucking Mud", "Shifting Sand Trap"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Swamp, BiomeType.Marsh],
            Probability  = 0.020f,
            PartA =
            [
                "The ground here looks perfectly solid.",
                "A sign reads CAUTION.",
                "You test the ground ahead with a stick.",
                "The mud here has the color and texture of confidence.",
            ],
            PartB =
            [
                "It is not solid. You sort this out with effort and approximately no dignity.",
                "The sign is halfway in. It has read CAUTIO for some time.",
                "The stick disagrees strongly with the ground. You take a different path. The stick does not return.",
                "You commit to a step. The mud commits to disagreeing. You negotiate for a while.",
            ],
        },

        // ── Mountain & cold ────────────────────────────────────────────────
        new()
        {
            Id           = "CaveEntrance",
            DisplayNames = ["Cave Entrance", "Dark Hollow", "Rocky Cavern", "Yawning Shaft"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Glacier],
            Probability  = 0.035f,
            PartA =
            [
                "The cave breathes. One long, slow breath in, one out — cool, mineral-smelling.",
                "The walls of the entrance are covered in pressed handprints, hundreds of them, every size.",
                "You call into the cave.",
                "Something lives in the dark beyond the entrance. You can hear it from here.",
            ],
            PartB =
            [
                "You breathe with it for a while. It doesn't notice. You notice.",
                "You press your hand against one that is roughly your size. It fits.",
                "Your voice comes back different — not an echo, but a version that has been somewhere.",
                "It breathes slow and rhythmic. It doesn't move toward you. It doesn't need to.",
            ],
        },
        new()
        {
            Id           = "FrozenShrine",
            DisplayNames = ["Frozen Shrine", "Ice-Locked Altar", "Glacial Monument", "Frost-Covered Stele"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Snow, BiomeType.Glacier, BiomeType.Tundra],
            Probability  = 0.020f,
            PartA =
            [
                "The altar is sealed in ice so clear the inscriptions are perfectly readable.",
                "The offerings inside the ice are perfectly preserved — flowers, tokens, a small meal.",
                "The ice is lit from within, blue-white, with no visible source.",
                "The cold radiates from the shrine itself, not from the surrounding air.",
            ],
            PartB =
            [
                "You read them. You wish you hadn't. You read them again.",
                "Someone placed these with no expectation that they would last. They lasted.",
                "A shape moves through its depths — large, deliberate, indifferent. Gone before you can describe it.",
                "You touch it briefly. Something very old and cold is not asleep. You pull your hand back. It lets go.",
            ],
        },
        new()
        {
            Id           = "IcyCavern",
            DisplayNames = ["Icy Cavern", "Frost Cave", "Glacial Grotto", "Blue-Ice Hollow"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Glacier, BiomeType.Snow],
            Probability  = 0.025f,
            PartA =
            [
                "The ice is every shade of blue, from pale surface to something close to black at depth.",
                "Trapped in a column of ice: a green leaf, perfect, from a tree that no longer grows here.",
                "Your footsteps arrive after you, the acoustics delivering them slightly late.",
                "The cavern opens into a vast blue chamber and you stop moving.",
            ],
            PartB =
            [
                "You stand in it and feel the word 'cathedral' without knowing why. The ice breathes back when you do.",
                "You stand and look at it for a long time. Nearby, a small fish, frozen mid-turn.",
                "When you stop, they continue two more steps, then stop. You wait. They do not resume.",
                "It is beautiful in a way that makes other beautiful things seem slightly embarrassed.",
            ],
        },
    ];
}
