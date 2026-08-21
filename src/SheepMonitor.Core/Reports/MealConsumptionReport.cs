namespace SheepMonitor.Core.Reports;

public sealed record MealConsumptionReport(
    string MealCode,
    decimal ActualKg,
    decimal WasteKg,
    decimal NetConsumptionKg,
    decimal TotalCost);
