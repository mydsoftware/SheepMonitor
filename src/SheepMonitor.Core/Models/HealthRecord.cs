namespace SheepMonitor.Core.Models;

public sealed class HealthRecord
{
    public int Id { get; set; }
    public int SheepId { get; set; }
    public DateTime RecordedAt { get; set; }
    public bool IsSick { get; set; }
    public string Status { get; set; } = "سالم";
    public string? DiseaseName { get; set; }
    public string? Symptoms { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
}
