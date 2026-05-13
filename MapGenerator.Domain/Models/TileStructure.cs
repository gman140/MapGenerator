using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class TileStructure
{
    public StructureType Type { get; set; }
    public string BuilderId { get; set; } = string.Empty;
    public string BuilderName { get; set; } = string.Empty;
    public DateTime BuiltAt { get; set; }
    public DateTime? LastHarvestedAt { get; set; }
}
