using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryConsumableDefinitionProvider : IConsumableDefinitionProvider
{
    private static readonly ConsumableDefinition[] _definitions =
    [
        // ── Raw food — gatherable ─────────────────────────────────────────────
        new()
        {
            Id = "Berries", Name = "Berries", SatietyRestore = 12,
            UseMessages =
            [
                "You eat a handful of berries. Tart and a little too small, but welcome.",
                "The berries are sour enough to make you wince, but you finish them.",
                "Sweet, mostly. You eat them quickly before reconsidering.",
                "They taste like something a bird would fight you for. You eat them anyway.",
            ],
        },
        new()
        {
            Id = "Mushroom", Name = "Mushroom", SatietyRestore = 10,
            UseMessages =
            [
                "You eat the mushroom raw. Earthy, chewy, not entirely unpleasant.",
                "It tastes of the forest floor. You eat it anyway.",
                "The mushroom is bland but filling enough to matter.",
                "You eat it in two bites and try not to think too hard about where it grew.",
            ],
        },
        new()
        {
            Id = "WildCarrot", Name = "Wild Carrot", SatietyRestore = 8,
            UseMessages =
            [
                "Thin and pale and faintly bitter at the core. You eat it anyway.",
                "It tastes almost like a carrot. Almost.",
                "Fibrous and slightly sweet toward the tip. You finish it quickly.",
                "You expected worse. You are pleased, if quietly.",
            ],
        },
        new()
        {
            Id = "Acorn", Name = "Acorn", SatietyRestore = 5,
            UseMessages =
            [
                "Bitter. Chalky. You finish it with the resigned efficiency of someone who knows what hunger is.",
                "The bitterness is sharp and stays. You eat another anyway.",
                "You understand now why squirrels bury these. It is not because they are good.",
                "Not good. But here you are, eating it. The acorn does not judge.",
            ],
        },
        new()
        {
            Id = "WildGarlic", Name = "Wild Garlic", SatietyRestore = 6,
            UseMessages =
            [
                "Pungent. You will be announcing your presence to things downwind for some time.",
                "Aggressively flavored. You finish it. You will smell like a decision.",
                "You eat it raw. The garlic wins, but so do you, eventually.",
                "The taste is memorable in several directions at once.",
            ],
        },
        new()
        {
            Id = "BaobabFruit", Name = "Baobab Fruit", SatietyRestore = 14,
            UseMessages =
            [
                "Dry and chalky and strangely tart. It is not unpleasant once you commit to it.",
                "It has a sourness that builds. You finish it and feel it sitting properly in your stomach.",
                "From a tree that predates most things you know, this fruit is quietly satisfying.",
                "The texture is powdery but the taste is bright. An unlikely combination that works.",
            ],
        },
        new()
        {
            Id = "Yam", Name = "Yam", SatietyRestore = 12,
            UseMessages =
            [
                "Raw yam is a commitment. You have made it. It is starchy and dense.",
                "Dense and slightly astringent. You eat it with the focused determination it requires.",
                "It would be much better cooked. You know this. You eat it anyway.",
                "Heavy and filling and not particularly interested in being pleasant about it.",
            ],
        },
        new()
        {
            Id = "HeartOfPalm", Name = "Heart of Palm", SatietyRestore = 10,
            UseMessages =
            [
                "Mild to the point of apology. Tender, pale, and almost politely flavorless.",
                "It tastes of very little, very gently. You are grateful for the texture.",
                "Soft and fibrous and almost sweet. You eat it quietly and it asks nothing of you.",
                "From deep inside the palm, pale and mild. A peaceful thing to eat.",
            ],
        },
        new()
        {
            Id = "CattailRoot", Name = "Cattail Root", SatietyRestore = 10,
            UseMessages =
            [
                "Starchy and wet and pulled from the mud. It has its own kind of dignity.",
                "Dense, earthy, with the particular flavor of something that grew in standing water.",
                "You eat it raw. It is filling in a way that carries no pretense.",
                "The texture is fibrous and the taste is muddy in a neutral, inoffensive way.",
            ],
        },
        new()
        {
            Id = "RockLichen", Name = "Rock Lichen", SatietyRestore = 4,
            UseMessages =
            [
                "It tastes of stone and time and very little else. You chew it for longer than you expected.",
                "Dry and slightly bitter. You eat it with the grim acknowledgment of someone who has tried everything else.",
                "The lichen does not taste good. It is not sorry about this. Neither are you.",
                "You scrape it off a boulder and eat it. This is where you are now. It is fine.",
            ],
        },
        new()
        {
            Id = "PineNut", Name = "Pine Nut", SatietyRestore = 8,
            UseMessages =
            [
                "Small and oily and good. Worth the trouble entirely.",
                "You eat them one at a time and each one is quietly excellent.",
                "Rich and slightly resinous. They taste like the inside of a forest.",
                "They are small and plentiful and you eat them all in one slow handful.",
            ],
        },

        // ── Cooked food — no buff ─────────────────────────────────────────────
        new()
        {
            Id = "CookedFish", Name = "Cooked Fish", SatietyRestore = 40,
            UseMessages =
            [
                "The fish is charred on the outside and perfectly soft inside. You finish it in silence.",
                "It flakes apart at the touch. You eat every last bit.",
                "The smoke from the fire got into it just right. One of the better meals you've had out here.",
                "Hot, filling, and exactly what you needed. You feel considerably better.",
                "The fish is simple and good. You sit with the satisfaction of it for a moment.",
            ],
        },
        new()
        {
            Id = "CookedMushroom", Name = "Cooked Mushroom", SatietyRestore = 25,
            UseMessages =
            [
                "Cooking made them something else entirely — rich and soft and good.",
                "The heat brought out something you didn't expect. You eat them quickly.",
                "Warm and earthy and much better than raw. You finish the whole thing.",
                "They collapse against the heat into something genuinely satisfying.",
            ],
        },
        new()
        {
            Id = "CattailCakes", Name = "Cattail Cakes", SatietyRestore = 30,
            UseMessages =
            [
                "Earthy and dense and made with the kind of patience that shows in the result.",
                "They are modest cakes but they are cakes. You eat them with appropriate appreciation.",
                "The grain and root make a good combination. Simple and filling and honest.",
                "They taste of the marsh and the fire and not much else. This is more than enough.",
            ],
        },

        // ── Cooked food — with out-of-combat buffs ────────────────────────────
        new()
        {
            Id = "HerbTea", Name = "Herb Tea", SatietyRestore = 22,
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.50, Charges = 20 },
            UseMessages =
            [
                "The tea is bitter and herbal and good in the way that things that are good for you often aren't. You feel quick afterward.",
                "You drink it slowly. It clears something in your chest you hadn't noticed. Your movements feel lighter.",
                "Fragrant, faintly medicinal, and warm. Something in it sets your pace.",
                "It tastes like the forest smells. You find this agreeable, and find yourself moving with more purpose.",
            ],
        },
        new()
        {
            Id = "MushroomSoup", Name = "Mushroom Soup", SatietyRestore = 50,
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.60, Charges = 24 },
            UseMessages =
            [
                "The soup is hot and deep and exactly right. You eat it slowly to make it last. It settles into you and stays.",
                "Salt and mushroom and warmth. You drink the last of it from the bowl. Your hunger feels far away.",
                "You didn't expect something this good out here. You eat every drop, and feel it sustaining you.",
                "The broth is thick and savory. You finish it and feel properly restored — and it lingers.",
                "It tastes like someone cared about making it. That care sits with you, working slowly.",
            ],
        },
        new()
        {
            Id = "BerryPie", Name = "Berry Pie", SatietyRestore = 60,
            Buff = new BuffDefinition { Type = BuffType.GatherBonus, Magnitude = 1.50, Charges = 16 },
            UseMessages =
            [
                "The crust is imperfect and the berries are too tart and it is very good. Something in it sharpens you.",
                "You eat it in careful slices and wish there were more. Afterward your hands feel very capable.",
                "Something about a pie made out here feels like a small act of defiance. You approve, and feel it in your fingers.",
                "It is better than it has any right to be. You eat it all, and the world feels more available.",
                "The berries burst sweet and sour under the crust. You take your time. Your senses follow.",
            ],
        },
        new()
        {
            Id = "GarlicFlatbread", Name = "Garlic Flatbread", SatietyRestore = 28,
            Buff = new BuffDefinition { Type = BuffType.GatherBonus, Magnitude = 1.50, Charges = 12 },
            UseMessages =
            [
                "The garlic is everywhere and you are glad for it. Your eyes are sharp afterward.",
                "Blistered and pungent and hot from the fire. You eat it fast and feel ready for something.",
                "The flavor is aggressive and the result is clarity. Strange but true.",
                "You finish it and smell unmistakably of garlic. You also feel uncomfortably alert.",
            ],
        },
        new()
        {
            Id = "YamStew", Name = "Yam Stew", SatietyRestore = 40,
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.60, Charges = 24 },
            UseMessages =
            [
                "Dense and slow and deeply filling. You feel it anchor you. Hunger seems like someone else's problem.",
                "The yam has absorbed everything and given it back as comfort. You eat it all.",
                "Salt and grain and root, cooked down to something steadying. You feel it working for hours.",
                "Heavy and good. The kind of meal that earns its place. Your stomach is very pleased with you.",
            ],
        },
        new()
        {
            Id = "AcornPorridge", Name = "Acorn Porridge", SatietyRestore = 22,
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.65, Charges = 12 },
            UseMessages =
            [
                "Patient preparation has made something tolerable out of something that wasn't. You feel a modest energy.",
                "It is no one's favorite but it is warm and filling and it moves through you efficiently.",
                "The bitterness is mostly gone. What remains is dense and quiet and somehow bracing.",
                "You eat the whole bowl with a certain respect for the effort it required. Your body responds in kind.",
            ],
        },
        new()
        {
            Id = "BaobabBrew", Name = "Baobab Brew", SatietyRestore = 18,
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.60, Charges = 32 },
            UseMessages =
            [
                "Tart and slightly fizzy and deeply strange. Something in it settles and hums.",
                "You drink it cautiously and then quickly. The tartness gives way to something that feels like readiness.",
                "From a tree that outlives everything, this brew has patience built into it. It lends you some.",
                "It is the most peculiar thing you have eaten out here. It is also, somehow, exactly right. You feel it for a long time.",
            ],
        },

        // ── Dual-use: gatherable AND usable in combat ─────────────────────────
        new()
        {
            Id = "Herbs", Name = "Herbs",
            Description    = "Fragrant leaves of ambiguous medicinal value. Edible, and sometimes medicinal.",
            SatietyRestore = 8,
            UseMessages    =
            [
                "You chew the herbs slowly. Bitter, green, and vaguely medicinal.",
                "They taste medicinal and not particularly inviting. You finish them.",
                "The herbs are more pungent than expected. You eat them and move on.",
                "You eat the herbs raw. Your stomach accepts this grudgingly.",
            ],
            UsableInCombat = true, CombatHpRestore = 15, CombatBuffLabel = "Heal +15",
        },
        new()
        {
            Id = "BoneFragment", Name = "Bone Fragment",
            Description    = "Old. You prefer not to speculate further.",
            UsableInCombat = true,
            CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 4f, CombatBuffTurns = 2,
            CombatBuffLabel = "Def +4",
        },
        new()
        {
            Id = "SlimeGel", Name = "Slime Gel",
            Description    = "A viscous, quivering substance left behind by the slime. Warm.",
            UsableInCombat = true, CombatHpRestore = 10, CombatBuffLabel = "Heal +10",
        },
        new()
        {
            Id = "GoldenYolk", Name = "Golden Yolk",
            Description    = "The yolk of a sentient egg. It is deeply unsettling how golden it is.",
            UsableInCombat = true,
            CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 6f, CombatBuffTurns = 2,
            CombatBuffLabel = "Atk +6",
        },

        // ── Crafted combat consumables ────────────────────────────────────────
        new()
        {
            Id = "Poultice", Name = "Poultice",
            Description    = "Herbs, moss, and reed, pressed together with intent. Smells of the ground. Works better than it smells.",
            UsableInCombat = true, CombatHpRestore = 40, CombatBuffLabel = "Heal +40",
        },
        new()
        {
            Id = "StaminaDraught", Name = "Stamina Draught",
            Description    = "A bitter, reedy brew. Tastes like effort. Restores your will to keep moving.",
            UsableInCombat = true, CombatStaminaRestore = 5, CombatBuffLabel = "Stamina +5",
        },
        new()
        {
            Id = "WarPaint", Name = "War Paint",
            Description    = "Charred bone and coal, smeared with purpose. It changes something behind the eyes.",
            UsableInCombat = true,
            CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 10f, CombatBuffTurns = 3,
            CombatBuffLabel = "Atk +10",
        },
        new()
        {
            Id = "Antidote", Name = "Antidote",
            Description    = "Herbs and amber steeped with pale mushroom. Bitter and immediate. The affliction recedes.",
            UsableInCombat = true, CombatBuffLabel = "Cure Disease/Venom",
            ClearsStatuses = [ModifierStat.Disease, ModifierStat.Venom],
        },
        new()
        {
            Id = "SoothingSalve", Name = "Soothing Salve",
            Description    = "Clay, moss, and herbs worked into a cooling paste. Applied quickly, it draws the harm out.",
            UsableInCombat = true, CombatBuffLabel = "Cure Burn/Curse",
            ClearsStatuses = [ModifierStat.Burn, ModifierStat.StaminaDrain],
        },
        // ── Cooked fish recipes ───────────────────────────────────────────────

        // ── Tier 0 ───────────────────────────────────────────────────────────
        new()
        {
            Id = "GrilledRiverTrout", Name = "Grilled River Trout", SatietyRestore = 35,
            Description = "Simple fish over a simple fire. The trout is everything it needed to be.",
            UseMessages = ["The trout is charred and flaky. A small, uncomplicated pleasure.", "Simple and good. You eat it in silence and feel the better for it."],
            UsableInCombat = true, CombatHpRestore = 25, CombatBuffLabel = "Heal +25",
        },
        new()
        {
            Id = "PanFriedPerch", Name = "Pan-Fried Perch", SatietyRestore = 28,
            Description = "Crispy-edged and soft in the middle. Quick to make, quicker to eat.",
            UseMessages = ["The perch crisps up nicely. Something in the fat sharpens your step.", "Eaten fast, as the perch would have wanted. You feel light afterward."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Speed, CombatBuffValue = 2f, CombatBuffTurns = 2, CombatBuffLabel = "Spd +2",
        },
        new()
        {
            Id = "CatfishStew", Name = "Catfish Stew", SatietyRestore = 55,
            Description = "Dense with garlic and root. The kind of stew that settles in and stays.",
            UseMessages = ["It is heavy and rich and exactly right. Your hunger retreats and does not immediately return.", "The catfish has surrendered completely to the stew. You are grateful for this."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.55, Charges = 30 },
        },
        new()
        {
            Id = "SmokedRiverEel", Name = "Smoked River Eel", SatietyRestore = 32,
            Description = "Long strips of eel slow-smoked over herbs. Stronger than it looks.",
            UseMessages = ["The smoke got into it deeply. You finish it and feel ready for something physical.", "Oily and savory and good. Your arms feel heavier in a useful way."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 4f, CombatBuffTurns = 2, CombatBuffLabel = "Atk +4",
        },
        new()
        {
            Id = "CrabBisque", Name = "Crab Bisque", SatietyRestore = 38,
            Description = "Salt-simmered crab in its own shell. The shell did the work the pot couldn't.",
            UseMessages = ["Rich and slightly briny. Something in the shell-broth settles around you like armor.", "You drink it hot. The salt and mineral linger. Your stance feels lower, more rooted."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 5f, CombatBuffTurns = 3, CombatBuffLabel = "Def +5",
        },
        new()
        {
            Id = "GiantCarpRoast", Name = "Giant Carp Roast", SatietyRestore = 70,
            Description = "The whole fish, roasted whole, yields enough to feel like a proper meal. You take your time with it.",
            UseMessages = ["You eat more than you expected to. The carp gives generously.", "Dense and slow and deeply filling. The hunger does not return quickly."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.60, Charges = 40 },
        },
        new()
        {
            Id = "SaltedSandfish", Name = "Salted Sandfish", SatietyRestore = 25,
            Description = "Pressed flat and salted, then cured over the fire. Your hands feel capable afterward.",
            UseMessages = ["Salty and dense. You eat it slowly and feel a sharpening of purpose.", "The salt does something to you. Your eyes are clearer than they were."],
            Buff = new BuffDefinition { Type = BuffType.GatherBonus, Magnitude = 1.30, Charges = 10 },
        },
        new()
        {
            Id = "GrilledMullet", Name = "Grilled Mullet", SatietyRestore = 28,
            Description = "Thin fish over a hot fire. Done quickly and eaten the same way.",
            UseMessages = ["Light and slightly smoky. Your feet move easier for a while afterward.", "You finish it in a few bites. Something in it settles your pace."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.80, Charges = 12 },
        },
        new()
        {
            Id = "ShoreCrabSoup", Name = "Shore Crab Soup", SatietyRestore = 32,
            Description = "Salt-water crab simmered until the shell gives up its secrets.",
            UseMessages = ["Briney and good. The minerals from the shell settle into your shoulders.", "You finish the soup and feel a subtle solidity."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 3f, CombatBuffTurns = 2, CombatBuffLabel = "Def +3",
        },
        new()
        {
            Id = "SteamedFlounder", Name = "Steamed Flounder", SatietyRestore = 30,
            Description = "Steamed over herbs until it flakes apart at a look. Restorative.",
            UseMessages = ["Delicate and soft. You eat it carefully. The herbs did their part.", "The flounder gives easily to the heat. So, in a small way, do your aches."],
            UsableInCombat = true, CombatHpRestore = 20, CombatBuffLabel = "Heal +20",
        },
        new()
        {
            Id = "SeahorseBroth", Name = "Seahorse Broth", SatietyRestore = 18,
            Description = "A pale broth from a tiny creature. Unexpectedly restorative.",
            UseMessages = ["The broth is almost nothing — and then it keeps working.", "Thin and clear and quietly effective. Small things, done well."],
            UsableInCombat = true, CombatHpRestore = 15, CombatBuffLabel = "Heal +15",
        },
        new()
        {
            Id = "SwampEelStew", Name = "Swamp Eel Stew", SatietyRestore = 35,
            Description = "Garlic-rich stew that cuts through the murk of the eel's flavor entirely.",
            UseMessages = ["The garlic won. The eel contributed. You feel resistant to several things.", "Heavy and pungent. Something in it sits at the edges of you, pushing back."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Resistance, CombatBuffValue = 3f, CombatBuffTurns = 2, CombatBuffLabel = "Res +3",
        },
        new()
        {
            Id = "MudfishPatties", Name = "Mudfish Patties", SatietyRestore = 30,
            Description = "Ground and formed with grain into something more than the sum of its parts.",
            UseMessages = ["Better than expected, which is its own kind of achievement.", "You eat them without complaint. They sit in you quietly and sustain."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.65, Charges = 20 },
        },
        new()
        {
            Id = "MarshCrabCakes", Name = "Marsh Crab Cakes", SatietyRestore = 30,
            Description = "The marsh crab did not go quietly. The grain helped.",
            UseMessages = ["Dense and satisfying. Something crunchy in the shell bits adds texture and grit.", "You eat them by the fire. Your limbs feel settled, braced."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 4f, CombatBuffTurns = 2, CombatBuffLabel = "Def +4",
        },

        // ── Tier 1 ───────────────────────────────────────────────────────────
        new()
        {
            Id = "MountainTroutBraise", Name = "Mountain Trout Braise", SatietyRestore = 40,
            Description = "Salt-braised over low heat until the mountain cold seems less relevant.",
            UseMessages = ["The salt and slow heat made something of it. Your legs move more freely.", "Dense and clean-tasting. The cold biome seems a smaller obstacle."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.70, Charges = 18 },
        },
        new()
        {
            Id = "SmokedSalmon", Name = "Smoked Salmon", SatietyRestore = 45,
            Description = "Slow-smoked over herbs until the flesh turns deep orange and pulls clean.",
            UseMessages = ["Rich and oily and exactly what the smoke promised. Your hands feel quick.", "You eat it in unhurried strips. Something sharpens behind the eyes."],
            Buff = new BuffDefinition { Type = BuffType.GatherBonus, Magnitude = 1.40, Charges = 18 },
        },
        new()
        {
            Id = "GlacialCharFillet", Name = "Glacial Char Fillet", SatietyRestore = 35,
            Description = "Cold-adapted fish over cold fire. Salt was all it needed.",
            UseMessages = ["The char is cold-tolerant by nature. Something transfers. The cold feels navigable.", "Lean and clean. The cold biome seems more familiar after."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.60, Charges = 22 },
        },
        new()
        {
            Id = "GoldenTroutTartare", Name = "Golden Trout Tartare", SatietyRestore = 25,
            Description = "Raw golden trout pressed with salt. Precious and deliberate.",
            UseMessages = ["You eat it slowly. The gold of it is in the taste too, somehow. The water seems to yield more.", "Something in the rarity of the fish passes to you. The unusual seems more findable."],
            Buff = new BuffDefinition { Type = BuffType.FishingRarityBonus, Magnitude = 2.00, Charges = 8 },
        },
        new()
        {
            Id = "ClownfishSkewers", Name = "Clownfish Skewers", SatietyRestore = 28,
            Description = "Bright little fish on a stick. The color survives the fire, mostly.",
            UseMessages = ["Unexpectedly good. Something in the reef-flavor unlocks something behind your eyes.", "The vivid taste of it is unusual. Your focus sharpens in a strange direction."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Magic, CombatBuffValue = 4f, CombatBuffTurns = 2, CombatBuffLabel = "Mag +4",
        },
        new()
        {
            Id = "ParrotfishCurry", Name = "Parrotfish Curry", SatietyRestore = 42,
            Description = "Garlic and herb curry built around the dense, sweet meat of the parrotfish.",
            UseMessages = ["The curry is bright and fragrant and the parrotfish gives it body. You feel capable of pace.", "Hot from the fire, pungent with garlic. Your feet remember what they are for."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Speed, CombatBuffValue = 3f, CombatBuffTurns = 3, CombatBuffLabel = "Spd +3",
        },
        new()
        {
            Id = "TriggersteakFillet", Name = "Triggerfish Steak", SatietyRestore = 38,
            Description = "The triggerfish resisted being caught. It resisted the fire too, briefly.",
            UseMessages = ["Firm and rich. Something of the trigger's aggression passes to you.", "The fight it gave was part of the flavour. You feel the benefit of it."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 5f, CombatBuffTurns = 2, CombatBuffLabel = "Atk +5",
        },
        new()
        {
            Id = "MorayEelSaute", Name = "Moray Eel Sauté", SatietyRestore = 35,
            Description = "Garlic and heat transformed the eel's hostility into something almost admirable.",
            UseMessages = ["The garlic did what the hook couldn't — made the eel agreeable. You eat it and feel dangerous.", "Dense and pungent and satisfying. Something in you sharpens to a point."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 6f, CombatBuffTurns = 3, CombatBuffLabel = "Atk +6",
        },
        new()
        {
            Id = "LionfishFillet", Name = "Lionfish Fillet", SatietyRestore = 30,
            Description = "Carefully deboned and herb-pressed. The venom sac, notably, is not included.",
            UseMessages = ["The danger was handled before the cooking. What remains is the aggression, applied usefully.", "You eat it knowing what it was. The body absorbs the lesson."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 7f, CombatBuffTurns = 2, CombatBuffLabel = "Atk +7",
            ClearsStatuses = [ModifierStat.Venom],
        },
        new()
        {
            Id = "MirrorCarpBraise", Name = "Mirror Carp Braise", SatietyRestore = 65,
            Description = "Slow-braised until the flesh falls apart. The most sustaining meal the lake offers.",
            UseMessages = ["You eat it over a long time and feel it working just as long. Hunger becomes distant.", "The carp gave everything to the pot. You take everything from it. This feels right."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.50, Charges = 36 },
        },
        new()
        {
            Id = "LakeBassStew", Name = "Lake Bass Stew", SatietyRestore = 40,
            Description = "Root and fish in slow water, made into something steady and filling.",
            UseMessages = ["The cattail root thickened it properly. You eat it and feel grounded.", "Simple and reliable, like the lake itself. It sustains without drama."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.60, Charges = 24 },
        },
        new()
        {
            Id = "SearedMoonfish", Name = "Seared Moonfish", SatietyRestore = 40,
            Description = "Herb-pressed and seared at the right moment. The fish still carries something of the dusk in it.",
            UseMessages = ["The moonfish gives a strange clarity. The water seems to offer more.", "You eat it as the fire dims. The rare seems closer now."],
            Buff = new BuffDefinition { Type = BuffType.FishingRarityBonus, Magnitude = 1.50, Charges = 16 },
        },
        new()
        {
            Id = "LegendaryMoonfishBroth", Name = "Legendary Moonfish Broth", SatietyRestore = 55,
            Description = "From the rarest fish in the lake — a broth that tastes like something earned.",
            UseMessages = ["You made something remarkable from something remarkable. The water feels different after.", "It heals and opens something. The rare seems very close now. The world offers more."],
            Buff = new BuffDefinition { Type = BuffType.FishingRarityBonus, Magnitude = 2.50, Charges = 24 },
            UsableInCombat = true, CombatHpRestore = 40, CombatBuffLabel = "Heal +40",
        },

        // ── Tier 2 ───────────────────────────────────────────────────────────
        new()
        {
            Id = "KelpWrappedGrouper", Name = "Kelp-Wrapped Grouper", SatietyRestore = 40,
            Description = "Wrapped in kelp and slow-cooked until the sea soaks through entirely.",
            UseMessages = ["The kelp steamed through it. You eat it and feel the ocean settle around you.", "Dense and briny. Something about the wrapping concentrated it. You feel armored."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 5f, CombatBuffTurns = 3, CombatBuffLabel = "Def +5",
        },
        new()
        {
            Id = "RockfishChowder", Name = "Rockfish Chowder", SatietyRestore = 48,
            Description = "A rockfish that survived centuries ended its run in a chowder. It is very good chowder.",
            UseMessages = ["Dense and mineral and deeply satisfying. Something about its age carries through.", "You eat every last drop. The endurance of the fish becomes yours, briefly."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.55, Charges = 28 },
        },
        new()
        {
            Id = "OctopusStew", Name = "Octopus Stew", SatietyRestore = 45,
            Description = "Eight arms simmered until tender. The garlic had its work cut out for it.",
            UseMessages = ["The octopus gives up its resilience to the pot. Some of it transfers to you.", "Garlic and salt and something the sea did to the meat. Your mind feels sharper."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Magic, CombatBuffValue = 5f, CombatBuffTurns = 3, CombatBuffLabel = "Mag +5",
        },
        new()
        {
            Id = "SeaBassFillet", Name = "Sea Bass Fillet", SatietyRestore = 42,
            Description = "Herb-crusted and reliable. The sea bass was always going to end up like this.",
            UseMessages = ["Clean and good. A reliable fish makes a reliable meal. Your hands feel able.", "The herbs did their part. Something practical sharpens in you."],
            Buff = new BuffDefinition { Type = BuffType.GatherBonus, Magnitude = 1.50, Charges = 20 },
        },
        new()
        {
            Id = "SquidInkStew", Name = "Squid Ink Stew", SatietyRestore = 55,
            Description = "The ink turned everything black. This is correct. It tastes of deep water and precision.",
            UseMessages = ["The stew is black and heavy and tastes of the abyss. You eat it all. Your timing improves.", "Something about the ink. The strike window feels longer now. More time in it."],
            Buff = new BuffDefinition { Type = BuffType.FishingStrikeBonus, Magnitude = 1.40, Charges = 20 },
        },
        new()
        {
            Id = "AnglerFishFillets", Name = "Anglerfish Fillets", SatietyRestore = 35,
            Description = "The lure is removed first. The fish that used it to hunt is now herb-dressed and useful.",
            UseMessages = ["The irony of being lured in is noted. The fillets are excellent regardless.", "Dense, deep-water meat pressed with herbs. Your hands feel sharp afterward."],
            Buff = new BuffDefinition { Type = BuffType.GatherBonus, Magnitude = 1.50, Charges = 16 },
        },
        new()
        {
            Id = "GhostfishSashimi", Name = "Ghostfish Sashimi", SatietyRestore = 30,
            Description = "Raw and translucent and dressed in salt. You can see through it, almost.",
            UseMessages = ["You eat something that was practically invisible. You feel difficult to miss afterward, which is strange.", "The translucence stays on the tongue. Your aim feels informed by it."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Accuracy, CombatBuffValue = 0.15f, CombatBuffTurns = 3, CombatBuffLabel = "Acc +15%",
        },
        new()
        {
            Id = "AbyssalEelRoast", Name = "Abyssal Eel Roast", SatietyRestore = 38,
            Description = "From very deep water, roasted until the discomfort of both parties was resolved.",
            UseMessages = ["The depth of it carries through. Something old and strong in the meat.", "You eat something that lived below light. The darkness doesn't feel like yours to own."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Attack, CombatBuffValue = 8f, CombatBuffTurns = 2, CombatBuffLabel = "Atk +8",
        },
        new()
        {
            Id = "CaveFishBroth", Name = "Cave Fish Broth", SatietyRestore = 28,
            Description = "A pale broth from a pale fish. Quiet and efficient.",
            UseMessages = ["It tastes of dark water and very little else. You move more easily afterward.", "Something about the cave-adapted fish. The cool dark no longer slows you."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.70, Charges = 20 },
        },
        new()
        {
            Id = "VolcanicEelRoast", Name = "Volcanic Eel Roast", SatietyRestore = 38,
            Description = "Roasted with sulfur, as it would have wanted. The heat stays in it.",
            UseMessages = ["The sulfur did something to it. The heat of the biome feels less like a wall.", "You eat something that lived in near-boiling water. The warmth becomes yours."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.55, Charges = 24 },
        },
        new()
        {
            Id = "SpicedShrimp", Name = "Spiced Shrimp", SatietyRestore = 22,
            Description = "Herb-pressed shrimp that lived at temperatures most things avoid. Small and fast.",
            UseMessages = ["Tiny and flavored with heat. You eat them quickly and move the same way.", "The shrimp was faster than anything had a right to be. Something transfers."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Speed, CombatBuffValue = 4f, CombatBuffTurns = 3, CombatBuffLabel = "Spd +4",
        },

        // ── Tier 3 ───────────────────────────────────────────────────────────
        new()
        {
            Id = "BioluminescentBroth", Name = "Bioluminescent Broth", SatietyRestore = 18,
            Description = "A broth that still faintly glows. You drink it in the dark and feel it moving.",
            UseMessages = ["It pulses faintly as it cools. You drink it and move like water afterward.", "The glow is gone from it but something else remains. Your stride lengthens."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.50, Charges = 40 },
        },
        new()
        {
            Id = "AbyssalAnglerStew", Name = "Abyssal Anglerfish Stew", SatietyRestore = 45,
            Description = "The deep-sea anglerfish, patient as it was, contributes to a stew that feels ancient.",
            UseMessages = ["The light of its lure is gone but something remains in the meat. Your focus deepens strangely.", "Dark and rich and from somewhere very far down. Something in you opens."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Magic, CombatBuffValue = 7f, CombatBuffTurns = 3, CombatBuffLabel = "Mag +7",
        },
        new()
        {
            Id = "GlowingSquidBroth", Name = "Glowing Squid Broth", SatietyRestore = 30,
            Description = "The ink glows even after cooking. The broth is the color of cold bioluminescence.",
            UseMessages = ["You drink something that was still glowing an hour ago. Your reaction time improves noticeably.", "The glow transferred somewhere. The strike window feels wider, more visible."],
            Buff = new BuffDefinition { Type = BuffType.FishingStrikeBonus, Magnitude = 1.50, Charges = 16 },
        },

        // ── Tier 4 ───────────────────────────────────────────────────────────
        new()
        {
            Id = "IceShrimpBisque", Name = "Ice Shrimp Bisque", SatietyRestore = 25,
            Description = "Tiny shrimp from near-frozen water, pressed into a pale bisque. Cold and clarifying.",
            UseMessages = ["Almost flavorless, but clarifying. The cold environments feel more navigable.", "It tastes of glacier water. The cold biome opens a little."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.60, Charges = 20 },
        },
        new()
        {
            Id = "CrystalFishSashimi", Name = "Crystal Fish Sashimi", SatietyRestore = 28,
            Description = "Thin-sliced raw crystal fish with salt. Each cut reveals more light.",
            UseMessages = ["The flesh is almost glass. You eat it in careful slices. The hook window seems to extend.", "Crystalline and cold and precise. The strike feels like something you knew before."],
            Buff = new BuffDefinition { Type = BuffType.FishingStrikeBonus, Magnitude = 1.60, Charges = 12 },
        },
        new()
        {
            Id = "FrozenEelStew", Name = "Frozen Eel Stew", SatietyRestore = 45,
            Description = "The frozen eel required patience to prepare. It rewards that patience directly.",
            UseMessages = ["Dense and heavy as glacier ice once was. Your defenses feel layered.", "You eat something that fought slowly but for a very long time. The endurance transfers."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 8f, CombatBuffTurns = 3, CombatBuffLabel = "Def +8",
        },
        new()
        {
            Id = "ArcticTartare", Name = "Arctic Tartare", SatietyRestore = 20,
            Description = "Raw arctic trout pressed with salt. Cold-adapted. So are you, briefly.",
            UseMessages = ["The cold of it goes in and stays. The frozen biomes feel less like barriers.", "You eat something built for extreme cold. Briefly, you understand why it managed."],
            Buff = new BuffDefinition { Type = BuffType.CooldownReduction, Magnitude = 0.40, Charges = 20 },
        },
        new()
        {
            Id = "PolarStarBroth", Name = "Polar Star Broth", SatietyRestore = 22,
            Description = "The polar star held on to things with remarkable commitment. The broth carries that.",
            UseMessages = ["Mineral and cold and strangely fortifying. Something about the arctic shelf.", "You eat something that persisted. Your body takes note."],
            UsableInCombat = true, CombatBuffStat = ModifierStat.Defense, CombatBuffValue = 6f, CombatBuffTurns = 2, CombatBuffLabel = "Def +6",
        },
        new()
        {
            Id = "AncientFishStew", Name = "Ancient Fish Stew", SatietyRestore = 70,
            Description = "From a species unchanged for fifty million years, a stew that feels like it has survived everything.",
            UseMessages = ["You eat something fifty million years in the making. Your hunger does not return for a very long time.", "The ancient fish gives everything. The stew is the most sustaining thing you have ever eaten."],
            Buff = new BuffDefinition { Type = BuffType.HungerDrainReduction, Magnitude = 0.40, Charges = 60 },
        },
    ];

    private static readonly Dictionary<string, ConsumableDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<ConsumableDefinition> All => _definitions;

    public ConsumableDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
