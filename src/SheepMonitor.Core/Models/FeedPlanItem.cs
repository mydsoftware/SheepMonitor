namespace SheepMonitor.Core.Models;

/// <summary>
/// یک ماده غذایی داخل برنامه غذایی.
/// </summary>
public sealed class FeedPlanItem
{
    public int Id { get; set; }
    public int FeedPlanId { get; set; }
    public string FeedName { get; set; } = string.Empty;
    public decimal AmountKgPerDay { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int MealsPerDay { get; set; }
    public string? Notes { get; set; }
}
