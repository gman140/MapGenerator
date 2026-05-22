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
    ];

    private static readonly Dictionary<string, ConsumableDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<ConsumableDefinition> All => _definitions;

    public ConsumableDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
