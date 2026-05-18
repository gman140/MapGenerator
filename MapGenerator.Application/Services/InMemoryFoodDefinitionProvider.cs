using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class InMemoryFoodDefinitionProvider : IFoodDefinitionProvider
{
    private static readonly FoodDefinition[] _definitions =
    [
        new()
        {
            ResourceId     = "Berries",
            Name           = "Berries",
            SatietyRestore = 12,
            EatMessages    =
            [
                "You eat a handful of berries. Tart and a little too small, but welcome.",
                "The berries are sour enough to make you wince, but you finish them.",
                "Sweet, mostly. You eat them quickly before reconsidering.",
                "They taste like something a bird would fight you for. You eat them anyway.",
            ],
        },
        new()
        {
            ResourceId     = "Mushroom",
            Name           = "Mushroom",
            SatietyRestore = 10,
            EatMessages    =
            [
                "You eat the mushroom raw. Earthy, chewy, not entirely unpleasant.",
                "It tastes of the forest floor. You eat it anyway.",
                "The mushroom is bland but filling enough to matter.",
                "You eat it in two bites and try not to think too hard about where it grew.",
            ],
        },
        new()
        {
            ResourceId     = "Herbs",
            Name           = "Herbs",
            SatietyRestore = 8,
            EatMessages    =
            [
                "You chew the herbs slowly. Bitter, green, and vaguely medicinal.",
                "They taste medicinal and not particularly inviting. You finish them.",
                "The herbs are more pungent than expected. You eat them and move on.",
                "You eat the herbs raw. Your stomach accepts this grudgingly.",
            ],
        },
        new()
        {
            ResourceId     = "CookedFish",
            Name           = "Cooked Fish",
            SatietyRestore = 40,
            EatMessages    =
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
            ResourceId     = "CookedMushroom",
            Name           = "Cooked Mushroom",
            SatietyRestore = 25,
            EatMessages    =
            [
                "Cooking made them something else entirely — rich and soft and good.",
                "The heat brought out something you didn't expect. You eat them quickly.",
                "Warm and earthy and much better than raw. You finish the whole thing.",
                "They collapse against the heat into something genuinely satisfying.",
            ],
        },
        new()
        {
            ResourceId     = "MushroomSoup",
            Name           = "Mushroom Soup",
            SatietyRestore = 50,
            EatMessages    =
            [
                "The soup is hot and deep and exactly right. You eat it slowly to make it last.",
                "Salt and mushroom and warmth. You drink the last of it from the bowl.",
                "You didn't expect something this good out here. You eat every drop.",
                "The broth is thick and savory. You finish it and feel properly restored.",
                "It tastes like someone cared about making it. You are grateful for that.",
            ],
        },
        new()
        {
            ResourceId     = "BerryPie",
            Name           = "Berry Pie",
            SatietyRestore = 60,
            EatMessages    =
            [
                "The crust is imperfect and the berries are too tart and it is very good.",
                "You eat it in careful slices and wish there were more.",
                "Something about a pie made out here feels like a small act of defiance. You approve.",
                "It is better than it has any right to be. You eat it all.",
                "The berries burst sweet and sour under the crust. You take your time with this one.",
            ],
        },
        new()
        {
            ResourceId     = "HerbTea",
            Name           = "Herb Tea",
            SatietyRestore = 22,
            EatMessages    =
            [
                "The tea is bitter and herbal and good in the way that things that are good for you often aren't.",
                "You drink it slowly. It clears something in your chest you hadn't noticed.",
                "Fragrant, faintly medicinal, and warm. You finish the cup.",
                "It tastes like the forest smells. You find this agreeable.",
            ],
        },
    ];

    private static readonly Dictionary<string, FoodDefinition> _byId =
        _definitions.ToDictionary(d => d.ResourceId);

    public IReadOnlyList<FoodDefinition> All => _definitions;

    public FoodDefinition? GetById(string resourceId) =>
        _byId.TryGetValue(resourceId, out var def) ? def : null;
}
