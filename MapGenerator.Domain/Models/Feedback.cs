namespace MapGenerator.Domain.Models;

public class Feedback
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}
