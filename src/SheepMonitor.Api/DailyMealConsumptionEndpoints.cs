using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Reports;
using SheepMonitor.Data;

namespace SheepMonitor.Api;

public static class DailyMealConsumptionEndpoints
{
    public static RouteGroupBuilder MapDailyMealConsumptionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/feed-consumption/daily");

        group.MapGet("/report", async (string? from, string? to, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            DateTime? fromDate = null;
            DateTime? toDateExclusive = null;

            if (!string.IsNullOrWhiteSpace(from) && !PersianDateRangeParser.TryParse(from, out var parsedFrom))
                return Results.BadRequest(new { message = "تاریخ شروع شمسی نامعتبر است." });

            if (!string.IsNullOrWhiteSpace(to) && !PersianDateRangeParser.TryParse(to, out var parsedTo))
                return Results.BadRequest(new { message = "تاریخ پایان شمسی نامعتبر است." });

            if (!string.IsNullOrWhiteSpace(from))
                fromDate = parsedFrom.Date;

            if (!string.IsNullOrWhiteSpace(to))
                toDateExclusive = parsedTo.Date.AddDays(1);

            if (fromDate.HasValue && toDateExclusive.HasValue && fromDate >= toDateExclusive)
                return Results.BadRequest(new { message = "بازه تاریخ نامعتبر است." });

            var query = db.FeedConsumptionRecords.AsNoTracking();
            if (fromDate.HasValue)
                query = query.Where(x => x.ConsumedAt >= fromDate.Value);
            if (toDateExclusive.HasValue)
                query = query.Where(x => x.ConsumedAt < toDateExclusive.Value);

            var records = await query.Select(x => new
            {
                x.ConsumedAt,
                x.MealCode,
                x.ActualAmountKg,
                WasteKg = x.WasteAmountKg ?? 0m,
                x.NetConsumedKg,
                x.FeedCode
            }).ToListAsync(cancellationToken);

            var prices = await db.FeedPrices.AsNoTracking().ToListAsync(cancellationToken);
            var input = records.Select(x => new
            {
                x.ConsumedAt,
                Input = new MealConsumptionInput(
                    x.MealCode,
                    x.ActualAmountKg,
                    x.WasteKg,
                    x.NetConsumedKg,
                    FeedCostCalculator.Calculate(x.NetConsumedKg, FeedPriceResolver.Resolve(prices, x.FeedCode, x.ConsumedAt)))
            }).Select(x => (x.ConsumedAt, x.Input));

            var reports = DailyMealConsumptionReportBuilder.Build(input);

            return Results.Ok(reports.Select(x => new
            {
                x.Date,
                PersianDate = PersianDateFormatter.Format(x.Date),
                x.Meals,
                x.TotalActualKg,
                x.TotalWasteKg,
                x.TotalNetConsumptionKg,
                x.TotalCost
            }));
        });

        return group;
    }
}
