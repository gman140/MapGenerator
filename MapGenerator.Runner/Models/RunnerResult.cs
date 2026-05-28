namespace MapGenerator.Runner.Models;

public class RunnerResult
{
    public bool Won { get; init; }
    public List<RunnerReward> Rewards { get; init; } = [];
}

public class RunnerReward
{
    public string ItemId { get; init; } = "";
    public int Quantity { get; init; } = 1;
}
