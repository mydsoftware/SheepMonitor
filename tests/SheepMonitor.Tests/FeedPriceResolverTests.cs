using SheepMonitor.Core.Models;
using SheepMonitor.Core.Reports;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedPriceResolverTests
{
    [Fact]
    public void Resolve_ShouldSelectLatestEffectivePrice()
    {
        var prices = new[]
        {
            new FeedPrice { FeedCode = "CONCENTRATE", PricePerKg = 1000m, EffectiveFrom = new DateTime(2026, 1, 1), EffectiveTo = new DateTime(2026, 6, 30) },
            new FeedPrice { FeedCode = "CONCENTRATE", PricePerKg = 1500m, EffectiveFrom = new DateTime(2026, 7, 1) }
        };

        var result = FeedPriceResolver.Resolve(prices, "CONCENTRATE", new DateTime(2026, 8, 1));

        Assert.Equal(1500m, result);
    }
}
