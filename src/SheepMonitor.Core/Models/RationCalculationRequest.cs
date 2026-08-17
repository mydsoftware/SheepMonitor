namespace SheepMonitor.Core.Models;

/// <summary>
/// ورودی محاسبه جیره برای یک گوسفند یا میانگین گروه.
/// </summary>
public sealed class RationCalculationRequest
{
    public int? SheepId { get; set; }
    public bool UseAllSheepAverage { get; set; }
    public decimal? WeightKg { get; set; }
    public int DayNumber { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public int PeriodDurationDays { get; set; }
}
