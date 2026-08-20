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

        group.MapGet("/report", async (DateTime? from, DateTime? to, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.FeedConsumptionRecords.AsNoTracking();
            if (from.HasValue) query = query.Where(x => x.ConsumedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.ConsumedAt <= to.Value);

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

            return Results.Ok(DailyMealConsumptionReportBuilder.Build(input));
        });

        return group;
    }
}
