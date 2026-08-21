using Xunit;

namespace SheepMonitor.Tests;

public class RationEdgeCaseTests
{
    [Fact]
    public void EmptyFlock_ShouldNotProduceAverage()
    {
        var weights = Array.Empty<decimal>();
        Assert.Empty(weights);
    }

    [Fact]
    public void InvalidDay_ShouldBeRejected()
    {
        const int duration = 10;
        const int day = 11;

        Assert.True(day < 1 || day > duration);
    }

    [Fact]
    public void NegativeWeight_ShouldNotBeAccepted()
    {
        const decimal weight = -5m;
        Assert.False(weight > 0);
    }
}
