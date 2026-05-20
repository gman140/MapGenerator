namespace MapGenerator.Domain.Models;

public class PlayerCompanion
{
    public string Id { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public string DefinitionId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public List<string> MoveIds { get; set; } = [];
    public string[] SpritePixels { get; set; } = [];
}
