namespace SheepMonitor.Core.Reports;

public static class MealConsumptionReportBuilder
{
    public static IReadOnlyList<MealConsumptionReport> Build(IEnumerable<MealConsumptionInput> records)
        => records
            .GroupBy(x => x.MealCode)
            .Select(g => new MealConsumptionReport(
                g.Key,
                g.Sum(x => x.ActualKg),
                g.Sum(x => x.WasteKg),
                g.Sum(x => x.NetConsumptionKg),
                g.Sum(x => x.Cost)))
            .OrderBy(x => x.MealCode)
            .ToList();
}

public sealed record MealConsumptionInput(
    string MealCode,
    decimal ActualKg,
    decimal WasteKg,
    decimal NetConsumptionKg,
    decimal Cost);
