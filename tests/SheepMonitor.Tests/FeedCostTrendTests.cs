using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedCostTrendTests
{
    [Theory]
    [InlineData(25, 500, 12500)]
    [InlineData(10.5, 800, 8400)]
    public void DailyCost_ShouldBeCalculatedFromNetKgAndPrice(decimal netKg, decimal pricePerKg, decimal expectedCost)
    {
        Assert.Equal(expectedCost, FeedCostCalculator.Calculate(netKg, pricePerKg));
    }
}
