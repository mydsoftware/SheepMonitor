using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedConsumptionSummaryTests
{
    [Fact]
    public void Summary_ShouldCalculatePerAnimalNetConsumption()
    {
        var summary = new FeedConsumptionSummary(100m, 110m, 10m, 20);

        Assert.Equal(10m, summary.VarianceKg);
        Assert.Equal(10m, summary.VariancePercent);
        Assert.Equal(100m, summary.NetConsumptionKg);
        Assert.Equal(5m, summary.NetConsumptionPerAnimalKg);
    }
}
