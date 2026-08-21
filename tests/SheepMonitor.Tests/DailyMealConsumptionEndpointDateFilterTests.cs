using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class DailyMealConsumptionEndpointDateFilterTests
{
    [Fact]
    public void PersianRange_ShouldIncludeWholeEndDay()
    {
        Assert.True(PersianDateRangeParser.TryParse("1405/05/27", out var end));
        var inclusiveEnd = end.Date.AddDays(1);

        Assert.Equal(new DateTime(2026, 8, 19), inclusiveEnd);
    }

    [Theory]
    [InlineData("1405/13/01")]
    [InlineData("1405/12/30")]
    [InlineData("invalid")]
    public void PersianRange_ShouldRejectInvalidDate(string value)
    {
        Assert.False(PersianDateRangeParser.TryParse(value, out _));
    }
}
