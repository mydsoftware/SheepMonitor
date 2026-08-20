using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Reports;

public static class FeedPriceResolver
{
    public static decimal Resolve(IEnumerable<FeedPrice> prices, string feedCode, DateTime at)
        => prices
            .Where(p => p.FeedCode == feedCode && p.EffectiveFrom <= at && (p.EffectiveTo == null || p.EffectiveTo >= at))
            .OrderByDescending(p => p.EffectiveFrom)
            .Select(p => p.PricePerKg)
            .FirstOrDefault();
}
