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
            ? GetFlavorText(tile.Biome, tile.Feature, player.Q, player.R)
            : "You look around carefully. There is not much to see. You look again. Still nothing. You are thorough and it has not helped.";
        var notes = await _noteRepo.GetNotesForTileAsync(player.Q, player.R);
        return (flavor, notes);
    }

    private static string GetFlavorText(BiomeType biome, TileFeature feature, int q, int r)
    {
        var lines = feature != TileFeature.None ? GetFeatureLines(feature) : GetBiomeLines(biome);
        return lines[Math.Abs(q * 11 + r * 17) % lines.Length];
    }

    private static string[] GetFeatureLines(TileFeature feature) => feature switch
    {
        TileFeature.SpookyWoods => [
            "The trees lean inward and their branches touch above your head. Something breathes out of rhythm with the wind. You look up. You wish you hadn't.",
            "A child's laugh echoes somewhere in the dark ahead. You call out. It laughs again. You do not call out again.",
            "Your shadow falls in the wrong direction. You watch it carefully for a moment. It watches back with what appears to be amusement. You leave.",
            "You find a ring of scorched earth between the trees, perfectly circular. Nothing grows inside it. Nothing will. You step inside anyway and immediately step back out.",
        ],
        TileFeature.MushroomGrove => [
            "The mushrooms here are the size of stools — the sitting kind — and they glow a gentle blue. You sit on one. It is extremely comfortable. It hums. You hum back.",
            "You eat a small mushroom. It tastes of strawberries. The strawberries taste of regret. The regret is inexplicably pleasant. You eat another.",
            "The largest mushroom has a door carved into it. A knocker, too. You knock. Nothing answers. The knocker is warm.",
            "Spores drift around you like snow. One lands on your hand and you understand something important about yourself. Then it dissolves and you forget what it was. This feels like the correct outcome.",
        ],
        TileFeature.AncientGlade => [
            "A shaft of light falls through the canopy and strikes the center of the glade at an angle that makes no geometric sense. You stand in it anyway. It is warm in a way that has nothing to do with the sun.",
            "At the heart of the glade grows a tree wider than a house. Carved into its bark are thousands of names — some in alphabets you don't recognize, some in no alphabet at all. You add yours.",
            "A deer stands at the edge of the glade and watches you with enormous golden eyes. When you blink, it is closer. When you blink again, it is gone. When you blink a third time you realize you are not sure how long you've been standing here.",
            "The grass here is impossibly soft. You lie down in it for what feels like a moment. The sun has moved considerably when you stand back up.",
        ],
        TileFeature.FernDell => [
            "The ferns close above you until the sky is only a green suggestion. Something small and quick moves between the fronds. You catch a glimpse: it is smiling at something.",
            "At the center of the dell, partially buried, you find a perfect glass marble. Inside it, a storm rages. Very tiny. Very real. Very angry.",
            "A path winds through the dell, too regular to be accident. It was made by feet. Many feet. The path goes somewhere you cannot see and turns toward it with great confidence.",
            "The ferns are cold and wet and press against your face. On the other side: more ferns. But these feel older. You cannot explain how ferns feel older. You feel it anyway.",
        ],
        TileFeature.BramblePatch => [
            "The thorns catch and hold with remarkable determination. As you pull free, you find a single perfect rose at the center of the tangle. It has no smell. It has instead an expression.",
            "Something shines at the heart of the brambles. You fight your way toward it with enthusiasm and without adequate planning. You reach it: a brass button. Engraved: a ship. The nearest sea is very far away.",
            "The brambles part unexpectedly to reveal a small clearing containing exactly one wooden chair. The chair faces away from you. You do not walk around it.",
            "You push through and on the other side find yourself exactly where you started. You push through again. Same result. You go around. This takes longer but feels correct.",
        ],
        TileFeature.MushroomRing => [
            "You step inside the ring and the world goes very quiet. Not silent — quiet. You hear your own heartbeat and realize it's been speaking in a rhythm you've never noticed. You listen until it stops trying to tell you something.",
            "A voice says your name, very clearly, in a tone of mild surprise, from no direction in particular. You spin around. There is no one there. The mushrooms do not blink because mushrooms don't have eyes. You note this carefully.",
            "You walk around the outside of the ring once, twice, three times. On the third pass you feel certain you are being counted. On the fourth pass you feel certain you passed.",
            "You feel an irresistible urge to dance. You dance, briefly, just a little. The mushrooms seem satisfied. You feel satisfied too, and confused about feeling satisfied.",
        ],
        TileFeature.TropicalGrove => [
            "A parrot lands on your shoulder and whispers something in your ear in a language you don't speak. It seems frustrated that you don't speak it. You are also a little frustrated.",
            "You find a hammock strung between two palms. You lie in it. The sound of waves. You close your eyes for a moment. When you open them, the light has changed. You don't know how long you've been here. You feel excellent.",
            "Bright fruit hangs heavy and ripe on every branch. You take one. It tastes exactly like what you needed right now. You reach for another. Both are gone. You check your pockets. Gone.",
            "The light here is gold and soft and something sings in the canopy that isn't quite a bird. The song makes you feel that things will probably be fine. You believe this more here than you normally do.",
        ],
        TileFeature.WildOrchard => [
            "The trees have developed strong opinions after years unsupervised. The apples are small and lopsided and taste better than any apple you've ever had. You eat three. You take a fourth for the road. The road will be fine.",
            "A rope swing hangs from the largest apple tree, well-knotted and recently used. You swing on it once. Twice. You feel briefly embarrassed about how good this is. You do it a third time.",
            "A stone wall surrounds the orchard, half-collapsed and covered in lichen. The gate still hangs. On the post: 'HELP YOURSELF.' You help yourself. It feels like the most legal thing you've ever done.",
            "You find a beehive in a hollow trunk, enormous and old and humming. One bee lands on your hand and regards you carefully before flying away. You feel assessed and cleared.",
        ],
        TileFeature.BurnedRuins => [
            "The chimney still stands. Everything else has fallen but the chimney stands tall and looks vaguely proud of itself. At its base, a hearthstone still bears the scorch marks of a thousand ordinary evenings.",
            "You find a door handle in the ash. Just the handle. It is heavy and ornate and clearly belonged to something that mattered. You set it back down carefully where you found it.",
            "The walls are black with old fire. One section of painted plaster survives — blue flowers, very cheerful blue flowers — protected by where a beam fell. You wish you hadn't seen them. You look at them for a while.",
            "Something moves in the ruins ahead. You stop. It stops. You wait. It waits longer. You decide this is its home now, not yours, and you respect that.",
        ],
        TileFeature.AncientShrine => [
            "The offerings span lifetimes: a coin worn smooth by a hundred hands, a small clay figure, a child's shoe, a lock of hair pressed between two stones. You feel that arriving with nothing is also a kind of offering.",
            "The god this shrine was built for has not been named in a long time. You say a few words anyway, improvised, for whatever needs them. The silence afterward feels receptive.",
            "A candle burns at the shrine. It was lit recently. You search the area. There is no one. The candle is burning and there is no one and you leave at a brisk, non-panicked pace.",
            "You kneel at the altar without meaning to. Your knees simply take you there. When you stand, something feels slightly different. Not better, not worse. Settled.",
        ],
        TileFeature.RuinedTower => [
            "At the top of the rubble, perched impossibly, a stone gargoyle still watches the horizon. One eye missing. The other extremely alert. You get the sense it has seen things that would not improve your day.",
            "The staircase still winds up the remaining wall, stopping at nothing. You climb it. At the top you can see very far. Someone is watching the tower from the trees. You wave. They do not wave back.",
            "Carved above the threshold in letters two feet high: a warning in a language you don't know. You feel you understand it anyway. It says something was kept here. It says they kept it as long as they could.",
            "The floor is covered in a mosaic you only realize gradually is a map — of somewhere, in a style that makes your eyes work harder than usual. One tile is missing at the center. You don't know what was there.",
        ],
        TileFeature.AbandonedFarm => [
            "The table is set as if someone left mid-preparation and simply never came back. The candle has burned down over years, not hours. The chairs face each other and have been facing each other for a long time.",
            "In the barn, the stalls are empty but the hay is stacked. Whoever left here planned to come back. The hay has mostly turned to dust. No one came back.",
            "A scarecrow stands in the field, impeccable, wearing a hat in considerably better shape than everything else. It has no face. The hat has opinions about you.",
            "You find a journal in the farmhouse. The last entry is about the weather, written by someone who expected to write the next entry. You close it carefully and put it back exactly where it was.",
        ],
        TileFeature.ForgottenGrave => [
            "The cairn is carefully made — each stone chosen. Someone who was sad and patient and very thorough built this. You add a stone. It feels correct.",
            "The wildflowers have grown so thick over the mound that in the right light it looks like the hill itself is blooming. You stand here for a moment. It seems important that someone stood here.",
            "The marker has fallen face-down in the grass. You consider turning it over. Some privacy should be kept even by strangers.",
            "The grave is very small. You stand here longer than you intended. You don't entirely understand why. You stay until it seems right to leave.",
        ],
        TileFeature.CrumbledFortress => [
            "The great hall is open to the sky. Birch trees grow up through the flagstones, pale and impertinent. Somewhere in thirty years of leaves, a goblet.",
            "You walk the ramparts — what remains of them — and look out over the land this fortress once defended. The land is fine. The land was always going to be fine.",
            "The portcullis has rusted in its raised position. Halfway down the gatehouse, someone has left a sign: 'CLOSED.' The sign is very old.",
            "In the armory, one sword remains, rusted to its stand. You try to pull it free. It makes a sound like the word 'no.' You respect the no.",
        ],
        TileFeature.StoneCircle => [
            "The stones are older than any name you know. You walk the circle slowly. On the third pass you feel briefly seen by something vast and patient and uninterested. On the fourth pass it has moved on.",
            "One stone leans slightly. Against it, in a crevice: a coin, a button, and a folded piece of paper. The paper is blank. Someone left a blank piece of paper here. You think about this for an unreasonable amount of time.",
            "The center of the circle feels like the inside of a held breath. You stand in it and feel the stones behind you, all of them, equidistant, as if they're doing something you cannot perceive.",
            "At midsummer at dawn the shadows of these stones would align to point somewhere specific. You don't know where. You feel you're meant to know. You feel this very strongly.",
        ],
        TileFeature.LonelyWell => [
            "You drop a coin. For a long time, nothing. Then from very far below: a sound that might be a splash or might be a word. You listen very carefully. You drop another coin. The sound does not repeat.",
            "The well is dry. You lower yourself in carefully and at the bottom find an iron box. It is empty. It smells faintly of something sweet. You have no idea what to do with this information.",
            "The bucket hangs on its rope, untouched. You pull it up. It is full of cold, clear water. This well has been dry for thirty years. You drink anyway and it is fine.",
            "Someone has painted a kind face on the stone rim. It is looking at you. You feel slightly better about wells in general.",
        ],
        TileFeature.WitchsCottage => [
            "You knock on the door. Inside: silence, then movement, then more silence. The door does not open. Through the window a hand draws a curtain. You leave a polite farewell and walk away at a pace you would describe as purposeful rather than fast.",
            "The garden grows things you don't have names for. Some of them seem to recognize you. One reaches. 'Manners,' you say, and it withdraws. You feel you've handled this well.",
            "The smoke from the chimney smells of lavender and burnt hair and something with no name. The smell follows you for an hour. You decide not to think about what this means.",
            "The cat from the windowsill drops to the ground and walks to your feet and sits and looks up at you with the authority of something that has decided about you already. You have been decided about.",
        ],
        TileFeature.HotSpring => [
            "You put your hand in the water. Every muscle you own relaxes simultaneously. You sit at the edge. Every worry relocates itself to a drawer you're not currently thinking about.",
            "The water is warm and impossibly clear. At the bottom, mineral formations have built into shapes — not quite like anything, but close to several things. One of them appears to be watching the surface.",
            "Steam rises in slow columns. In the steam, briefly, you see a face. Your face. It looks considerably more rested. It winks. You decide not to think too hard about this.",
            "You emerge from the spring and the cold air hits you and you feel clean in a way that has nothing to do with being clean. Something has been lifted. You don't ask what.",
        ],
        TileFeature.TidePools => [
            "A hermit crab has made its home in a teacup. It has decorated. It has done a genuinely good job. You approve and move on, feeling oddly proud of the crab.",
            "You crouch at the largest pool and something crouches back immediately. A crab. It raises one claw. You raise one finger. You regard each other for a very long time. Neither backs down. You are here for eleven minutes.",
            "Something in the farthest pool glows a soft green. When you wade toward it, it moves to the next pool, always the same distance ahead, until it reaches the open sea and is gone. You stand there for a while.",
            "A starfish settles over your hand when you reach into the water. You feel strangely honored. You carefully put your hand down and let it stay as long as it wants.",
        ],
        TileFeature.ReedBeds => [
            "Something large moves through the reeds ahead of you, parting them without sound. You stand very still. The reeds close back over where it was. You wait a long time before moving.",
            "A red-winged blackbird tells you something with great urgency from a swaying reed. You cannot help with whatever it is. It tells you again. You nod. It seems to accept this.",
            "Your reflection in the still water between the reeds watches you with perhaps more attention than reflections normally do. When you stop, it stops half a second later.",
            "The reeds bend in a pattern you almost understand. Something is moving under the water beside you, keeping pace. It is large. You cannot see it. It seems aware that you cannot see it.",
        ],
        TileFeature.SaltFlats => [
            "The silence here is not the absence of sound. It is a presence. Something old and dry lives in it and is completely untroubled by your visit.",
            "At the center of the flats stands a single wooden post. No sign, no rope, nothing attached. Just a post. Standing, for reasons that belonged to someone else entirely.",
            "The heat haze reshapes the horizon into something like a city, then a forest, then a face. When you walk toward it, it remains exactly as far away. You walk for a while anyway.",
            "The cracks in the salt form a perfect map of something — veins, rivers, streets. You try to memorize it. It's too large and too precise and you carry only a piece of it away.",
        ],
        TileFeature.OasisGrove => [
            "The water is cold and sweet and real and you drink from it with your hands like an animal and feel no shame about this whatsoever.",
            "The palms cast shade of a quality you haven't experienced before — dense and golden and warm in the way that shade is rarely warm. You sit in it. Something passes overhead that isn't a bird. You don't look up.",
            "You fill your hands with water and drink. At the bottom of the pool, coins thrown by every traveler who found this place. You add yours. It feels correct in a way you don't need to explain.",
            "An ibis stands at the water's edge, motionless, watching. It has been watching longer than you've been here. It will be watching long after you leave. It watches the way water watches.",
        ],
        TileFeature.DeadForest => [
            "The bleached trees stand like testimony. Whatever came through here was thorough and final. Even the wind avoids the dead branches. The silence is of the complete variety.",
            "A crow sits on every tenth snag, motionless, watching you in shifts as you pass. One follows. Always the same distance behind. You stop. It stops. You walk. It walks. You do not look back again.",
            "The light in a dead forest is harsh and unfiltered. You see yourself clearly in it. You squint. It is not entirely comfortable but you feel it might be good for you.",
            "One tree fell and in falling caught another. They lean together, holding each other up, dead and inseparable. Lichens have grown over them both. The lichens are very much alive and seem pleased about the arrangement.",
        ],
        TileFeature.QuickSand => [
            "The ground gives under you with a sound of polite disagreement. You extract yourself with effort and dignity. Mostly effort.",
            "The sign reads CAUTION. The sign is halfway in. It has read CAUTIO for some time now.",
            "You test the ground with a stick. The stick disagrees strongly with the ground. You take a different path. The stick does not return.",
            "You sink to your knees before registering what is happening, and then spend a considerable amount of time reconsidering your commitment to this direction specifically.",
        ],
        TileFeature.CaveEntrance => [
            "The cave breathes — not like a living thing, but slower. One breath in, one out, every few minutes, cool and mineral-smelling. You breathe with it for a while.",
            "You call into the cave. Your voice returns changed — not an echo exactly, but a version of itself that has been somewhere and learned something. You call again. This version sounds wiser.",
            "The walls of the entrance are covered in pressed handprints, hundreds of them, all sizes, as if reaching through from the other side. You press your hand against one that is roughly your size.",
            "Something lives in the dark beyond the entrance. You can hear it, slow and rhythmic — breathing, or something like breathing. It does not approach. It does not retreat. It simply continues being there.",
        ],
        TileFeature.FrozenShrine => [
            "The altar is sealed in ice so clear you can read the inscription without touching it. You read it. You wish you hadn't. You read it again.",
            "The offerings inside the ice are perfectly preserved — flowers, food, tokens — placed by someone who had no reason to think they'd last. They lasted. The cold kept what the warmth would have lost.",
            "You touch the ice. Your fingers stick briefly, and in that moment you feel the weight of something very old and very cold and not asleep. You pull your hand away. It lets go.",
            "The ice is lit from within, blue-white, sourceless. A shape moves slowly through its depths — large, deliberate, indifferent to you. Gone before you can describe it. You describe it anyway, to yourself, quietly.",
        ],
        TileFeature.IcyCavern => [
            "The ice is every shade of blue and you stand in the middle of it feeling the word 'cathedral' without knowing why. You breathe out. The ice breathes back.",
            "Trapped in a column of ice: a leaf, green and perfect, from a tree that no longer grows here. Nearby: a small fish, frozen mid-turn. They have been here a long time. They will be here longer.",
            "Your footsteps arrive after you, slightly delayed by the acoustics of ice. You stop. Your footsteps continue two more steps and stop. You wait. They do not resume. You decide not to think about this.",
            "At the far end of the cavern the ice is so deep it looks black. Something moves in it, enormous and patient, something that thinks in seasons. You feel very small. This feels correct.",
        ],
        _ => ["You search the area carefully. You find it interesting, in your own way."],
    };

    private static string[] GetBiomeLines(BiomeType biome) => biome switch
    {
        BiomeType.Ocean => [
            "The waves are relentless and patient. They have been doing this forever. They will do this forever after you. You are a brief event in a very long process.",
            "The water goes dark very quickly. You don't know what is below. The water knows you don't know.",
        ],
        BiomeType.Shallows => [
            "The water is clear enough to see every pebble on the bottom. Small fish investigate your feet with scientific interest. One nudges your toe deliberately. You have been studied.",
            "Sunlight refracts through the water and makes patterns on the sandy bottom that almost look like writing. Almost. You squint. You're not sure.",
        ],
        BiomeType.Beach => [
            "The sand holds the heat of the day. A crab assesses you from a respectful distance and decides you are probably not a threat. It is generous to think so.",
            "The surf pulls at the sand under your feet with each wave, making the ground briefly liquid. You sink a little. You stay.",
            "The tide left a line of shells and kelp and small mysterious things. You examine one of the small mysterious things. It examines you back with its many small eyes. You put it down.",
        ],
        BiomeType.River => [
            "The current is patient and consistent. It does not care where you want to go — only where it's going. For a moment you consider letting it decide.",
            "The water is cold and fast and clear. Something silver passes beneath — too quick to be sure of.",
        ],
        BiomeType.Swamp => [
            "Something bubbles from the mud nearby — steady, purposeful, as if reporting something to the mud. You do not ask what.",
            "The water here is the color of strong tea and smells of things breaking down into other things. This is the natural order. You still find it unsettling.",
        ],
        BiomeType.Marsh => [
            "The ground makes complicated promises about being solid and breaks most of them. You test each step. The marsh tests back.",
            "A heron stands ahead of you, still as a carved thing, watching for something with absolute focus. You stand just as still out of competitive instinct. The heron remains focused. You eventually give up.",
        ],
        BiomeType.Grassland => [
            "The grass bends in a wave when the wind comes through, like something breathing. You are briefly part of the breath.",
            "Crickets start and stop in shifts, handing the silence between them. You sit in the grass and listen to the negotiation.",
        ],
        BiomeType.Plains => [
            "The horizon is very far away and perfectly flat. You feel both significant and appropriately small, which is the correct ratio.",
            "The wind crosses the plains with nothing to stop it and arrives at you carrying the smell of somewhere you haven't been yet.",
        ],
        BiomeType.Savanna => [
            "The golden grass stretches in every direction. In the distance, something stands very still watching you. When you look directly, it moves. When you look away, it stills again.",
            "The dry grass rasps in the wind, a continuous sound like sand over stone. Somewhere a bird calls once. Nothing answers.",
        ],
        BiomeType.Forest => [
            "Light filters through the canopy in shifting columns. The forest is doing what forests do — being enormous and patient and absorbed in its own concerns. You are a brief parenthesis in something much longer.",
            "A branch moves with no wind behind it. Then another. Then the forest settles and is very still and you walk more carefully.",
        ],
        BiomeType.Jungle => [
            "Everything here is growing with total commitment. The trees compete for the light overhead. The undergrowth competes for what's left. You push through it all, briefly disputed territory.",
            "Something in the canopy watches you with great yellow eyes. You look up and lose it in the green. When you look down, it has moved. When you look up again, it's in the same place it started. You feel it has made a point.",
        ],
        BiomeType.Desert => [
            "The heat is physical. It presses against you like a hand. The silence presses the same way. They weigh about the same.",
            "Nothing moves here that doesn't have to. You move carefully, out of respect.",
        ],
        BiomeType.Mountain => [
            "The rock underfoot is very old and very sure of itself. It makes no accommodations for you. You find your own way across it and feel this is the correct relationship between you.",
            "The wind at this elevation is a different thing than the wind below — sharper, colder, more certain. It knows where it's been.",
        ],
        BiomeType.Tundra => [
            "The permafrost is under your boots, ancient and silent and completely unimpressed. You are a warm soft thing walking briefly over something cold and very old.",
            "The wind on the tundra has been traveling for a long time and still has somewhere to be. It passes through you on its way without stopping.",
        ],
        BiomeType.Snow => [
            "Your breath makes small clouds that vanish quickly. You name one of them. This is the most important thing you'll do today.",
            "The snow absorbs sound. Each step falls into silence. You have the impression of walking in a very large room that wants quiet.",
        ],
        BiomeType.Glacier => [
            "The ice groans somewhere beneath your feet — a sound like the world settling. The glacier is moving, imperceptibly, every moment. You are moving with it.",
            "The ice at this depth is a blue that doesn't have a name in most languages. It is a color that existed before anyone named colors.",
        ],
        BiomeType.Volcano => [
            "The ground here radiates an uncomfortable heat and smells of sulfur and something older. You feel very strongly that you should not be here.",
        ],
        _ => ["You look around carefully. There is not much to see. You look again to make sure."],
    };
}
