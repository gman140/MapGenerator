using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryBiomeDefinitionProvider : IBiomeDefinitionProvider
{
    private static readonly BiomeDefinition[] _definitions =
    [
        new()
        {
            Type             = BiomeType.Ocean,
            CooldownMs       = 0,
            MinimapRgb       = (28, 78, 140),
            NeighborPriority = 8,
            NeighborText     =
            [
                "Salt air finds you from the direction of the water.",
                "The sea is close enough to smell, maybe to hear.",
                "Something maritime is in the air here — borrowed from wherever the water begins.",
                "The ocean is present as a sound before it is a sight.",
                "Tidal air reaches this far. It always does.",
                "Something about the light here has traveled over open water to get here.",
            ],
            InvestigatePartA =
            [
                "The waves are relentless and patient.",
                "The water goes dark very quickly.",
                "The sea is doing what the sea does, which is everything and nothing.",
                "The horizon is a perfectly ruled line that moves as you approach it.",
                "A wave arrives, considers the shore, and retreats to reconsider.",
                "The smell of the sea is specific and ancient and does not belong to anyone.",
            ],
            InvestigatePartB =
            [
                "They have been doing this forever. They will do this forever after you. You are a brief event.",
                "You don't know what lives below. The water knows you don't know.",
                "You are here very briefly and it has no opinion about this.",
                "It always has been, and you find this either comforting or not, depending.",
                "You understand this is not an invitation.",
                "You are an interesting shape on an otherwise empty coastline. The sea notes this without comment.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fish",      Probability = 0.30f },
                new() { ResourceId = "Driftwood", Probability = 0.15f },
                new() { ResourceId = "Salt",      Probability = 0.10f },
                new() { ResourceId = "TidalCoin", Probability = 0.03f },
            ],
        },
        new()
        {
            Type             = BiomeType.Lake,
            CooldownMs       = 0,
            MinimapRgb       = (58, 110, 165),
            NeighborPriority = 0,
            InvestigatePartA =
            [
                "The lake sits perfectly still.",
                "The water here is enclosed, inland, its own world.",
                "The lake holds the sky on its surface like a second sky.",
                "The surface is unmarked in any direction. Nothing has disturbed it recently.",
                "Nothing here is in a hurry, including the water.",
                "A single bird crosses the far shore and is gone before it matters.",
            ],
            InvestigatePartB =
            [
                "It reflects without comment.",
                "Nothing leaves and nothing arrives. It has been this way for a long time.",
                "You find this somehow more unsettling than open water.",
                "It has been this way before you. It will continue after.",
                "You stand at the edge and the lake does not invite or discourage.",
                "You feel the stillness as a kind of pressure.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fish",       Probability = 0.35f },
                new() { ResourceId = "Reed",       Probability = 0.20f },
                new() { ResourceId = "Clay",       Probability = 0.15f },
                new() { ResourceId = "RiverGlass", Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.Shallows,
            CooldownMs       = 1200,
            MinimapRgb       = (58, 138, 170),
            NeighborPriority = 2,
            NeighborText     =
            [
                "The air smells faintly of shallow water and wet sand.",
                "A coastal quality settles over the light here.",
                "Water is close — shallow, warm, audible when the wind shifts.",
                "The smell of warm tidal flats reaches here on the right wind.",
                "A shallow-water quality finds the light here from one direction.",
                "Something coastal and unhurried reaches the air from nearby.",
            ],
            InvestigatePartA =
            [
                "The water is clear enough to see every pebble on the bottom.",
                "Sunlight bends through the water and makes patterns on the sandy floor.",
                "Small fish move in purposeful formations around your feet.",
                "The bottom is fully legible — every stone, every small movement, every shadow.",
                "The water is shallow enough to make the sky seem very close.",
                "Small crabs pick their way sideways through the shallows with extreme purpose.",
            ],
            InvestigatePartB =
            [
                "Small fish investigate your feet. One nudges deliberately. You have been studied.",
                "They almost look like writing. You squint. You're still not sure.",
                "One stops and faces you directly for an unreasonable amount of time.",
                "The water clarifies everything at this depth, including several things you weren't ready to examine.",
                "It has its own schedule and you are not on it.",
                "You are briefly part of the underwater record of things that have passed through here.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fish",       Probability = 0.45f },
                new() { ResourceId = "Sand",       Probability = 0.30f },
                new() { ResourceId = "Salt",       Probability = 0.12f },
                new() { ResourceId = "Reed",       Probability = 0.15f },
                new() { ResourceId = "RiverGlass", Probability = 0.08f },
                new() { ResourceId = "TidalCoin",  Probability = 0.03f },
            ],
        },
        new()
        {
            Type             = BiomeType.Beach,
            CooldownMs       = 400,
            MinimapRgb       = (210, 195, 140),
            NeighborPriority = 3,
            NeighborText     =
            [
                "Surf sounds reach you on the right wind.",
                "The ground is sandier here, borrowed from wherever the shore begins.",
                "Something coastal is in the light here — reflected off water not far away.",
                "Sand has been arriving here on the wind — a grain at a time, over a long time.",
                "The quality of light changes from the direction of open water.",
                "Wave-sound reaches here from the right direction.",
            ],
            InvestigatePartA =
            [
                "The sand holds the warmth of the day.",
                "The surf pulls at the sand with each wave, making the ground briefly liquid.",
                "The tide has left a line of shells and kelp and small mysterious things.",
                "Sandpipers work the surf line with professional indifference to your presence.",
                "The tide has written something in the wrack line you can almost read.",
                "The beach narrows and widens with the tides, never quite the same twice.",
            ],
            InvestigatePartB =
            [
                "A crab assesses you from a respectful distance and decides you are probably not a threat. It is generous.",
                "You sink a little with each wave. You stay.",
                "You examine one of the small mysterious things. It examines you back with its many small eyes.",
                "You spend more time trying to read it than you intended. You give up, but not without effort.",
                "They have their own concerns. You have yours. The arrangement is comfortable.",
                "The ocean takes it back eventually. This is one of the agreements.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Sand",      Probability = 0.60f },
                new() { ResourceId = "Driftwood", Probability = 0.20f },
                new() { ResourceId = "Salt",      Probability = 0.20f },
                new() { ResourceId = "Feathers",  Probability = 0.12f },
                new() { ResourceId = "Flint",     Probability = 0.10f },
                new() { ResourceId = "TidalCoin", Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.River,
            CooldownMs       = 1500,
            MinimapRgb       = (64, 120, 190),
            NeighborPriority = 5,
            NeighborText     =
            [
                "You can hear running water nearby if you listen for it.",
                "The sound of a river reaches here — intermittent, comfortable.",
                "Water is moving somewhere close. You can't see it, but the sound finds you.",
                "A fresh mineral smell reaches here from wherever the water runs.",
                "The air carries a slight coolness from the direction of the river.",
                "River sound finds this place from the right direction — low, consistent.",
            ],
            InvestigatePartA =
            [
                "The current is patient and consistent.",
                "The water is cold and fast and clear.",
                "The river bends away ahead of you with great purpose.",
                "The river moves with an urgency that makes standing beside it feel like a choice.",
                "The water is clear enough to read the riverbed in detail.",
                "Eddies form behind the rocks, spin briefly, then are reclaimed by the current.",
            ],
            InvestigatePartB =
            [
                "It does not care where you want to go — only where it's going. You consider letting it decide.",
                "Something silver passes beneath — too quick to be sure of.",
                "It knows where it is going and has known for a long time.",
                "You watch the eddies for longer than you meant to.",
                "The river has been deciding this for longer than you have been asking.",
                "You stand at the bank for a while and let it be louder than your thoughts.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fish",       Probability = 0.50f },
                new() { ResourceId = "Reed",       Probability = 0.30f },
                new() { ResourceId = "Clay",       Probability = 0.22f },
                new() { ResourceId = "Flint",      Probability = 0.15f },
                new() { ResourceId = "RiverGlass", Probability = 0.10f },
            ],
        },
        new()
        {
            Type             = BiomeType.Swamp,
            CooldownMs       = 3000,
            MinimapRgb       = (60, 80, 55),
            NeighborPriority = 6,
            NeighborText     =
            [
                "A wet, organic smell drifts in from somewhere low and still.",
                "The air carries a note of standing water from somewhere nearby.",
                "The edge of wetter ground is close enough to smell.",
                "Something low and still is nearby. The smell is the confirmation.",
                "The air from one side has a heaviness that isn't about the weather.",
                "You know this smell. Swamp.",
            ],
            InvestigatePartA =
            [
                "Something bubbles from the mud nearby — steady, purposeful.",
                "The water here is the color of strong tea.",
                "The air here is complicated.",
                "The water holds reflections of things you'd rather not look at twice.",
                "Frogs announce themselves with great regularity and no sign of self-consciousness.",
                "Something moves just under the surface — slow, unhurried, uninterested in being seen.",
            ],
            InvestigatePartB =
            [
                "You do not ask what it is reporting.",
                "It smells of things breaking down into other things. This is the natural order. You still find it unsettling.",
                "You breathe carefully through your nose and make a series of decisions.",
                "You watch the surface for a moment. The surface watches back.",
                "They have been announcing themselves here for longer than you have been capable of listening.",
                "You leave it to its business and it leaves you to yours. This is the swamp's version of hospitality.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Peat",        Probability = 0.45f },
                new() { ResourceId = "Reed",        Probability = 0.30f },
                new() { ResourceId = "Herbs",       Probability = 0.15f },
                new() { ResourceId = "PaleMushroom",Probability = 0.08f },
                new() { ResourceId = "RottenSilks", Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.Marsh,
            CooldownMs       = 2000,
            MinimapRgb       = (85, 104, 72),
            NeighborPriority = 6,
            NeighborText     =
            [
                "A wet, organic smell drifts in from somewhere low and still.",
                "The air carries a note of standing water from somewhere nearby.",
                "The edge of wetter ground is close enough to smell.",
                "Wet grass smell reaches here from wherever the ground gets soft.",
                "A flat, reedy quality arrives with the air from one direction.",
                "The edge of somewhere low and waterlogged is close enough to register.",
            ],
            InvestigatePartA =
            [
                "The ground makes complicated promises about being solid.",
                "A heron stands ahead of you, still as a carved thing.",
                "The water and land here have come to an arrangement that benefits neither.",
                "The reed beds move in slow waves, as if considering something collectively.",
                "The mud underfoot makes a sound like a considered opinion.",
                "The marsh has its own smell — green and deep and not entirely unpleasant.",
            ],
            InvestigatePartB =
            [
                "You test each step. The marsh tests back.",
                "It is watching for something with absolute focus. You stand still out of competitive instinct. It wins.",
                "You navigate it carefully and feel a modest but genuine pride at the other side.",
                "The ground here demands more than ground usually demands. You negotiate.",
                "This is the marsh's consistent position and you respect it for the consistency.",
                "A reasonable thing to be proud of. The marsh did not make it easy.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Reed",        Probability = 0.45f },
                new() { ResourceId = "Peat",        Probability = 0.25f },
                new() { ResourceId = "Clay",        Probability = 0.20f },
                new() { ResourceId = "Herbs",       Probability = 0.15f },
                new() { ResourceId = "PaleMushroom",Probability = 0.08f },
            ],
        },
        new()
        {
            Type             = BiomeType.Grassland,
            CooldownMs       = 400,
            MinimapRgb       = (110, 175, 65),
            NeighborPriority = 0,
            InvestigatePartA =
            [
                "The grass bends in a wave when the wind comes through.",
                "Crickets start and stop in shifts across the field.",
                "The smell here is green and warm and uncomplicated.",
                "A meadowlark somewhere is delivering a long and specific opinion.",
                "The grass is tall enough to lose the horizon in if you sit down in it.",
                "The light here is open and even and has nothing to hide.",
            ],
            InvestigatePartB =
            [
                "You are briefly part of the breath.",
                "You sit in the grass and listen to the negotiation.",
                "You stand in it for a while. It does not ask anything of you.",
                "You listen to the whole thing. It does not repeat. You feel honored.",
                "You try it. The grass closes over your head and the world becomes very simple.",
                "You appreciate it for this.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fiber",      Probability = 0.55f },
                new() { ResourceId = "Herbs",      Probability = 0.20f },
                new() { ResourceId = "Feathers",   Probability = 0.12f },
                new() { ResourceId = "Flint",      Probability = 0.10f },
                new() { ResourceId = "DeadGrass",  Probability = 0.08f },
                new() { ResourceId = "CrowFeather",Probability = 0.04f },
            ],
        },
        new()
        {
            Type             = BiomeType.Plains,
            CooldownMs       = 400,
            MinimapRgb       = (185, 175, 90),
            NeighborPriority = 0,
            InvestigatePartA =
            [
                "The horizon is very far away and perfectly flat.",
                "The wind crosses the plains with nothing to stop it.",
                "The sky here is enormous in a way you don't notice until you look up.",
                "The grass here is low and even and goes as far as you care to look.",
                "Sound travels farther here than it should. You hear something distant.",
                "A hawk turns slow circles above something you can't identify from this distance.",
            ],
            InvestigatePartB =
            [
                "You feel both significant and appropriately small. This is the correct ratio.",
                "It arrives at you carrying the smell of somewhere you haven't been yet.",
                "You look up. You stay looking up for a while.",
                "You aren't sure whether to investigate. You file it away.",
                "You stand still and let the sky be enormous for a moment. It cooperates.",
                "The hawk has information you don't. You accept this.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fiber",    Probability = 0.45f },
                new() { ResourceId = "Flint",    Probability = 0.22f },
                new() { ResourceId = "Feathers", Probability = 0.15f },
                new() { ResourceId = "Stone",    Probability = 0.10f },
                new() { ResourceId = "DeadGrass",Probability = 0.10f },
            ],
        },
        new()
        {
            Type             = BiomeType.Savanna,
            CooldownMs       = 500,
            MinimapRgb       = (200, 184, 80),
            NeighborPriority = 0,
            InvestigatePartA =
            [
                "The golden grass stretches in every direction.",
                "The dry grass rasps in the wind, a continuous sound like sand over stone.",
                "Something large stands very still in the distance, watching.",
                "The light here is hard and specific, with no intention of being kind about it.",
                "Termite mounds rise with great confidence across the plain.",
                "A vulture turns patient circles in the distance with tremendous efficiency.",
            ],
            InvestigatePartB =
            [
                "In the distance something stands very still watching you. When you look directly it moves.",
                "Somewhere a bird calls once. Nothing answers.",
                "When you look directly at it, it moves. When you look away, it stills again.",
                "You are briefly uncertain whose errand you are on.",
                "They have been here longer than you and will be here after. You find this correct.",
                "You find this useful to contemplate. You don't linger.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Fiber",       Probability = 0.40f },
                new() { ResourceId = "Feathers",    Probability = 0.18f },
                new() { ResourceId = "Flint",       Probability = 0.15f },
                new() { ResourceId = "DeadGrass",   Probability = 0.15f },
                new() { ResourceId = "BoneFragment",Probability = 0.10f },
            ],
        },
        new()
        {
            Type             = BiomeType.Forest,
            CooldownMs       = 700,
            MinimapRgb       = (40, 90, 40),
            NeighborPriority = 3,
            NeighborText     =
            [
                "The treeline is close enough to feel — a change in light and temperature.",
                "Birdsong from the nearby forest finds this place.",
                "The smell of damp wood and leaf litter drifts over from the trees.",
                "A deeper shade begins at the treeline not far from here.",
                "Leaf litter smell reaches this far — cool, layered, specific.",
                "The forest edge changes the sound here. You can feel it from this side.",
            ],
            InvestigatePartA =
            [
                "Light filters through the canopy in shifting columns.",
                "A branch moves with no wind behind it.",
                "The forest is doing what forests do — being enormous and absorbed in its own concerns.",
                "The canopy closes over you gradually until it is the ceiling of everything.",
                "Leaves turn without wind, one at a time, deciding about the light.",
                "A tree has fallen and opened a gap overhead. The gap is already being addressed.",
            ],
            InvestigatePartB =
            [
                "The forest is patient and absorbed in its own concerns. You are a brief parenthesis in something longer.",
                "Then another. Then the forest settles and is very still and you walk more carefully.",
                "You are a brief event here. The forest does not mind.",
                "The forest absorbed that conversation a long time ago and has filed it away.",
                "The forest is deciding something at photosynthetic speed. You will not live to hear the outcome.",
                "You are a brief event here. The forest has seen many. It does not grade them.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Wood",        Probability = 0.55f },
                new() { ResourceId = "Herbs",       Probability = 0.20f },
                new() { ResourceId = "Moss",        Probability = 0.18f },
                new() { ResourceId = "PaleMushroom",Probability = 0.10f },
                new() { ResourceId = "Amber",       Probability = 0.08f },
                new() { ResourceId = "CrowFeather", Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.Jungle,
            CooldownMs       = 1200,
            MinimapRgb       = (26, 92, 26),
            NeighborPriority = 4,
            NeighborText     =
            [
                "Dense green presses in from one direction.",
                "The jungle is audible from here — layered, relentless.",
                "Jungle heat and humidity find this place from one side.",
                "The humidity from one direction is a kind of weather of its own.",
                "Green presses in from the treeline with obvious intention.",
                "The jungle makes itself known by smell before anything else.",
            ],
            InvestigatePartA =
            [
                "Everything here is growing with total commitment.",
                "Something watches you from the canopy with large yellow eyes.",
                "The undergrowth presses in from every direction.",
                "The noise here is layered — something calling, something answering, something that is neither.",
                "Vines have covered everything that stood still long enough.",
                "The light filters through a hundred layers before it reaches the ground. It arrives exhausted.",
            ],
            InvestigatePartB =
            [
                "The trees compete for the light overhead. The undergrowth competes for what's left. You are briefly disputed territory.",
                "You look up and lose it in the green. When you look down it has moved. When you look up it is where it started.",
                "You push through it carefully, borrowing space.",
                "You make a note of where you entered. The jungle is not interested in helping you find it again.",
                "Everything here is covered in something else. Ownership is approximate.",
                "You keep moving before it decides to reclaim the space you're standing in.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Wood",       Probability = 0.50f },
                new() { ResourceId = "Fiber",      Probability = 0.30f },
                new() { ResourceId = "Herbs",      Probability = 0.28f },
                new() { ResourceId = "Amber",      Probability = 0.10f },
                new() { ResourceId = "RottenSilks",Probability = 0.08f },
            ],
        },
        new()
        {
            Type             = BiomeType.Desert,
            CooldownMs       = 900,
            MinimapRgb       = (210, 175, 100),
            NeighborPriority = 5,
            NeighborText     =
            [
                "Fine grit finds your teeth from the direction of the open dry land.",
                "The air on one side is noticeably drier than the rest.",
                "The desert's edge is close enough to taste.",
                "The temperature from one direction is lower and drier. The desert's edge.",
                "The dry air reaches this far. You feel it in your throat.",
                "The smell of hot stone and emptiness arrives from the open ground nearby.",
            ],
            InvestigatePartA =
            [
                "The heat is physical. It presses against you like a hand.",
                "Nothing moves here that doesn't have to.",
                "The silence and the heat are the same weight.",
                "The sand has rearranged itself since the last time anyone was here. No one has been here.",
                "A lizard sits on a rock absorbing heat with the focused commitment of a vocation.",
                "The sky here is a specific, aggressive blue that tolerates no interruption.",
            ],
            InvestigatePartB =
            [
                "The silence presses the same way. They weigh about the same.",
                "You move carefully, out of respect.",
                "You stand still for a moment and become briefly part of the landscape.",
                "It looks at you once and returns to the sun. You respect the focus.",
                "You consider this for a moment, standing in the heat, and find it accurate.",
                "You shade your eyes and look at the sky for a while. It does not acknowledge you.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Sand",        Probability = 0.55f },
                new() { ResourceId = "Flint",       Probability = 0.25f },
                new() { ResourceId = "Stone",       Probability = 0.20f },
                new() { ResourceId = "Salt",        Probability = 0.15f },
                new() { ResourceId = "DeadGrass",   Probability = 0.10f },
                new() { ResourceId = "BoneFragment",Probability = 0.08f },
            ],
        },
        new()
        {
            Type             = BiomeType.Mountain,
            CooldownMs       = 2000,
            MinimapRgb       = (130, 125, 120),
            NeighborPriority = 3,
            NeighborText     =
            [
                "The terrain rises sharply not far from here.",
                "Elevation is visible in one direction, patient and permanent.",
                "The shadow of higher ground passes through here at certain hours.",
                "Higher ground is visible from here, watching without comment.",
                "The air gets cooler from the direction the terrain rises.",
                "Stone has a smell. You can smell it from here.",
            ],
            InvestigatePartA =
            [
                "The rock underfoot is very old and very sure of itself.",
                "The wind at this elevation is a different thing than the wind below.",
                "The view from here is significant.",
                "Your breath comes a little shorter here. The mountain is indifferent to this.",
                "The scree shifts underfoot with a sound like small agreements coming apart.",
                "The peaks above are doing something with the clouds — collecting them, dismissing them, unsure.",
            ],
            InvestigatePartB =
            [
                "It makes no accommodations for you. You find your own way across it.",
                "Sharper, colder, more certain. It knows where it's been.",
                "You look at it for a while and feel correctly small.",
                "The mountain was here before you and will be here after. This is not a threat. It's just geography.",
                "You test each foothold carefully. The mountain tests your assessment of each foothold.",
                "You watch the clouds move for a while. They do not ask permission.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Stone",      Probability = 0.55f },
                new() { ResourceId = "Ore",        Probability = 0.30f },
                new() { ResourceId = "Flint",      Probability = 0.20f },
                new() { ResourceId = "Coal",       Probability = 0.15f },
                new() { ResourceId = "Quartz",     Probability = 0.10f },
                new() { ResourceId = "HollowStone",Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.Tundra,
            CooldownMs       = 1800,
            MinimapRgb       = (142, 168, 152),
            NeighborPriority = 4,
            NeighborText     =
            [
                "The wind from one direction has been traveling over open frozen ground.",
                "A spare, cold air reaches here from the tundra nearby.",
                "Something wide and cold is close. You feel its edge.",
                "The cold from the open flat ground reaches here without ceremony.",
                "A wide, featureless chill arrives from one direction.",
                "The tundra sends its air ahead of it. You've received it.",
            ],
            InvestigatePartA =
            [
                "The permafrost is under your boots, ancient and silent and unimpressed.",
                "The wind on the tundra has been traveling for a long time.",
                "The landscape here is spare and specific.",
                "The lichen here is very old and very deliberate about covering everything slowly.",
                "The tundra is flat in a way that holds nothing back. Everything is visible, nothing is hidden.",
                "The only sound is wind crossing open ground. It is not welcoming. It is not unwelcoming either.",
            ],
            InvestigatePartB =
            [
                "You are a warm soft thing walking briefly over something cold and very old.",
                "It still has somewhere to be. It passes through you on its way.",
                "It does not perform for visitors.",
                "The lichen does not acknowledge you. You appreciate the consistency.",
                "You stand in the full visibility of it and feel appropriately exposed.",
                "You walk through it anyway. The tundra makes no comment.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Peat",        Probability = 0.30f },
                new() { ResourceId = "Flint",       Probability = 0.20f },
                new() { ResourceId = "DeadGrass",   Probability = 0.15f },
                new() { ResourceId = "Moss",        Probability = 0.12f },
                new() { ResourceId = "BoneFragment",Probability = 0.10f },
                new() { ResourceId = "FrozenFlower",Probability = 0.03f },
            ],
        },
        new()
        {
            Type             = BiomeType.Snow,
            CooldownMs       = 2500,
            MinimapRgb       = (230, 235, 240),
            NeighborPriority = 4,
            NeighborText     =
            [
                "The air carries a clean cold from whatever is white on the horizon.",
                "Snow is visible somewhere close, lending the air an edge.",
                "The cold from nearby elevation finds this place reliably.",
                "A clean cold finds this place from the direction of the snowpack.",
                "The whiteness from nearby is in the light here too, reflected in every direction.",
                "Something about the sound changes from the direction of snow. It gets quieter.",
            ],
            InvestigatePartA =
            [
                "Your breath makes small clouds that vanish quickly.",
                "The snow absorbs sound completely.",
                "The world here is white and still and very deliberate about it.",
                "The snow has preserved everything exactly as it was when it fell.",
                "Animal tracks cross here, meet, and diverge with apparent purpose.",
                "The light has a sourceless quality — even and cold and coming from everywhere at once.",
            ],
            InvestigatePartB =
            [
                "You name one of the clouds. This is the most important thing you'll do today.",
                "Each step falls into silence. You have the impression of a very large room that wants quiet.",
                "You move through it carefully, feeling like an intrusion.",
                "You try to determine what met here and what was decided. The snow does not say.",
                "You stand in it for a moment and try to find where the light is coming from. There isn't one source.",
                "It becomes your footprints. The snow continues.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Ice",         Probability = 0.55f },
                new() { ResourceId = "Flint",       Probability = 0.15f },
                new() { ResourceId = "BoneFragment",Probability = 0.08f },
                new() { ResourceId = "FrozenFlower",Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.Glacier,
            CooldownMs       = 3000,
            MinimapRgb       = (200, 220, 232),
            NeighborPriority = 7,
            NeighborText     =
            [
                "One direction is noticeably colder than the others.",
                "Glacial air presses in from one side with quiet certainty.",
                "The cold here has been traveling over ice to reach you.",
                "A deep cold radiates from one direction that has nothing to do with the weather.",
                "Something vast and frozen is making itself felt from nearby.",
                "The cold from that direction is geological, not seasonal.",
            ],
            InvestigatePartA =
            [
                "The ice groans somewhere beneath your feet — a sound like the world settling.",
                "The ice at this depth is a blue that doesn't exist in most languages.",
                "The glacier is moving. Imperceptibly. Every moment.",
                "The surface is crossed with fracture lines — old agreements in the process of coming apart.",
                "Air bubbles are sealed in the ice at eye level, trapped since before you were born.",
                "The scale of the glacier only becomes clear when you see something small beside it. You are that thing.",
            ],
            InvestigatePartB =
            [
                "The glacier is moving, imperceptibly, every moment. You are moving with it.",
                "It is a color that existed before anyone named colors.",
                "You are moving with it whether you mean to or not.",
                "Each one is a small archive of the old atmosphere. You breathe new air next to very old air.",
                "You stand at its edge and understand that everything here is on a schedule you are not part of.",
                "You look at the ice for a while and feel the size of the time it represents.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Ice",         Probability = 0.65f },
                new() { ResourceId = "Quartz",      Probability = 0.15f },
                new() { ResourceId = "FrozenFlower",Probability = 0.08f },
                new() { ResourceId = "HollowStone", Probability = 0.05f },
            ],
        },
        new()
        {
            Type             = BiomeType.Volcano,
            CooldownMs       = 0,
            MinimapRgb       = (90, 40, 24),
            NeighborPriority = 10,
            NeighborText     =
            [
                "Sulfur drifts in from somewhere it shouldn't.",
                "The air on one side carries a warmth that has nothing to do with the sun.",
                "Ash has settled here in fine gray layers from whatever burns nearby.",
                "Ash has been settling here in amounts too small to notice until you notice.",
                "The ground temperature from one direction is wrong in a way you feel in your feet.",
                "Sulfur finds the air here from wherever the earth is less composed.",
            ],
            InvestigatePartA =
            [
                "The ground radiates an uncomfortable heat.",
                "The air here smells of sulfur and something older.",
                "The rock underfoot is dark and sharp and recent by geological standards.",
                "The cooled lava here has set into shapes that suggest speed, panic, and finality.",
                "Steam vents release with the regularity of something that has been doing this for a very long time.",
                "The ash underfoot is so fine it behaves more like water than earth.",
            ],
            InvestigatePartB =
            [
                "Everything here is communicating that you should not be here.",
                "You agree with the smell. You leave.",
                "You feel very strongly that this is not your biome.",
                "You look at the shapes and believe all three words.",
                "You stand near one briefly. This is as near as you need to be.",
                "You move carefully, and it rises around your feet like a pale, unhurried judgment.",
            ],
            ResourceYields =
            [
                new() { ResourceId = "Ash",    Probability = 0.45f },
                new() { ResourceId = "Sulfur", Probability = 0.35f },
                new() { ResourceId = "Stone",  Probability = 0.40f },
                new() { ResourceId = "Ore",    Probability = 0.25f },
                new() { ResourceId = "Coal",   Probability = 0.20f },
                new() { ResourceId = "Flint",  Probability = 0.10f },
            ],
        },
    ];

    private static readonly Dictionary<BiomeType, BiomeDefinition> _byType =
        _definitions.ToDictionary(d => d.Type);

    public IReadOnlyList<BiomeDefinition> All => _definitions;

    public BiomeDefinition? GetByType(BiomeType type) =>
        _byType.TryGetValue(type, out var def) ? def : null;
}
