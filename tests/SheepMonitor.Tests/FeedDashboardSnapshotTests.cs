using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedDashboardSnapshotTests
{
    [Fact]
    public void Snapshot_ShouldPreserveDashboardMetrics()
    {
        var generated = DateTime.UtcNow;
        var snapshot = new FeedDashboardSnapshot(100, 110, 10, 100, 10, 10, 5, 20, generated);

        Assert.Equal(100, snapshot.PlannedKg);
        Assert.Equal(110, snapshot.ActualKg);
        Assert.Equal(100, snapshot.NetConsumptionKg);
        Assert.Equal(5, snapshot.NetConsumptionPerAnimalKg);
        Assert.Equal(20, snapshot.AnimalCount);
        Assert.Equal(generated, snapshot.GeneratedAtUtc);
    }
}
