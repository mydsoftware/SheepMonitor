namespace SheepMonitor.Core.Models;

/// <summary>
/// قانون قابل تنظیم محاسبه جیره که تمام پارامترهای آن از SQL Server خوانده می‌شود.
/// </summary>
public sealed class RationCalculationRule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string FeedCode { get; set; } = string.Empty;
    public string? TargetGroupCode { get; set; }
    public decimal BasePercent { get; set; }
    public decimal WeightCoefficient { get; set; }
    public decimal MinimumKg { get; set; }
    public decimal MaximumKg { get; set; }
    public decimal? ProteinPercent { get; set; }
    public decimal? EnergyPerKg { get; set; }
    public decimal? DryMatterPercent { get; set; }
    public bool IsActive { get; set; }
    public string? Formula { get; set; }
    public string? Notes { get; set; }
}
