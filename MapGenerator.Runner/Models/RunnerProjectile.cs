namespace MapGenerator.Runner.Models;

public class RunnerProjectile
{
    public double X { get; set; }
    public double Y { get; set; }
    public double SpeedPx { get; init; } = 320.0;
    public int Damage { get; init; }
    public string Color { get; init; } = "#ffaa00";
    public bool IsShockwave { get; init; }
}
