namespace MapGenerator.Domain.Models;

public class MovementResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresOceanConfirmation { get; set; }
    public bool PlayerDrowned { get; set; }
    public int? NewQ { get; set; }
    public int? NewR { get; set; }
    public long? CooldownUntil { get; set; }
}
