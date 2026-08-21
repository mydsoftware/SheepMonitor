using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedCostApiTests
{
    [Theory]
    [InlineData(10, 250, 2500)]
    [InlineData(12.5, 400, 5000)]
    public void CostCalculation_ShouldMatchExpected(decimal kg, decimal price, decimal expected)
    {
        Assert.Equal(expected, FeedCostCalculator.Calculate(kg, price));
    }
}
