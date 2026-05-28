namespace MapGenerator.Runner.Models;

public class RunnerObstacle
{
    /// <summary>World-space X position. Screen X = WorldX - state.WorldOffset.</summary>
    public double WorldX { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    /// <summary>True = gap in the ground; false = solid boulder.</summary>
    public bool IsGap { get; init; }
    public string Color { get; init; } = "#808090";
    public bool IsActive { get; set; } = true;
}
