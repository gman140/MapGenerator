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

        // ── Ocean & deep water ─────────────────────────────────────────────
        new()
        {
            Id           = "Shipwreck",
            DisplayNames = ["Shipwreck", "Sunken Hulk", "Lost Vessel", "Drowned Ship", "Salt-Eaten Hull", "Keel in the Deep", "Foundered Wreck", "Waterlogged Hulk"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Ocean, BiomeType.Shallows],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Driftwood", Probability = 0.35f },
                new() { ResourceId = "RiverGlass",Probability = 0.12f },
                new() { ResourceId = "TidalCoin", Probability = 0.10f },
            ],
            PartA =
            [
                "The mast breaks the surface at low tide, wrapped in kelp and barnacles and old intention.",
                "You can make out the hull through the water — a dark shape that is where it was going and not where it meant to be.",
                "The crow's nest is still attached. Something has made a home in it.",
                "The figurehead faces forward, still optimistic, still pointing somewhere it will never arrive.",
                "Cargo spills from a broken hatch into the sand below — crates, rope, things that were important once.",
                "The ship's bell is visible through the water, tilted, silted over, not ringing.",
                "The name on the hull is almost legible. You squint. You get two letters. The rest has been taken.",
            ],
            PartB =
            [
                "The kelp has made it home. The barnacles have made it permanent. The sea has made it its.",
                "The water carries it somewhere more slowly than intended. It will get there.",
                "A gull lands on the remnant of the crow's nest and makes a noise of proprietary confidence.",
                "You look at where it points for a while. The horizon is indifferent to what it wanted.",
                "You search through the scattered things carefully. You find one thing that has kept. You keep it further.",
                "You stare at it for a long time. It would ring if you asked it. You don't ask.",
                "You decide the two letters are the beginning of something hopeful. This may not be accurate.",
            ],
        },
        new()
        {
            Id           = "KelpForest",
            DisplayNames = ["Kelp Forest", "Waving Canopy", "Underwater Grove", "Green Deep", "Frond Curtain", "Sea-Tree Stand", "Swaying Kelp", "Drift-Forest"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Ocean, BiomeType.Shallows],
            Probability  = 0.035f,
            ResourceYields =
            [
                new() { ResourceId = "Fish", Probability = 0.40f },
                new() { ResourceId = "Fiber",Probability = 0.20f },
            ],
            PartA =
            [
                "The kelp rises from the sea floor in long slow columns, reaching for a surface they never quite touch.",
                "Light filters down through the canopy in shafts that shift and sway with the current.",
                "A seal drifts between the fronds with complete boneless ease, watching you sideways.",
                "The forest sways in one direction, then the other, as if considering two equally good answers.",
                "Small fish move in coordinated schools between the kelp stalks with the efficiency of practiced commuters.",
                "Looking down into the kelp from the surface, you see it go deep — deeper than makes obvious sense.",
                "The smell here is specific and green and old and has been this smell for a long time.",
            ],
            PartB =
            [
                "They catch the current and make shapes you watch for a long time without wanting to look away.",
                "A fish catches a shaft of light and turns once. It is briefly extraordinary. Then it isn't.",
                "It tilts to look at you with one eye. It is deciding something. It decides to leave. You feel assessed.",
                "You watch it for a while. The kelp seems unsurprised by both answers simultaneously.",
                "One school passes through another. They rearrange without incident. They continue.",
                "The depth holds no answer. You stop looking for one and just look.",
                "You breathe it in. It settles in your lungs like something settled. You find this acceptable.",
            ],
        },
        new()
        {
            Id           = "BioluminescentBloom",
            DisplayNames = ["Bioluminescent Bloom", "Glowing Shallows", "Night Lantern", "Cold Glow", "Living Light", "Bloom of the Deep", "Light-Bloom", "Plankton Fire"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Ocean, BiomeType.Lake],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Salt",     Probability = 0.20f },
                new() { ResourceId = "TidalCoin",Probability = 0.05f },
            ],
            PartA =
            [
                "After dark the water lights up from within — cold blue-green, sourceless, alive.",
                "Each wave that breaks leaves a glowing edge along the sand, brief and perfectly itself.",
                "You disturb the surface with one finger and the light blooms out in a ring from the point of contact.",
                "The glow pulses slowly — not in rhythm with the waves, in rhythm with something else.",
                "Small creatures trail light as they move through the water, painting paths that fade behind them.",
                "The whole surface glows faintly enough that you can read by it if you hold still.",
                "You cannot see what is producing the light. You are not certain it is something small.",
            ],
            PartB =
            [
                "You stand at the edge of it and feel something close to awe and something close to unease and can't separate them.",
                "You watch one break and fade and the next arrives. You stand here through several arrivals.",
                "The ring expands and fades. You do it again. You do it several more times. You stop and feel briefly childish and not sorry.",
                "You try to find the source of the pulse. You can't. It continues with or without your understanding.",
                "You follow one with your eyes until the trail fades. The path it made existed and was gone. This feels significant.",
                "You read nothing. You sit in the glow anyway. It asks nothing of you. You find this restful.",
                "You look at the water for a long time. You decide you prefer not knowing.",
            ],
        },

        // ── Lake ───────────────────────────────────────────────────────────
        new()
        {
            Id           = "SunkenRuins",
            DisplayNames = ["Sunken Ruins", "Drowned Settlement", "Submerged Walls", "Flooded Town", "Lost Foundation", "Underwater Remnants", "Drowned Stone", "Waterlogged Ruin"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Lake, BiomeType.Shallows],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "Stone",        Probability = 0.25f },
                new() { ResourceId = "RiverGlass",   Probability = 0.12f },
                new() { ResourceId = "TarnishedRing",Probability = 0.10f },
                new() { ResourceId = "CrackedOrb",   Probability = 0.06f },
            ],
            PartA =
            [
                "A tower rises from the lake floor to just below the surface, its upper stones visible when the water is still.",
                "The walls are fully legible from above — a square, a courtyard, a room that once had a purpose.",
                "A road leads from the bank straight into the water, paved and deliberate and continuing.",
                "Through the clear water you can see a threshold, a door frame, a window with no glass.",
                "The water has preserved things below: pottery, timber, a cart, all softened by time and silt.",
                "Fish school in the empty rooms below, moving between window and doorway with the ease of residents.",
                "At the center of the sunken structure, still upright, a chimney. The hearth is thirty feet below the surface.",
            ],
            PartB =
            [
                "You look through the water at the stone for a long time. It looks back with the patience of something that has waited a while.",
                "You trace the outline of the courtyard from above. It was a reasonable size. It held a reasonable amount of living.",
                "You stand at the point where the road enters the water. Someone paved this very carefully.",
                "You stare through the water at the window. What looked through it last is not available for comment.",
                "You reach for something just below the surface. You pull back. It belongs here now.",
                "You watch them for a while. They navigate the rooms with confidence. No walls mean anything to them.",
                "You look at the chimney for a long time. The water above it is entirely still.",
            ],
        },
        new()
        {
            Id           = "MirrorLake",
            DisplayNames = ["Mirror Lake", "Glass Surface", "Still Water", "Reflective Mere", "Polished Mere", "Dead Calm Water", "The Still One", "Unbroken Surface"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Lake],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "RiverGlass", Probability = 0.15f },
            ],
            PartA =
            [
                "The surface is so still it seems less like water than a decision about what water could be.",
                "Your reflection arrives before you do and watches you approach with mild interest.",
                "A bird crosses the far shore. Its reflection crosses in the opposite direction at the same speed.",
                "Nothing breaks the surface. Not a ripple, not an insect. The stillness is not natural but it is not hostile.",
                "The sky in the water is more detailed than the sky above. You check. You check again.",
                "You toss a small stone. The ripples expand perfectly and take a long time to reach the edges.",
                "The reflection shows a time of day different from the one you are standing in.",
            ],
            PartB =
            [
                "You stare into it for a while. It is doing an excellent impression of depth.",
                "You look at the reflection for a moment before looking at yourself. The reflection seems more confident.",
                "You watch until both birds are gone. The reflections stay a fraction longer.",
                "You crouch and look straight down at your own face. It looks up at you with a perfectly acceptable expression.",
                "You look back and forth until you stop being sure which is the original. You pick one and keep walking.",
                "You watch the rings reach the bank and fade. The center of the lake remains perfectly centered.",
                "You note the angle of the light in the reflection. You note the angle above. You do not reconcile them.",
            ],
        },

        // ── Shallows ───────────────────────────────────────────────────────
        new()
        {
            Id           = "CoralReef",
            DisplayNames = ["Coral Reef", "Color Shelf", "Living Stone", "Reef Garden", "Brain Reef", "Staghorn Shallows", "Painted Shelf", "Warm Reef"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.Shallows],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "Fish",      Probability = 0.35f },
                new() { ResourceId = "Salt",      Probability = 0.25f },
                new() { ResourceId = "RiverGlass",Probability = 0.15f },
                new() { ResourceId = "TidalCoin", Probability = 0.08f },
            ],
            PartA =
            [
                "The reef is more color than you were prepared for — every color you know and some you don't.",
                "Something large and blue-gray moves through the reef slowly, sovereign and unimpressed.",
                "A fish the exact color of the coral sits absolutely still on the coral, not hiding, just at home.",
                "The coral makes small sounds — clicks and pops — a language for things that live underwater.",
                "Brain coral rises from the reef floor in a shape that takes a moment to accept as natural.",
                "A sea turtle moves through the reef with the unhurried certainty of something that has time.",
                "The reef at its deepest edge drops into water that is a color for which there is no good word.",
            ],
            PartB =
            [
                "You stand at the edge for a long time. It is doing its best work without any audience. It has been doing this.",
                "It passes through the reef and doesn't accelerate. The reef adjusts around it. You are acknowledged and filed.",
                "You watch it for a while. It doesn't move. You aren't sure it's breathing. You decide it is and leave it to it.",
                "You listen to it for a while. You don't understand it. You feel this is probably fine.",
                "You put your hand near it. The size of the brain it resembles is unsettling. You move your hand.",
                "It passes close enough that you could touch its shell. You don't. You watch it go. You feel good about not touching.",
                "You look into the blue for a while. There is a lot of it.",
            ],
        },

        // ── Savanna ────────────────────────────────────────────────────────
        new()
        {
            Id           = "TermiteMounds",
            DisplayNames = ["Termite Mounds", "Earthen Towers", "Red Columns", "Insect Citadels", "Clay Spires", "Builder's Colony", "Reddish Towers", "Active Mound City"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Savanna],
            Probability  = 0.040f,
            ResourceYields =
            [
                new() { ResourceId = "Clay", Probability = 0.20f },
                new() { ResourceId = "Peat", Probability = 0.30f },
                new() { ResourceId = "BoneFragment", Probability = 0.10f },
            ],
            PartA =
            [
                "The mounds rise to shoulder height, reddish clay, hard as brick, perfectly engineered.",
                "A mound has been broken open — not by you — and the interior is a city of the most practical variety.",
                "The largest mound is taller than you and warmer than the surrounding air by several degrees.",
                "A hundred thousand decisions made by ten thousand individuals have produced this exact shape.",
                "Soldier termites have massed at the base, facing outward, arranged with military precision.",
                "You knock on the tallest mound. Nothing answers. Everything inside continues.",
                "The mounds cast long shadows at this hour. In the shadows, perfectly still, a lizard.",
            ],
            PartB =
            [
                "The engineering involved is considerable and no one involved has a title or a salary.",
                "A civilization, clearly. A population, certainly. An agenda, yes. A name for any of it, no.",
                "You press a hand against it. The warmth is from effort, ongoing, distributed.",
                "You consider this for a while. You have decisions to make. You do not have this process.",
                "You don't cross the line they've drawn. You step back one step. The line holds.",
                "You put your ear against it. You hear the interior of something entirely occupied with itself.",
                "The lizard becomes a feature of the landscape. You become a feature of its landscape. Mutually acknowledged.",
            ],
        },
        new()
        {
            Id           = "WateringHole",
            DisplayNames = ["Watering Hole", "Muddy Pool", "Animal Gathering", "Track-Ringed Pool", "Communal Water", "Herd Pool", "Low-Water Gathering", "Print-Ringed Puddle"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Savanna, BiomeType.Plains],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "Clay", Probability = 0.30f },
                new() { ResourceId = "Fish", Probability = 0.20f },
            ],
            PartA =
            [
                "The water is more mud than water, churned by a hundred sets of hooves and paws that came before you.",
                "Tracks ring the pool — every shape and size. Something very large came here recently.",
                "A stillness falls as you approach. You are the wrong shape for this gathering.",
                "A crocodile occupies one side of the pool with the calm authority of having been here since before the pool.",
                "Dozens of birds work the muddy edges in patient rotation, each with its own system.",
                "The water is warm and the color of old tea and is clearly the best water available for fifty miles.",
                "At dawn this place was full. You've arrived at the quiet hour. The prints are still fresh.",
            ],
            PartB =
            [
                "You drink carefully from the cleanest edge. You feel your assessment of 'clean' has been adjusted by circumstances.",
                "You find the large tracks and follow them a short distance. They continue. You do not.",
                "You wait at the edge until the stillness forgets you. Slowly, things return. You are accepted as furniture.",
                "You give it the whole pool and stand back. Its patience is total. Its claim is total.",
                "You watch the rotation for a while. No conflicts. Everyone knows their system.",
                "You drink from it. It is, objectively, not good. It is, subjectively, exactly what was needed.",
                "You crouch and look at the prints. Something had breakfast here. It was not concerned.",
            ],
        },
        new()
        {
            Id           = "AcaciaStand",
            DisplayNames = ["Acacia Stand", "Flat-Crowned Trees", "Thorn Tree Grove", "Savanna Canopy", "Umbrella Trees", "Wide-Limbed Stand", "Thorn-Armed Trees", "Sentinel Trees"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Savanna],
            Probability  = 0.035f,
            ResourceYields =
            [
                new() { ResourceId = "Wood",     Probability = 0.30f },
                new() { ResourceId = "Fiber",    Probability = 0.20f },
                new() { ResourceId = "Feathers", Probability = 0.15f },
            ],
            PartA =
            [
                "The flat-topped canopy spreads above like a deliberate ceiling, its shade clean and even.",
                "Thorns line every branch — long, white, specific. The tree has opinions about who touches it.",
                "A weaver bird has filled every available branch with round woven nests, hundreds of them.",
                "The trunk divides low and sends three arms wide in three directions, confident in the geometry.",
                "A giraffe stands beyond the nearest tree, reaching the highest branches without difficulty or apology.",
                "The shade here has a sound — insect and wind and the dry crack of settling wood.",
                "Something has carved a single line into the bark, waist-high. No more and no less.",
            ],
            PartB =
            [
                "You stand under it and feel correctly placed. The shade has put some thought into its coverage.",
                "You try to touch one carefully. Carefully is not carefully enough. You note this for later.",
                "You examine one nest. The opening is exact. The engineering is exact. The bird who returns watches you with polite concern.",
                "You stand in the angle of two branches and feel held without being touched.",
                "It does not notice you. It has eaten today and is content. You feel the same.",
                "You stand in it for a while and let it inform you about where you are.",
                "You trace the line with one finger. It was made with intention. The intention is no longer available.",
            ],
        },

        // ── Tundra ─────────────────────────────────────────────────────────
        new()
        {
            Id           = "PermafrostPit",
            DisplayNames = ["Permafrost Pit", "Sunken Ground", "Ancient Collapse", "Thermokarst Hollow", "Frozen Depth", "Collapsed Tundra", "Ice-Pit", "Ground-Fallen Hollow"],
            Category     = FeatureCategory.Cold,
            AllowedBiomes = [BiomeType.Tundra],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Ice",          Probability = 0.30f },
                new() { ResourceId = "Peat",         Probability = 0.25f },
                new() { ResourceId = "BoneFragment",  Probability = 0.20f },
                new() { ResourceId = "FrozenFlower",  Probability = 0.08f },
            ],
            PartA =
            [
                "The ground has collapsed here into a shallow pit, exposing the frozen strata beneath — layers of time, compressed.",
                "A mammoth tusk curves up from the exposed permafrost, yellow-white, enormous, patient.",
                "Seeds in the frozen soil have not germinated. They are waiting for a condition that has not occurred for ten thousand years.",
                "Bones visible in the wall of the pit belong to something large and something small and something you can't name.",
                "The air rising from the pit is cold in a way that has nothing to do with the season.",
                "Ancient plant matter is compressed in dark bands through the ice, a record of summers older than writing.",
                "At the bottom of the pit, almost entirely encased, the shape of a small animal, perfectly preserved.",
            ],
            PartB =
            [
                "You read the layers from the top down. Each one is a year that is done. There are a great many.",
                "You stand beside it and feel the scale of it and don't know what to do with the feeling.",
                "You hold one in your palm until it thaws slightly. It does not germinate. It is waiting for something you cannot provide.",
                "You crouch at the edge and look carefully. You find more than you expected. Some of it you do not identify.",
                "You let it wash over your face. It is the cold of something kept, not the cold of something ambient.",
                "You count the bands for as long as you can. You lose count somewhere in a summer from before agriculture.",
                "You look at it for a long time. It has been preserved through effort that was not effort — just cold, just time.",
            ],
        },
        new()
        {
            Id           = "CaribouTrail",
            DisplayNames = ["Caribou Trail", "Migration Path", "Herd Road", "Worn Tundra Track", "Animal Highway", "Ancient Herd Path", "Beaten Ground", "Seasonal Route"],
            Category     = FeatureCategory.Nature,
            AllowedBiomes = [BiomeType.Tundra, BiomeType.Snow],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "Feathers",    Probability = 0.20f },
                new() { ResourceId = "BoneFragment", Probability = 0.15f },
            ],
            PartA =
            [
                "The trail is worn deep into the tundra — not by one passing but by ten thousand years of the same passing.",
                "Antler velvet has been rubbed from a shrub beside the trail, leaving bright exposed wood.",
                "The path continues north and south beyond sight, the most permanent feature of an otherwise featureless landscape.",
                "Tracks in the mud along the trail are layered over each other — fresh prints, old prints, the ghost of older prints.",
                "A few stragglers from the last migration pass in single file, their breath making small clouds.",
                "The trail is wider than you expected. The width of it puts the number of animals in scale.",
                "In a sheltered hollow beside the trail, a set of antlers, shed and weathered white.",
            ],
            PartB =
            [
                "You stand in it for a moment. You are a new print on a path of very old prints. The path accommodates you without comment.",
                "You run a finger along the exposed wood. It is still pale. They were here very recently.",
                "You follow it for a short distance north. It does not change. The land changes around it. The path stays.",
                "You crouch over the layers. The oldest ones are still legible. Everything that came through here left a record.",
                "You step aside and watch them pass. They don't look at you. You are furniture in a route they do not question.",
                "You step off the trail and look at it from outside. It makes more sense from here.",
                "You set them upright in the ground. They stand with more dignity than most things planted here.",
            ],
        },

        // ── Miscellaneous ──────────────────────────────────────────────────
        new()
        {
            Id           = "MeteorCrater",
            DisplayNames = ["Meteor Crater", "Impact Site", "Sky-Fallen Bowl", "Star Scar", "Fallen Stone Bowl", "Heavenly Scar", "Ancient Impact", "The Bowl"],
            Category     = FeatureCategory.Desert,
            AllowedBiomes = [BiomeType.Desert, BiomeType.Plains],
            Probability  = 0.010f,
            ResourceYields =
            [
                new() { ResourceId = "Stone",      Probability = 0.30f },
                new() { ResourceId = "Flint",      Probability = 0.35f },
                new() { ResourceId = "HollowStone",Probability = 0.20f },
                new() { ResourceId = "Quartz",     Probability = 0.15f },
            ],
            PartA =
            [
                "The bowl is perfectly circular and old enough that the edges have softened but not gone.",
                "At the center, half-buried, a mass of dark metal that has not corroded and has no intention of doing so.",
                "The ground at the impact site is fused in places — glass beneath the surface wherever you dig.",
                "The crater is larger than it looks from the rim. You understand this only once you are in it.",
                "Nothing grows at the very center. It grows up to a certain distance and then it doesn't.",
                "Standing at the rim you can see the ejecta field stretching away in an unmistakable pattern.",
                "Someone has built a cairn at the center of the crater. It is very small and very deliberate.",
            ],
            PartB =
            [
                "You walk the rim. It holds its circle. It has been holding its circle since before you were a possibility.",
                "You put your hand on it. It is cooler than the surrounding ground. It has been here since before people had words for 'from the sky.'",
                "You find a piece. You turn it over. It is smooth on one side from the entry. You keep it.",
                "You stand at the center for a while and think about the direction things can come from.",
                "You stand at the exact center. You look at the ground. You look at the sky. You consider the journey.",
                "You follow the scatter with your eyes. The geometry of it is legible and final.",
                "You add a stone to the cairn. Someone thought it was important to mark the center. You think so too.",
            ],
        },
        new()
        {
            Id           = "BanditCamp",
            DisplayNames = ["Bandit Camp", "Abandoned Outlaw Camp", "Scattered Camp", "Robber's Rest", "Brigand's Rest", "Empty Camp", "Outlaw's Clearing", "Gone-to-Ground Camp"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Plains, BiomeType.Grassland, BiomeType.Forest],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "Fiber", Probability = 0.30f },
                new() { ResourceId = "Flint", Probability = 0.20f },
                new() { ResourceId = "Coal",  Probability = 0.15f },
            ],
            PartA =
            [
                "The camp has been abandoned in a hurry — fire doused, gear left, the kind of departure that wasn't a choice.",
                "A rope hangs from a tree at the edge of camp, tied with a specific and instructional knot.",
                "Wanted notices have been pinned to a board. Some names are crossed off. The crossing-off was not done by the law.",
                "The fire ring is cold but the ashes are recent. They were here within the week.",
                "A playing card is nailed to a tree at eye level — the ace of spades — with a knife through it.",
                "The food stores are still here. Whatever made them leave did not give them time for the food.",
                "A lookout post is built into the crook of the tallest tree, with a clear view in three directions.",
            ],
            PartB =
            [
                "You search carefully. You find: a coin, a key, a map with one location circled and a date that has passed.",
                "You leave it alone. Some things are a message and you are not the intended audience.",
                "You read the names. You recognize one. You put the notice down and don't mention it to anyone.",
                "You kick the ashes. They are, in fact, recent. You look around with renewed attention.",
                "You pull the knife out. The card falls. You put the knife away. You leave the card on the ground.",
                "You take a small amount of the food. You leave a coin. The arrangement feels reasonable given the circumstances.",
                "You climb to it. From up here you see why they chose this spot. You also see that it didn't help in the end.",
            ],
        },
        new()
        {
            Id           = "AncientRoad",
            DisplayNames = ["Ancient Road", "Old Paving", "Forgotten Path", "Crumbled Highway", "Sunken Road", "Road of Somewhere", "Old Stone Way", "Half-Buried Path"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.Plains, BiomeType.Grassland, BiomeType.Desert],
            Probability  = 0.020f,
            ResourceYields =
            [
                new() { ResourceId = "Stone",        Probability = 0.25f },
                new() { ResourceId = "TarnishedRing", Probability = 0.08f },
            ],
            PartA =
            [
                "The paving stones still hold their line — cracked, grass-pushed, but resolutely going somewhere.",
                "Wheel ruts worn into the stone mark the exact path of a thousand thousand journeys.",
                "The road emerges from one hillside and disappears into another, both destinations invisible.",
                "Milestones still stand at intervals — the numbers worn smooth, the intention clear.",
                "The road is perfectly straight in both directions for as far as you can see.",
                "The trees along the road's edge are all the same species and the same age, planted by the builders.",
                "At a junction in the road, a waymarker post still stands, its directions worn to shapes.",
            ],
            PartB =
            [
                "You walk it for a while. It is going somewhere with more conviction than you are.",
                "You crouch and run a hand along one rut. The stone has been compressed by the weight of repetition.",
                "You stand between its appearances and try to understand what was on either side. The hills do not say.",
                "You try to read the numbers. You get some of them. You find your location on someone's old road.",
                "You follow it with your eyes in both directions. Someone built this very seriously.",
                "They have been growing for a long time. They have forgotten the road. The road has not forgotten its purpose.",
                "You turn the post. The shapes suggest something. You face one direction and decide it might as well be that.",
            ],
        },

        // ── River ──────────────────────────────────────────────────────────
        new()
        {
            Id           = "Waterfall",
            DisplayNames = ["Waterfall", "Cascade", "Falling Water", "White Rush", "Plunge Fall", "Thundering Drop", "Curtain Fall", "Long Fall"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.River, BiomeType.Mountain],
            Probability  = 0.025f,
            ResourceYields =
            [
                new() { ResourceId = "Stone",     Probability = 0.20f },
                new() { ResourceId = "RiverGlass",Probability = 0.15f },
                new() { ResourceId = "Quartz",    Probability = 0.10f },
            ],
            PartA =
            [
                "The water arrives at the edge and commits to the decision without hesitation.",
                "The sound reaches you before the water does — white noise made of ten thousand small impacts.",
                "A permanent mist hangs at the base, watering ferns that have lived here their whole fern lives.",
                "Standing behind the curtain of water looking out, the world is a sheet of moving glass.",
                "A rainbow holds its fixed position in the mist below, unwilling to move.",
                "The pool at the base is deep and clear and cold and catches every drop with professional resignation.",
                "Someone has scratched their initials into the rock beside the falls. The date is over a century ago.",
            ],
            PartB =
            [
                "You stand beside it for a while. The river had no choice in this. It did it well anyway.",
                "You stand in it and let it be louder than everything you were thinking.",
                "You crouch among the ferns. They are doing well. They are not thinking about the waterfall. They live here.",
                "You stand behind it and look through the water at the world. Everything outside is moving and blurred and still there.",
                "You stand in the mist and watch it be fixed. Nothing else about this place is fixed. It doesn't care.",
                "You kneel and put your hand in. It is cold in the specific way that moving mountain water is cold.",
                "You trace the initials. There is no way to know who. There is a way to know they stood exactly here.",
            ],
        },
        new()
        {
            Id           = "RiverFord",
            DisplayNames = ["River Ford", "Old Crossing", "Stepping Stones", "Shallow Crossing", "Worn Crossing", "Known Ford", "Traveler's Crossing", "Stepping Path"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.River],
            Probability  = 0.030f,
            ResourceYields =
            [
                new() { ResourceId = "Flint",     Probability = 0.20f },
                new() { ResourceId = "RiverGlass",Probability = 0.15f },
                new() { ResourceId = "Clay",      Probability = 0.12f },
            ],
            PartA =
            [
                "Stepping stones cross the river at this point — placed deliberately, worn smooth by generations of feet.",
                "The water is shallow here and clear and faster than it looks from the bank.",
                "A rope has been strung across the river at chest height, knotted to two trees, frayed by use.",
                "The ford is marked with cairns on both banks — a small pair on the left, a larger pair on the right.",
                "The stones at the bottom are smooth and loose and make the crossing a negotiation.",
                "This is where the road from the other side begins. There was a road on this side once.",
                "Worn into the bank on both sides, the compressed ground of a thousand arrivals and departures.",
            ],
            PartB =
            [
                "You cross carefully. The water is cold and fast against your legs. The far bank feels earned.",
                "You step off the bank and the current immediately tells you what it thinks. You cross anyway.",
                "You test it before trusting it. It holds. It has held a lot of weight over a long time.",
                "You find the line between the two cairns and cross it. You feel entered and confirmed.",
                "You make the crossing. Each stone moves slightly. Each one holds. You feel each negotiation.",
                "You stand on this side and look at where the road begins over there. You file this away.",
                "You add your own arrival to the bank. Tomorrow the water will erase it. Today it is there.",
            ],
        },
        new()
        {
            Id           = "FishingHole",
            DisplayNames = ["Fishing Hole", "Calm Eddy", "Trout Pool", "Deep Bend", "Still Pocket", "Angler's Spot", "The Bend", "Quiet Pocket"],
            Category     = FeatureCategory.Water,
            AllowedBiomes = [BiomeType.River, BiomeType.Lake],
            Probability  = 0.035f,
            ResourceYields =
            [
                new() { ResourceId = "Fish", Probability = 0.60f },
                new() { ResourceId = "Reed", Probability = 0.20f },
            ],
            PartA =
            [
                "The river bends here and slows into a pool behind a sunken log, clear and cold and full of movement.",
                "You can see them from the bank — a dozen fish hovering in the current, doing the fish version of waiting.",
                "A heron stands absolutely still at the pool's edge, a creature that has elevated patience to an art form.",
                "Someone has worn a flat spot into the bank here — a place for sitting, used for a long time.",
                "The eddy behind the bend collects things: leaves, seeds, a feather, one small perfect piece of driftwood.",
                "A kingfisher lands on a branch over the pool, surveys it with professional focus, and departs.",
                "The surface of the pool breaks in rings where fish rise to take insects you cannot see.",
            ],
            PartB =
            [
                "You sit at the edge for a while. The fish do not leave. They have things to do here.",
                "You watch them hold in the current. The effort involved is invisible. The result is effortless.",
                "You stand at the opposite bank and it does not move. You have met its patience limit.",
                "You sit in it. It fits exactly. You understand all at once why someone came here regularly.",
                "You examine what it has collected. Everything has come from somewhere upstream. You think about upstream.",
                "You watch it for a while. It comes back. It finds nothing this time. It files that and continues.",
                "You sit at the pool's edge and watch the rings arrive and spread and overlap and fade.",
            ],
        },
        new()
        {
            Id           = "MillstoneRuins",
            DisplayNames = ["Millstone Ruins", "Ruined Mill", "Broken Waterwheel", "Old Mill Site", "Mill Remains", "Waterwheel Remnants", "Collapsed Mill", "Grinding Stone Ruin"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.River],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "Stone", Probability = 0.30f },
                new() { ResourceId = "Wood",  Probability = 0.20f },
                new() { ResourceId = "Flint", Probability = 0.15f },
            ],
            PartA =
            [
                "The millstone lies in the shallows, half-submerged, its grinding surface still legible.",
                "One wall of the mill still stands beside the water, the wheel-mount bracket rusted but intact.",
                "The channel that fed water to the wheel has silted over but its shape is still clear in the bank.",
                "The millstone is still perfectly round, still perfectly flat. The rest of the mill is not here.",
                "The water still runs through the gap where the wheel hung, doing the job the wheel was meant to do, unrewarded.",
                "Wooden gears, swollen and rotted, lie in the shallows where they fell. They were complicated, once.",
                "A grain of flour is lodged in a crack in the millstone. Very old. Still, technically, flour.",
            ],
            PartB =
            [
                "You press your hand against the flat surface. It has ground a great deal of grain. You feel the life of that.",
                "You find the axle hole and try to understand the mechanism. You get most of it.",
                "You trace its line with your foot. The geometry of it is still there under the silt.",
                "You try to move it. It is extremely not moveable. It has opinions about where it is.",
                "You stand where the wheel would have been. The water passes through you at mill-wheel height.",
                "You hold one piece and try to assemble the simple machine in your head. You get it working. Then it fell. Then it was this.",
                "You look at it for a while. A flour grain, still flour, in a ruin that is centuries old. This is something.",
            ],
        },
        new()
        {
            Id           = "RiverShrine",
            DisplayNames = ["River Shrine", "Water Offering Place", "Streamside Altar", "Riverside Sanctum", "Flowing Offering Stone", "Waterside Holy Place", "Current's Altar", "Riverman's Shrine"],
            Category     = FeatureCategory.Ruins,
            AllowedBiomes = [BiomeType.River, BiomeType.Marsh],
            Probability  = 0.015f,
            ResourceYields =
            [
                new() { ResourceId = "RiverGlass",   Probability = 0.15f },
                new() { ResourceId = "TidalCoin",    Probability = 0.08f },
                new() { ResourceId = "TarnishedRing",Probability = 0.06f },
            ],
            PartA =
            [
                "A flat stone at the river's edge holds small offerings — a ring, a coin, a folded cloth — placed with obvious intention.",
                "The shrine is small enough to miss if you aren't looking. Whoever built it was not looking to be seen.",
                "Offerings have been made here across many years: the oldest are stone, the newest are wood, and there is everything between.",
                "A small carved figure sits at the water's edge, facing the current, worn nearly smooth.",
                "Fresh flowers have been placed here recently, their stems in the water to keep them.",
                "The stone on which the offerings rest has been placed here deliberately — it is not a river stone.",
                "A single candle has been burned here, down to nothing. The wax dripped into the water and was carried away.",
            ],
            PartB =
            [
                "You don't touch the offerings. You sit beside them for a while. The river moves past all of it.",
                "You find it only because you are looking at the bank very carefully. You feel you've found something private.",
                "You look at all the layers and feel the long conversation between this place and the people who found it.",
                "You pick it up and feel the weight of the original detail. You set it back exactly.",
                "You stand beside them and let the current reach your feet. Someone wanted something very specifically from this river.",
                "You sit with it for a moment. It came from somewhere else. Someone wanted it here specifically.",
                "You find the hardened wax at the base. The candle burned for a long time. The river took the ending.",
            ],
        },
    ];
}
