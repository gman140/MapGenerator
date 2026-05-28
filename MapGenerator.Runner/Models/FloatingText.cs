namespace MapGenerator.Runner.Models;

public class FloatingText
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Text { get; init; } = "";
    public string Color { get; init; } = "#ffffff";
    public double LifetimeMs { get; set; } = 800.0;
}
