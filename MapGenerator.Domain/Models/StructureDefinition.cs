using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class StructureDefinition
{
    public StructureType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public CraftingIngredient[] Ingredients { get; init; } = [];
    public BiomeType[]? AllowedBiomes { get; init; }
}
