namespace MapGenerator.Domain.Models;

public class ChatMessage
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string? SenderId { get; set; }
    public int? TileQ { get; set; }
    public int? TileR { get; set; }
    public bool IsWorldChat { get; set; }
    public bool IsSystemMessage { get; set; }
    public DateTime SentAt { get; set; }
}
