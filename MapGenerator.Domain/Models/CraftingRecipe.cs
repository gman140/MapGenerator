using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class CraftingRecipe
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsConsumable { get; init; }
    public bool RequiresWorkshop { get; init; }
    public CraftingIngredient[] Ingredients { get; init; } = [];
    public ItemEffect[] Effects { get; init; } = [];
}
