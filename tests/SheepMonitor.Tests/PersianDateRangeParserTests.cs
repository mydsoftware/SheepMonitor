using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class PersianDateRangeParserTests
{
    [Fact]
    public void TryParse_ShouldConvertValidPersianDate()
    {
        var success = PersianDateRangeParser.TryParse("1405/05/27", out var date);

        Assert.True(success);
        Assert.Equal(new DateTime(2026, 8, 18), date);
    }

    [Theory]
    [InlineData("1405/13/01")]
    [InlineData("1405/02/31")]
    [InlineData("invalid")]
    public void TryParse_ShouldRejectInvalidDate(string value)
    {
        Assert.False(PersianDateRangeParser.TryParse(value, out _));
    }
}
