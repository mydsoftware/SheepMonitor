namespace SheepMonitor.Core.Models;

/// <summary>
/// یک رکورد وزن‌گیری برای یک گوسفند در یک تاریخ مشخص.
/// </summary>
public sealed class WeightRecord
{
    public int Id { get; set; }
    public int SheepId { get; set; }
    public DateTime WeighedAt { get; set; }
    public decimal WeightKg { get; set; }
    public string? Notes { get; set; }
}
