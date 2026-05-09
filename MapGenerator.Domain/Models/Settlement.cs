using MapGenerator.Domain.Enums;

namespace MapGenerator.Domain.Models;

public class Settlement
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SettlementType Type { get; set; }
    public int CenterQ { get; set; }
    public int CenterR { get; set; }
    public List<SettlementTile> Tiles { get; set; } = [];
}

public class SettlementTile
{
    public int Q { get; set; }
    public int R { get; set; }
    public SettlementTileRole Role { get; set; }
}
