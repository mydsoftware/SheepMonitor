namespace SheepMonitor.Core.Models;

/// <summary>
/// ثبت مصرف واقعی خوراک در یک روز و یک وعده.
/// </summary>
public sealed class FeedConsumptionRecord
{
    public long Id { get; set; }
    public DateTime ConsumedAt { get; set; }
    public string FeedCode { get; set; } = string.Empty;
    public string FeedTitle { get; set; } = string.Empty;
    public string MealCode { get; set; } = string.Empty;
    public decimal ActualAmountKg { get; set; }
    public decimal? WasteAmountKg { get; set; }
    public int? SheepId { get; set; }
    public string? Notes { get; set; }

    public decimal NetConsumedKg => Math.Max(0m, ActualAmountKg - (WasteAmountKg ?? 0m));
}
