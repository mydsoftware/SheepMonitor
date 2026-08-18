using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public class FeedConsumptionReportTests
{
    [Fact]
    public void Report_ShouldCalculateVarianceAndNetConsumption()
    {
        var report = new FeedConsumptionReport(10m, 12m, 1m);

        Assert.Equal(2m, report.VarianceKg);
        Assert.Equal(20m, report.VariancePercent);
        Assert.Equal(11m, report.ConsumedNetKg);
    }
}
