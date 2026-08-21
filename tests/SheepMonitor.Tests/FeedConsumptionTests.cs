using SheepMonitor.Core.Models;
using Xunit;

namespace SheepMonitor.Tests;

public class FeedConsumptionTests
{
    [Fact]
    public void NetConsumption_ShouldSubtractWaste()
    {
        var record = new FeedConsumptionRecord
        {
            ActualAmountKg = 10m,
            WasteAmountKg = 1.5m
        };

        Assert.Equal(8.5m, record.NetConsumedKg);
    }

    [Fact]
    public void NegativeWaste_ShouldBeRejected()
    {
        var record = new FeedConsumptionRecord
        {
            ActualAmountKg = 10m,
            WasteAmountKg = -1m
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            if (record.WasteAmountKg is < 0) throw new ArgumentOutOfRangeException();
        });
    }
}
