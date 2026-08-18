namespace SheepMonitor.Core.Models;

public sealed class FeedConsumptionItem
{
    public int Id { get; set; }
    public int FeedConsumptionRecordId { get; set; }
    public string FeedCode { get; set; } = string.Empty;
    public decimal PlannedKg { get; set; }
    public decimal ActualKg { get; set; }
    public decimal WasteKg { get; set; }
}
