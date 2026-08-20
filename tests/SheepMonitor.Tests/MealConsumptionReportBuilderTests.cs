using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class MealConsumptionReportBuilderTests
{
    [Fact]
    public void Build_ShouldGroupRecordsByMeal()
    {
        var records = new[]
        {
            new MealConsumptionInput("صبح", 10m, 1m, 9m, 900m),
            new MealConsumptionInput("صبح", 6m, 0.5m, 5.5m, 550m),
            new MealConsumptionInput("شب", 8m, 1m, 7m, 700m)
        };

        var result = MealConsumptionReportBuilder.Build(records);

        Assert.Equal(2, result.Count);
        var morning = result.Single(x => x.MealCode == "صبح");
        Assert.Equal(16m, morning.ActualKg);
        Assert.Equal(1.5m, morning.WasteKg);
        Assert.Equal(14.5m, morning.NetConsumptionKg);
        Assert.Equal(1450m, morning.TotalCost);
    }
}
