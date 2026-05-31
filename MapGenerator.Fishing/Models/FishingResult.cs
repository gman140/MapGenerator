namespace MapGenerator.Fishing.Models;

public class FishingResult
{
    public string? CaughtFishId { get; init; }
    public string? CaughtFishName { get; init; }
    public double CaughtWeightKg { get; init; }
    public bool Success => CaughtFishId != null;
}
