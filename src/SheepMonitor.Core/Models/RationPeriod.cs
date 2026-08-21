namespace SheepMonitor.Core.Models;

/// <summary>
/// دوره جیره که تعداد روز آن از اطلاعات پایه قابل تنظیم است.
/// </summary>
public sealed class RationPeriod
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
