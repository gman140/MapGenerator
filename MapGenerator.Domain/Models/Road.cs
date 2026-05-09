namespace MapGenerator.Domain.Models;

public class Road
{
    public string Id { get; set; } = string.Empty;
    public string FromSettlementId { get; set; } = string.Empty;
    public string ToSettlementId { get; set; } = string.Empty;
    public List<RoadPoint> Path { get; set; } = [];
}

public class RoadPoint
{
    public int Q { get; set; }
    public int R { get; set; }
}
