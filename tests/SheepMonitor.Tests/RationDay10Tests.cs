using Xunit;

namespace SheepMonitor.Tests;

public class RationDay10Tests
{
    [Fact]
    public void Day10_ShouldBe_NinthDayAfterStart()
    {
        var startDate = new DateTime(2026, 1, 1);
        var day10 = startDate.AddDays(10 - 1);

        Assert.Equal(new DateTime(2026, 1, 10), day10);
    }

    [Fact]
    public void MealPercentages_ShouldNotExceed100Percent()
    {
        var total = 0.40m + 0.35m + 0.25m;

        Assert.Equal(1.00m, total);
        Assert.True(total <= 1.00m);
    }
}
