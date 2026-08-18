namespace SheepMonitor.Core.Reports;

public sealed record FeedConsumptionReport(
    decimal PlannedKg,
    decimal ActualKg,
    decimal WasteKg)
{
    public decimal VarianceKg => ActualKg - PlannedKg;
    public decimal VariancePercent => PlannedKg == 0 ? 0 : VarianceKg / PlannedKg * 100m;
    public decimal ConsumedNetKg => ActualKg - WasteKg;
}
