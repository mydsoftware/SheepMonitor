using SheepMonitor.Core.Services;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class WeightCalculatorTests
{
    [Fact]
    public void CalculateAverage_ShouldReturnAverageWeight()
    {
        var result = WeightCalculator.CalculateAverage([42.5m, 45m, 41m, 47.5m]);

        Assert.Equal(44m, result);
    }

    [Fact]
    public void CalculateAverage_ShouldRejectEmptyCollection()
    {
        Assert.Throws<ArgumentException>(() => WeightCalculator.CalculateAverage([]));
    }
}
