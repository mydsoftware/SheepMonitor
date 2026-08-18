namespace SheepMonitor.Core.Reports;

public sealed record FeedConsumptionSummary(
    decimal PlannedKg,
    decimal ActualKg,
    decimal WasteKg,
    int AnimalCount)
{
    public decimal VarianceKg => ActualKg - PlannedKg;
    public decimal VariancePercent => PlannedKg == 0 ? 0 : VarianceKg / PlannedKg * 100m;
    public decimal NetConsumptionKg => ActualKg - WasteKg;
    public decimal NetConsumptionPerAnimalKg => AnimalCount == 0 ? 0 : NetConsumptionKg / AnimalCount;
}
