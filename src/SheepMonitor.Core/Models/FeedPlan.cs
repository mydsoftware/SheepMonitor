namespace SheepMonitor.Core.Models;

public sealed class FeedPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TargetGroup { get; set; }
    public string? Notes { get; set; }
}

public sealed class FeedPlanItem
{
    public int Id { get; set; }
    public int FeedPlanId { get; set; }
    public string FeedName { get; set; } = string.Empty;
    public decimal AmountKgPerDay { get; set; }
}
