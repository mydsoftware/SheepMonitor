namespace SheepMonitor.Core.Models;

/// <summary>
/// آستانه‌های قابل تنظیم برای ارزیابی اختلاف مصرف واقعی با جیره برنامه‌ریزی‌شده.
/// </summary>
public sealed class FeedConsumptionThreshold
{
    public int Id { get; set; }
    public string FeedCode { get; set; } = string.Empty;
    public decimal LowDeviationPercent { get; set; }
    public decimal HighDeviationPercent { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
