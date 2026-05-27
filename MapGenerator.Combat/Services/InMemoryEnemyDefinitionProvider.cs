using MapGenerator.Combat.Enums;
using MapGenerator.Combat.Interfaces;
using MapGenerator.Combat.Models;
using MapGenerator.Domain.Enums;
using DT = MapGenerator.Domain.Enums.DamageType;

namespace MapGenerator.Combat.Services;

public class InMemoryEnemyDefinitionProvider : IEnemyDefinitionProvider
{
    private static readonly EnemyDefinition[] _definitions =
    [
        new()
        {
            Id = "Slime", Name = "Slime",
            Description = "Amorphous masses of corrosive gel that form naturally in wet, low-light environments. Blunt force and plant matter only seem to feed them — bring lightning.",
            BaseHp = 20, BaseAttack = 8, BaseDefense = 2,
            FleeChance = 0.80f, FleeHpThreshold = 0.30f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 70 },
                new() { Action = EnemyActionType.Defend, Weight = 30 },
            ],
            BaseXp = 4,
            BaseSpeed = 3, CritChance = 0.00f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses   = [DT.Storm],
            Resistances  = [DT.Bludgeoning, DT.Nature],
            WeaknessText  = { [DT.Storm]       = "The slime conducts electricity perfectly!" },
            ResistanceText = { [DT.Bludgeoning] = "Your blow sinks in and is simply absorbed.", [DT.Nature] = "It seems to relish the contact." },
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
            Description = "An unhatched creature pulsing with restless, angry energy. Nobody knows what's inside, and those who've tried to find out usually regret it.",
            BaseHp = 55, BaseAttack = 14, BaseDefense = 7,
            FleeChance = 0.30f, FleeHpThreshold = 0.15f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 50 },
                new() { Action = EnemyActionType.Buff,   Weight = 30 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 6,
            BaseSpeed = 3, CritChance = 0.05f,
            AttackDamageType = DT.Bludgeoning,
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
            Description = "Cunning pack hunters that coordinate with howls and flanking charges. A lone wolf is dangerous; a howling one usually means more are close behind.",
            BaseHp = 50, BaseAttack = 18, BaseDefense = 8,
            FleeChance = 0.60f, FleeHpThreshold = 0.25f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 60 },
                new() { Action = EnemyActionType.Buff,   Weight = 20 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 10,
            BaseSpeed = 8, CritChance = 0.05f,
            AttackDamageType = DT.Slashing,
            Weaknesses   = [DT.Fire],
            Resistances  = [DT.Frost],
            WeaknessText  = { [DT.Fire]  = "The wolf recoils from the heat — fire is its oldest fear." },
            ResistanceText = { [DT.Frost] = "Its thick pelt shrugs off the cold." },
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
            Description = "Massive and deceptively patient, bears rear up to channel their weight into devastating follow-up strikes. They absorb tremendous damage before conceding ground.",
            BaseHp = 80, BaseAttack = 22, BaseDefense = 14,
            FleeChance = 0.20f, FleeHpThreshold = 0.15f,
            DefendDamageBonus = 0.75f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 50 },
                new() { Action = EnemyActionType.Defend,      Weight = 30 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 20 },
            ],
            BaseXp = 16,
            BaseSpeed = 5, CritChance = 0.05f,
            AttackDamageType = DT.Slashing,
            Weaknesses   = [DT.Storm],
            Resistances  = [DT.Bludgeoning, DT.Piercing, DT.Slashing],
            WeaknessText  = { [DT.Storm]       = "Lightning finds every nerve — the bear convulses." },
            ResistanceText = { [DT.Bludgeoning] = "Its bulk absorbs the impact without complaint.", [DT.Piercing] = "The hide is too dense to penetrate cleanly.", [DT.Slashing] = "Its thick fur and fat deflect the edge." },
            LootTable =
            [
                new() { ItemId = "BearHide",     Chance = 0.50f },
                new() { ItemId = "Claw",         Chance = 0.50f },
                new() { ItemId = "Herbs",        Chance = 0.40f },
                new() { ItemId = "Coal",         Chance = 0.20f },
                new() { ItemId = "SoothingSalve",Chance = 0.10f },
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
            Description = "Massive dungeon denizens with stone-like hide and terrifying regeneration. Fire and nature find the gaps; blunt force simply bounces off.",
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
            BaseXp = 30,
            BaseSpeed = 3, CritChance = 0.10f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses   = [DT.Fire, DT.Nature],
            Resistances  = [DT.Bludgeoning],
            WeaknessText  = { [DT.Fire]  = "The troll's hide chars and cracks — fire is an old enemy.", [DT.Nature] = "Vines and root find the joints between its stones." },
            ResistanceText = { [DT.Bludgeoning] = "Stone against stone. The troll barely registers it." },
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
            Description = "Small, deceptively fast creatures marked with arcane patterns. Their docile appearance belies sharp territorial instincts when cornered.",
            BaseHp = 12, BaseAttack = 4, BaseDefense = 1,
            FleeChance = 0.90f, FleeHpThreshold = 0.60f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 80 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 3,
            BaseSpeed = 9, CritChance = 0.03f,
            AttackDamageType = DT.Bludgeoning,
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
            Description = "A river-dwelling hybrid with the body of a beaver and the venomous strike of a serpent. Fiercely territorial near waterways and bogs.",
            BaseHp = 48, BaseAttack = 14, BaseDefense = 7,
            FleeChance = 0.35f, FleeHpThreshold = 0.20f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 55 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 15 },
                new() { Action = EnemyActionType.Defend,      Weight = 30 },
            ],
            BaseXp = 6,
            BaseSpeed = 6, CritChance = 0.03f,
            AttackDamageType = DT.Piercing,
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
            Description = "Fungal entities that drift through damp forests releasing toxic spores. They are drawn to decomposing matter and seem to share a hive awareness.",
            BaseHp = 18, BaseAttack = 7, BaseDefense = 2,
            FleeChance = 0.65f, FleeHpThreshold = 0.35f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,     Weight = 45 },
                new() { Action = EnemyActionType.Buff,       Weight = 35 },
                new() { Action = EnemyActionType.Regenerate, Weight = 20 },
            ],
            BaseXp = 5,
            BaseSpeed = 7, CritChance = 0.00f,
            AttackDamageType = DT.Nature,
            Weaknesses   = [DT.Fire],
            Resistances  = [DT.Nature],
            WeaknessText  = { [DT.Fire]   = "The cap catches immediately — mushrooms and fire never mix well." },
            ResistanceText = { [DT.Nature] = "It absorbs the natural energy, almost gratefully." },
            BuffStat = ModifierStat.Attack, BuffValue = 3f, BuffTurns = 2,
            RegenerateAmount = 5,
            LootTable =
            [
                new() { ItemId = "Herbs",       Chance = 0.55f },
                new() { ItemId = "Mushroom",    Chance = 0.45f },
                new() { ItemId = "PaleMushroom",Chance = 0.20f },
                new() { ItemId = "Moss",        Chance = 0.30f },
                new() { ItemId = "Antidote",    Chance = 0.15f },
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
            Description = "Spectral remnants that drift on hot, dry winds, barely distinguishable from the desert haze. They are drawn to the living like moths to a flame.",
            BaseHp = 38, BaseAttack = 17, BaseDefense = 4,
            FleeChance = 0.50f, FleeHpThreshold = 0.30f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 65 },
                new() { Action = EnemyActionType.Buff,   Weight = 35 },
            ],
            BaseXp = 6,
            BaseSpeed = 8, CritChance = 0.05f,
            AttackDamageType = DT.Dark,
            Weaknesses   = [DT.Storm],
            Resistances  = [DT.Dark],
            WeaknessText  = { [DT.Storm] = "Lightning scatters the sand-form, disrupting whatever binds it." },
            ResistanceText = { [DT.Dark]  = "Darkness does not harm darkness. The wraith is indifferent." },
            BuffStat = ModifierStat.Speed, BuffValue = 3f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "Sand",         Chance = 0.50f },
                new() { ItemId = "Ash",          Chance = 0.40f },
                new() { ItemId = "Flint",        Chance = 0.25f },
                new() { ItemId = "SoothingSalve",Chance = 0.12f },
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

        new()
        {
            Id = "TwiceBornHeron", Name = "Twice-Born Heron",
            Description = "A waterbird that died once and came back wrong — larger, colder, and far more aggressive. It haunts frozen shores and riverbanks with eerie patience.",
            BaseHp = 60, BaseAttack = 20, BaseDefense = 6,
            FleeChance = 0.65f, FleeHpThreshold = 0.30f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 50 },
                new() { Action = EnemyActionType.Buff,   Weight = 30 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 10,
            BaseSpeed = 8, CritChance = 0.05f,
            AttackDamageType = DT.Slashing,
            Weaknesses   = [DT.Storm],
            Resistances  = [DT.Frost],
            WeaknessText  = { [DT.Storm] = "Lightning crackles through the heron's hollow bones — it screams without a throat." },
            ResistanceText = { [DT.Frost] = "Death already took its warmth. Cold does nothing to what isn't alive." },
            BuffStat = ModifierStat.Speed, BuffValue = 4f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "Feathers",   Chance = 0.50f },
                new() { ItemId = "HeronQuill", Chance = 0.30f },
                new() { ItemId = "Reed",       Chance = 0.40f },
                new() { ItemId = "BoneFragment", Chance = 0.20f },
            ],
            AppearTexts =
            [
                "A heron stands perfectly still in the middle of the room. It was dead when it arrived. It still is.",
                "Something wading-bird-shaped turns toward you. Its eyes are the wrong color. Both of them.",
                "It was here before you. You get the sense it has been here for a long time, and was not alive for most of it.",
            ],
            AttackTexts =
            [
                "The beak drives forward with the precise efficiency of something that learned to hunt before it died.",
                "It strikes in a single motion. The dead don't telegraph.",
                "The wing catches you across the face. Cold and very solid.",
                "It lunges without any of the tells a living thing would have. This is worse.",
            ],
            BuffTexts =
            [
                "The heron shifts its weight onto one leg and seems to become slightly less present.",
                "It spreads its wings once, slowly. You realize you can see through it, slightly.",
            ],
            DefendTexts =
            [
                "It stands completely still. This is either a defense posture or what it normally does. Difficult to tell.",
                "The heron folds its wings in and stares at you from a direction that doesn't match where it is standing.",
            ],
            FleeTexts =
            [
                "It rises without sound and passes through the wall above your head.",
                "One moment it is there. Then it is simply gone, as if it remembered it was dead.",
                "The heron pivots on one leg and departs in a direction that should not be possible.",
            ],
            DeathTexts =
            [
                "It folds over sideways and does not move. This time it stays that way.",
                "The color goes out of it — what little remained — and it is still.",
                "It simply stops. Whatever was animating it has somewhere else to be.",
            ],
        },

        new()
        {
            Id = "StoneShepherd", Name = "Stone Shepherd",
            Description = "Ancient golems carved from mountain rock, tasked with guarding something long forgotten. They move with slow deliberation and hit like a falling cliff face.",
            BaseHp = 95, BaseAttack = 21, BaseDefense = 20,
            FleeChance = 0.08f, FleeHpThreshold = 0.08f,
            DefendDamageBonus = 0.80f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 40 },
                new() { Action = EnemyActionType.Defend,      Weight = 35 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
            ],
            BaseXp = 18,
            BaseSpeed = 4, CritChance = 0.05f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses   = [DT.Frost, DT.Storm],
            Resistances  = [DT.Bludgeoning, DT.Piercing, DT.Slashing],
            WeaknessText  = { [DT.Frost] = "Ice finds the hairline cracks in its stone and pries them open.", [DT.Storm] = "Lightning splits the rock — the shepherd staggers." },
            ResistanceText = { [DT.Bludgeoning] = "Stone is not moved by stone.", [DT.Piercing] = "Nothing penetrates granite.", [DT.Slashing] = "Your edge scrapes uselessly across the surface." },
            LootTable =
            [
                new() { ItemId = "Stone",       Chance = 0.60f },
                new() { ItemId = "Ore",         Chance = 0.35f },
                new() { ItemId = "Flint",       Chance = 0.25f },
                new() { ItemId = "HollowStone", Chance = 0.20f },
                new() { ItemId = "CrushedRock", Chance = 0.35f },
            ],
            AppearTexts =
            [
                "A shape steps out of the shadows. It is very large, and very slow, and made entirely of stone.",
                "You hear it before you see it: a deep, rhythmic grinding, like a millstone with somewhere to be.",
                "Something monumental turns toward you. It has been standing here for a long time. You have interrupted its standing.",
            ],
            AttackTexts =
            [
                "A stone fist descends like something geological.",
                "It swings an arm and the air moves before it does.",
                "The blow arrives slowly and then all at once.",
                "It reaches out with something that takes a full second to arrive. That second does not help.",
            ],
            HeavyAttackTexts =
            [
                "Both arms raise. Whatever comes next has been decided.",
                "The ground shifts slightly as it loads the swing. Then it lands.",
                "It brings everything to bear. There is a cracking sound that is not the stone.",
            ],
            DefendTexts =
            [
                "The shepherd plants both feet and does not move. There is no gap between it and the floor.",
                "It hunches slightly, pulling its stone arms inward. The surface area decreases meaningfully.",
                "It stops. This is, functionally, also its offense.",
            ],
            FleeTexts =
            [
                "It retreats with the unhurried gravity of something that has decided this fight is over.",
                "The stone shepherd turns and walks away. It does not quicken. It does not need to.",
                "It simply recedes, each step a small seismic event.",
            ],
            DeathTexts =
            [
                "It fragments slowly, from the outside in, until the floor is covered in pieces.",
                "The shepherd lists to one side and comes apart. The dust takes a while to settle.",
                "Whatever purpose held it together releases. It becomes a pile.",
            ],
        },

        new()
        {
            Id = "PaleLibrarian", Name = "Pale Librarian",
            Description = "Undead scholars still cataloguing texts that crumbled to dust ages ago. They can invoke terrible words mid-fight to amplify their own deadliness.",
            BaseHp = 55, BaseAttack = 18, BaseDefense = 9,
            FleeChance = 0.35f, FleeHpThreshold = 0.20f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 40 },
                new() { Action = EnemyActionType.Buff,   Weight = 40 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 14,
            BaseSpeed = 5, CritChance = 0.03f,
            AttackDamageType = DT.Dark,
            Weaknesses   = [DT.Fire],
            Resistances  = [DT.Dark],
            WeaknessText  = { [DT.Fire] = "The pages catch first. The librarian makes a sound like old paper igniting." },
            ResistanceText = { [DT.Dark] = "It is already in the dark. This changes nothing for it." },
            BuffStat = ModifierStat.Attack, BuffValue = 10f, BuffTurns = 3,
            LootTable =
            [
                new() { ItemId = "DampParchment", Chance = 0.40f },
                new() { ItemId = "AncientShard",  Chance = 0.35f },
                new() { ItemId = "BoneRune",      Chance = 0.25f },
                new() { ItemId = "BoneFragment",  Chance = 0.30f },
            ],
            AppearTexts =
            [
                "A robed figure stands against the far wall, writing in a book. It looks up. You are in the book now.",
                "Something dead and scholarly regards you with its empty sockets. It has already started writing.",
                "It was cataloging the room. You have become part of the room. This was not your plan.",
            ],
            AttackTexts =
            [
                "It strikes you with the corner of its book with more force than scholarship suggests.",
                "A gesture, precise and deliberate. You were not expecting it to hurt.",
                "It closes its book and uses the hand for something less academic.",
                "The swing is measured. It has probably done this before and taken notes on it.",
            ],
            BuffTexts =
            [
                "It opens its book to a specific page and reads something. Its posture changes.",
                "It writes something very quickly, consults it, and seems to find what it was looking for.",
                "It studies you for a moment with the thoroughness of something that is going to use this information.",
            ],
            DefendTexts =
            [
                "The librarian holds up its book like a shield. The cover is, unfortunately, quite thick.",
                "It steps back and takes careful notes on your position. This is still threatening.",
            ],
            FleeTexts =
            [
                "It closes its book and departs with the dignity of something that has documented enough.",
                "The librarian retreats down a corridor, still writing as it goes.",
                "It notes something in its book, underlines it, and leaves.",
            ],
            DeathTexts =
            [
                "The book falls open. The last entry ends mid-sentence.",
                "It collapses with a dry sound, like old paper. The notes scatter.",
                "The robes settle around nothing. The book remains.",
            ],
        },

        new()
        {
            Id = "FermentedThing", Name = "Fermented Thing",
            Description = "What happens when swamp rot crosses a threshold into something that moves on its own. Foul-smelling, surprisingly fast, and deeply hostile to everything.",
            BaseHp = 75, BaseAttack = 24, BaseDefense = 7,
            FleeChance = 0.25f, FleeHpThreshold = 0.15f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,     Weight = 45 },
                new() { Action = EnemyActionType.Buff,       Weight = 30 },
                new() { Action = EnemyActionType.Regenerate, Weight = 15 },
                new() { Action = EnemyActionType.Defend,     Weight = 10 },
            ],
            BaseXp = 16,
            BaseSpeed = 3, CritChance = 0.00f,
            AttackDamageType = DT.Nature,
            Weaknesses   = [DT.Fire],
            Resistances  = [DT.Nature, DT.Dark],
            WeaknessText  = { [DT.Fire]   = "The fermentation ignites with a deep, nauseating whump." },
            ResistanceText = { [DT.Nature] = "It accepts the organic energy and incorporates it.", [DT.Dark] = "Something in it has already rotted past the point where darkness can find purchase." },
            BuffStat = ModifierStat.Attack, BuffValue = 10f, BuffTurns = 2,
            RegenerateAmount = 8,
            LootTable =
            [
                new() { ItemId = "Peat",     Chance = 0.55f },
                new() { ItemId = "Moss",     Chance = 0.50f },
                new() { ItemId = "Herbs",    Chance = 0.35f },
                new() { ItemId = "Amber",    Chance = 0.15f },
                new() { ItemId = "Antidote", Chance = 0.12f },
            ],
            AppearTexts =
            [
                "The smell arrives first. Then the thing. Then you understand why it smells the way it does.",
                "Something that may once have been identifiable shuffles into view. That time has clearly passed.",
                "It has been fermenting for a very long time. You cannot determine what it was fermenting from.",
            ],
            AttackTexts =
            [
                "It lurches forward and makes contact in a way that is both imprecise and very effective.",
                "The impact is wet and warm and you would rather not dwell on why.",
                "It swings an appendage — or a protrusion — in your direction. It connects.",
                "Whatever it hit you with is already reattaching.",
            ],
            BuffTexts =
            [
                "It gurgles. Something inside it shifts. It seems more solid than before, in the worst sense.",
                "The fermentation intensifies. This is audible.",
                "It absorbs something from the air and visibly enlarges.",
            ],
            RegenerateTexts =
            [
                "New material rises from its surface and fills the gaps you made.",
                "The fermentation process is, apparently, ongoing. It looks less damaged than before.",
                "You watch it closing. The smell worsens.",
            ],
            DefendTexts =
            [
                "It lets itself soften slightly, becoming harder to meaningfully strike.",
                "It settles into itself and presents the most coherent surface it can manage.",
            ],
            FleeTexts =
            [
                "It withdraws by a method you cannot identify. One moment it is receding, then it is gone.",
                "It moves considerably faster than something of its constitution should be able to.",
                "The trail it leaves suggests it went that way. You do not follow.",
            ],
            DeathTexts =
            [
                "It deflates with a long, final exhale. The smell peaks, then fades slowly.",
                "Whatever coherence it had releases. It becomes, technically, a floor.",
                "It stops moving all at once. The fermentation, however, appears to continue.",
            ],
        },

        new()
        {
            Id = "TheArrangement", Name = "The Arrangement",
            Description = "Something that should not have a shape, wearing one anyway. Its origins and motives remain entirely opaque to all who have survived the encounter.",
            BaseHp = 150, BaseAttack = 24, BaseDefense = 14,
            FleeChance = 0.05f, FleeHpThreshold = 0.05f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 45 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
                new() { Action = EnemyActionType.Buff,        Weight = 20 },
                new() { Action = EnemyActionType.Defend,      Weight = 10 },
            ],
            BaseXp = 35,
            BaseSpeed = 5, CritChance = 0.10f,
            AttackDamageType = DT.Dark,
            Weaknesses   = [DT.Fire],
            Resistances  = [DT.Dark, DT.Slashing],
            WeaknessText  = { [DT.Fire] = "The objects scatter from the heat — fire disrupts whatever binds them." },
            ResistanceText = { [DT.Dark] = "The arrangement operates in the dark. It does not register the change.", [DT.Slashing] = "Blades pass through the gaps in the formation. Nothing there to cut." },
            BuffStat = ModifierStat.DamageMultiplier, BuffValue = 0.25f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "CrackedOrb",    Chance = 0.45f },
                new() { ItemId = "TarnishedRing", Chance = 0.30f },
                new() { ItemId = "RiverGlass",    Chance = 0.40f },
                new() { ItemId = "HollowStone",   Chance = 0.30f },
                new() { ItemId = "Amber",         Chance = 0.25f },
            ],
            AppearTexts =
            [
                "A chair. A wooden crate. A glass orb. They float in deliberate formation and turn toward you.",
                "You enter and something waits for you — but not a creature. A configuration. It adjusts.",
                "Three ordinary objects occupy the center of the room. They have a purpose. It involves you.",
            ],
            AttackTexts =
            [
                "The crate swings forward at exactly the wrong moment.",
                "The orb intercepts you from the left. You didn't know it was tracking you.",
                "They coordinate. You don't know how. The result is a very precise impact.",
                "Something hard, moving fast, from a direction you weren't watching.",
            ],
            HeavyAttackTexts =
            [
                "All three objects converge at once. The chair, the crate, the orb — unanimous.",
                "The arrangement reconfigures and delivers something considerably worse.",
                "They form a line and execute together. It feels more deliberate than it should.",
            ],
            BuffTexts =
            [
                "The objects rotate in formation. The configuration tightens. This has a purpose.",
                "A new arrangement. You do not know what it means, but it means something.",
            ],
            DefendTexts =
            [
                "They form a wall between you and themselves. It is a wall of ordinary objects. It still works.",
                "The arrangement spreads, making itself harder to address in one place.",
            ],
            FleeTexts =
            [
                "The objects drift apart and pass through separate walls. The room is empty.",
                "They reconfigure and are gone before you can determine where they went.",
                "The formation disperses with the unhurried calm of things that made a decision.",
            ],
            DeathTexts =
            [
                "The orb drops first. Then the crate. The chair takes a moment longer.",
                "They fall in sequence and are ordinary objects again.",
                "Whatever held them in formation lets go. They land on the floor and stay there.",
            ],
        },

        new()
        {
            Id = "CoronatedRat", Name = "Coronated Rat",
            Description = "A rat of unusual size and even more unusual ambition, wearing a crown of bent copper. It commands lesser vermin with imperious squeaks and surprising tactical cunning.",
            BaseHp = 175, BaseAttack = 26, BaseDefense = 12,
            FleeChance = 0.15f, FleeHpThreshold = 0.10f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,     Weight = 35 },
                new() { Action = EnemyActionType.Buff,       Weight = 30 },
                new() { Action = EnemyActionType.Regenerate, Weight = 20 },
                new() { Action = EnemyActionType.Defend,     Weight = 15 },
            ],
            BaseXp = 28,
            BaseSpeed = 8, CritChance = 0.12f,
            AttackDamageType = DT.Piercing,
            Weaknesses   = [DT.Fire, DT.Storm],
            WeaknessText  = { [DT.Fire] = "The crown rats scatter in panic. Fire breaks the quorum.", [DT.Storm] = "Lightning disperses the crown — the vote cannot be held in this weather." },
            BuffStat = ModifierStat.Attack, BuffValue = 8f, BuffTurns = 2,
            RegenerateAmount = 10,
            LootTable =
            [
                new() { ItemId = "RatCrown",    Chance = 0.35f },
                new() { ItemId = "BoneFragment",Chance = 0.55f },
                new() { ItemId = "Fiber",       Chance = 0.45f },
                new() { ItemId = "Stone",       Chance = 0.30f },
            ],
            AppearTexts =
            [
                "A crown enters the room first. Then you realize the crown is made of rats. Then you realize there is a large rat wearing them.",
                "Something regal in the worst sense rounds the corner. The rats that form its crown are voting.",
                "It is very large for a rat. The other rats riding atop it are, somehow, the more alarming feature.",
            ],
            AttackTexts =
            [
                "The coronated rat charges forward. The crown rats lean into it. They are in agreement.",
                "It lunges with the conviction of something that has been democratically endorsed to do so.",
                "A motion, fast and decisive. The vote was unanimous.",
                "It attacks with the full weight of a rat that carries other rats and has been given a mandate.",
            ],
            BuffTexts =
            [
                "The crown convenes. Something is decided. The rat's posture shifts accordingly.",
                "A rapid, chittering discussion from the top of its head. The decision is reached quickly. Unanimously.",
                "The rats confer. The coronated rat straightens. Whatever was just voted on bodes poorly for you.",
            ],
            RegenerateTexts =
            [
                "New rats arrive from somewhere and join the crown. The crown grows larger.",
                "Reinforcements. They came from the walls. The existing rats welcome them formally.",
                "The crown reconstitutes. The quorum is re-established.",
            ],
            DefendTexts =
            [
                "The crown rats form a layered barricade. The coronated rat holds position.",
                "It sits very still while the crown deliberates. Then: a decision to wait.",
            ],
            FleeTexts =
            [
                "The crown votes to leave. The motion passes. They go.",
                "The entire arrangement retreats through a hole that should not fit them. It fits them.",
                "The coronated rat reverses direction. The crown rats do not seem surprised by this outcome.",
            ],
            DeathTexts =
            [
                "The crown disperses first. The rats scatter in a dozen directions. The large rat follows more slowly.",
                "It tips over sideways. The crown rats file off in an orderly fashion before it lands.",
                "It stops. The last rat in the crown steps off, looks at you once, and leaves. This feels intentional.",
            ],
        },

        // ── Coven Hollow enemies ──────────────────────────────────────────────
        new()
        {
            Id = "Witch", Name = "Witch",
            Description = "A hedge witch with too much knowledge of uncomfortable things. She operates at the edge of forests and bogs, professional and unpleasant, and hexes herself before hexing you.",
            BaseHp = 45, BaseAttack = 16, BaseDefense = 7,
            FleeChance = 0.40f, FleeHpThreshold = 0.20f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 40 },
                new() { Action = EnemyActionType.Buff,   Weight = 40 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 12,
            BaseSpeed = 6, CritChance = 0.08f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Fire],
            Resistances = [DT.Dark, DT.Nature],
            WeaknessText  = { [DT.Fire]   = "Fire cuts through the hex. Witches have always had that problem." },
            ResistanceText = { [DT.Dark]   = "She works in the dark. It finds nothing to hold.", [DT.Nature] = "She knows every plant that could harm her. They hesitate." },
            BuffStat = ModifierStat.Attack, BuffValue = 10f, BuffTurns = 3,
            LootTable =
            [
                new() { ItemId = "Herbs",       Chance = 0.50f },
                new() { ItemId = "RottenSilks", Chance = 0.35f },
                new() { ItemId = "CrackedOrb",  Chance = 0.20f },
                new() { ItemId = "PaleMushroom",Chance = 0.25f },
            ],
            AppearTexts =
            [
                "She was already here. She was expecting you, specifically. She does not seem pleased about it.",
                "A woman at the edge of the treeline turns to face you. She has been watching for some time.",
                "She looks you over with the expression of someone pricing a replacement.",
            ],
            AttackTexts =
            [
                "She points at you with two fingers and something dark arrives from that direction.",
                "A word you do not recognize, delivered flatly, and then pain.",
                "She strikes without preamble or apparent effort.",
                "The hex lands before she finishes saying it. She is very efficient.",
            ],
            BuffTexts =
            [
                "She recites something under her breath and her eyes go briefly the color of mud.",
                "She traces a symbol on her own arm. It does not look comfortable. She does not look bothered.",
                "A short, clipped invocation. Her hands stop shaking. This is worse.",
            ],
            DefendTexts =
            [
                "She raises one hand and the air between you thickens slightly.",
                "She takes a step back, muttering, and something settles around her like a second skin.",
            ],
            FleeTexts =
            [
                "She turns and walks into the treeline at a pace that is not running but is still faster than yours.",
                "She says something over her shoulder that you cannot quite hear. The air smells different afterward.",
                "She steps sideways, and the undergrowth simply closes around her.",
                "She is gone. You are not sure when she decided to go.",
            ],
            DeathTexts =
            [
                "She sits down in the dirt and does not get up. She seems annoyed about this.",
                "She folds forward and the air around her goes flat and ordinary.",
                "She says one more word. It doesn't do anything. Then she is still.",
            ],
        },

        new()
        {
            Id = "WitchesBroom", Name = "Witch's Broom",
            Description = "An autonomous broom that has been given too many instructions or none at all. It has no eyes. It has tremendous purpose. These two facts combine badly.",
            BaseHp = 42, BaseAttack = 18, BaseDefense = 5,
            FleeChance = 0.60f, FleeHpThreshold = 0.30f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 70 },
                new() { Action = EnemyActionType.Defend, Weight = 30 },
            ],
            BaseXp = 7,
            BaseSpeed = 9, CritChance = 0.03f,
            AttackDamageType = DT.Slashing,
            Weaknesses  = [DT.Fire],
            Resistances = [DT.Dark],
            WeaknessText  = { [DT.Fire]  = "The bristles catch immediately. A broom has always been a fire hazard." },
            ResistanceText = { [DT.Dark]  = "It operates in the dark as a matter of routine. The darkness is not a threat." },
            LootTable =
            [
                new() { ItemId = "CrowFeather", Chance = 0.50f },
                new() { ItemId = "Fiber",       Chance = 0.40f },
                new() { ItemId = "Herbs",       Chance = 0.20f },
            ],
            AppearTexts =
            [
                "A broom stands upright in the center of the room. It turns toward you. It does not have a face to turn.",
                "You hear bristles on stone, advancing with the rhythm of something that knows exactly where it is going.",
                "The broom was leaning against the wall. Now it is not leaning. Now it is moving toward you.",
            ],
            AttackTexts =
            [
                "The bristle end catches you across the shins with surprising authority.",
                "It sweeps at your legs with the focused intensity of something that has always done this.",
                "The handle end swings around and connects with the side of your head.",
                "It makes no sound. It just arrives.",
            ],
            DefendTexts =
            [
                "The broom plants its handle and bristles outward, becoming an improbable barrier.",
                "It spins in place once, rapidly, as if resetting.",
            ],
            FleeTexts =
            [
                "The broom shoots across the floor and through a gap in the wall it should not have fit through.",
                "It reverses direction instantly and is simply gone.",
                "It retreats at speed, trailing bristles, vanishing around the corner before you can follow.",
                "It sweeps itself out of the situation.",
            ],
            DeathTexts =
            [
                "The broom falls over sideways. The bristles twitch once.",
                "It clatters to the floor and stays there, which is all it ever was before.",
                "The purpose goes out of it all at once and it is just a broom again.",
            ],
        },

        new()
        {
            Id = "WitchesCoven", Name = "Witches' Coven",
            Description = "Three or more witches achieving consensus, which is rare and dangerous. Treat as a single entity. It speaks in overlapping voices and has already decided what to do with you.",
            BaseHp = 55, BaseAttack = 19, BaseDefense = 8,
            FleeChance = 0.25f, FleeHpThreshold = 0.15f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 30 },
                new() { Action = EnemyActionType.Buff,   Weight = 50 },
                new() { Action = EnemyActionType.Defend, Weight = 20 },
            ],
            BaseXp = 15,
            BaseSpeed = 6, CritChance = 0.05f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Fire, DT.Storm],
            Resistances = [DT.Dark, DT.Nature],
            WeaknessText  = { [DT.Fire]  = "Fire disrupts consensus. Witches forget their arguments when something is burning.", [DT.Storm] = "Lightning strikes the tallest point. The group has several tallest points and loses composure." },
            ResistanceText = { [DT.Dark]  = "They have been meeting in the dark for years. This is familiar.", [DT.Nature] = "They grow half of what they use. The natural world cooperates with them." },
            BuffStat = ModifierStat.DamageMultiplier, BuffValue = 0.35f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "RottenSilks", Chance = 0.40f },
                new() { ItemId = "CrackedOrb",  Chance = 0.30f },
                new() { ItemId = "Herbs",       Chance = 0.35f },
                new() { ItemId = "PaleMushroom",Chance = 0.20f },
            ],
            AppearTexts =
            [
                "They are already speaking when you enter. Not to each other. To the room.",
                "Three figures. Then a fourth. The voices don't quite match the mouths they come from.",
                "They reach agreement about you before you finish opening the door.",
            ],
            AttackTexts =
            [
                "They speak the same word at the same moment and direct it at you.",
                "One points. The others continue working. The pointing is sufficient.",
                "The attack emerges from all of them simultaneously. This is why consensus is dangerous.",
                "They do not look up. The hex arrives anyway.",
            ],
            BuffTexts =
            [
                "They confer quietly. The voices overlap until they become one voice. Something shifts in the air.",
                "A brief, crackling silence. Then agreement. Then the air around them tightens.",
                "They reach consensus. You can tell because the room gets colder.",
            ],
            DefendTexts =
            [
                "They arrange themselves into a shape that is not quite a circle and seems to help anyway.",
                "They speak together and the space between you goes briefly resistant.",
            ],
            FleeTexts =
            [
                "They agree to leave and do so in unison, their footsteps perfectly matched.",
                "The voices stop. The figures go. Not running — withdrawing, with dignity.",
                "They disperse through separate exits simultaneously, as if this was always the plan.",
            ],
            DeathTexts =
            [
                "The consensus breaks. They become individuals again, and then they are still.",
                "One voice drops out, then another, then the last. The room is quiet.",
                "They settle to the ground in sequence, still arranged, even at the end.",
            ],
        },

        new()
        {
            Id = "WitchesOven", Name = "Witch's Oven",
            Description = "A furnace that has developed opinions. It does not flee. It has a door. The door opens. This is how it attacks. Something inside is always cooking. The smell is deeply wrong.",
            BaseHp = 55, BaseAttack = 18, BaseDefense = 10,
            FleeChance = 0.0f, FleeHpThreshold = 0.0f,
            DefendDamageBonus = 0.50f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 30 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 50 },
                new() { Action = EnemyActionType.Defend,      Weight = 20 },
            ],
            BaseXp = 18,
            BaseSpeed = 2, CritChance = 0.00f,
            AttackDamageType = DT.Fire,
            Weaknesses  = [DT.Frost, DT.Storm],
            Resistances = [DT.Fire],
            WeaknessText  = { [DT.Frost]  = "Cold extinguishes the fire inside. The oven goes quiet.", [DT.Storm] = "Lightning finds the iron body and grounds itself thoroughly through it." },
            ResistanceText = { [DT.Fire]   = "The fire is on the inside. Additional fire is simply fuel." },
            LootTable =
            [
                new() { ItemId = "Ash",       Chance = 0.50f },
                new() { ItemId = "Coal",      Chance = 0.40f },
                new() { ItemId = "Herbs",     Chance = 0.30f },
                new() { ItemId = "CrackedOrb",Chance = 0.10f },
            ],
            AppearTexts =
            [
                "An oven stands in the center of the room. It is running. Nobody lit it. The door is closed. For now.",
                "You smell it before you see it. Then you see it, and the smell becomes more relevant.",
                "The oven is bolted to nothing. It is also watching you, in the way that ovens should not be able to.",
            ],
            AttackTexts =
            [
                "The door opens. Something comes out briefly. You are involved.",
                "A burst of heat and intent, directed.",
                "It leans — somehow — and makes contact with the corner of its iron body.",
                "The door swings wide and the inside temperature becomes your problem.",
            ],
            HeavyAttackTexts =
            [
                "The door flings fully open and the interior commits to you entirely.",
                "It heaves forward and brings the full weight of cast iron to bear.",
                "The door opens wide and something that has been cooking too long comes out with conviction.",
            ],
            DefendTexts =
            [
                "The door closes with a decisive clang. The surface gets hotter.",
                "It hunkers down, sealing every gap. You could not open it if you tried.",
                "It becomes very still. The heat radiating from it is the only indication that anything is happening.",
            ],
            DeathTexts =
            [
                "The door swings open one final time and does not close. The fire inside goes out.",
                "It tips over with an enormous crash. The smell dissipates slowly. The contents remain unidentified.",
                "It goes cold. Not gradually — all at once, which is its own kind of unsettling.",
            ],
        },

        new()
        {
            Id = "WitchesMotherInLaw", Name = "Witch's Mother-in-Law",
            Description = "Nobody summoned her. She arrived on her own. She has extremely detailed opinions about everything you are doing wrong and the patience to enumerate them during combat.",
            BaseHp = 140, BaseAttack = 22, BaseDefense = 12,
            FleeChance = 0.05f, FleeHpThreshold = 0.05f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 30 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
                new() { Action = EnemyActionType.Buff,        Weight = 30 },
                new() { Action = EnemyActionType.Regenerate,  Weight = 15 },
            ],
            BaseXp = 32,
            BaseSpeed = 6, CritChance = 0.10f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Fire, DT.Storm],
            Resistances = [DT.Dark, DT.Nature],
            WeaknessText  = { [DT.Fire]  = "Fire is one of very few things she has nothing to say about. This is disorienting for her.", [DT.Storm] = "Lightning interrupts her mid-sentence. She does not recover immediately." },
            ResistanceText = { [DT.Dark]  = "She has seen things you cannot imagine. The dark is not among them.", [DT.Nature] = "She has been managing a garden for sixty years. Nature does not surprise her." },
            BuffStat = ModifierStat.DamageMultiplier, BuffValue = 0.25f, BuffTurns = 2,
            RegenerateAmount = 8,
            LootTable =
            [
                new() { ItemId = "RottenSilks", Chance = 0.60f },
                new() { ItemId = "CrackedOrb",  Chance = 0.50f },
                new() { ItemId = "PaleMushroom",Chance = 0.30f },
                new() { ItemId = "Amber",       Chance = 0.25f },
                new() { ItemId = "Spyglass",    Chance = 0.03f },
            ],
            AppearTexts =
            [
                "She is already in the room. She has opinions about that too.",
                "A voice precedes her — calm, precise, cataloguing your faults in order of severity.",
                "She enters without announcement and begins making corrections immediately.",
            ],
            AttackTexts =
            [
                "She strikes with the authority of someone who has been right about everything for a very long time.",
                "A hex delivered mid-sentence, without interrupting the sentence.",
                "She does not raise her voice. The impact is considerable anyway.",
                "You are hit, and then told exactly what you did to cause it.",
            ],
            HeavyAttackTexts =
            [
                "She reaches the end of a point and punctuates it with something catastrophic.",
                "The full force of accumulated grievance, delivered precisely.",
                "She hits you with the confidence of someone who has been waiting for this moment for years.",
            ],
            BuffTexts =
            [
                "She squares her shoulders and decides to take this more seriously.",
                "She recites a list of her own credentials. Something in the air shifts in her favor.",
                "She finds a new thread of grievance and follows it somewhere terrible.",
            ],
            RegenerateTexts =
            [
                "She straightens up, looks you over, and seems to find you lacking in sufficient ways to recover.",
                "She draws something from a deep, private reserve of indignation.",
                "She takes a long breath and lets it out slowly. She appears freshened.",
            ],
            FleeTexts =
            [
                "She leaves on her own terms, having documented this thoroughly.",
                "She withdraws with a final observation that continues to sting after she is gone.",
            ],
            DeathTexts =
            [
                "She goes still mid-sentence. The sentence was not finished. This will bother you.",
                "She sits down, primly, and does not get up. Her posture remains impeccable.",
                "The last thing she does is give you a look that communicates her opinion of how this went.",
            ],
        },

        // ── Buried Carnival enemies ───────────────────────────────────────────
        new()
        {
            Id = "StiltedMan", Name = "Stilted Man",
            Description = "A carnival performer on stilts that are, at this point, part of him. He has been on them too long. He moves wrong. He is very tall. Hitting him requires considerable upward effort.",
            BaseHp = 35, BaseAttack = 13, BaseDefense = 5,
            FleeChance = 0.35f, FleeHpThreshold = 0.20f,
            DefendDamageBonus = 0.60f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 50 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 30 },
                new() { Action = EnemyActionType.Defend,      Weight = 20 },
            ],
            BaseXp = 9,
            BaseSpeed = 5, CritChance = 0.08f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses = [DT.Slashing, DT.Storm],
            WeaknessText  = { [DT.Slashing] = "The stilts are structural. A blade finds the joints.", [DT.Storm] = "He is very tall. Lightning notices." },
            LootTable =
            [
                new() { ItemId = "Fiber",      Chance = 0.40f },
                new() { ItemId = "Rope",       Chance = 0.30f },
                new() { ItemId = "Feathers",   Chance = 0.20f },
                new() { ItemId = "CrowFeather",Chance = 0.15f },
            ],
            AppearTexts =
            [
                "Something very tall moves in the dark above you. Then it steps forward and you realize it has feet, eventually.",
                "He enters from the far end and crosses the space between you in fewer steps than he should need.",
                "He is looking down at you from a height that requires you to recalibrate the encounter.",
            ],
            AttackTexts =
            [
                "A stilt comes down in your vicinity. It is both slower and worse than you anticipated.",
                "He leans and strikes from a direction you weren't watching because it was above you.",
                "He shuffles his weight and one arm swings down from a great height.",
                "The reach is longer than you planned for. This is a recurring problem with tall things.",
            ],
            HeavyAttackTexts =
            [
                "He rocks back on his stilts and falls forward deliberately. This is the attack.",
                "He tips toward you with the full weight of his considerable altitude.",
                "He teeters, and for a moment you think he's falling. He is. At you.",
            ],
            DefendTexts =
            [
                "He plants the stilts and spreads his arms. He is now a very large, very awkward obstacle.",
                "He crouches — slightly — and the reduction in height is more threatening than you expected.",
            ],
            FleeTexts =
            [
                "He turns and lopes away with a stride that covers too much ground per step.",
                "The stilts carry him away over obstacles that would stop someone at normal elevation.",
                "He ducks through a door that was too large for a normal person and too small for him. He goes anyway.",
                "He retreats with surprising grace for someone whose feet are six feet off the ground.",
            ],
            DeathTexts =
            [
                "He tips over. The fall takes a very long time. The landing is significant.",
                "He goes in stages — stilts first, then the rest of him, then the stilts again.",
                "He comes down. It takes a while. The floor notices.",
            ],
        },

        new()
        {
            Id = "CollapsedClown", Name = "Collapsed Clown",
            Description = "A clown who has given up on most things but not this. The makeup is still on. Some of it. The shoes are still enormous. Combat is chaotic and low to the ground. The honking is involuntary.",
            BaseHp = 36, BaseAttack = 15, BaseDefense = 4,
            FleeChance = 0.60f, FleeHpThreshold = 0.35f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 70 },
                new() { Action = EnemyActionType.Buff,   Weight = 30 },
            ],
            BaseXp = 5,
            BaseSpeed = 4, CritChance = 0.03f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses = [DT.Fire, DT.Slashing],
            WeaknessText  = { [DT.Fire]    = "The costume is extremely flammable. This was always a workplace safety concern.", [DT.Slashing] = "He moves too low to dodge something that comes at ankle height. The blade finds him anyway." },
            BuffStat = ModifierStat.Speed, BuffValue = 4f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "Fiber",      Chance = 0.50f },
                new() { ItemId = "CrowFeather",Chance = 0.30f },
                new() { ItemId = "Feathers",   Chance = 0.35f },
            ],
            AppearTexts =
            [
                "Something is already on the ground. Then it gets up. This is the clown.",
                "The shoes arrive first, sliding around the corner. The rest of the clown follows at speed.",
                "He was lying in the middle of the room. He is not anymore. He has noticed you.",
            ],
            AttackTexts =
            [
                "He dives at your ankles with the full commitment of someone with nothing to lose.",
                "A shoe connects with something. The honking is not intentional.",
                "He rolls, stands, hits, falls over. Only the middle part was planned.",
                "The attack is low, fast, and badly organized. It still lands.",
            ],
            BuffTexts =
            [
                "He hits himself on both sides of the head and something clarifies behind his eyes.",
                "He shakes his collar and does something with his feet that seems to help.",
                "He stands up very straight, which is new, and seems briefly more dangerous for it.",
            ],
            FleeTexts =
            [
                "He falls over in the direction of the exit and keeps going.",
                "He scrambles away on all fours, which is faster than his upright mode.",
                "The shoes carry him away at a speed that seems to surprise him as much as you.",
                "He is gone before you can articulate what he was.",
            ],
            DeathTexts =
            [
                "He falls over for the last time. The honking stops.",
                "He goes flat against the floor with the resignation of something that was already mostly there.",
                "The shoes remain. The rest of him is still inside them, technically.",
            ],
        },

        new()
        {
            Id = "FunhouseMirror", Name = "Funhouse Mirror",
            Description = "A mirror that reflects wrong and hits back. The reflection inside is not yours. It is doing things you are not doing. When it moves toward the glass from the inside, that is the attack.",
            BaseHp = 40, BaseAttack = 15, BaseDefense = 6,
            FleeChance = 0.50f, FleeHpThreshold = 0.25f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 40 },
                new() { Action = EnemyActionType.Buff,   Weight = 50 },
                new() { Action = EnemyActionType.Defend, Weight = 10 },
            ],
            BaseXp = 13,
            BaseSpeed = 6, CritChance = 0.05f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Bludgeoning],
            Resistances = [DT.Slashing, DT.Piercing],
            WeaknessText  = { [DT.Bludgeoning] = "Glass breaks. The reflection breaks with it." },
            ResistanceText = { [DT.Slashing]    = "The blade passes through at an angle the glass redirects.", [DT.Piercing] = "The point finds the reflection rather than the surface." },
            BuffStat = ModifierStat.Speed, BuffValue = 5f, BuffTurns = 3,
            LootTable =
            [
                new() { ItemId = "RiverGlass",  Chance = 0.50f },
                new() { ItemId = "CrackedOrb",  Chance = 0.35f },
                new() { ItemId = "HollowStone", Chance = 0.20f },
            ],
            AppearTexts =
            [
                "A tall mirror stands in the middle of the room. Your reflection is already doing something different.",
                "The reflection in it is you, adjusted. The adjustments are not flattering and they are moving on their own.",
                "You look at it. What is inside looks back, and then does something you did not do.",
            ],
            AttackTexts =
            [
                "The reflection presses itself against the glass from the inside. The glass comes forward.",
                "It mimics a strike you didn't make and it lands anyway.",
                "Something comes through the surface. It should not be able to do that.",
                "The reflection reaches the edge of the mirror and keeps going briefly.",
            ],
            BuffTexts =
            [
                "The image inside shifts and warps until you can barely locate it. It likes this.",
                "The reflection performs a gesture that does something to the glass, which then does something to the air.",
                "It stretches itself across the surface until it is everywhere and nowhere in particular.",
            ],
            DefendTexts =
            [
                "The mirror tilts slightly and you see only the ceiling. The reflection waits.",
                "The surface goes opaque. Whatever is on the other side of it is now invisible.",
            ],
            FleeTexts =
            [
                "The reflection walks away inside the mirror and takes the mirror with it somehow.",
                "The glass goes dark and the whole thing slides backward into the wall.",
                "The reflection turns away from the glass, and the mirror follows it into somewhere else.",
            ],
            DeathTexts =
            [
                "The glass cracks. The reflection cracks with it, and then both go still.",
                "It shatters. What was inside is gone. What is on the floor is just glass.",
                "The image fragments and the surface goes flat and ordinary. The pieces are just pieces.",
            ],
        },

        new()
        {
            Id = "TheRingmaster", Name = "The Ringmaster",
            Description = "The show must go on. He has been saying this for too long, to an audience that left, in a tent that is no longer standing. His voice still carries. His presence commands the room.",
            BaseHp = 160, BaseAttack = 25, BaseDefense = 13,
            FleeChance = 0.08f, FleeHpThreshold = 0.08f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 30 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
                new() { Action = EnemyActionType.Buff,        Weight = 35 },
                new() { Action = EnemyActionType.Defend,      Weight = 10 },
            ],
            BaseXp = 30,
            BaseSpeed = 7, CritChance = 0.12f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Fire],
            Resistances = [DT.Dark],
            WeaknessText  = { [DT.Fire]  = "The coat catches. His authority does not extend to fire." },
            ResistanceText = { [DT.Dark]  = "He has worked in dark tents his entire career. Darkness is atmosphere." },
            BuffStat = ModifierStat.DamageMultiplier, BuffValue = 0.45f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "Rope",          Chance = 0.60f },
                new() { ItemId = "CrowFeather",   Chance = 0.50f },
                new() { ItemId = "TarnishedRing", Chance = 0.40f },
                new() { ItemId = "Amber",         Chance = 0.25f },
                new() { ItemId = "Compass",       Chance = 0.04f },
            ],
            AppearTexts =
            [
                "He steps into the center of the room and gestures as though presenting something. There is nothing. He presents it anyway.",
                "His voice arrives before he does. It is a voice that expects to be listened to, and has not yet noticed that nobody is.",
                "He is wearing the coat. The coat is enormous. He fills the room the way only someone who has done this for decades can.",
            ],
            AttackTexts =
            [
                "The whip cracks and the sound alone is a commitment.",
                "He gestures and something happens. He has always been very good at that.",
                "He strikes with the precision of someone who spent years directing others to do exactly this.",
                "The cane connects with a flourish that he clearly finds satisfying.",
            ],
            HeavyAttackTexts =
            [
                "He builds to it — a full introduction — and then delivers.",
                "He announces this attack. You are still hit.",
                "The show reaches its climax. You are the climax. This is not good.",
            ],
            BuffTexts =
            [
                "He addresses the empty stands and draws something from their attention anyway.",
                "He straightens his coat, adjusts his hat, and seems taller. He was already very tall.",
                "He claps twice and the echo in the room changes. He used that.",
            ],
            DefendTexts =
            [
                "He raises a hand and the act pauses. Even now, he controls the pace.",
                "He steps back and looks at you with the calm of someone between acts.",
            ],
            FleeTexts =
            [
                "He announces an intermission and withdraws through a curtain that is not there but parts anyway.",
                "He tips his hat and departs with the unhurried authority of someone ending a performance on his terms.",
            ],
            DeathTexts =
            [
                "He goes down slowly, with ceremony, as if he planned this too.",
                "The coat settles around him. The hat stays on. He remains presentable.",
                "He falls. The empty stands receive it in silence. He would have preferred applause.",
            ],
        },

        // ── Drowned Estate enemies ────────────────────────────────────────────
        new()
        {
            Id = "SoggyButler", Name = "Soggy Butler",
            Description = "A formally dressed drowned servant who insists on protocol. He has been in the water for decades but his posture has not suffered. The tray is still level.",
            BaseHp = 50, BaseAttack = 17, BaseDefense = 10,
            FleeChance = 0.20f, FleeHpThreshold = 0.10f,
            DefendDamageBonus = 0.65f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 35 },
                new() { Action = EnemyActionType.Defend,      Weight = 50 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 15 },
            ],
            BaseXp = 13,
            BaseSpeed = 4, CritChance = 0.03f,
            AttackDamageType = DT.Bludgeoning,
            Resistances = [DT.Bludgeoning],
            ResistanceText = { [DT.Bludgeoning] = "He has absorbed many impacts over the years. Decades underwater have only improved his tolerance." },
            LootTable =
            [
                new() { ItemId = "Peat",          Chance = 0.40f },
                new() { ItemId = "Fiber",         Chance = 0.35f },
                new() { ItemId = "BoneFragment",  Chance = 0.20f },
                new() { ItemId = "TarnishedRing", Chance = 0.10f },
            ],
            AppearTexts =
            [
                "A butler enters. He is dripping. His uniform is immaculate. He presents these facts as equally important.",
                "He arrives with the bearing of someone who has been waiting to be of service and is now available.",
                "Water runs from his cuffs as he straightens his jacket. He does not acknowledge it. Neither should you.",
            ],
            AttackTexts =
            [
                "He strikes with the tray. He does not put it down first.",
                "A precise blow delivered without expression or commentary.",
                "He addresses you with the elbow and returns to attention.",
                "He indicates, formally, that this is necessary, and then does it.",
            ],
            HeavyAttackTexts =
            [
                "He sets the tray down, squares himself, and provides service of a different kind.",
                "He delivers it with both hands. The tray is retrieved immediately afterward.",
                "He briefly abandons decorum. The result is considerable.",
            ],
            DefendTexts =
            [
                "He raises the tray and waits. His posture is exactly correct.",
                "He does not move. He simply becomes more present, which turns out to be a defense.",
                "He positions himself between you and whatever he is protecting, which appears to be his dignity.",
            ],
            FleeTexts =
            [
                "He excuses himself, formally, and steps backward through a door he did not come through.",
                "He withdraws with a short bow and does not explain where he is going.",
                "He leaves. The water on the floor marks where he stood.",
            ],
            DeathTexts =
            [
                "He folds at the waist, catches himself, and then completes the fold.",
                "He goes down still holding the tray level. It takes another moment for the tray to fall.",
                "He settles into the water on the floor. He looks, if anything, more comfortable.",
            ],
        },

        new()
        {
            Id = "DecomposedHound", Name = "Decomposed Hound",
            Description = "A dog that returned from a state of being dead. Still loyal. Still a good dog. The condition is temporary in the sense that most conditions are. It is very fast and the barking is wrong.",
            BaseHp = 40, BaseAttack = 19, BaseDefense = 6,
            FleeChance = 0.45f, FleeHpThreshold = 0.20f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 70 },
                new() { Action = EnemyActionType.Buff,   Weight = 30 },
            ],
            BaseXp = 11,
            BaseSpeed = 7, CritChance = 0.05f,
            AttackDamageType = DT.Slashing,
            Weaknesses  = [DT.Fire, DT.Storm],
            Resistances = [DT.Dark],
            WeaknessText  = { [DT.Fire]  = "It recoils from the flame. Some instincts survive the transition.", [DT.Storm] = "Lightning disperses whatever holds the pieces together. It briefly becomes more of a suggestion." },
            ResistanceText = { [DT.Dark]  = "It has been in the dark for a long time. The dark is comfortable." },
            BuffStat = ModifierStat.Attack, BuffValue = 6f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "BoneFragment", Chance = 0.50f },
                new() { ItemId = "Peat",         Chance = 0.35f },
                new() { ItemId = "Feathers",     Chance = 0.20f },
            ],
            AppearTexts =
            [
                "It rounds the corner at speed and skids to a stop. It is very obviously a dog. It is very obviously not fine.",
                "You hear it before you see it — the nails, the panting, and then a third sound that is not either of those.",
                "It sits. It looks at you. Its tail moves. This is the worst part.",
            ],
            AttackTexts =
            [
                "It lunges with the loyalty of something that has found a purpose and applied it to you.",
                "The bite is fast and certain. It has done this before, in life and since.",
                "It circles once and then commits, completely.",
                "It is very eager. The eagerness is the most unsettling thing about it.",
            ],
            BuffTexts =
            [
                "It makes a sound that is trying to be a bark and is something adjacent to that.",
                "It shakes itself once, which dislodges some things. It seems to find this clarifying.",
                "It lowers its head and the wrong sound comes again and it looks more focused.",
            ],
            FleeTexts =
            [
                "It backs away, still facing you, still wagging, and then turns and goes.",
                "It retreats with the same speed it arrived with, which is considerable.",
                "It decides and leaves before you can follow the decision.",
                "It goes somewhere. It will go there very fast.",
            ],
            DeathTexts =
            [
                "It lies down. It looks comfortable. It is still.",
                "It settles into the mud as if returning to something it knows.",
                "Whatever kept it moving lets go. It stays a dog.",
            ],
        },

        new()
        {
            Id = "BogNoble", Name = "Bog Noble",
            Description = "An armored aristocrat who sank with his estate and never left. He is still technically the lord of this land. The land is the bottom of a bog. He is very formal about this.",
            BaseHp = 65, BaseAttack = 20, BaseDefense = 16,
            FleeChance = 0.10f, FleeHpThreshold = 0.08f,
            DefendDamageBonus = 0.75f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 30 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 20 },
                new() { Action = EnemyActionType.Defend,      Weight = 50 },
            ],
            BaseXp = 17,
            BaseSpeed = 4, CritChance = 0.05f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses  = [DT.Storm],
            Resistances = [DT.Bludgeoning, DT.Piercing],
            WeaknessText  = { [DT.Storm]        = "Lightning finds the metal in the armor and makes its displeasure known through every joint." },
            ResistanceText = { [DT.Bludgeoning]  = "The armor absorbed impacts for three centuries before the bog. Still works.", [DT.Piercing] = "The plate has not worn through. Whatever you are pointing at it is not getting through." },
            LootTable =
            [
                new() { ItemId = "TarnishedRing",  Chance = 0.40f },
                new() { ItemId = "BoneFragment",   Chance = 0.35f },
                new() { ItemId = "Peat",           Chance = 0.30f },
                new() { ItemId = "TarnishedRelic", Chance = 0.15f },
            ],
            AppearTexts =
            [
                "He rises from the water already standing, which is not the order things usually go.",
                "He is wearing armor that has been in a bog for a very long time. He is wearing it correctly.",
                "He regards you with the measured displeasure of a landowner who has found trespassers.",
            ],
            AttackTexts =
            [
                "He strikes with a formality that suggests he has done this to many unauthorized visitors.",
                "The mace comes around with the weight of centuries of accumulated grievance.",
                "A measured blow, properly executed. He does things correctly.",
                "He says nothing. He acts. This is what lords do.",
            ],
            HeavyAttackTexts =
            [
                "He draws himself up to full height and delivers a statement.",
                "He swings with the authority of someone who still considers this his land.",
                "It is a declaration more than a strike. The declaration has mass.",
            ],
            DefendTexts =
            [
                "He plants himself and holds. The armor and the attitude are both considerable.",
                "He raises his shield in a way that makes clear he will not be moved by anything you can manage.",
                "He stands his ground. This is, to him, simply standing. He has always stood on this ground.",
            ],
            FleeTexts =
            [
                "He retreats without turning, maintaining the authority of someone who chose to go.",
                "He withdraws into the deeper water, still upright, until he is gone.",
            ],
            DeathTexts =
            [
                "He sinks back into the bog from which he rose. The water closes over the armor.",
                "He goes down piece by piece, with the slow collapse of something that refused to fall quickly.",
                "He stops. For the second time. He stays down this time.",
            ],
        },

        new()
        {
            Id = "TheDrowningLord", Name = "The Drowning Lord",
            Description = "He drowned in his own estate. He has since drowned several times more. He has developed a position on this. His hands are very cold. The water in his lungs has opinions.",
            BaseHp = 130, BaseAttack = 22, BaseDefense = 13,
            FleeChance = 0.05f, FleeHpThreshold = 0.05f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 35 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
                new() { Action = EnemyActionType.Regenerate,  Weight = 25 },
                new() { Action = EnemyActionType.Defend,      Weight = 15 },
            ],
            BaseXp = 31,
            BaseSpeed = 5, CritChance = 0.10f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Storm],
            Resistances = [DT.Dark, DT.Frost],
            WeaknessText  = { [DT.Storm]  = "Lightning and water are old enemies. He is mostly water." },
            ResistanceText = { [DT.Dark]   = "He has been in the dark at the bottom of his estate for decades. It is his home.", [DT.Frost] = "He has been cold for a very long time. This is not news to him." },
            RegenerateAmount = 10,
            LootTable =
            [
                new() { ItemId = "TarnishedRelic", Chance = 0.60f },
                new() { ItemId = "BoneFragment",   Chance = 0.50f },
                new() { ItemId = "Peat",           Chance = 0.40f },
                new() { ItemId = "RiverGlass",     Chance = 0.30f },
                new() { ItemId = "Lantern",        Chance = 0.03f },
            ],
            AppearTexts =
            [
                "He rises from the water and the water rises with him, briefly.",
                "Something vast and waterlogged assembles itself at the far end of the room.",
                "He has drowned several times. It shows. He is very calm about it.",
            ],
            AttackTexts =
            [
                "He reaches forward and his hands are very cold and very certain.",
                "He strikes with the accumulated weight of repeated drowning.",
                "The blow carries water and depth and several decades of unresolved situation.",
                "He does not hurry. He has been in the water long enough to understand patience.",
            ],
            HeavyAttackTexts =
            [
                "He gathers himself and delivers something that carries the pressure of deep water.",
                "He pulls back and swings forward and the water in him goes with the motion.",
                "He makes a sound like lungs, then commits entirely.",
            ],
            DefendTexts =
            [
                "He stands very still. The water dripping from him is the only sound.",
                "He raises his arms and the water around him helps, slightly.",
                "He holds his ground with the confidence of something that has already survived the worst.",
            ],
            RegenerateTexts =
            [
                "Water seeps into the wounds from outside. They close, slowly, from the inside.",
                "He breathes in. The sound this makes is not the sound breathing should make. He looks better.",
                "He draws himself back together. It takes a moment. It works.",
            ],
            FleeTexts =
            [
                "He sinks back into the floor, which is wet enough to accept this.",
                "He withdraws through the water with the ease of something that lives there.",
            ],
            DeathTexts =
            [
                "He goes under. This time the water keeps him.",
                "He sinks into himself, slowly, and the water follows.",
                "He stops. For the last time. The water does not.",
            ],
        },

        // ── Ancient Tomb enemies ──────────────────────────────────────────────
        new()
        {
            Id = "TombRobber", Name = "Tomb Robber",
            Description = "A living thief who got trapped in here with everything else. He was here before you. He has been here for a long time. He is not doing well. He will take everything you have if you let him.",
            BaseHp = 50, BaseAttack = 18, BaseDefense = 7,
            FleeChance = 0.50f, FleeHpThreshold = 0.25f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 50 },
                new() { Action = EnemyActionType.Buff,   Weight = 35 },
                new() { Action = EnemyActionType.Defend, Weight = 15 },
            ],
            BaseXp = 8,
            BaseSpeed = 7, CritChance = 0.08f,
            AttackDamageType = DT.Piercing,
            Weaknesses = [DT.Storm],
            WeaknessText  = { [DT.Storm]  = "He grew up aboveground. Lightning still startles him the way it startles the living." },
            BuffStat = ModifierStat.Speed, BuffValue = 3f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "TidalCoin",   Chance = 0.40f },
                new() { ItemId = "BoneFragment",Chance = 0.30f },
                new() { ItemId = "Flint",       Chance = 0.25f },
                new() { ItemId = "AncientShard",Chance = 0.15f },
            ],
            AppearTexts =
            [
                "A man steps out of the dark with the look of someone who has been here long enough to stop being surprised.",
                "He was already watching you. He has been watching the entrance for some time.",
                "He is wearing things from the tomb. Some of them belonged to someone. He found them here.",
            ],
            AttackTexts =
            [
                "He stabs quickly and steps back before you can answer it.",
                "The blade comes from the low angle, which is not where you were looking.",
                "He is fast and quiet and has been practicing on things that can't move.",
                "A quick thrust, efficient, and he is already repositioning.",
            ],
            BuffTexts =
            [
                "He moves into a shadow and you lose him for a moment. He uses that moment.",
                "He rolls his shoulder and shifts his weight and becomes harder to read.",
                "He takes a breath and something in his stance clarifies.",
            ],
            DefendTexts =
            [
                "He puts something between you and himself and waits.",
                "He backs into a corner with the calm of someone who knows where all the corners are.",
            ],
            FleeTexts =
            [
                "He goes. Fast. Through a passage you did not know was there.",
                "He was here and then he was not. He has had a lot of practice leaving quickly.",
                "He takes something on the way out. You notice this a moment too late.",
                "He exits considerably faster than he entered.",
            ],
            DeathTexts =
            [
                "He falls between two sarcophagi and stays there, which feels appropriate.",
                "He goes down with the look of someone who knew this was a possibility.",
                "He collapses and does not get up. He has been in worse positions. This one is final.",
            ],
        },

        new()
        {
            Id = "SarcophagusGuard", Name = "Sarcophagus Guard",
            Description = "An ancient mummified soldier, still at post. The wrappings are somewhat worse for wear after several thousand years but the intention has held up extremely well. It has not received updated orders. It does not require them.",
            BaseHp = 85, BaseAttack = 23, BaseDefense = 18,
            FleeChance = 0.05f, FleeHpThreshold = 0.05f,
            DefendDamageBonus = 0.80f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 35 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
                new() { Action = EnemyActionType.Defend,      Weight = 40 },
            ],
            BaseXp = 19,
            BaseSpeed = 4, CritChance = 0.05f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses  = [DT.Storm, DT.Nature],
            Resistances = [DT.Bludgeoning, DT.Slashing, DT.Piercing],
            WeaknessText  = { [DT.Storm]        = "Lightning disrupts the binding. The wrappings spark.", [DT.Nature] = "Vines find the gaps in the wrappings and lever them apart." },
            ResistanceText = { [DT.Bludgeoning]  = "You are hitting something that is mostly bone and wrapping. The force distributes.", [DT.Slashing] = "The bindings catch the blade and redirect it.", [DT.Piercing] = "The wrappings deflect the point. There is a great deal of wrapping." },
            LootTable =
            [
                new() { ItemId = "BoneFragment", Chance = 0.60f },
                new() { ItemId = "AncientShard", Chance = 0.40f },
                new() { ItemId = "BoneRune",     Chance = 0.25f },
                new() { ItemId = "Stone",        Chance = 0.35f },
            ],
            AppearTexts =
            [
                "It was standing here when you arrived. It has been standing here since before the architecture around it was new.",
                "It turns toward you. Slowly. With the certainty of something that has turned toward intruders before and will again.",
                "It was still. Then you crossed a threshold it remembers. Now it is not still.",
            ],
            AttackTexts =
            [
                "It strikes with the form of someone who learned this correctly a very long time ago.",
                "The blow is not fast. It does not need to be fast. It needs to connect.",
                "It reaches out and applies force in the way it was trained to apply force.",
                "Several thousand years of standing guard, and then this. You are this.",
            ],
            HeavyAttackTexts =
            [
                "It draws back in a way that looks ceremonial. It is not ceremonial.",
                "It commits the full weight of a very old soldier to the next strike.",
                "It steps forward — one step, deliberate — and delivers everything.",
            ],
            DefendTexts =
            [
                "It plants itself between you and the sarcophagus. This is its function. It fulfills it.",
                "It raises its arms in a defensive position that has not changed in several thousand years.",
                "It stands. The standing is the defense. It has very good standing.",
            ],
            FleeTexts =
            [
                "It does not flee. It steps back. There is a distinction, and it knows the distinction.",
                "It retreats to its post and holds. This is not running. This is repositioning to the objective.",
            ],
            DeathTexts =
            [
                "It falls. The wrappings settle. The guard is no longer at post.",
                "It comes apart slowly, the bindings loosening. What they were holding together dissipates.",
                "It leans against the wall it was guarding and slides down it. A final report to no one.",
            ],
        },

        new()
        {
            Id = "TheUnwrapped", Name = "The Unwrapped",
            Description = "The wrappings came off at some point. What was underneath kept going. It does not recall having them. It is enormous and extremely old and the bandages trailing from its wrists are the only evidence it was ever anything other than this.",
            BaseHp = 175, BaseAttack = 26, BaseDefense = 15,
            FleeChance = 0.05f, FleeHpThreshold = 0.05f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 35 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 20 },
                new() { Action = EnemyActionType.Regenerate,  Weight = 30 },
                new() { Action = EnemyActionType.Buff,        Weight = 15 },
            ],
            BaseXp = 30,
            BaseSpeed = 6, CritChance = 0.10f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses  = [DT.Fire],
            Resistances = [DT.Dark],
            WeaknessText  = { [DT.Fire]  = "The trailing bandages catch. It noticed this and does not like it." },
            ResistanceText = { [DT.Dark]  = "It has been in the dark since before you were born. Possibly since before your civilization." },
            RegenerateAmount = 14,
            BuffStat = ModifierStat.Attack, BuffValue = 8f, BuffTurns = 2,
            LootTable =
            [
                new() { ItemId = "BoneRune",       Chance = 0.60f },
                new() { ItemId = "AncientShard",   Chance = 0.50f },
                new() { ItemId = "TarnishedRelic", Chance = 0.30f },
                new() { ItemId = "Compass",        Chance = 0.03f },
            ],
            AppearTexts =
            [
                "Something very large fills the far end of the passage. It was there before the torch reached it.",
                "You hear it moving — a dry, papery sound at a scale that should not make that sound.",
                "The bandages trailing from its wrists reach you before it does. Then it does.",
            ],
            AttackTexts =
            [
                "It strikes with an arm that is approximately the size of a small tree and equally intentional.",
                "The blow is old and enormous and does not care about the specific details.",
                "It reaches and you are at the end of the reach.",
                "It connects. The only announcement was the air moving ahead of it.",
            ],
            HeavyAttackTexts =
            [
                "It raises both arms and holds the position for a moment that is somehow worse than the blow.",
                "It commits everything to the next motion. The room shudders slightly ahead of impact.",
                "It brings down something that feels geological and is only technically a punch.",
            ],
            DefendTexts =
            [
                "It stands in the way. There is a great deal of it to stand in the way.",
                "It draws in its trailing wrappings and becomes slightly more consolidated.",
            ],
            RegenerateTexts =
            [
                "The wrappings tighten across something. It looks more intact than before.",
                "It is very old. The thing that keeps it going is not slowing down.",
                "Something in it resets. The damage you made is less visible now.",
            ],
            BuffTexts =
            [
                "It shakes itself once, slowly, and seems to remember something about what it was.",
                "The trailing bandages pull taut and it stands differently. More deliberately.",
                "It inhales — the first sound it has made — and its posture shifts.",
            ],
            FleeTexts =
            [
                "It turns and walks away with the unhurried confidence of something that has never needed to run.",
                "It retreats. The passage fills completely as it goes. Then it doesn't.",
            ],
            DeathTexts =
            [
                "It sits down, very slowly, and the sitting becomes the end.",
                "The wrappings go slack. Whatever the wrappings were containing releases.",
                "It stops in the middle of a step and stays there. Then it doesn't stay there. Then it is on the floor.",
            ],
        },

        // ── Sunken Mill enemies ───────────────────────────────────────────────
        new()
        {
            Id = "MillGhost", Name = "Mill Ghost",
            Description = "The ghost of someone who worked the mill when the mill worked. The mill stopped. The wheel stopped. The water stopped. The ghost did not. It treats your presence as an interruption.",
            BaseHp = 46, BaseAttack = 18, BaseDefense = 5,
            FleeChance = 0.40f, FleeHpThreshold = 0.25f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack, Weight = 40 },
                new() { Action = EnemyActionType.Buff,   Weight = 50 },
                new() { Action = EnemyActionType.Defend, Weight = 10 },
            ],
            BaseXp = 8,
            BaseSpeed = 7, CritChance = 0.05f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Fire, DT.Storm],
            Resistances = [DT.Slashing, DT.Piercing, DT.Dark],
            WeaknessText  = { [DT.Fire]  = "Fire disrupts the form. It cannot maintain its shape in the heat.", [DT.Storm] = "Lightning passes through and finds something to ground itself on regardless." },
            ResistanceText = { [DT.Slashing]  = "The blade passes through. There is nothing there to cut.", [DT.Piercing] = "It parts around the point and reforms.", [DT.Dark] = "It has spent decades in a dark mill. The dark finds no purchase here." },
            BuffStat = ModifierStat.Speed, BuffValue = 5f, BuffTurns = 3,
            LootTable =
            [
                new() { ItemId = "Driftwood", Chance = 0.40f },
                new() { ItemId = "Fiber",     Chance = 0.35f },
                new() { ItemId = "RiverGlass",Chance = 0.20f },
            ],
            AppearTexts =
            [
                "A figure is going through the motions of work that is no longer there to be done.",
                "It does not look up when you enter. It is busy. The work finished decades ago.",
                "Something translucent and purposeful moves through the machinery. The machinery does not respond.",
            ],
            AttackTexts =
            [
                "It turns from its task and swings with the force of something that has been interrupted.",
                "It passes through you briefly. Cold. Wrong.",
                "It brings a hand down in the motion of work and the motion connects.",
                "It strikes with the irritation of someone who had a schedule.",
            ],
            BuffTexts =
            [
                "It goes translucent and continues working, and both things happen at once.",
                "It fades slightly at the edges, becoming less there to hit.",
                "It presses itself into the motion of labor and becomes harder to locate.",
            ],
            DefendTexts =
            [
                "It steps into the machinery and lets the gaps between things protect it.",
                "It becomes briefly more wall than ghost.",
            ],
            FleeTexts =
            [
                "It passes through the nearest wall and the work sounds continue briefly on the other side.",
                "It disperses into the space between the planks and is gone.",
                "It simply goes elsewhere, with the efficiency of something that has somewhere to be.",
                "It fades mid-motion and does not return.",
            ],
            DeathTexts =
            [
                "It stops in the middle of a motion it has completed ten thousand times. Does not complete it.",
                "The work finally ends. It takes the ghost with it.",
                "It disperses gradually, like mist off water in the morning, until there is nothing there.",
            ],
        },

        new()
        {
            Id = "WaterloggedWorker", Name = "Waterlogged Worker",
            Description = "A drowned laborer still carrying out duties. The work is not finished. The work will never be finished. Water drips from everything it carries. The grinding sound it makes when moving is the worker, not the mill.",
            BaseHp = 55, BaseAttack = 18, BaseDefense = 9,
            FleeChance = 0.30f, FleeHpThreshold = 0.15f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 55 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 20 },
                new() { Action = EnemyActionType.Defend,      Weight = 25 },
            ],
            BaseXp = 13,
            BaseSpeed = 4, CritChance = 0.03f,
            AttackDamageType = DT.Bludgeoning,
            Weaknesses  = [DT.Storm, DT.Fire],
            Resistances = [DT.Frost],
            WeaknessText  = { [DT.Storm]  = "The water in it conducts. Lightning finds the whole body at once.", [DT.Fire] = "Steam and confusion. Something about the heat disrupts whatever keeps it going." },
            ResistanceText = { [DT.Frost]  = "It is already saturated. Cold finds nothing new to freeze." },
            LootTable =
            [
                new() { ItemId = "Driftwood",   Chance = 0.50f },
                new() { ItemId = "Rope",        Chance = 0.35f },
                new() { ItemId = "Wood",        Chance = 0.30f },
                new() { ItemId = "BoneFragment",Chance = 0.15f },
            ],
            AppearTexts =
            [
                "It is carrying something. It has been carrying it for a very long time. It will continue.",
                "Water runs from it constantly. The sound of work is coming from inside it.",
                "It rounds the corner at a slow, unstoppable pace and does not acknowledge your presence.",
            ],
            AttackTexts =
            [
                "It swings whatever it is carrying. The tool and the intention are equally blunt.",
                "It walks into you, which is the attack.",
                "It sets down its burden briefly, addresses you, picks the burden back up.",
                "The strike is not angry. It is procedural. That is somehow worse.",
            ],
            HeavyAttackTexts =
            [
                "It stops walking, focuses, and applies everything to the next moment.",
                "It sets down its load, uses both hands, and picks the load back up. Sequentially.",
                "The grinding intensifies briefly and then it delivers something with full commitment.",
            ],
            DefendTexts =
            [
                "It holds its burden between you and itself. The burden is considerable.",
                "It plants its feet and continues working. You are a complication, not a stoppage.",
                "It angles itself and keeps moving, incorporating your presence into the route.",
            ],
            FleeTexts =
            [
                "It turns and walks away at the same pace it arrived. It does not change speed for anything.",
                "It takes a different route. It has a route. It takes it.",
                "It goes. Water marks where it was. The grinding fades.",
            ],
            DeathTexts =
            [
                "It sets down its burden and does not pick it back up.",
                "It stops mid-stride and the water drains out of it and the work is done.",
                "It goes down slowly, still reaching for the work, not quite completing the motion.",
            ],
        },

        new()
        {
            Id = "TheFerryman", Name = "The Ferryman",
            Description = "He ferries things across. He has always ferried things across. He does not specify what, or where to, or from where. You did not pay the toll. The pole he carries has been in use since before the mill existed.",
            BaseHp = 130, BaseAttack = 22, BaseDefense = 11,
            FleeChance = 0.10f, FleeHpThreshold = 0.08f,
            ActionTable =
            [
                new() { Action = EnemyActionType.Attack,      Weight = 35 },
                new() { Action = EnemyActionType.HeavyAttack, Weight = 25 },
                new() { Action = EnemyActionType.Buff,        Weight = 30 },
                new() { Action = EnemyActionType.Defend,      Weight = 10 },
            ],
            BaseXp = 29,
            BaseSpeed = 6, CritChance = 0.12f,
            AttackDamageType = DT.Dark,
            Weaknesses  = [DT.Storm],
            Resistances = [DT.Frost, DT.Dark],
            WeaknessText  = { [DT.Storm]  = "Lightning on the water. He has been struck before. He does not forget." },
            ResistanceText = { [DT.Frost]  = "The river does not freeze while he works it. It is an old arrangement.", [DT.Dark] = "The crossing is made in darkness. He navigates by other means." },
            BuffStat = ModifierStat.Speed, BuffValue = 2f, BuffTurns = 3,
            LootTable =
            [
                new() { ItemId = "RiverGlass",  Chance = 0.60f },
                new() { ItemId = "Driftwood",   Chance = 0.50f },
                new() { ItemId = "TidalCoin",   Chance = 0.40f },
                new() { ItemId = "BoneFragment",Chance = 0.30f },
                new() { ItemId = "Spyglass",    Chance = 0.04f },
            ],
            AppearTexts =
            [
                "He is already at the far end of the water. He has already seen you. He is already coming.",
                "A pole extends from the mist. He is attached to the other end of it. He does not hurry.",
                "He arrives at the bank and looks at you with the patience of someone who always arrives eventually.",
            ],
            AttackTexts =
            [
                "The pole comes across at head height. He has done this many times.",
                "He pushes forward with the pole and the force is old and specific.",
                "He strikes without announcement. The toll is the announcement.",
                "He brings the pole around in a motion that is still technically ferrying, just applied to you.",
            ],
            HeavyAttackTexts =
            [
                "He plants the pole and uses it to bring himself to you, very fast.",
                "He winds back and delivers the full length of it across the narrowest space.",
                "He steps into the stroke. The river moves with him.",
            ],
            BuffTexts =
            [
                "He drifts backward on something that is not quite the water and becomes harder to place.",
                "The mist comes with him and he arranges it to his advantage.",
                "He recedes slightly and the water closes around where he was standing.",
            ],
            DefendTexts =
            [
                "He holds the pole in front of him crosswise. It is longer than the gap between you.",
                "He goes still on the water in a way that suggests he is waiting, not stopped.",
            ],
            FleeTexts =
            [
                "He pushes back into the mist and the mist accepts him.",
                "He returns to the water. The crossing is over. He will take you another time.",
                "He poles away without comment. He did not need to explain before, and he does not now.",
            ],
            DeathTexts =
            [
                "He goes into the water and does not resurface. For the first time, he is the one crossing.",
                "The pole floats. He does not.",
                "He settles into the shallows and is still. The river moves around him and keeps going.",
            ],
        },
    ];

    private static readonly Dictionary<string, EnemyDefinition> _byId =
        _definitions.ToDictionary(d => d.Id);

    public IReadOnlyList<EnemyDefinition> All => _definitions;

    public EnemyDefinition? GetById(string id) =>
        _byId.TryGetValue(id, out var def) ? def : null;
}
