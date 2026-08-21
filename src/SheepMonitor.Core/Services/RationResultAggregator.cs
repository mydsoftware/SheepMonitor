using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// تجمیع خروجی جیره برای نمایش روزانه به تفکیک وعده و ماده غذایی.
/// </summary>
public sealed class RationResultAggregator
{
    public IReadOnlyList<RationCalculationResult> Aggregate(IEnumerable<RationCalculationResult> results)
    {
        return results
            .GroupBy(x => new { x.DayNumber, x.Date, x.MealCode, x.FeedCode })
            .Select(g => new RationCalculationResult
            {
                DayNumber = g.Key.DayNumber,
                Date = g.Key.Date,
                MealCode = g.Key.MealCode,
                FeedCode = g.Key.FeedCode,
                WeightKg = g.First().WeightKg,
                DailyAmountKg = g.First().DailyAmountKg,
                MealAmountKg = g.Sum(x => x.MealAmountKg)
            })
            .OrderBy(x => x.DayNumber)
            .ThenBy(x => x.MealCode)
            .ThenBy(x => x.FeedCode)
            .ToList();
    }
}
