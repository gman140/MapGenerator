namespace MapGenerator.Domain.Models;

public class PlayerTileVisit
{
    public string Id { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public int Q { get; set; }
    public int R { get; set; }
    public DateTime ArrivedAt { get; set; }
    public DateTime? LeftAt { get; set; }
}
