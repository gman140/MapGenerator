namespace MapGenerator.Combat.Models;

public class LootEntry
{
    public string ItemId { get; set; } = string.Empty;
    public float Chance { get; set; }
    public int MinQty { get; set; } = 1;
    public int MaxQty { get; set; } = 1;
}
