namespace SheepMonitor.Core.Models;

/// <summary>
/// خروجی محاسبه جیره برای یک روز و یک ماده غذایی.
/// </summary>
public sealed class RationCalculationResult
{
    public int DayNumber { get; set; }
    public DateTime Date { get; set; }
    public string MealCode { get; set; } = string.Empty;
    public string FeedCode { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public decimal DailyAmountKg { get; set; }
    public decimal MealAmountKg { get; set; }
}
