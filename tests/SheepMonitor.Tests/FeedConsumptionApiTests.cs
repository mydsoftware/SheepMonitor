using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedConsumptionApiTests
{
    [Fact]
    public void Summary_ShouldExposeExpectedValues()
    {
        var summary = new FeedConsumptionSummary(100m, 110m, 10m, 20);

        Assert.Equal(100m, summary.PlannedKg);
        Assert.Equal(110m, summary.ActualKg);
        Assert.Equal(10m, summary.WasteKg);
        Assert.Equal(5m, summary.NetConsumptionPerAnimalKg);
    }
}
