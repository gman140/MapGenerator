using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;

namespace MapGenerator.Combat.Services;

public class InMemoryEnemyDefinitionProvider : IEnemyDefinitionProvider
{
    private static readonly EnemyDefinition[] _definitions =
    [
        new()
        {
            Id = "Slime", Name = "Slime",
            BaseHp = 20, BaseAttack = 8, BaseDefense = 2,
            FleeChance = 0.80f, FleeHpThreshold = 0.30f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 70 },
                new() { Action = EnemyActionType.Defend, Weight = 30 },
            ],
            LootTable =
            [
                new() { ItemId = "SlimeGel",  Chance = 0.50f },
                new() { ItemId = "GooShard",  Chance = 0.30f },
                new() { ItemId = "Herbs",     Chance = 0.40f },
                new() { ItemId = "Stone",     Chance = 0.20f },
                new() { ItemId = "Lantern",   Chance = 0.02f },
            ],
            AppearTexts =
            [
                "Something gelatinous drops from the ceiling — one wet, eyeless impact against the stone.",
                "You hear it before you see it: a thick, rhythmic squelch advancing in the dark.",
                "A glistening shape detaches itself from the wall and orients toward you with terrible patience.",
            ],
            AttackTexts =
            [
                "The slime gurgles wetly and engulfs your boot.",
                "A quivering mass of goo launches itself at you with unsettling enthusiasm.",
                "It sort of just oozes at you. The result is, unfortunately, contact.",
                "The slime makes a noise like a wet handshake and impacts your shin.",
            ],
            DefendTexts =
            [
                "The slime puddles itself flat, reducing itself to a nearly-unpunchable disc.",
                "It does something with its surface tension that you would rather not think about.",
            ],
            FleeTexts =
            [
                "The slime jiggles frantically and oozes through a crack in the floor.",
                "It makes an alarming squelching sound and retreats with surprising purpose.",
                "You blink and it is simply gone, leaving only a damp spot and a sense of unease.",
            ],
            DeathTexts =
            [
                "The slime collapses with a wet, apologetic noise.",
                "It disperses into a thin film across the floor. Unfortunate for everyone.",
                "Whatever held it together lets go all at once.",
            ],
        },

        new()
        {
            Id = "SentientEgg", Name = "Sentient Egg",
            BaseHp = 35, BaseAttack = 10, BaseDefense = 5,
            FleeChance = 0.50f, FleeHpThreshold = 0.20f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 50 },
                new() { Action = EnemyActionType.Buff,   Weight = 30 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BuffStat = ModifierStat.Attack, BuffValue = 4f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "EggFragment", Chance = 0.40f },
                new() { ItemId = "GoldenYolk",  Chance = 0.20f },
                new() { ItemId = "Fiber",        Chance = 0.40f },
                new() { ItemId = "Reed",         Chance = 0.30f },
                new() { ItemId = "Compass",      Chance = 0.03f },
            ],
            AppearTexts =
            [
                "An egg-shaped figure regards you from the middle of the room. It has eyes. They blink.",
                "You were not expecting an egg here. The egg, however, seems entirely comfortable with the situation.",
                "It was sitting very still. When you entered, it turned. You do not know how. It has no neck.",
            ],
            AttackTexts =
            [
                "The egg wobbles forward aggressively and impacts you with its considerable mass.",
                "It headbutts you with surprising speed and zero apparent regard for self-preservation.",
                "The egg rushes you with what can only be described as reckless confidence.",
                "It rotates somehow and strikes you with the blunt end. Both ends are blunt. It chose one anyway.",
            ],
            BuffTexts =
            [
                "The egg vibrates at a frequency you feel in your teeth. Its eyes narrow.",
                "It hums something low and rhythmic — whatever it is doing, it seems intentional.",
                "A faint golden sheen washes across its shell. This cannot be good.",
            ],
            DefendTexts =
            [
                "The egg tucks into itself somehow, presenting only shell to you.",
                "It sits very still and looks at you as if daring you to try something.",
            ],
            FleeTexts =
            [
                "The egg rolls toward the far wall at alarming speed and disappears into a crack.",
                "It exits the situation at a rolling sprint. You did not know eggs could sprint.",
                "The egg wobbles twice, then rockets away without explanation.",
            ],
            DeathTexts =
            [
                "The egg cracks apart with a dense, final sound. Yolk everywhere.",
                "It falls over with a soft thud and is still. The eyes do not close.",
                "It splits cleanly. Something golden spills out. You try not to look at it directly.",
            ],
        },

        new()
        {
            Id = "Wolf", Name = "Wolf",
            BaseHp = 50, BaseAttack = 18, BaseDefense = 8,
            FleeChance = 0.60f, FleeHpThreshold = 0.25f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 60 },
                new() { Action = EnemyActionType.Buff,   Weight = 20 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BuffStat = ModifierStat.Attack, BuffValue = 6f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "WolfPelt",    Chance = 0.50f },
                new() { ItemId = "Fang",        Chance = 0.40f },
                new() { ItemId = "BoneFragment",Chance = 0.30f },
                new() { ItemId = "Wood",        Chance = 0.30f },
            ],
            AppearTexts =
            [
                "A low growl precedes it by a full second — then the wolf steps into view.",
                "It does not move immediately. It just watches you, calculating.",
                "Yellow eyes in the dark. Then a shape that is entirely too fast.",
            ],
            AttackTexts =
            [
                "The wolf lunges with practiced precision, teeth first.",
                "A flash of grey and then it is on you — fast, purposeful, and unpleasant.",
                "It snaps at your arm with a speed that suggests this is not its first time.",
                "The wolf darts low, pivots, and catches you before you finish tracking it.",
            ],
            BuffTexts =
            [
                "The wolf tilts its head back and howls once, short and sharp. Its posture shifts.",
                "It growls, low and sustained, and something hardens in its eyes.",
                "A sound you feel rather than hear — and then it is moving differently.",
            ],
            DefendTexts =
            [
                "The wolf lowers its head and bares its teeth — daring you to move first.",
                "It circles just outside your reach, patient.",
                "It drops onto its haunches, watching. Waiting.",
            ],
            FleeTexts =
            [
                "The wolf breaks and sprints, vanishing into the dark before you can act.",
                "It turns without a sound and is gone — you barely see it go.",
                "One moment it is there. Then it decides it is not.",
            ],
            DeathTexts =
            [
                "The wolf drops with a heavy thud and does not rise.",
                "It collapses mid-motion. The silence that follows is complete.",
                "It goes still all at once, like something switched off.",
            ],
        },

        new()
        {
            Id = "Bear", Name = "Bear",
            BaseHp = 80, BaseAttack = 22, BaseDefense = 14,
            FleeChance = 0.20f, FleeHpThreshold = 0.15f,
            DefendDamageBonus = 0.75f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 50 },
                new() { Action = EnemyActionType.Defend,      Weight = 30 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 20 },
            ],
            LootTable =
            [
                new() { ItemId = "BearHide", Chance = 0.50f },
                new() { ItemId = "Claw",     Chance = 0.50f },
                new() { ItemId = "Herbs",    Chance = 0.40f },
                new() { ItemId = "Coal",     Chance = 0.20f },
            ],
            AppearTexts =
            [
                "Something very large turns around slowly. Your instincts are already running.",
                "The bear does not rush. It just fills the space between you and the exit.",
                "You smell it first. Then you hear it breathing. Then it becomes visible, which is the worst part.",
            ],
            AttackTexts =
            [
                "The bear swipes with one paw — the impact is casual and enormous.",
                "It drops toward you and drives a shoulder into your chest.",
                "A blow that arrives without warning and departs with your equilibrium.",
                "The swipe is almost lazy. It still connects.",
            ],
            HeavyAttackTexts =
            [
                "The bear rises to full height and brings both forepaws down.",
                "It charges the last three steps without slowing — the impact is a statement.",
                "It commits entirely to the hit. There is a moment of suspension, then it arrives.",
            ],
            DefendTexts =
            [
                "The bear rears up on its hind legs and draws in a long breath. Whatever comes next will be worse.",
                "It plants itself and stares at you. The floor seems to creak under the intention.",
                "It settles into a stance that looks almost contemplative. This is not reassuring.",
            ],
            FleeTexts =
            [
                "The bear backs into the dark, growling low, unwilling to turn its back on you.",
                "It retreats — slowly, deliberately, watching you the whole way.",
                "It decides you are not worth finishing, and leaves with the confidence of something that made a choice.",
            ],
            DeathTexts =
            [
                "The bear falls in pieces, like something that took a long time to decide.",
                "It sits down heavily and then lies down. It does not get up.",
                "It is not a dramatic death. It simply stops.",
            ],
        },

        new()
        {
            Id = "CaveTroll", Name = "Cave Troll",
            BaseHp = 150, BaseAttack = 28, BaseDefense = 18,
            FleeChance = 0.10f, FleeHpThreshold = 0.10f,
            RegenerateAmount = 12,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 40 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 30 },
                new() { Action = EnemyActionType.Regenerate,  Weight = 20 },
                new() { Action = EnemyActionType.Defend,      Weight = 10 },
            ],
            LootTable =
            [
                new() { ItemId = "TrollHide",    Chance = 0.50f },
                new() { ItemId = "CrushedRock",  Chance = 0.60f },
                new() { ItemId = "BoneFragment", Chance = 0.50f },
                new() { ItemId = "Stone",        Chance = 0.60f },
                new() { ItemId = "DeepOre",      Chance = 0.30f },
                new() { ItemId = "Spyglass",     Chance = 0.02f },
            ],
            AppearTexts =
            [
                "Something ancient and vast stirs in the darkness.",
                "A low rumble announces its arrival — and it is not pleased.",
                "The chamber changes before you see it. The air thickens. Then you see it.",
                "It was here long before the dungeon was built around it. You can tell.",
            ],
            AttackTexts =
            [
                "The troll brings a fist down like a falling boulder.",
                "It swings one arm in a wide arc — the kind of arc that clears rooms.",
                "You were standing still. That was your mistake.",
                "The blow is not fast, but it does not need to be.",
            ],
            HeavyAttackTexts =
            [
                "The troll raises both fists and brings them down in unison.",
                "A thunderous blow shakes dust from the ceiling. You feel it before it lands.",
                "It coils everything it has into the next swing, and the floor shakes.",
                "The impact is architectural. Something in the ceiling shifts.",
            ],
            DefendTexts =
            [
                "The troll draws its arms inward and hunches — making itself into a dense, unwelcoming shape.",
                "Stone scrapes against stone. You realize the sound is its hide.",
                "It does not move, it just becomes harder to look at directly.",
            ],
            RegenerateTexts =
            [
                "The troll shudders once and the wounds on its surface close, slowly, like something settling.",
                "A deep, rhythmic rumble passes through it — and it looks slightly more intact than before.",
                "It inhales something from the stone. The cracks seal.",
            ],
            FleeTexts =
            [
                "The troll retreats into a tunnel you did not know was there, hauling itself away.",
                "It backs into the wall and pulls the dark around itself. Gone.",
                "It turns and moves with a speed its size should not allow.",
            ],
            DeathTexts =
            [
                "The troll collapses with finality, and the chamber feels larger.",
                "It falls in stages — each piece of it deciding separately that it is done.",
                "It does not fall so much as cease, all at once.",
            ],
        },

        new()
        {
            Id = "HexRabbit", Name = "Six-Legged Rabbit",
            BaseHp = 12, BaseAttack = 4, BaseDefense = 1,
            FleeChance = 0.90f, FleeHpThreshold = 0.60f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 80 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            LootTable =
            [
                new() { ItemId = "Fiber",    Chance = 0.50f },
                new() { ItemId = "Feathers", Chance = 0.40f },
                new() { ItemId = "Herbs",    Chance = 0.25f },
            ],
            AppearTexts =
            [
                "Six legs. You count them twice. Six.",
                "Something small and fast enters from the wrong direction and stops, staring at you with too many eyes.",
                "It has too many legs for a rabbit. This seems to bother only you.",
            ],
            AttackTexts =
            [
                "Three of its legs kick at you simultaneously. It's honestly impressive.",
                "It launches off all six legs at once and impacts you squarely.",
                "It nips at your ankle with surprising conviction.",
                "The rabbit executes what can only be described as a coordinated attack with itself.",
            ],
            DefendTexts =
            [
                "It crouches low on all six legs, making itself improbably small.",
                "It folds its extra limbs in a way that should not be anatomically possible.",
            ],
            FleeTexts =
            [
                "It turns and sprints away on all six legs, which is frankly faster than anything should go.",
                "It vanishes into the undergrowth at a speed that makes you question what you saw.",
                "Six legs carry it away from this situation immediately.",
                "It thumps twice with its rearmost legs and is gone.",
            ],
            DeathTexts =
            [
                "It folds in on itself in a very small way.",
                "All six legs go still at once.",
                "It tips over sideways, which takes longer than expected given the legs.",
            ],
        },

        new()
        {
            Id = "BeaverSerpent", Name = "Snake-Headed Beaver",
            BaseHp = 28, BaseAttack = 9, BaseDefense = 4,
            FleeChance = 0.55f, FleeHpThreshold = 0.30f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 55 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 15 },
                new() { Action = EnemyActionType.Defend,      Weight = 30 },
            ],
            LootTable =
            [
                new() { ItemId = "Wood",  Chance = 0.60f },
                new() { ItemId = "Fiber", Chance = 0.40f },
                new() { ItemId = "Reed",  Chance = 0.35f },
                new() { ItemId = "Peat",  Chance = 0.20f },
            ],
            AppearTexts =
            [
                "Something emerges from the water. The body is a beaver. The head is not.",
                "It hisses at you with the flat face of something that should not hiss.",
                "You notice the tail first, then the body, then the head, in that order of increasing wrongness.",
            ],
            AttackTexts =
            [
                "The snake head strikes forward with the flat-toothed lunge of something confused about what it is.",
                "It gnaws at you with fangs that are slightly too large for the rest of its face.",
                "It slaps you with its broad tail and follows up with a bite that you weren't expecting from that direction.",
                "The head lunges independently of the body, which does its own thing.",
            ],
            HeavyAttackTexts =
            [
                "It rears back and drives its flat head into you with the full weight of a confused animal.",
                "The tail and the head attack simultaneously. This is somehow worse.",
            ],
            DefendTexts =
            [
                "It wedges itself against a tree and hisses with the air of something waiting you out.",
                "It flattens its tail like a shield and pulls the snake head behind it.",
            ],
            FleeTexts =
            [
                "It retreats into the nearest body of water, headfirst.",
                "The beaver body and snake head reach an agreement and leave together.",
                "It slithers and waddles away in a motion that is hard to categorize.",
            ],
            DeathTexts =
            [
                "The head and the body stop disagreeing about direction at the same time.",
                "It settles into the mud with a long, wet exhale.",
                "Whatever held its two natures together finally lets go.",
            ],
        },

        new()
        {
            Id = "MushroomSprite", Name = "Mushroom Sprite",
            BaseHp = 18, BaseAttack = 7, BaseDefense = 2,
            FleeChance = 0.65f, FleeHpThreshold = 0.35f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,     Weight = 45 },
                new() { Action = EnemyActionType.Buff,       Weight = 35 },
                new() { Action = EnemyActionType.Regenerate, Weight = 20 },
            ],
            BuffStat = ModifierStat.Attack, BuffValue = 3f, BuffTurns = 2,
            RegenerateAmount = 5,
            LootTable =
            [
                new() { ItemId = "Herbs",       Chance = 0.55f },
                new() { ItemId = "Mushroom",    Chance = 0.45f },
                new() { ItemId = "PaleMushroom",Chance = 0.20f },
                new() { ItemId = "Moss",        Chance = 0.30f },
            ],
            AppearTexts =
            [
                "A small figure detaches from the forest floor. It is wearing a cap that is also its head.",
                "The mushroom blinks. You were not expecting that.",
                "Something small and spore-dusted emerges from the undergrowth with an air of purpose.",
            ],
            AttackTexts =
            [
                "It releases a small cloud of spores directly into your face.",
                "It headbutts you with its cap. This is more effective than it sounds.",
                "Tiny fists, moving very fast. The cap wobbles aggressively.",
                "It leaps at you and makes contact with what it clearly considers dignity.",
            ],
            BuffTexts =
            [
                "It shakes itself and a golden dust rises from its cap. Its eyes go bright.",
                "The sprite inhales deeply and seems to grow slightly taller, though it may just be the cap.",
            ],
            RegenerateTexts =
            [
                "It presses a hand to its own cap and draws something from it. Color returns to its face.",
                "Spores curl inward. The sprite is briefly more solid than before.",
            ],
            FleeTexts =
            [
                "It releases a dense cloud of spores and is gone before the cloud settles.",
                "It rolls itself up like a pill bug and bounces away at speed.",
                "It vanishes into the leaf litter with a sound like a small door closing.",
            ],
            DeathTexts =
            [
                "Its cap falls off. Then the rest of it.",
                "The sprite disperses into a fine spore cloud that drifts sideways.",
                "It sits down abruptly and does not get up.",
            ],
        },

        new()
        {
            Id = "DustWraith", Name = "Dust Wraith",
            BaseHp = 22, BaseAttack = 11, BaseDefense = 1,
            FleeChance = 0.70f, FleeHpThreshold = 0.40f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 65 },
                new() { Action = EnemyActionType.Buff,   Weight = 35 },
            ],
            BuffStat = ModifierStat.DodgeChance, BuffValue = 0.15f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "Sand",  Chance = 0.50f },
                new() { ItemId = "Ash",   Chance = 0.40f },
                new() { ItemId = "Flint", Chance = 0.25f },
            ],
            AppearTexts =
            [
                "The sand lifts in a column and stays there. Then it turns toward you.",
                "Something made of dry air and grit coheres in front of you, regarding you without eyes.",
                "The heat shimmer takes a shape it should not have.",
            ],
            AttackTexts =
            [
                "It drives a column of grit into your face at speed.",
                "The wraith simply passes through you, briefly. The cold is wrong for the climate.",
                "Sand, fast and directed, impacts you from a direction you weren't watching.",
                "It condenses for a moment and strikes before dispersing again.",
            ],
            BuffTexts =
            [
                "The wraith spins in place, becoming briefly indistinct and ungraspable.",
                "It spreads itself thin across the air. Less to hit.",
            ],
            FleeTexts =
            [
                "It collapses flat against the ground and is indistinguishable from the sand.",
                "The wind picks up and there is suddenly nothing there.",
                "It disperses, unwilling to continue the conversation.",
            ],
            DeathTexts =
            [
                "The sand drops all at once into a flat, damp circle.",
                "Whatever was holding it together releases, and the wind takes it.",
                "It becomes, very suddenly, just sand.",
            ],
        },
    ];

    private static readonly Dictionary<string, EnemyDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<EnemyDefinition> All => _definitions;

    public EnemyDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
