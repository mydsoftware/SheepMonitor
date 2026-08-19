namespace SheepMonitor.Core.Reports;

public sealed record FeedCostDashboardSnapshot(
    decimal TotalCost,
    decimal CostPerAnimal,
    decimal NetConsumptionKg,
    int AnimalCount,
    string Currency,
    DateTime GeneratedAtUtc);
