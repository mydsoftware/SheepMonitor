namespace SheepMonitor.Core.Models;

/// <summary>
/// خلاصه روند رشد و تغییرات وزن یک گوسفند.
/// </summary>
public sealed class SheepGrowthReport
{
    public int SheepId { get; set; }
    public string SheepNumber { get; set; } = string.Empty;
    public decimal InitialWeightKg { get; set; }
    public decimal LatestWeightKg { get; set; }
    public decimal TotalWeightChangeKg { get; set; }
    public decimal AverageWeightChangeKg { get; set; }
    public IReadOnlyList<WeightGrowthPoint> Points { get; set; } = [];
}

/// <summary>
/// یک نقطه از روند تغییر وزن در گزارش رشد.
/// </summary>
public sealed class WeightGrowthPoint
{
    public DateTime Date { get; set; }
    public decimal WeightKg { get; set; }
    public decimal ChangeFromPreviousKg { get; set; }
}
