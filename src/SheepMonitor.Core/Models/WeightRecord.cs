namespace SheepMonitor.Core.Models;

public sealed class WeightRecord
{
    public int Id { get; set; }
    public int SheepId { get; set; }
    public DateTime WeighedAt { get; set; }
    public decimal WeightKg { get; set; }
    public string? Notes { get; set; }
}
