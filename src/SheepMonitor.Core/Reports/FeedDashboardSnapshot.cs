namespace SheepMonitor.Core.Reports;

public sealed record FeedDashboardSnapshot(
    decimal PlannedKg,
    decimal ActualKg,
    decimal WasteKg,
    decimal NetConsumptionKg,
    decimal VarianceKg,
    decimal VariancePercent,
    decimal NetConsumptionPerAnimalKg,
    int AnimalCount,
    DateTime GeneratedAtUtc);
