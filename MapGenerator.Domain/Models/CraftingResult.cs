namespace MapGenerator.Domain.Models;

public class CraftingResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public string? CraftedItemId { get; init; }
    public string? CraftedItemName { get; init; }
}
