namespace SheepMonitor.Core.Reports;

public sealed record DailyMealConsumptionReport(
    DateTime Date,
    IReadOnlyList<MealConsumptionReport> Meals,
    decimal TotalActualKg,
    decimal TotalWasteKg,
    decimal TotalNetConsumptionKg,
    decimal TotalCost);

public static class DailyMealConsumptionReportBuilder
{
    public static IReadOnlyList<DailyMealConsumptionReport> Build(IEnumerable<(DateTime Date, MealConsumptionInput Input)> records)
        => records
            .GroupBy(x => x.Date.Date)
            .OrderBy(x => x.Key)
            .Select(day =>
            {
                var meals = MealConsumptionReportBuilder.Build(day.Select(x => x.Input));
                return new DailyMealConsumptionReport(
                    day.Key,
                    meals,
                    meals.Sum(x => x.ActualKg),
                    meals.Sum(x => x.WasteKg),
                    meals.Sum(x => x.NetConsumptionKg),
                    meals.Sum(x => x.TotalCost));
            })
            .ToList();
}
