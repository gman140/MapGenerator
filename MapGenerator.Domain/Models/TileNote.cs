namespace MapGenerator.Domain.Models;

public class TileNote
{
    public string Id { get; set; } = string.Empty;
    public int Q { get; set; }
    public int R { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
