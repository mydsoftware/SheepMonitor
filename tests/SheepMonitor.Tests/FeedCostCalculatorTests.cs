using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedCostCalculatorTests
{
    [Fact]
    public void Calculate_ShouldReturnRoundedCost()
    {
        Assert.Equal(12500m, FeedCostCalculator.Calculate(25m, 500m));
        Assert.Equal(12501.00m, FeedCostCalculator.Calculate(25.001m, 500.02m));
    }
}
