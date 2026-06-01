namespace MapGenerator.Runner.Models;

public class RunnerSpider
{
    /// <summary>World-space X center. Scrolls left with the world — no separate velocity needed.</summary>
    public double WorldX { get; set; }
    /// <summary>Oscillation phase in ms. Drives the up/down bobbing on the web.</summary>
    public double PhaseMs { get; set; }
}
