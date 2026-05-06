using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface ICraftingRecipeProvider
{
    IReadOnlyList<CraftingRecipe> All { get; }
    CraftingRecipe? GetById(string? id);
}
