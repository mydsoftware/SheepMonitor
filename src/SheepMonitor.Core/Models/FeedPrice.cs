namespace SheepMonitor.Core.Models;

public sealed class FeedPrice
{
    public int Id { get; set; }
    public string FeedCode { get; set; } = string.Empty;
    public decimal PricePerKg { get; set; }
    public string Currency { get; set; } = "IRR";
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
