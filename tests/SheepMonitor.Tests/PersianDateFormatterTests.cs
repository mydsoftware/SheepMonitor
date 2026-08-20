using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class PersianDateFormatterTests
{
    [Fact]
    public void Format_ShouldReturnPersianCalendarDate()
    {
        var date = new DateTime(2026, 8, 18);

        var result = PersianDateFormatter.Format(date);

        Assert.Equal("1405/05/27", result);
    }
}
