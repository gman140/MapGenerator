using MapGenerator.Fishing.Interfaces;
using MapGenerator.Fishing.Models;

namespace MapGenerator.Fishing.Services;

public class InMemoryFishDefinitionProvider : IFishDefinitionProvider
{
    private readonly IReadOnlyList<FishDefinition> _all;
    private readonly Dictionary<string, FishDefinition> _byId;

    public InMemoryFishDefinitionProvider()
    {
        _all  = Build();
        _byId = _all.ToDictionary(f => f.Id);
    }

    public IReadOnlyList<FishDefinition> All => _all;

    public FishDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var f) ? f : null;

    public IReadOnlyList<FishDefinition> GetPool(string[] fishIds, int poleTier) =>
        fishIds.Select(id => _byId.TryGetValue(id, out var f) ? f : null)
               .Where(f => f != null && f.PoleTier <= poleTier)
               .Select(f => f!)
               .ToList();

    private static IReadOnlyList<FishDefinition> Build() =>
    [
        // ── Tier 0 — FishingHole, TidePools, ReedBeds ────────────────────────

        new() {
            Id = "RiverTrout", Name = "River Trout", PoleTier = 0, RarityWeight = 3f, FishCategory = "Freshwater",
            Description = "A common trout that has spent its life being slightly too fast for everything that hunts it.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 5, StrikeWindowMs = 750, MaxMissedStrikes = 2,
            TensionDrainRate = 0.06, ReelResistance = 0.08, BurstChancePerSec = 0.10, BurstStrength = 0.12, ReelRate = 0.16,
        },
        new() {
            Id = "FreshwaterPerch", Name = "Freshwater Perch", PoleTier = 0, RarityWeight = 3f, FishCategory = "Freshwater",
            Description = "A perch. It would prefer not to be here, but here it is.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 800, MaxMissedStrikes = 2,
            TensionDrainRate = 0.05, ReelResistance = 0.07, BurstChancePerSec = 0.08, BurstStrength = 0.10, ReelRate = 0.17,
        },
        new() {
            Id = "Catfish", Name = "Catfish", PoleTier = 0, RarityWeight = 2f, FishCategory = "Freshwater",
            Description = "Bottom-dweller. Scavenges. Incredibly stubborn when pulled upward.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 8, StrikeWindowMs = 650, MaxMissedStrikes = 2,
            TensionDrainRate = 0.10, ReelResistance = 0.14, BurstChancePerSec = 0.12, BurstStrength = 0.16, ReelRate = 0.13,
        },
        new() {
            Id = "RiverEel", Name = "River Eel", PoleTier = 0, RarityWeight = 1.5f, FishCategory = "Freshwater",
            Description = "Long, sinuous, and deeply unhappy about the line situation.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 9, StrikeWindowMs = 550, MaxMissedStrikes = 1,
            TensionDrainRate = 0.12, ReelResistance = 0.15, BurstChancePerSec = 0.18, BurstStrength = 0.20, ReelRate = 0.11,
        },
        new() {
            Id = "FreshwaterCrab", Name = "Freshwater Crab", PoleTier = 0, RarityWeight = 1.5f, FishCategory = "Freshwater",
            Description = "It holds on. That is its entire strategy. It has been working.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 4, StrikeWindowMs = 900, MaxMissedStrikes = 3,
            TensionDrainRate = 0.04, ReelResistance = 0.06, BurstChancePerSec = 0.05, BurstStrength = 0.08, ReelRate = 0.20,
        },
        new() {
            Id = "GiantCarp", Name = "Giant Carp", PoleTier = 0, RarityWeight = 0.4f, FishCategory = "Freshwater",
            Description = "Enormous. Slow. It has survived longer than most things in this river and it knows it.",
            WaitTimeMinSec = 6, WaitTimeMaxSec = 12, StrikeWindowMs = 500, MaxMissedStrikes = 1,
            TensionDrainRate = 0.16, ReelResistance = 0.20, BurstChancePerSec = 0.20, BurstStrength = 0.28, ReelRate = 0.10,
        },
        new() {
            Id = "Sandfish", Name = "Sandfish", PoleTier = 0, RarityWeight = 2.5f, FishCategory = "Saltwater",
            Description = "Pale and flat. Lives near the sand and has opinions about being lifted from it.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 5, StrikeWindowMs = 750, MaxMissedStrikes = 2,
            TensionDrainRate = 0.06, ReelResistance = 0.09, BurstChancePerSec = 0.09, BurstStrength = 0.11, ReelRate = 0.16,
        },
        new() {
            Id = "TideMullet", Name = "Tide Mullet", PoleTier = 0, RarityWeight = 3f, FishCategory = "Saltwater",
            Description = "Appears at dusk and dawn. Skittish. Vanishes at the slightest disturbance.",
            WaitTimeMinSec = 1, WaitTimeMaxSec = 4, StrikeWindowMs = 600, MaxMissedStrikes = 1,
            TensionDrainRate = 0.08, ReelResistance = 0.10, BurstChancePerSec = 0.14, BurstStrength = 0.14, ReelRate = 0.15,
        },
        new() {
            Id = "ShoreCrab", Name = "Shore Crab", PoleTier = 0, RarityWeight = 2f, FishCategory = "Saltwater",
            Description = "Has never lost a staring contest. Has never entered one intentionally.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 4, StrikeWindowMs = 900, MaxMissedStrikes = 3,
            TensionDrainRate = 0.03, ReelResistance = 0.05, BurstChancePerSec = 0.04, BurstStrength = 0.06, ReelRate = 0.22,
        },
        new() {
            Id = "Flounder", Name = "Flounder", PoleTier = 0, RarityWeight = 2f, FishCategory = "Saltwater",
            Description = "Both eyes on one side of its head. It adapted. It has concerns.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 700, MaxMissedStrikes = 2,
            TensionDrainRate = 0.07, ReelResistance = 0.09, BurstChancePerSec = 0.08, BurstStrength = 0.12, ReelRate = 0.15,
        },
        new() {
            Id = "Seahorse", Name = "Seahorse", PoleTier = 0, RarityWeight = 0.5f, FishCategory = "Saltwater",
            Description = "Tiny. Technically a fish. Deeply unusual.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 8, StrikeWindowMs = 850, MaxMissedStrikes = 2,
            TensionDrainRate = 0.03, ReelResistance = 0.04, BurstChancePerSec = 0.03, BurstStrength = 0.05, ReelRate = 0.25,
        },
        new() {
            Id = "SwampEel", Name = "Swamp Eel", PoleTier = 0, RarityWeight = 2f, FishCategory = "Freshwater",
            Description = "Grey-brown. Slick. Absolutely does not want to be above water.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 7, StrikeWindowMs = 580, MaxMissedStrikes = 1,
            TensionDrainRate = 0.11, ReelResistance = 0.14, BurstChancePerSec = 0.16, BurstStrength = 0.18, ReelRate = 0.12,
        },
        new() {
            Id = "Mudfish", Name = "Mudfish", PoleTier = 0, RarityWeight = 2.5f, FishCategory = "Freshwater",
            Description = "Tastes of mud. Smells of mud. Is made of mud to a concerning degree.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 5, StrikeWindowMs = 780, MaxMissedStrikes = 2,
            TensionDrainRate = 0.07, ReelResistance = 0.10, BurstChancePerSec = 0.08, BurstStrength = 0.12, ReelRate = 0.15,
        },
        new() {
            Id = "MarshCrab", Name = "Marsh Crab", PoleTier = 0, RarityWeight = 1.5f, FishCategory = "Freshwater",
            Description = "Small. Aggressive. Its claws are disproportionate to its understanding of the situation.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 4, StrikeWindowMs = 820, MaxMissedStrikes = 3,
            TensionDrainRate = 0.04, ReelResistance = 0.06, BurstChancePerSec = 0.06, BurstStrength = 0.09, ReelRate = 0.20,
        },

        // ── Tier 0 chain material ─────────────────────────────────────────────
        new() {
            Id = "RiverScale", Name = "River Scale", PoleTier = 0, RarityWeight = 0.3f,
            Description = "A large iridescent scale shed by something in the deep current. Strong enough to reinforce a fishing rod.",
            IsChainMaterial = true,
            WaitTimeMinSec = 8, WaitTimeMaxSec = 14, StrikeWindowMs = 450, MaxMissedStrikes = 1,
            TensionDrainRate = 0.14, ReelResistance = 0.18, BurstChancePerSec = 0.22, BurstStrength = 0.24, ReelRate = 0.09,
        },

        // ── Tier 1 — Waterfall, CoralReef, MirrorLake ────────────────────────

        new() {
            Id = "MountainTrout", Name = "Mountain Trout", PoleTier = 1, RarityWeight = 2.5f, FishCategory = "Freshwater",
            Description = "Lean and fast. Lives in fast-moving cold water and has the physique to show it.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 680, MaxMissedStrikes = 2,
            TensionDrainRate = 0.09, ReelResistance = 0.12, BurstChancePerSec = 0.14, BurstStrength = 0.16, ReelRate = 0.14,
        },
        new() {
            Id = "Salmon", Name = "Salmon", PoleTier = 1, RarityWeight = 2f, FishCategory = "Freshwater",
            Description = "A determined fish. It swam upstream to get here and it intends to stay.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 7, StrikeWindowMs = 600, MaxMissedStrikes = 2,
            TensionDrainRate = 0.12, ReelResistance = 0.16, BurstChancePerSec = 0.18, BurstStrength = 0.22, ReelRate = 0.12,
        },
        new() {
            Id = "GlacialChar", Name = "Glacial Char", PoleTier = 1, RarityWeight = 1.5f, FishCategory = "Cold",
            Description = "Pale orange. Cold-adapted. Looks like it was carved from ice with a vague fish intention.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 9, StrikeWindowMs = 620, MaxMissedStrikes = 1,
            TensionDrainRate = 0.10, ReelResistance = 0.14, BurstChancePerSec = 0.16, BurstStrength = 0.20, ReelRate = 0.13,
        },
        new() {
            Id = "GoldenTrout", Name = "Golden Trout", PoleTier = 1, RarityWeight = 0.4f, FishCategory = "Freshwater",
            Description = "Gold-scaled and rare. It knows it is rare. It behaves accordingly.",
            WaitTimeMinSec = 7, WaitTimeMaxSec = 12, StrikeWindowMs = 480, MaxMissedStrikes = 1,
            TensionDrainRate = 0.15, ReelResistance = 0.20, BurstChancePerSec = 0.22, BurstStrength = 0.28, ReelRate = 0.09,
        },
        new() {
            Id = "Clownfish", Name = "Clownfish", PoleTier = 1, RarityWeight = 2f, FishCategory = "Saltwater",
            Description = "Bright orange and white. Lives among stinging things. Has negotiated a peace.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 5, StrikeWindowMs = 750, MaxMissedStrikes = 2,
            TensionDrainRate = 0.06, ReelResistance = 0.08, BurstChancePerSec = 0.08, BurstStrength = 0.10, ReelRate = 0.18,
        },
        new() {
            Id = "Parrotfish", Name = "Parrotfish", PoleTier = 1, RarityWeight = 1.5f, FishCategory = "Saltwater",
            Description = "Beak-like mouth. Colorful. Eats coral and then goes about its day.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 700, MaxMissedStrikes = 2,
            TensionDrainRate = 0.08, ReelResistance = 0.11, BurstChancePerSec = 0.10, BurstStrength = 0.14, ReelRate = 0.15,
        },
        new() {
            Id = "Triggerfish", Name = "Triggerfish", PoleTier = 1, RarityWeight = 1.5f, FishCategory = "Saltwater",
            Description = "Has a locking spine. Territorial. Has bitten divers. Is not apologetic about it.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 7, StrikeWindowMs = 620, MaxMissedStrikes = 1,
            TensionDrainRate = 0.11, ReelResistance = 0.15, BurstChancePerSec = 0.15, BurstStrength = 0.20, ReelRate = 0.12,
        },
        new() {
            Id = "MorayEel", Name = "Moray Eel", PoleTier = 1, RarityWeight = 1f, FishCategory = "Saltwater",
            Description = "Long. Toothed. Hides in rock crevices and has strong opinions about being removed from them.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 9, StrikeWindowMs = 540, MaxMissedStrikes = 1,
            TensionDrainRate = 0.14, ReelResistance = 0.18, BurstChancePerSec = 0.20, BurstStrength = 0.25, ReelRate = 0.10,
        },
        new() {
            Id = "Lionfish", Name = "Lionfish", PoleTier = 1, RarityWeight = 0.5f, FishCategory = "Saltwater",
            Description = "Beautiful. Venomous. Invasive. Aware that it is unwelcome and indifferent to this.",
            WaitTimeMinSec = 6, WaitTimeMaxSec = 10, StrikeWindowMs = 500, MaxMissedStrikes = 1,
            TensionDrainRate = 0.14, ReelResistance = 0.18, BurstChancePerSec = 0.20, BurstStrength = 0.26, ReelRate = 0.10,
        },
        new() {
            Id = "MirrorCarp", Name = "Mirror Carp", PoleTier = 1, RarityWeight = 2f, FishCategory = "Freshwater",
            Description = "Scaled unevenly, as if designed in a hurry. Slow and powerful.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 8, StrikeWindowMs = 620, MaxMissedStrikes = 2,
            TensionDrainRate = 0.12, ReelResistance = 0.16, BurstChancePerSec = 0.14, BurstStrength = 0.20, ReelRate = 0.12,
        },
        new() {
            Id = "LakeBass", Name = "Lake Bass", PoleTier = 1, RarityWeight = 2.5f, FishCategory = "Freshwater",
            Description = "Medium. Reliable. Exactly what you expect from a lake fish in a lake.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 700, MaxMissedStrikes = 2,
            TensionDrainRate = 0.08, ReelResistance = 0.11, BurstChancePerSec = 0.10, BurstStrength = 0.14, ReelRate = 0.15,
        },
        new() {
            Id = "Moonfish", Name = "Moonfish", PoleTier = 1, RarityWeight = 0.6f, FishCategory = "Freshwater",
            Description = "Silver-white and round. Only bites in the hours around dusk. Patient.",
            WaitTimeMinSec = 7, WaitTimeMaxSec = 13, StrikeWindowMs = 560, MaxMissedStrikes = 1,
            TensionDrainRate = 0.11, ReelResistance = 0.15, BurstChancePerSec = 0.17, BurstStrength = 0.22, ReelRate = 0.12,
        },
        new() {
            Id = "LegendaryMoonfish", Name = "Legendary Moonfish", PoleTier = 1, RarityWeight = 0.08f, FishCategory = "Freshwater",
            Description = "Silver as still water under a full moon. Ancient. Possibly aware of you.",
            WaitTimeMinSec = 12, WaitTimeMaxSec = 20, StrikeWindowMs = 380, MaxMissedStrikes = 1,
            TensionDrainRate = 0.20, ReelResistance = 0.26, BurstChancePerSec = 0.28, BurstStrength = 0.34, ReelRate = 0.07,
        },

        // ── Tier 1 chain material ─────────────────────────────────────────────
        new() {
            Id = "DeepwaterPearl", Name = "Deepwater Pearl", PoleTier = 1, RarityWeight = 0.25f,
            Description = "A perfect sphere, pale blue. Formed in the cold dark below where light reaches. Useful for delicate lure work.",
            IsChainMaterial = true,
            WaitTimeMinSec = 10, WaitTimeMaxSec = 16, StrikeWindowMs = 420, MaxMissedStrikes = 1,
            TensionDrainRate = 0.16, ReelResistance = 0.22, BurstChancePerSec = 0.25, BurstStrength = 0.28, ReelRate = 0.08,
        },

        // ── Tier 2 — KelpForest, Shipwreck, HotSpring ────────────────────────

        new() {
            Id = "KelpGrouper", Name = "Kelp Grouper", PoleTier = 2, RarityWeight = 2f, FishCategory = "Saltwater",
            Description = "Mottled brown-green. Ambushes small things from inside the kelp. Resists ambush in turn.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 8, StrikeWindowMs = 650, MaxMissedStrikes = 2,
            TensionDrainRate = 0.10, ReelResistance = 0.14, BurstChancePerSec = 0.14, BurstStrength = 0.18, ReelRate = 0.13,
        },
        new() {
            Id = "Rockfish", Name = "Rockfish", PoleTier = 2, RarityWeight = 2f, FishCategory = "Saltwater",
            Description = "Spiny. Old. Some rockfish live centuries. This one might be among them.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 9, StrikeWindowMs = 630, MaxMissedStrikes = 2,
            TensionDrainRate = 0.11, ReelResistance = 0.15, BurstChancePerSec = 0.13, BurstStrength = 0.18, ReelRate = 0.13,
        },
        new() {
            Id = "Octopus", Name = "Octopus", PoleTier = 2, RarityWeight = 1f, FishCategory = "Saltwater",
            Description = "Not technically a fish. Does not care. Has eight arms and uses all of them to resist.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 7, StrikeWindowMs = 700, MaxMissedStrikes = 2,
            TensionDrainRate = 0.13, ReelResistance = 0.17, BurstChancePerSec = 0.20, BurstStrength = 0.24, ReelRate = 0.11,
        },
        new() {
            Id = "SeaBass", Name = "Sea Bass", PoleTier = 2, RarityWeight = 2.5f, FishCategory = "Saltwater",
            Description = "Common in restaurants and ocean alike. Neither place suits it particularly.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 680, MaxMissedStrikes = 2,
            TensionDrainRate = 0.09, ReelResistance = 0.12, BurstChancePerSec = 0.12, BurstStrength = 0.16, ReelRate = 0.14,
        },
        new() {
            Id = "GiantSquid", Name = "Giant Squid", PoleTier = 2, RarityWeight = 0.2f, FishCategory = "Deep",
            Description = "Large enough to be uncomfortable about. Has never been fully documented in the wild. You caught one.",
            WaitTimeMinSec = 10, WaitTimeMaxSec = 18, StrikeWindowMs = 400, MaxMissedStrikes = 1,
            TensionDrainRate = 0.22, ReelResistance = 0.28, BurstChancePerSec = 0.30, BurstStrength = 0.36, ReelRate = 0.07,
        },
        new() {
            Id = "Anglerfish", Name = "Anglerfish", PoleTier = 2, RarityWeight = 0.6f, FishCategory = "Deep",
            Description = "Has its own light source. Uses it to lure things closer in the dark. The irony of being lured is not lost on it.",
            WaitTimeMinSec = 7, WaitTimeMaxSec = 12, StrikeWindowMs = 500, MaxMissedStrikes = 1,
            TensionDrainRate = 0.16, ReelResistance = 0.21, BurstChancePerSec = 0.24, BurstStrength = 0.28, ReelRate = 0.09,
        },
        new() {
            Id = "Ghostfish", Name = "Ghostfish", PoleTier = 2, RarityWeight = 0.8f, FishCategory = "Deep",
            Description = "Translucent. Its organs are visible from the outside. It finds your concern unnecessary.",
            WaitTimeMinSec = 6, WaitTimeMaxSec = 11, StrikeWindowMs = 540, MaxMissedStrikes = 1,
            TensionDrainRate = 0.13, ReelResistance = 0.17, BurstChancePerSec = 0.18, BurstStrength = 0.23, ReelRate = 0.11,
        },
        new() {
            Id = "AbyssalEel", Name = "Abyssal Eel", PoleTier = 2, RarityWeight = 0.5f, FishCategory = "Deep",
            Description = "From very deep water. Uncomfortable in light. Extremely uncomfortable on a hook.",
            WaitTimeMinSec = 8, WaitTimeMaxSec = 13, StrikeWindowMs = 480, MaxMissedStrikes = 1,
            TensionDrainRate = 0.17, ReelResistance = 0.23, BurstChancePerSec = 0.26, BurstStrength = 0.30, ReelRate = 0.09,
        },
        new() {
            Id = "PaleCavefish", Name = "Pale Cavefish", PoleTier = 2, RarityWeight = 1.5f, FishCategory = "Deep",
            Description = "No eyes. No pigment. Navigates entirely by pressure sense. Being on a hook is novel for it.",
            WaitTimeMinSec = 4, WaitTimeMaxSec = 8, StrikeWindowMs = 620, MaxMissedStrikes = 2,
            TensionDrainRate = 0.09, ReelResistance = 0.12, BurstChancePerSec = 0.12, BurstStrength = 0.16, ReelRate = 0.14,
        },
        new() {
            Id = "VolcanicEel", Name = "Volcanic Eel", PoleTier = 2, RarityWeight = 1f, FishCategory = "Deep",
            Description = "Adapted to near-boiling water. Is now at a normal temperature and is furious about it.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 9, StrikeWindowMs = 560, MaxMissedStrikes = 1,
            TensionDrainRate = 0.14, ReelResistance = 0.19, BurstChancePerSec = 0.22, BurstStrength = 0.26, ReelRate = 0.10,
        },
        new() {
            Id = "ThermophilicShrimp", Name = "Thermophilic Shrimp", PoleTier = 2, RarityWeight = 0.6f, FishCategory = "Deep",
            Description = "Pink. Tiny. Thrives in heat that kills other things. Remarkable and small.",
            WaitTimeMinSec = 2, WaitTimeMaxSec = 5, StrikeWindowMs = 860, MaxMissedStrikes = 3,
            TensionDrainRate = 0.03, ReelResistance = 0.04, BurstChancePerSec = 0.04, BurstStrength = 0.05, ReelRate = 0.24,
        },

        // ── Tier 2 chain material ─────────────────────────────────────────────
        new() {
            Id = "CoralChip", Name = "Coral Chip", PoleTier = 2, RarityWeight = 0.22f,
            Description = "A fragment of living coral, dense and iridescent. Carries luck within it, according to those who know.",
            IsChainMaterial = true,
            WaitTimeMinSec = 11, WaitTimeMaxSec = 17, StrikeWindowMs = 400, MaxMissedStrikes = 1,
            TensionDrainRate = 0.18, ReelResistance = 0.24, BurstChancePerSec = 0.28, BurstStrength = 0.32, ReelRate = 0.07,
        },

        // ── Tier 3 — BioluminescentBloom ─────────────────────────────────────

        new() {
            Id = "BioluminescentJellyfish", Name = "Bioluminescent Jellyfish", PoleTier = 3, RarityWeight = 1.5f, FishCategory = "Deep",
            Description = "Pulses blue in the dark. Does not sting through fishing line. Does not appreciate being tested.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 10, StrikeWindowMs = 600, MaxMissedStrikes = 2,
            TensionDrainRate = 0.10, ReelResistance = 0.13, BurstChancePerSec = 0.14, BurstStrength = 0.17, ReelRate = 0.14,
        },
        new() {
            Id = "AbyssalAnglerfish", Name = "Abyssal Anglerfish", PoleTier = 3, RarityWeight = 0.7f, FishCategory = "Deep",
            Description = "Larger than its shallow-water cousin. Its lure is brighter. Its patience is longer.",
            WaitTimeMinSec = 9, WaitTimeMaxSec = 15, StrikeWindowMs = 460, MaxMissedStrikes = 1,
            TensionDrainRate = 0.19, ReelResistance = 0.25, BurstChancePerSec = 0.28, BurstStrength = 0.33, ReelRate = 0.08,
        },
        new() {
            Id = "GlowingSquid", Name = "Glowing Squid", PoleTier = 3, RarityWeight = 1f, FishCategory = "Deep",
            Description = "Emits cold blue light as it retreats. Leaves a faintly glowing trail in the water.",
            WaitTimeMinSec = 6, WaitTimeMaxSec = 11, StrikeWindowMs = 530, MaxMissedStrikes = 1,
            TensionDrainRate = 0.15, ReelResistance = 0.20, BurstChancePerSec = 0.22, BurstStrength = 0.27, ReelRate = 0.10,
        },

        // ── Tier 3 chain material ─────────────────────────────────────────────
        new() {
            Id = "AncientLure", Name = "Ancient Lure", PoleTier = 3, RarityWeight = 0.18f,
            Description = "A hook carved from bone and wrapped in deep-sea fiber. Older than any record. Draws the rarest fish.",
            IsChainMaterial = true,
            WaitTimeMinSec = 13, WaitTimeMaxSec = 20, StrikeWindowMs = 380, MaxMissedStrikes = 1,
            TensionDrainRate = 0.21, ReelResistance = 0.28, BurstChancePerSec = 0.32, BurstStrength = 0.36, ReelRate = 0.07,
        },

        // ── Tier 4 — IcyCavern, FrozenShrine ─────────────────────────────────

        new() {
            Id = "IceShrimp", Name = "Ice Shrimp", PoleTier = 4, RarityWeight = 2f, FishCategory = "Cold",
            Description = "Translucent and small. Survives in near-frozen water by not doing much.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 820, MaxMissedStrikes = 3,
            TensionDrainRate = 0.04, ReelResistance = 0.06, BurstChancePerSec = 0.05, BurstStrength = 0.07, ReelRate = 0.22,
        },
        new() {
            Id = "CrystalFish", Name = "Crystal Fish", PoleTier = 4, RarityWeight = 1f, FishCategory = "Cold",
            Description = "Semi-transparent. Scales catch and refract light like glass. Beautiful and cold.",
            WaitTimeMinSec = 6, WaitTimeMaxSec = 11, StrikeWindowMs = 560, MaxMissedStrikes = 1,
            TensionDrainRate = 0.13, ReelResistance = 0.17, BurstChancePerSec = 0.18, BurstStrength = 0.23, ReelRate = 0.11,
        },
        new() {
            Id = "FrozenEel", Name = "Frozen Eel", PoleTier = 4, RarityWeight = 0.8f, FishCategory = "Cold",
            Description = "White as glacier ice. Moves slowly. Fights slowly. Fights for a very long time.",
            WaitTimeMinSec = 8, WaitTimeMaxSec = 14, StrikeWindowMs = 500, MaxMissedStrikes = 1,
            TensionDrainRate = 0.17, ReelResistance = 0.23, BurstChancePerSec = 0.22, BurstStrength = 0.28, ReelRate = 0.09,
        },
        new() {
            Id = "ArcticTrout", Name = "Arctic Trout", PoleTier = 4, RarityWeight = 1.5f, FishCategory = "Cold",
            Description = "Silver-blue and lean. Lives its entire life near the freezing point. Has adapted completely.",
            WaitTimeMinSec = 5, WaitTimeMaxSec = 9, StrikeWindowMs = 600, MaxMissedStrikes = 2,
            TensionDrainRate = 0.11, ReelResistance = 0.15, BurstChancePerSec = 0.16, BurstStrength = 0.20, ReelRate = 0.13,
        },
        new() {
            Id = "PolarStar", Name = "Polar Star", PoleTier = 4, RarityWeight = 0.5f, FishCategory = "Cold",
            Description = "A starfish from the arctic shelf. White with faint blue veins. Holds on to things with remarkable commitment.",
            WaitTimeMinSec = 3, WaitTimeMaxSec = 6, StrikeWindowMs = 900, MaxMissedStrikes = 3,
            TensionDrainRate = 0.03, ReelResistance = 0.04, BurstChancePerSec = 0.03, BurstStrength = 0.05, ReelRate = 0.26,
        },
        new() {
            Id = "AncientFish", Name = "Ancient Fish", PoleTier = 4, RarityWeight = 0.12f, FishCategory = "Cold",
            Description = "A species thought extinct. Unchanged for fifty million years. It has seen things. It does not share them.",
            WaitTimeMinSec = 14, WaitTimeMaxSec = 22, StrikeWindowMs = 350, MaxMissedStrikes = 1,
            TensionDrainRate = 0.24, ReelResistance = 0.30, BurstChancePerSec = 0.34, BurstStrength = 0.40, ReelRate = 0.06,
        },

        // ── Tier 4 chain material ─────────────────────────────────────────────
        new() {
            Id = "GlacierShard", Name = "Glacier Shard", PoleTier = 4, RarityWeight = 0.15f,
            Description = "A fragment of ancient ice containing something that moved once, long ago. Perfect for cutting through frozen water.",
            IsChainMaterial = true,
            WaitTimeMinSec = 15, WaitTimeMaxSec = 24, StrikeWindowMs = 340, MaxMissedStrikes = 1,
            TensionDrainRate = 0.24, ReelResistance = 0.32, BurstChancePerSec = 0.36, BurstStrength = 0.42, ReelRate = 0.06,
        },
    ];
}
