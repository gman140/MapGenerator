namespace MapGenerator.Domain.Models;

public class GatherResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public List<GatheredItem> Gathered { get; init; } = [];
    public long CooldownUntil { get; init; }
}

public class GatheredItem
{
    public string ResourceId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Quantity { get; init; }
}
