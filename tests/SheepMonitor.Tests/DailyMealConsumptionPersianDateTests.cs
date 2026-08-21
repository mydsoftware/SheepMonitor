using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class DailyMealConsumptionPersianDateTests
{
    [Fact]
    public void DailyReportDate_ShouldBeFormattedAsPersianDate()
    {
        var records = new[]
        {
            (new DateTime(2026, 8, 18, 7, 0, 0), new MealConsumptionInput("صبح", 10m, 1m, 9m, 900m))
        };

        var report = DailyMealConsumptionReportBuilder.Build(records).Single();
        var persianDate = PersianDateFormatter.Format(report.Date);

        Assert.Equal("1405/05/27", persianDate);
    }
}
