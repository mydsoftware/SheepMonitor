using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedCostDashboardReportTests
{
    [Fact]
    public void Snapshot_ShouldExposeCostPerAnimal()
    {
        var snapshot = new FeedCostDashboardSnapshot(120_000m, 6_000m, 240m, 20, "IRR", DateTime.UtcNow);

        Assert.Equal(120_000m, snapshot.TotalCost);
        Assert.Equal(6_000m, snapshot.CostPerAnimal);
        Assert.Equal(240m, snapshot.NetConsumptionKg);
        Assert.Equal(20, snapshot.AnimalCount);
        Assert.Equal("IRR", snapshot.Currency);
    }
}
