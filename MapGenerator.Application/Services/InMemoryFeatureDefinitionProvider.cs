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
            DisplayNames = ["Spooky Woods", "Eerie Thicket", "Haunted Grove", "Shadowed Hollows", "Whispering Thicket", "Gloaming Wood", "Murk-Bowed Trees", "Bone-White Copse"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Swamp],
            Probability  = 0.040f,
            ResourceYields =
            [
                new() { ResourceId = "CrowFeather",  Probability = 0.12f },
                new() { ResourceId = "PaleMushroom", Probability = 0.10f },
            ],
            PartA =
            [
                "The trees lean inward and their branches touch above your head, blocking the sky.",
                "Something laughs, very softly, from a direction you cannot determine.",
                "Your shadow is behaving incorrectly. You can't identify how, exactly.",
                "The birds stopped singing when you entered. You only just noticed.",
                "The path you came in on is no longer visible.",
                "One tree has a face. It does not move. It watches you with its whole face.",
                "Footprints in the mud ahead. Yours, from later.",
            ],
            PartB =
            [
                "You stay longer than you should, trying to name what is wrong. You leave without naming it.",
                "You call out. It laughs again. You do not call out a second time.",
                "You watch it for a long moment. It watches back with what appears to be mild amusement.",
                "You find a small, careful arrangement of stones nearby. A warning or a welcome. Impossible to say.",
                "You find it eventually. You are no longer sure which direction out is. You pick one.",
                "It watches back patiently, through its whole face, with complete equanimity.",
                "This is not possible. You walk out quickly. The prints are gone when you look back.",
            ],
        },
        new()
        {
            Id           = "MushroomGrove",
            DisplayNames = ["Mushroom Grove", "Fungal Clearing", "Mycelium Dell", "Toadstool Hollow", "Spore-Drift Hollow", "Bioluminescent Dell", "Cap-Crowded Glade", "Velvet-Footed Garden"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Swamp, BiomeType.Marsh],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "PaleMushroom", Probability = 0.50f },
                new() { ResourceId = "Herbs",        Probability = 0.25f },
            ],
            PartA =
            [
                "The mushrooms here are enormous — the sitting kind — and they glow a gentle blue.",
                "Spores drift through the air like slow snow, catching the light.",
                "The largest mushroom has a small door carved into it. There is a brass knocker.",
                "One mushroom is significantly larger than the others and appears to be in charge.",
                "The smell here is rich and old and has opinions about you.",
                "Two mushrooms of different sizes are clearly having an argument. You have interrupted it.",
                "The ground is soft underfoot in a way that suggests it is paying attention.",
            ],
            PartB =
            [
                "You sit on one. It is extremely comfortable. It hums. You hum back.",
                "One lands on your hand and you understand something important. Then it dissolves. The understanding dissolves with it.",
                "You knock. Nothing answers. The knocker is warm.",
                "You eat a small one. It tastes of strawberries and old regret. The regret is pleasant.",
                "The smell stays with you. It is saying something. You just can't hear it clearly.",
                "They resume when you step back. The smaller one wins.",
                "You step carefully. It adjusts slightly under each foot. You both pretend this is not happening.",
            ],
        },
        new()
        {
            Id           = "AncientGlade",
            DisplayNames = ["Ancient Glade", "Old Growth Clearing", "Elder Grove", "Forgotten Glade", "Timeless Clearing", "Memory Glade", "Root-Wrapped Hollow", "Sacred Stand"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Jungle],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Amber", Probability = 0.20f },
                new() { ResourceId = "Herbs", Probability = 0.20f },
            ],
            PartA =
            [
                "A shaft of light falls through the canopy at an angle that no sun position explains.",
                "At the heart of the glade stands a tree wider than a house, older than most things with names.",
                "A deer stands at the far edge, watching you with eyes that are a color deer eyes should not be.",
                "The grass here is implausibly soft — the kind of soft that takes centuries.",
                "The air here is different — stiller, older, slightly too respectful.",
                "At the base of the great tree, offerings: a coin, a feather, three small polished stones arranged in a line.",
                "The glade is perfectly circular. Not approximately. Exactly.",
            ],
            PartB =
            [
                "You stand in it. It is warm in a way that has nothing to do with heat.",
                "Carved into its bark are thousands of names — some in alphabets you don't recognize, some in no alphabet at all.",
                "When you blink it is closer. When you blink again it is gone.",
                "You lie down in it for what feels like a moment. The sun has moved considerably when you stand.",
                "You lower your voice without meaning to. Then you stop speaking entirely.",
                "You add a stone you picked up earlier. It feels like completing a sentence.",
                "You stand at the edge of it and feel the exactness of it as a kind of pressure.",
            ],
        },
        new()
        {
            Id           = "FernDell",
            DisplayNames = ["Fern Dell", "Brackish Hollow", "Frond-Draped Nook", "Green Gully", "Verdant Hollow", "Frond Corridor", "Dim-Leaved Cranny", "Soft-Floor Nook"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Jungle, BiomeType.Grassland],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "Herbs", Probability = 0.25f },
                new() { ResourceId = "Fiber", Probability = 0.20f },
            ],
            PartA =
            [
                "The ferns grow so tall here that the sky is only a green suggestion overhead.",
                "At the center of the dell, partially buried, is a perfect glass marble with a very small storm inside it.",
                "A path winds between the fronds — too regular to be accidental.",
                "Something small and very quick moves between the fronds just ahead of you.",
                "Everything here smells of rain that happened somewhere else and traveled.",
                "The ferns move in a wave, from one side to the other, as if following something invisible.",
                "A small stone bench sits at the curve of the path, worn smooth and perfectly positioned.",
            ],
            PartB =
            [
                "Something watches you from within. You catch a glimpse: it is smiling about something.",
                "The storm is real and very angry. You put the marble back. It continues.",
                "You follow it a short distance. It leads somewhere you cannot see and turns with great confidence.",
                "You push through to the other side. More ferns. These ones feel older. You cannot explain this.",
                "You stand in the smell for a while and feel it settle on you.",
                "You stand still and watch it cross the dell. It reaches the far side and starts again.",
                "You sit on it. Someone knew exactly where it should go. You feel grateful to that person.",
            ],
        },
        new()
        {
            Id           = "BramblePatch",
            DisplayNames = ["Brambly Thicket", "Thorned Tangle", "Bristled Hedge", "Snaring Brambles", "Thorn-Woven Tangle", "Pricking Hedge", "Snag-Fast Brush", "Twisted Thicket"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Grassland, BiomeType.Swamp],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "Fiber",      Probability = 0.25f },
                new() { ResourceId = "Herbs",      Probability = 0.15f },
                new() { ResourceId = "CrowFeather",Probability = 0.08f },
            ],
            PartA =
            [
                "The thorns catch at your clothes and hold with great personal conviction.",
                "Something shines at the heart of the bramble tangle, catching the light.",
                "The brambles have formed a natural wall around a small clearing with exactly one wooden chair in it.",
                "A single perfect rose grows at the center of all this thorn and difficulty.",
                "You find a gap in the brambles just wide enough to slip through — as if it was left.",
                "Birds nest deep in the tangle. You can hear the arguments.",
                "The thorns have caught pieces of passing things: a ribbon, a scrap of map, three blue buttons.",
            ],
            PartB =
            [
                "You fight through and find: a brass button engraved with a ship. The nearest sea is very far away.",
                "You stand looking at it for a long time before deciding not to want it.",
                "The chair faces away from you. You consider walking around it. You decide not to.",
                "It has no smell. It has, instead, an expression.",
                "You slip through carefully and emerge on the other side unscratched. This feels like a gift.",
                "You listen for a while. The disputes are numerous and unresolved.",
                "You examine the scrap of map. It shows somewhere you haven't been. The part with the X is torn off.",
            ],
        },
        new()
        {
            Id           = "MushroomRing",
            DisplayNames = ["Mushroom Ring", "Fairy Ring", "Circle of Caps", "Fungal Crown", "Toadstool Circle", "Enchanted Ring", "Spore-Circle", "Rim of Caps"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Swamp, BiomeType.Marsh],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "PaleMushroom", Probability = 0.35f },
                new() { ResourceId = "RottenSilks",  Probability = 0.08f },
            ],
            PartA =
            [
                "The ring of mushrooms is perfect. Too perfect. Mathematically perfect.",
                "A voice says your name clearly, from no specific direction.",
                "The air inside the ring is perfectly still, even as the wind moves outside it.",
                "You feel an irresistible urge to dance upon seeing the ring.",
                "The grass inside the ring is a noticeably different shade of green than outside. It has been for a long time.",
                "A single stone sits in the exact center of the ring. It has been placed, not fallen.",
                "You feel watched before you see the ring. The ring explains the feeling.",
            ],
            PartB =
            [
                "You step inside. The world goes very quiet. Not silent — quiet. You stay longer than intended.",
                "You spin around. There is no one. The mushrooms do not blink. You note this carefully.",
                "You stand in the center and feel every stone equidistant behind you. They are doing something. You cannot tell what.",
                "You dance. Briefly. Just a little. The mushrooms seem satisfied. You feel satisfied and confused about feeling satisfied.",
                "You study the difference from the outside for a while. The grass inside does not explain itself.",
                "You pick it up. It is warm. You put it back exactly where it was.",
                "The feeling doesn't stop when you leave. It fades slowly, like something reluctant.",
            ],
        },
        new()
        {
            Id           = "TropicalGrove",
            DisplayNames = ["Tropical Grove", "Palm-Fringed Shore", "Lush Bower", "Seaside Thicket", "Shaded Shoreline", "Frond-Bright Alcove", "Warm-Air Bower", "Coconut Hollow"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Jungle, BiomeType.Beach],
            Probability  = 0.035f,
            ResourceYields =
            [
                new() { ResourceId = "Wood",     Probability = 0.30f },
                new() { ResourceId = "Feathers", Probability = 0.20f },
                new() { ResourceId = "Fiber",    Probability = 0.20f },
            ],
            PartA =
            [
                "The palms cast shade of a quality you haven't encountered before — dense and slightly golden.",
                "A parrot lands on your shoulder with the confidence of something that has decided to be there.",
                "Bright fruit hangs heavy on every branch, ripe beyond what should be possible this season.",
                "A hammock is strung between two palms, well-knotted and worn smooth by use.",
                "The light filters through palm fronds in slow gold patterns that don't match the wind.",
                "A small fire has been built recently and allowed to burn out neatly.",
                "The insects here are enormous and have a general atmosphere of contentment.",
            ],
            PartB =
            [
                "You sit in the shade for a while. Something passes overhead that isn't a bird. You do not look up.",
                "It whispers something in a language you don't speak. It seems frustrated. You both manage.",
                "You eat one. It tastes exactly like what you needed. When you reach for another, both are gone.",
                "You lie in it. The waves sound close. When you open your eyes, the light has changed completely.",
                "You track one with your eyes for a while. It doesn't match the fronds. You stop tracking it.",
                "Someone was here not long ago and left it tidy. You feel an irrational warmth toward them.",
                "One lands on your hand. It seems pleased. You feel you have been approved of.",
            ],
        },
        new()
        {
            Id           = "WildOrchard",
            DisplayNames = ["Wild Orchard", "Gnarled Fruit Trees", "Tangled Grove", "Feral Garden", "Untended Grove", "Overgrown Arbor", "Rogue Fruit Trees", "Self-Made Garden"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains, BiomeType.Forest],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Fiber", Probability = 0.30f },
                new() { ResourceId = "Herbs", Probability = 0.20f },
                new() { ResourceId = "Amber", Probability = 0.10f },
            ],
            PartA =
            [
                "The fruit trees have had years to develop their own opinions and have done so enthusiastically.",
                "A rope swing hangs from the largest tree, well-knotted and worn smooth by use.",
                "An old stone wall circles the orchard, half-collapsed, with a gate that still swings.",
                "A beehive fills a hollow trunk near the center, enormous and humming with serious business.",
                "Bees move in purposeful lines between the blossoms, confident about where they are going.",
                "A broken ladder leans against the tallest tree, the top rung still well short of the lowest branch.",
                "The oldest tree in the center is hollow and full of something living — you can hear it breathing.",
            ],
            PartB =
            [
                "The apples are small, lopsided, and taste better than any apple has a right to. You eat three.",
                "You swing on it once. You feel briefly embarrassed. You swing again. You stop feeling embarrassed.",
                "Someone carved 'HELP YOURSELF' on the gatepost. You help yourself. It feels genuinely fine.",
                "One bee lands on your hand and examines you at length before flying away. You have been cleared.",
                "You follow one for a while, then lose it in the blossoms. The blossoms were worth stopping for.",
                "You stand on the first rung anyway. The branches are still too high. You feel this says something.",
                "You stand quietly beside it and listen to the breathing. It listens back. Mutually acceptable.",
            ],
        },

        // ── Ruins & man-made ───────────────────────────────────────────────
        new()
        {
            Id           = "BurnedRuins",
            DisplayNames = ["Burned Ruins", "Scorched Settlement", "Charred Remains", "Ash-Fallen Hamlet", "Cinder Hollow", "Fire-Eaten Walls", "Blackened Settlement", "Smoke-Touched Remains"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Grassland, BiomeType.Plains],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Ash",         Probability = 0.40f },
                new() { ResourceId = "Coal",        Probability = 0.30f },
                new() { ResourceId = "BoneFragment",Probability = 0.12f },
            ],
            PartA =
            [
                "One chimney still stands amid everything that fell, looking vaguely proud of the distinction.",
                "The walls are black with old fire. Behind a fallen beam, a patch of painted plaster survives.",
                "You find a door handle in the ash — ornate, heavy, belonging to something that mattered.",
                "Something moves in the ruins ahead, pausing when you pause.",
                "The chimneys stand like gravestones. There were more people here than you expected.",
                "In what was a kitchen: a pot still on its hook, black but intact.",
                "Among the ash you find a single playing card, perfectly preserved — the king of hearts.",
            ],
            PartB =
            [
                "At its base, a hearthstone still bears the scorch marks of a thousand ordinary evenings.",
                "Blue flowers. Very cheerful blue flowers. You wish you hadn't seen them.",
                "You set it back down carefully, exactly where you found it.",
                "You wait it out. It waits longer. You decide it lives here now, and you respect that.",
                "You count them. You stop counting when the number becomes uncomfortable.",
                "You don't touch it. Some things have earned the right to be left where they are.",
                "You wonder what it means. You know it doesn't mean anything. You keep it anyway.",
            ],
        },
        new()
        {
            Id           = "AncientShrine",
            DisplayNames = ["Ancient Shrine", "Crumbled Altar", "Moss-Covered Shrine", "Weathered Offering Stone", "Crumbled Sanctum", "Offering Stone", "Sacred Ruin", "Weathered Holy Place"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Forest, BiomeType.Mountain, BiomeType.Plains],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "TarnishedRing", Probability = 0.08f },
                new() { ResourceId = "CrackedOrb",    Probability = 0.05f },
            ],
            PartA =
            [
                "The offerings here span lifetimes: coins, a clay figure, a shoe, a lock of hair.",
                "A candle burns at the altar. It was lit recently. The area is empty.",
                "The inscriptions are in a language you don't know but feel you almost understand.",
                "Your knees bring you down before you've decided to kneel.",
                "A bird sits on top of the altar, still as a carving, watching you with one eye.",
                "Rain and centuries have worn the carvings smooth, but something of the intent survives.",
                "The wildflowers that grow around the base shouldn't grow here. They are growing here.",
            ],
            PartB =
            [
                "You feel that arriving with nothing is also a kind of offering. You arrived with nothing.",
                "You search carefully. There is no one. The candle continues anyway.",
                "You trace them with one finger. You feel briefly obligated to something unnamed.",
                "When you stand, something feels different. Not better or worse. Settled.",
                "It doesn't move when you approach. It was here before you and will be here after.",
                "You trace the worn shapes. They are doing their best. You feel a kinship.",
                "You stand among them for a moment, feeling out of jurisdiction in a comfortable way.",
            ],
        },
        new()
        {
            Id           = "RuinedTower",
            DisplayNames = ["Ruined Tower", "Crumbled Watchtower", "Collapsed Spire", "Broken Keep", "Toppled Spire", "Half-Standing Keep", "Tumbled Watchtower", "Weather-Worn Turret"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains, BiomeType.Mountain],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "Stone",        Probability = 0.30f },
                new() { ResourceId = "TarnishedRing",Probability = 0.08f },
                new() { ResourceId = "HollowStone",  Probability = 0.08f },
            ],
            PartA =
            [
                "At the very top of the rubble, impossibly balanced, a stone gargoyle still watches the horizon.",
                "The staircase still winds up the remaining wall, ending at a floor that is no longer there.",
                "Carved above the threshold in letters two feet tall: a warning in an unfamiliar language.",
                "The floor inside was once a mosaic. You piece together what you can of its subject.",
                "The storeroom below is intact, still sealed. Something inside has been keeping.",
                "A window frame remains in the rubble, fitted with colored glass — one green pane, one red, one missing.",
                "Ivy has covered the remaining walls completely, holding the stones together with patient intention.",
            ],
            PartB =
            [
                "One eye is missing. The remaining one is extremely alert. It has seen things.",
                "You climb it anyway. At the top you can see very far. Someone is watching from the trees. They don't wave.",
                "You understand it anyway, somehow. It says something was kept here. It says they did their best.",
                "It is a map of somewhere. One tile is missing at the center. You look for it. It isn't here.",
                "You manage the door eventually. Inside: nothing. The nothing has been preserved carefully.",
                "You find the missing pane nearby. It is intact. You hold it up and the world goes briefly red.",
                "You press your hand flat against it. The ivy adjusts slightly. Something about this feels like a greeting.",
            ],
        },
        new()
        {
            Id           = "AbandonedFarm",
            DisplayNames = ["Abandoned Farm", "Overgrown Homestead", "Derelict Farmstead", "Forgotten Fields", "Empty Homestead", "Weed-Claimed Fields", "Neglected Acreage", "Last Farm"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Fiber", Probability = 0.30f },
                new() { ResourceId = "Flint", Probability = 0.15f },
            ],
            PartA =
            [
                "The table inside is set as if someone left mid-meal, fully intending to return.",
                "A scarecrow stands in the overgrown field, clothes in better shape than anything else here.",
                "In the barn the stalls are empty but the hay is stacked — optimistically.",
                "You find a journal on the windowsill, left open to its last entry.",
                "The garden still grows, untended — the vegetables have developed strong opinions about direction.",
                "A child's drawing is pinned to the wall, protected by the overhang. Still bright.",
                "Two rocking chairs on the porch face the field. One is still rocking, very slightly, from no wind.",
            ],
            PartB =
            [
                "The candle burned down over years, not hours. The chairs still face each other.",
                "It has no face. The hat it wears has strong opinions about you.",
                "The hay has mostly turned to dust. Someone planned to come back.",
                "It is about the weather. Written by someone who expected to write the next one.",
                "They grow exactly as they like now. You respect the freedom. You take nothing.",
                "Blue house, red barn, green field, a yellow sun. Everyone accounted for.",
                "You watch it for a while. It gradually stills. You feel you have arrived at the wrong moment.",
            ],
        },
        new()
        {
            Id           = "ForgottenGrave",
            DisplayNames = ["Forgotten Grave", "Unmarked Burial", "Moss-Grown Cairn", "Lonely Burial Mound", "Nameless Burial", "Lone Grave Mound", "Windswept Cairn", "Unmarked Resting Place"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Grassland, BiomeType.Plains, BiomeType.Desert],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "BoneFragment",  Probability = 0.20f },
                new() { ResourceId = "TarnishedRing", Probability = 0.08f },
            ],
            PartA =
            [
                "A cairn of carefully chosen stones, stacked by someone sad and patient and thorough.",
                "The grave marker has fallen face-down in the grass.",
                "Wildflowers have grown so thick over the mound it looks like the ground itself is blooming.",
                "The grave is very small.",
                "A small wooden cross has been placed here — not old, recently made, recently placed.",
                "The grave faces east. Someone thought about this.",
                "A pair of worn boots has been left at the head of the mound, soles-up.",
            ],
            PartB =
            [
                "You add a stone. It feels correct in a way you cannot explain.",
                "You consider turning it over. You decide against it. Some privacy belongs to strangers too.",
                "You stand here for a moment. It seems important that someone stood here.",
                "You stay longer than you intended. You stay until it seems right to leave.",
                "Someone still knows this name. You feel briefly relieved on behalf of someone you never met.",
                "It is a small act of consideration across a long distance. You appreciate it on their behalf.",
                "You don't know the significance. You step quietly around them.",
            ],
        },
        new()
        {
            Id           = "CrumbledFortress",
            DisplayNames = ["Crumbled Fortress", "Ruined Battlement", "Fallen Citadel", "Ancient Rampart", "Ruined Stronghold", "Collapsed Battlement", "Ancient Fortification", "Broken Walls"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Plains],
            Probability  = 0.010f,
            ResourceYields =
            [
                new() { ResourceId = "Stone", Probability = 0.30f },
                new() { ResourceId = "Ore",   Probability = 0.15f },
                new() { ResourceId = "Coal",  Probability = 0.12f },
            ],
            PartA =
            [
                "The great hall is open to the sky now, birch trees growing up through the old flagstones.",
                "The ramparts — what remains of them — look out over everything this place once defended.",
                "The portcullis has rusted in its raised position, permanently open.",
                "In the armory, one sword has rusted to its stand, patient and immovable.",
                "The great bell still hangs in the remains of the belfry. The rope hangs down, waiting.",
                "In the corner of the great hall, protected by a fallen column, a tapestry survives, still vivid.",
                "The well in the courtyard is still usable. Someone has left a fresh cup beside it.",
            ],
            PartB =
            [
                "In thirty years of leaves, somewhere underfoot, a goblet waits.",
                "The land is fine. The land was always going to be fine.",
                "Halfway down the gatehouse someone has posted a sign: 'CLOSED.' It is old. It is serious.",
                "You try to pull the sword free. It says no. Not with a sound — with a quality of no.",
                "You pull the rope. The bell rings once, deeply, across the whole valley. The sound takes a long time to stop.",
                "It shows a battle. You search for the ending. The tapestry ends before the battle does.",
                "You drink from the cup. It is cold and tastes like very old stone. You refill it and leave it.",
            ],
        },
        new()
        {
            Id           = "StoneCircle",
            DisplayNames = ["Stone Circle", "Standing Stones", "Ancient Ring", "Megalith Ring", "Henge of Old", "Ritual Stones", "The Upright Ones", "Shaped Silence"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Plains, BiomeType.Grassland, BiomeType.Mountain],
            Probability  = 0.010f,
            ResourceYields =
            [
                new() { ResourceId = "HollowStone", Probability = 0.15f },
                new() { ResourceId = "Quartz",      Probability = 0.10f },
                new() { ResourceId = "CrackedOrb",  Probability = 0.08f },
            ],
            PartA =
            [
                "The stones are older than any name you know, and they are aware of this.",
                "One stone leans slightly. In a crevice against it: a coin, a button, a folded blank piece of paper.",
                "The center of the circle feels like the inside of a held breath.",
                "At a specific moment of the year, the shadows here align to point somewhere. You feel this.",
                "The moss on the shaded side is a color you don't have a name for.",
                "A bird circles the stone circle exactly once, then leaves. You have the sense this was a report.",
                "Someone has been here recently — fresh flowers placed at the base of the tallest stone, still bright.",
            ],
            PartB =
            [
                "You walk the circle. On the third pass you feel briefly seen. On the fourth pass, it has moved on.",
                "You think about the blank piece of paper for an unreasonable amount of time.",
                "You stand in the center. You feel every stone equally behind you. They are doing something you cannot perceive.",
                "You don't know where they point. You feel very strongly that you are meant to.",
                "You examine it closely. You stare at it for a while. You cannot name it. This bothers you.",
                "You watch it fly away and wonder what it said and to whom.",
                "You don't know who left them. You feel certain they'll be back.",
            ],
        },
        new()
        {
            Id           = "LonelyWell",
            DisplayNames = ["Lonely Well", "Crumbling Well", "Stone-Rimmed Well", "Forgotten Well", "Dry Stone Well", "Roadside Well", "Ancient Cistern", "Abandoned Water Source"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Plains, BiomeType.Desert, BiomeType.Grassland],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "Clay",      Probability = 0.25f },
                new() { ResourceId = "RiverGlass",Probability = 0.12f },
                new() { ResourceId = "TidalCoin", Probability = 0.06f },
            ],
            PartA =
            [
                "A stone-rimmed well, older than the nearest building, standing without context.",
                "The bucket hangs on its rope, perfectly preserved and waiting.",
                "The well is dry. You lower yourself in to be sure.",
                "Someone has painted a small, kind face on the stone rim.",
                "The rope on the bucket is new. The well is centuries old. Someone still comes here.",
                "Names have been carved into the stone rim by a hundred different hands, in a dozen styles.",
                "A crow sits on the rim, looking down into the dark with professional interest.",
            ],
            PartB =
            [
                "You drop a coin. A long time passes. Then: a sound from below — a word, maybe. You drop another. Nothing.",
                "You pull it up. It is full of cold, clear water. The well has been dry for thirty years.",
                "At the bottom: an iron box. Empty. Smelling faintly of something sweet.",
                "It is looking at you. You feel better about the well, specifically.",
                "You try to understand who. The well doesn't say. The new rope doesn't say.",
                "You find your finger tracing one. You don't know whose name it is. You trace it again.",
                "It looks up at you once, then returns to the dark. It has found something. It is not sharing.",
            ],
        },
        new()
        {
            Id           = "WitchsCottage",
            DisplayNames = ["Witch's Cottage", "Hermit's Hovel", "Crumbled Hut", "Smoke-Stained Den", "Wise Woman's Den", "Herb-Hung Hut", "Edge-of-Woods Cottage", "Old One's House"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Swamp, BiomeType.Forest],
            Probability  = 0.010f,
            ResourceYields =
            [
                new() { ResourceId = "Herbs",       Probability = 0.35f },
                new() { ResourceId = "PaleMushroom",Probability = 0.20f },
                new() { ResourceId = "RottenSilks", Probability = 0.12f },
                new() { ResourceId = "CrackedOrb",  Probability = 0.06f },
            ],
            PartA =
            [
                "The smoke from the chimney smells of lavender and burnt hair and something that has no name.",
                "The garden grows things you don't have names for. Some of them appear to have yours.",
                "A cat sits on the windowsill, watching you with the calm authority of ownership.",
                "You knock on the door.",
                "Bundles of dried herbs hang from every rafter, dense enough to darken the room.",
                "A cauldron sits cold over the dead fire. Something inside it is still warm.",
                "Footprints in the mud: one set in, none out — or one set out, none in.",
            ],
            PartB =
            [
                "The smell follows you for an hour afterward. You decide not to investigate why.",
                "One of them reaches toward you. 'Manners,' you say. It withdraws. You feel you've done well.",
                "It has made a decision about you. You are not certain of the verdict.",
                "Inside: silence, then movement, then silence. The door stays closed. A hand draws the curtain.",
                "The smell is complex and specific and fills a room you are standing outside of.",
                "You touch the side of it. The warmth is not from the fire. It is from something that left recently.",
                "You stand at the door looking at this for a while. You decide the direction doesn't matter.",
            ],
        },

        // ── Water & wet ────────────────────────────────────────────────────
        new()
        {
            Id           = "HotSpring",
            DisplayNames = ["Hot Spring", "Bubbling Vent", "Steaming Pool", "Geothermal Basin", "Mineral Pool", "Steaming Bath", "Thermal Spring", "Earth-Warmed Water"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Snow, BiomeType.Tundra],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Salt",   Probability = 0.30f },
                new() { ResourceId = "Sulfur", Probability = 0.25f },
                new() { ResourceId = "Quartz", Probability = 0.15f },
            ],
            PartA =
            [
                "The water is warm and perfectly clear and smells faintly of minerals and something peaceful.",
                "Steam rises in slow columns that the air seems reluctant to disturb.",
                "The rocks around the spring are coated in orange and yellow minerals, bright as stained glass.",
                "The sound the water makes entering the pool is exactly the sound you needed to hear.",
                "Small colorful fish live in the water's warmth, unbothered by temperatures that would ruin other fish.",
                "The rocks around the pool are worn smooth in one place — a seat, used for a hundred years.",
                "The steam rises and forms shapes briefly before dissolving. You watch for a while.",
            ],
            PartB =
            [
                "You put your hand in. Every muscle you own relaxes simultaneously.",
                "In the steam, briefly: your face. It looks considerably more rested. It winks.",
                "It is beautiful and slightly alarming. You appreciate both things simultaneously.",
                "You sit at the edge with your feet in the water. Something has been lifted. You don't ask what.",
                "They investigate you with polite thoroughness and return to their business. You've been processed.",
                "You find it and sit in it. It fits perfectly. You feel this is not a coincidence and are glad.",
                "You stop trying to read the shapes. You sit with the ambiguity. The ambiguity is warm.",
            ],
        },
        new()
        {
            Id           = "TidePools",
            DisplayNames = ["Tide Pools", "Rocky Shallows", "Shell-Strewn Shore", "Barnacled Flats", "Rocky Shore Pools", "Wet Stone Gallery", "Littoral Flats", "Stranded Sea"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Beach, BiomeType.Shallows],
            Probability  = 0.040f,
            ResourceYields =
            [
                new() { ResourceId = "Fish",      Probability = 0.40f },
                new() { ResourceId = "Salt",      Probability = 0.25f },
                new() { ResourceId = "RiverGlass",Probability = 0.12f },
            ],
            PartA =
            [
                "The pools are full of small, improbable creatures committed entirely to their improbable lives.",
                "A hermit crab has established a home in a porcelain teacup and decorated it thoroughly.",
                "Something in the farthest pool glows a soft, cold green.",
                "You crouch at the largest pool and something immediately crouches back.",
                "A starfish has arranged itself in the clearest pool with the poise of something that knows it will be looked at.",
                "Something small and bright has claimed the largest pool entirely. It regards you from the center.",
                "At low tide the pools connect briefly. You watch a small fish take the opportunity.",
            ],
            PartB =
            [
                "You examine them for a while. One examines you back with equivalent seriousness.",
                "You approve. Moving on, you feel oddly invested in the crab's future.",
                "When you wade toward it, it moves to the next pool. It reaches the sea and is gone.",
                "It is a crab. It raises one claw. You raise one finger. You are here for eleven minutes.",
                "You crouch and look at it for a long time. It looks back. Neither of you blinks. You blink first.",
                "You stay at the edge of its pool. Respect is offered on both sides.",
                "You follow the fish with your eyes until the water covers the path again. You feel good about its decision.",
            ],
        },
        new()
        {
            Id           = "ReedBeds",
            DisplayNames = ["Reed Beds", "Cattail Marsh", "Papyrus Stand", "Waving Reeds", "Whispering Reeds", "Green Corridor", "Rustling Stands", "Cattail Dense"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Marsh, BiomeType.River],
            Probability  = 0.040f,
            ResourceYields =
            [
                new() { ResourceId = "Reed", Probability = 0.45f },
                new() { ResourceId = "Fish", Probability = 0.20f },
                new() { ResourceId = "Clay", Probability = 0.15f },
            ],
            PartA =
            [
                "The reeds grow taller than your head, turning the world into corridors of green.",
                "Your reflection in the still water watches you with more attention than reflections usually manage.",
                "Something large moves through the reeds ahead of you, parting them without a sound.",
                "A red-winged blackbird addresses you from a swaying reed with considerable urgency.",
                "Wind moves through the reeds in a sound like a crowd murmuring something just below hearing.",
                "The reeds have been cut here recently — clean cuts, in a pattern. Someone was working.",
                "A heron lifts off ahead of you with enormous unhurried wings.",
            ],
            PartB =
            [
                "Something small watches from within the fronds. You feel its specific interest.",
                "When you stop, it stops a half-second late. When you walk, it follows.",
                "The reeds close back where it passed. You stand still for a long time before continuing.",
                "You cannot help with whatever it is. You nod sympathetically. It seems to accept this.",
                "You try to hear the words. The sound shifts just before they become words. You keep trying.",
                "You wonder what they were making. The pattern is deliberate. The answer isn't here.",
                "It clears the reeds and banks once before continuing. You watch it out of sight.",
            ],
        },

        // ── Desert & dry ───────────────────────────────────────────────────
        new()
        {
            Id           = "SaltFlats",
            DisplayNames = ["Salt Flats", "Bleached Expanse", "Cracked Saltpan", "White Wastes", "Pale Wastes", "Dry Mirror", "Crusted Expanse", "Baked Ground"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Beach],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Salt",     Probability = 0.40f },
                new() { ResourceId = "Flint",    Probability = 0.15f },
                new() { ResourceId = "DeadGrass",Probability = 0.12f },
            ],
            PartA =
            [
                "The silence here is not an absence of sound but a presence of something old and dry and patient.",
                "A single wooden post stands at the center of the flats, attached to nothing, explaining nothing.",
                "The heat haze reshapes the horizon continuously: a city, a forest, a face.",
                "The cracks in the salt form a pattern too deliberate to be geological.",
                "Your shadow is the only vertical thing here and it seems uncomfortable about the distinction.",
                "A pair of boots stands in the middle of the flats, upright, ownerless.",
                "The wind crosses the flats and makes no sound. The silence it leaves behind is different from regular silence.",
            ],
            PartB =
            [
                "It is completely untroubled by your visit.",
                "It belongs to reasons that are no longer available for consultation.",
                "You walk toward the face. It stays the same distance away. You walk for a while anyway.",
                "You try to memorize it. You carry only part of it away. Not the part you wanted.",
                "You watch it for a while. You are not sure whether you feel sympathy or kinship.",
                "You approach them. They are not new. You walk around them and do not look back.",
                "You stand in it. It presses in from all sides. You find it clarifying.",
            ],
        },
        new()
        {
            Id           = "OasisGrove",
            DisplayNames = ["Oasis Grove", "Desert Spring", "Palmed Hollow", "Hidden Waterhole", "Desert Refuge", "Shaded Pool", "Wayfarers' Rest", "Hidden Springs"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Herbs",    Probability = 0.35f },
                new() { ResourceId = "Fish",     Probability = 0.25f },
                new() { ResourceId = "TidalCoin",Probability = 0.08f },
            ],
            PartA =
            [
                "The water appears suddenly between the palms, cold and clear and entirely real.",
                "The shade here has a quality of shade you don't encounter most places.",
                "An ibis stands at the water's edge with the stillness of something that has been here a long time.",
                "Coins cover the bottom of the pool — every traveler who found this place left one.",
                "Butterflies of colors you haven't seen elsewhere move between the flowers.",
                "At the base of the oldest palm, carved in careful letters: a list of names. A guest book.",
                "The air here is thirty degrees cooler than ten steps back. The boundary is abrupt.",
            ],
            PartB =
            [
                "You drink from it with your hands like an animal and feel absolutely no shame about this.",
                "You sit in it and something passes overhead that isn't a bird. You don't look up.",
                "It was standing here before you arrived. It will be standing here when you leave.",
                "You add yours. It feels correct in a way that needs no explaining.",
                "You follow one until you lose it in the light. You feel this was worth doing.",
                "You add yours below the last. Someone will be glad you came.",
                "You stand in the cool air and feel the heat on your back and refuse to move for a while.",
            ],
        },
        new()
        {
            Id           = "DeadForest",
            DisplayNames = ["Dead Forest", "Bleached Snags", "Skeletal Timbers", "Withered Copse", "Bone Wood", "Pale Stand", "Scoured Timber", "Dry-Rotten Grove"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Plains],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Driftwood", Probability = 0.30f },
                new() { ResourceId = "Ash",       Probability = 0.20f },
                new() { ResourceId = "DeadGrass", Probability = 0.20f },
                new() { ResourceId = "CrowFeather",Probability = 0.08f },
            ],
            PartA =
            [
                "The bleached trees stand like columns of testimony to something thorough and final.",
                "A crow sits on every tenth snag, watching you in shifts as you move through.",
                "One fallen tree caught another in falling and they lean together, holding each other up.",
                "The light here is harsh and unfiltered. Nothing soft remains to catch it.",
                "The wind in the dead branches makes a sound like something being listed carefully.",
                "In one hollow tree: a nest, long empty, built with great care by something that intended to return.",
                "The sun passes straight through the bare canopy. There is no shade here. There hasn't been for a long time.",
            ],
            PartB =
            [
                "Even the wind moves around them. The silence is of the complete variety.",
                "One follows at a consistent distance. You stop. It stops. This continues for a while.",
                "Lichens have grown over them both. The lichens are flourishing and seem delighted.",
                "You see yourself clearly in it. You squint. You feel it might be good for you.",
                "You listen. The list is long. You cannot understand the language but you recognize the form.",
                "You look at it for a while. The nest is intact. The builder was thorough and optimistic.",
                "You stand in the full light of the dead forest and find it clarifying.",
            ],
        },
        new()
        {
            Id           = "QuickSand",
            DisplayNames = ["Quicksand", "Treacherous Silt", "Sucking Mud", "Shifting Sand Trap", "Deceiving Ground", "Soft Trap", "Sinking Silt", "Unreliable Earth"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Swamp, BiomeType.Marsh],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Clay", Probability = 0.25f },
                new() { ResourceId = "Sand", Probability = 0.30f },
                new() { ResourceId = "Peat", Probability = 0.15f },
            ],
            PartA =
            [
                "The ground here looks perfectly solid.",
                "A sign reads CAUTION.",
                "You test the ground ahead with a stick.",
                "The mud here has the color and texture of confidence.",
                "The surface catches the light beautifully, a kind of luminous tan, very inviting.",
                "A shoe sits on the surface, half-submerged, looking resigned.",
                "The ground here has a sound when you tap it — hollow, uncertain, provisional.",
            ],
            PartB =
            [
                "It is not solid. You sort this out with effort and approximately no dignity.",
                "The sign is halfway in. It has read CAUTIO for some time.",
                "The stick disagrees strongly with the ground. You take a different path. The stick does not return.",
                "You commit to a step. The mud commits to disagreeing. You negotiate for a while.",
                "It is a trap and it does not hide this well. You find another way.",
                "You stare at it for longer than is useful. You leave it to its business.",
                "You tap it again. It taps back once, differently. You take a very different path.",
            ],
        },

        // ── Mountain & cold ────────────────────────────────────────────────
        new()
        {
            Id           = "CaveEntrance",
            DisplayNames = ["Cave Entrance", "Dark Hollow", "Rocky Cavern", "Yawning Shaft", "Gaping Hollow", "Mountain Mouth", "Stone Throat", "Shadowed Opening"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Mountain, BiomeType.Glacier],
            Probability  = 0.035f,
            ResourceYields =
            [
                new() { ResourceId = "Stone", Probability = 0.35f },
                new() { ResourceId = "Flint", Probability = 0.20f },
                new() { ResourceId = "Ore",   Probability = 0.20f },
                new() { ResourceId = "Coal",  Probability = 0.15f },
            ],
            PartA =
            [
                "The cave breathes. One long, slow breath in, one out — cool, mineral-smelling.",
                "The walls of the entrance are covered in pressed handprints, hundreds of them, every size.",
                "You call into the cave.",
                "Something lives in the dark beyond the entrance. You can hear it from here.",
                "A fire was made just inside the entrance, recently, and allowed to burn out. Warmth remains.",
                "Water drips in an irregular rhythm that sounds like it is counting something.",
                "Something has made a mark on the cave wall — a circle, a line, a shape that takes a moment to identify as a hand.",
            ],
            PartB =
            [
                "You breathe with it for a while. It doesn't notice. You notice.",
                "You press your hand against one that is roughly your size. It fits.",
                "Your voice comes back different — not an echo, but a version that has been somewhere.",
                "It breathes slow and rhythmic. It doesn't move toward you. It doesn't need to.",
                "You warm your hands over the ashes. The person who left them was here not long ago.",
                "You count with it for a while. You lose the count. You think the cave may have been counting from before you.",
                "You press yours over it. The sizes are different. The gesture is the same.",
            ],
        },
        new()
        {
            Id           = "FrozenShrine",
            DisplayNames = ["Frozen Shrine", "Ice-Locked Altar", "Glacial Monument", "Frost-Covered Stele", "Cold Holy Place", "Glacial Offering Stone", "Frost Sanctuary", "Winter Altar"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Snow, BiomeType.Glacier, BiomeType.Tundra],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "FrozenFlower",Probability = 0.15f },
                new() { ResourceId = "Ice",         Probability = 0.25f },
                new() { ResourceId = "Quartz",      Probability = 0.12f },
                new() { ResourceId = "CrackedOrb",  Probability = 0.06f },
            ],
            PartA =
            [
                "The altar is sealed in ice so clear the inscriptions are perfectly readable.",
                "The offerings inside the ice are perfectly preserved — flowers, tokens, a small meal.",
                "The ice is lit from within, blue-white, with no visible source.",
                "The cold radiates from the shrine itself, not from the surrounding air.",
                "The cold around the shrine is not wind-cold — it is an older cold, the kind that has been here since before the ice.",
                "A bell hangs beside the altar, frozen mid-swing. It has been ringing in one continuous moment for a long time.",
                "Everything placed here has been kept perfectly by the cold. You feel the ice is doing its job.",
            ],
            PartB =
            [
                "You read them. You wish you hadn't. You read them again.",
                "Someone placed these with no expectation that they would last. They lasted.",
                "A shape moves through its depths — large, deliberate, indifferent. Gone before you can describe it.",
                "You touch it briefly. Something very old and cold is not asleep. You pull your hand back. It lets go.",
                "You stand in it and feel briefly ancient. Then just cold.",
                "You touch the bell with one finger. The moment doesn't break. The bell doesn't ring. Everything stays.",
                "You add something small — a stone, a button, whatever you have. The cold keeps it with everything else.",
            ],
        },
        new()
        {
            Id           = "IcyCavern",
            DisplayNames = ["Icy Cavern", "Frost Cave", "Glacial Grotto", "Blue-Ice Hollow", "Blue-Ice Chamber", "Glacial Hall", "Frozen Grotto", "Crystal Ice Cave"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Glacier, BiomeType.Snow],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Ice",         Probability = 0.40f },
                new() { ResourceId = "Quartz",      Probability = 0.20f },
                new() { ResourceId = "HollowStone", Probability = 0.12f },
                new() { ResourceId = "FrozenFlower",Probability = 0.10f },
            ],
            PartA =
            [
                "The ice is every shade of blue, from pale surface to something close to black at depth.",
                "Trapped in a column of ice: a green leaf, perfect, from a tree that no longer grows here.",
                "Your footsteps arrive after you, the acoustics delivering them slightly late.",
                "The cavern opens into a vast blue chamber and you stop moving.",
                "A frozen waterfall has been caught mid-fall at the far end, every droplet preserved.",
                "The acoustics here are perfect. You hum one note. The cavern completes it.",
                "In one wall of ice, very deep, a shape that might be an animal — large, calm, old.",
            ],
            PartB =
            [
                "You stand in it and feel the word 'cathedral' without knowing why. The ice breathes back when you do.",
                "You stand and look at it for a long time. Nearby, a small fish, frozen mid-turn.",
                "When you stop, they continue two more steps, then stop. You wait. They do not resume.",
                "It is beautiful in a way that makes other beautiful things seem slightly embarrassed.",
                "You stand beneath it and feel the weight of all the water that stopped being water.",
                "You hold the note until you run out of breath. The cavern finishes your note without you.",
                "You stare at it until your eyes hurt. When you look away it is in a different position. You don't look again.",
            ],
        },
    ];
}
