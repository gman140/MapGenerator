namespace MapGenerator.Domain.Models;

public class FishLogEntry
{
    public DateTime FirstCaughtAt { get; set; }
    public int TotalCaught { get; set; }
    public double PersonalBestWeightKg { get; set; }
}
