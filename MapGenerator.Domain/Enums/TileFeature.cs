namespace MapGenerator.Domain.Enums;

public enum TileFeature
{
    None = 0,

    // Forest & nature
    SpookyWoods   = 1,
    MushroomGrove = 2,
    AncientGlade  = 3,
    FernDell      = 4,
    BramblePatch  = 5,
    MushroomRing  = 6,
    TropicalGrove = 7,
    WildOrchard   = 8,

    // Ruins & man-made
    BurnedRuins      =  9,
    AncientShrine    = 10,
    RuinedTower      = 11,
    AbandonedFarm    = 12,
    ForgottenGrave   = 13,
    CrumbledFortress = 14,
    StoneCircle      = 15,
    LonelyWell       = 16,
    WitchsCottage    = 17,

    // Water & wet
    HotSpring = 18,
    TidePools = 19,
    ReedBeds  = 20,

    // Desert & dry
    SaltFlats  = 21,
    OasisGrove = 22,
    DeadForest = 23,
    QuickSand  = 24,

    // Mountain & cold
    CaveEntrance = 25,
    FrozenShrine = 26,
    IcyCavern    = 27,
}

public static class TileFeatureHelper
{
    public static string GetDisplayName(TileFeature feature, int q, int r)
    {
        var names = GetNames(feature);
        return names[Math.Abs(q * 7 + r * 13) % names.Length];
    }

    private static string[] GetNames(TileFeature feature) => feature switch
    {
        TileFeature.SpookyWoods      => ["Spooky Woods",       "Eerie Thicket",       "Haunted Grove",         "Shadowed Hollows"],
        TileFeature.MushroomGrove    => ["Mushroom Grove",      "Fungal Clearing",     "Mycelium Dell",         "Toadstool Hollow"],
        TileFeature.AncientGlade     => ["Ancient Glade",       "Old Growth Clearing", "Elder Grove",           "Forgotten Glade"],
        TileFeature.FernDell         => ["Fern Dell",           "Brackish Hollow",     "Frond-Draped Nook",     "Green Gully"],
        TileFeature.BramblePatch     => ["Brambly Thicket",     "Thorned Tangle",      "Bristled Hedge",        "Snaring Brambles"],
        TileFeature.MushroomRing     => ["Mushroom Ring",       "Fairy Ring",          "Circle of Caps",        "Fungal Crown"],
        TileFeature.TropicalGrove    => ["Tropical Grove",      "Palm-Fringed Shore",  "Lush Bower",            "Seaside Thicket"],
        TileFeature.WildOrchard      => ["Wild Orchard",        "Gnarled Fruit Trees", "Tangled Grove",         "Feral Garden"],
        TileFeature.BurnedRuins      => ["Burned Ruins",        "Scorched Settlement", "Charred Remains",       "Ash-Fallen Hamlet"],
        TileFeature.AncientShrine    => ["Ancient Shrine",      "Crumbled Altar",      "Moss-Covered Shrine",   "Weathered Offering Stone"],
        TileFeature.RuinedTower      => ["Ruined Tower",        "Crumbled Watchtower", "Collapsed Spire",       "Broken Keep"],
        TileFeature.AbandonedFarm    => ["Abandoned Farm",      "Overgrown Homestead", "Derelict Farmstead",    "Forgotten Fields"],
        TileFeature.ForgottenGrave   => ["Forgotten Grave",     "Unmarked Burial",     "Moss-Grown Cairn",      "Lonely Burial Mound"],
        TileFeature.CrumbledFortress => ["Crumbled Fortress",   "Ruined Battlement",   "Fallen Citadel",        "Ancient Rampart"],
        TileFeature.StoneCircle      => ["Stone Circle",        "Standing Stones",     "Ancient Ring",          "Megalith Ring"],
        TileFeature.LonelyWell       => ["Lonely Well",         "Crumbling Well",      "Stone-Rimmed Well",     "Forgotten Well"],
        TileFeature.WitchsCottage    => ["Witch's Cottage",     "Hermit's Hovel",      "Crumbled Hut",          "Smoke-Stained Den"],
        TileFeature.HotSpring        => ["Hot Spring",          "Bubbling Vent",       "Steaming Pool",         "Geothermal Basin"],
        TileFeature.TidePools        => ["Tide Pools",          "Rocky Shallows",      "Shell-Strewn Shore",    "Barnacled Flats"],
        TileFeature.ReedBeds         => ["Reed Beds",           "Cattail Marsh",       "Papyrus Stand",         "Waving Reeds"],
        TileFeature.SaltFlats        => ["Salt Flats",          "Bleached Expanse",    "Cracked Saltpan",       "White Wastes"],
        TileFeature.OasisGrove       => ["Oasis Grove",         "Desert Spring",       "Palmed Hollow",         "Hidden Waterhole"],
        TileFeature.DeadForest       => ["Dead Forest",         "Bleached Snags",      "Skeletal Timbers",      "Withered Copse"],
        TileFeature.QuickSand        => ["Quicksand",           "Treacherous Silt",    "Sucking Mud",           "Shifting Sand Trap"],
        TileFeature.CaveEntrance     => ["Cave Entrance",       "Dark Hollow",         "Rocky Cavern",          "Yawning Shaft"],
        TileFeature.FrozenShrine     => ["Frozen Shrine",       "Ice-Locked Altar",    "Glacial Monument",      "Frost-Covered Stele"],
        TileFeature.IcyCavern        => ["Icy Cavern",          "Frost Cave",          "Glacial Grotto",        "Blue-Ice Hollow"],
        _                            => [feature.ToString()],
    };
}
