namespace SheepMonitor.Core.Models;

/// <summary>
/// اطلاعات پایه یک گوسفند.
/// مقادیر قابل تغییر مانند جنسیت و وضعیت سلامت از داده‌های مرجع SQL Server انتخاب می‌شوند.
/// </summary>
public sealed class Sheep
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime InitialWeighingDate { get; set; }
    public decimal InitialWeightKg { get; set; }
    public bool IsSick { get; set; }
    public string HealthStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
