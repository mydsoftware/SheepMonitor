using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class DailyMealConsumptionReportBuilderTests
{
    [Fact]
    public void Build_ShouldGroupByDateAndThenByMeal()
    {
        var records = new[]
        {
            (new DateTime(2026, 8, 18, 7, 0, 0), new MealConsumptionInput("صبح", 10m, 1m, 9m, 900m)),
            (new DateTime(2026, 8, 18, 13, 0, 0), new MealConsumptionInput("ظهر", 8m, 0.5m, 7.5m, 750m)),
            (new DateTime(2026, 8, 18, 21, 0, 0), new MealConsumptionInput("شب", 6m, 0m, 6m, 600m)),
            (new DateTime(2026, 8, 19, 7, 0, 0), new MealConsumptionInput("صبح", 12m, 1m, 11m, 1100m))
        };

        var result = DailyMealConsumptionReportBuilder.Build(records);

        Assert.Equal(2, result.Count);
        Assert.Equal(new DateTime(2026, 8, 18), result[0].Date);
        Assert.Equal(24m, result[0].TotalActualKg);
        Assert.Equal(1.5m, result[0].TotalWasteKg);
        Assert.Equal(22.5m, result[0].TotalNetConsumptionKg);
        Assert.Equal(2250m, result[0].TotalCost);
        Assert.Equal(3, result[0].Meals.Count);
        Assert.Equal(new DateTime(2026, 8, 19), result[1].Date);
        Assert.Equal(12m, result[1].TotalActualKg);
    }
}
