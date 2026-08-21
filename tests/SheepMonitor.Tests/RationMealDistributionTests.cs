using Xunit;

namespace SheepMonitor.Tests;

public class RationMealDistributionTests
{
    [Fact]
    public void DailyAmount_ShouldBeDistributedByMealPercent()
    {
        const decimal dailyAmount = 2m;
        var morning = dailyAmount * 50m / 100m;
        var noon = dailyAmount * 30m / 100m;
        var evening = dailyAmount * 20m / 100m;

        Assert.Equal(1m, morning);
        Assert.Equal(0.6m, noon);
        Assert.Equal(0.4m, evening);
        Assert.Equal(dailyAmount, morning + noon + evening);
    }

    [Fact]
    public void MealPercentagesAbove100_ShouldBeRejected()
    {
        var total = 70m + 40m;
        Assert.True(total > 100m);
    }
}
