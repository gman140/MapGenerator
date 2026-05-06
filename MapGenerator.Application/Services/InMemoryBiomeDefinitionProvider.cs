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
            ],
            InvestigatePartA =
            [
                "The waves are relentless and patient.",
                "The water goes dark very quickly.",
                "The sea is doing what the sea does, which is everything and nothing.",
            ],
            InvestigatePartB =
            [
                "They have been doing this forever. They will do this forever after you. You are a brief event.",
                "You don't know what lives below. The water knows you don't know.",
                "You are here very briefly and it has no opinion about this.",
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
            ],
            InvestigatePartB =
            [
                "It reflects without comment.",
                "Nothing leaves and nothing arrives. It has been this way for a long time.",
                "You find this somehow more unsettling than open water.",
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
            ],
            InvestigatePartA =
            [
                "The water is clear enough to see every pebble on the bottom.",
                "Sunlight bends through the water and makes patterns on the sandy floor.",
                "Small fish move in purposeful formations around your feet.",
            ],
            InvestigatePartB =
            [
                "Small fish investigate your feet. One nudges deliberately. You have been studied.",
                "They almost look like writing. You squint. You're still not sure.",
                "One stops and faces you directly for an unreasonable amount of time.",
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
            ],
            InvestigatePartA =
            [
                "The sand holds the warmth of the day.",
                "The surf pulls at the sand with each wave, making the ground briefly liquid.",
                "The tide has left a line of shells and kelp and small mysterious things.",
            ],
            InvestigatePartB =
            [
                "A crab assesses you from a respectful distance and decides you are probably not a threat. It is generous.",
                "You sink a little with each wave. You stay.",
                "You examine one of the small mysterious things. It examines you back with its many small eyes.",
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
            ],
            InvestigatePartA =
            [
                "The current is patient and consistent.",
                "The water is cold and fast and clear.",
                "The river bends away ahead of you with great purpose.",
            ],
            InvestigatePartB =
            [
                "It does not care where you want to go — only where it's going. You consider letting it decide.",
                "Something silver passes beneath — too quick to be sure of.",
                "It knows where it is going and has known for a long time.",
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
            ],
            InvestigatePartA =
            [
                "Something bubbles from the mud nearby — steady, purposeful.",
                "The water here is the color of strong tea.",
                "The air here is complicated.",
            ],
            InvestigatePartB =
            [
                "You do not ask what it is reporting.",
                "It smells of things breaking down into other things. This is the natural order. You still find it unsettling.",
                "You breathe carefully through your nose and make a series of decisions.",
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
            ],
            InvestigatePartA =
            [
                "The ground makes complicated promises about being solid.",
                "A heron stands ahead of you, still as a carved thing.",
                "The water and land here have come to an arrangement that benefits neither.",
            ],
            InvestigatePartB =
            [
                "You test each step. The marsh tests back.",
                "It is watching for something with absolute focus. You stand still out of competitive instinct. It wins.",
                "You navigate it carefully and feel a modest but genuine pride at the other side.",
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
            ],
            InvestigatePartB =
            [
                "You are briefly part of the breath.",
                "You sit in the grass and listen to the negotiation.",
                "You stand in it for a while. It does not ask anything of you.",
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
            ],
            InvestigatePartB =
            [
                "You feel both significant and appropriately small. This is the correct ratio.",
                "It arrives at you carrying the smell of somewhere you haven't been yet.",
                "You look up. You stay looking up for a while.",
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
            ],
            InvestigatePartB =
            [
                "In the distance something stands very still watching you. When you look directly it moves.",
                "Somewhere a bird calls once. Nothing answers.",
                "When you look directly at it, it moves. When you look away, it stills again.",
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
            ],
            InvestigatePartA =
            [
                "Light filters through the canopy in shifting columns.",
                "A branch moves with no wind behind it.",
                "The forest is doing what forests do — being enormous and absorbed in its own concerns.",
            ],
            InvestigatePartB =
            [
                "The forest is patient and absorbed in its own concerns. You are a brief parenthesis in something longer.",
                "Then another. Then the forest settles and is very still and you walk more carefully.",
                "You are a brief event here. The forest does not mind.",
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
            ],
            InvestigatePartA =
            [
                "Everything here is growing with total commitment.",
                "Something watches you from the canopy with large yellow eyes.",
                "The undergrowth presses in from every direction.",
            ],
            InvestigatePartB =
            [
                "The trees compete for the light overhead. The undergrowth competes for what's left. You are briefly disputed territory.",
                "You look up and lose it in the green. When you look down it has moved. When you look up it is where it started.",
                "You push through it carefully, borrowing space.",
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
            ],
            InvestigatePartA =
            [
                "The heat is physical. It presses against you like a hand.",
                "Nothing moves here that doesn't have to.",
                "The silence and the heat are the same weight.",
            ],
            InvestigatePartB =
            [
                "The silence presses the same way. They weigh about the same.",
                "You move carefully, out of respect.",
                "You stand still for a moment and become briefly part of the landscape.",
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
            ],
            InvestigatePartA =
            [
                "The rock underfoot is very old and very sure of itself.",
                "The wind at this elevation is a different thing than the wind below.",
                "The view from here is significant.",
            ],
            InvestigatePartB =
            [
                "It makes no accommodations for you. You find your own way across it.",
                "Sharper, colder, more certain. It knows where it's been.",
                "You look at it for a while and feel correctly small.",
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
            ],
            InvestigatePartA =
            [
                "The permafrost is under your boots, ancient and silent and unimpressed.",
                "The wind on the tundra has been traveling for a long time.",
                "The landscape here is spare and specific.",
            ],
            InvestigatePartB =
            [
                "You are a warm soft thing walking briefly over something cold and very old.",
                "It still has somewhere to be. It passes through you on its way.",
                "It does not perform for visitors.",
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
            ],
            InvestigatePartA =
            [
                "Your breath makes small clouds that vanish quickly.",
                "The snow absorbs sound completely.",
                "The world here is white and still and very deliberate about it.",
            ],
            InvestigatePartB =
            [
                "You name one of the clouds. This is the most important thing you'll do today.",
                "Each step falls into silence. You have the impression of a very large room that wants quiet.",
                "You move through it carefully, feeling like an intrusion.",
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
            ],
            InvestigatePartA =
            [
                "The ice groans somewhere beneath your feet — a sound like the world settling.",
                "The ice at this depth is a blue that doesn't exist in most languages.",
                "The glacier is moving. Imperceptibly. Every moment.",
            ],
            InvestigatePartB =
            [
                "The glacier is moving, imperceptibly, every moment. You are moving with it.",
                "It is a color that existed before anyone named colors.",
                "You are moving with it whether you mean to or not.",
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
            ],
            InvestigatePartA =
            [
                "The ground radiates an uncomfortable heat.",
                "The air here smells of sulfur and something older.",
                "The rock underfoot is dark and sharp and recent by geological standards.",
            ],
            InvestigatePartB =
            [
                "Everything here is communicating that you should not be here.",
                "You agree with the smell. You leave.",
                "You feel very strongly that this is not your biome.",
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
