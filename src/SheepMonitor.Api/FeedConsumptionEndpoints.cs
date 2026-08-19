using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Reports;
using SheepMonitor.Data;

namespace SheepMonitor.Api;

public static class FeedConsumptionEndpoints
{
    public static RouteGroupBuilder MapFeedConsumptionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/feed-consumption");

        group.MapGet("/summary", async (SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var records = await db.FeedConsumptionItems.AsNoTracking().ToListAsync(cancellationToken);
            var animalCount = await db.Sheep.AsNoTracking().CountAsync(cancellationToken);
            return Results.Ok(new FeedConsumptionSummary(records.Sum(x => x.PlannedKg), records.Sum(x => x.ActualKg), records.Sum(x => x.WasteKg), animalCount));
        });

        group.MapGet("/dashboard", async (SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var records = await db.FeedConsumptionItems.AsNoTracking().ToListAsync(cancellationToken);
            var animalCount = await db.Sheep.AsNoTracking().CountAsync(cancellationToken);
            var summary = new FeedConsumptionSummary(records.Sum(x => x.PlannedKg), records.Sum(x => x.ActualKg), records.Sum(x => x.WasteKg), animalCount);
            return Results.Ok(new FeedDashboardSnapshot(summary.PlannedKg, summary.ActualKg, summary.WasteKg, summary.NetConsumptionKg, summary.VarianceKg, summary.VariancePercent, summary.NetConsumptionPerAnimalKg, summary.AnimalCount, DateTime.UtcNow));
        });

        group.MapGet("/cost", async (DateTime? from, DateTime? to, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.FeedConsumptionRecords.AsNoTracking();
            if (from.HasValue) query = query.Where(x => x.ConsumedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.ConsumedAt <= to.Value);

            var records = await query.Select(x => new { x.ConsumedAt, x.FeedCode, x.FeedTitle, x.NetConsumedKg }).ToListAsync(cancellationToken);
            var prices = await db.FeedPrices.AsNoTracking().ToListAsync(cancellationToken);
            var animalCount = await db.Sheep.AsNoTracking().CountAsync(cancellationToken);
            var currency = prices.Select(x => x.Currency).FirstOrDefault() ?? "IRR";

            decimal ResolvePrice(string feedCode, DateTime consumedAt) => prices
                .Where(p => p.FeedCode == feedCode && p.EffectiveFrom <= consumedAt && (p.EffectiveTo == null || p.EffectiveTo >= consumedAt))
                .OrderByDescending(p => p.EffectiveFrom)
                .Select(p => (decimal?)p.PricePerKg).FirstOrDefault() ?? 0m;

            var byFeed = records
                .GroupBy(x => new { x.FeedCode, x.FeedTitle })
                .Select(g =>
                {
                    var netKg = g.Sum(x => x.NetConsumedKg);
                    var cost = g.Sum(x => FeedCostCalculator.Calculate(x.NetConsumedKg, ResolvePrice(x.FeedCode, x.ConsumedAt)));
                    return new { g.Key.FeedCode, g.Key.FeedTitle, NetConsumedKg = netKg, TotalCost = cost };
                })
                .OrderByDescending(x => x.TotalCost)
                .ToList();

            var totalCost = byFeed.Sum(x => x.TotalCost);
            var netKgTotal = byFeed.Sum(x => x.NetConsumedKg);
            var costPerAnimal = animalCount > 0 ? decimal.Round(totalCost / animalCount, 2, MidpointRounding.AwayFromZero) : 0m;
            return Results.Ok(new
            {
                TotalCost = totalCost,
                CostPerAnimal = costPerAnimal,
                NetConsumptionKg = netKgTotal,
                AnimalCount = animalCount,
                Currency = currency,
                ByFeed = byFeed,
                GeneratedAtUtc = DateTime.UtcNow
            });
        });

        group.MapGet("/trend", async (DateTime? from, DateTime? to, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.FeedConsumptionRecords.AsNoTracking();
            if (from.HasValue) query = query.Where(x => x.ConsumedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.ConsumedAt <= to.Value);
            var trend = await query.GroupBy(x => x.ConsumedAt.Date).Select(g => new { Date = g.Key, ActualKg = g.Sum(x => x.ActualAmountKg), WasteKg = g.Sum(x => x.WasteAmountKg ?? 0m), NetConsumedKg = g.Sum(x => x.NetConsumedKg) }).OrderBy(x => x.Date).ToListAsync(cancellationToken);
            return Results.Ok(trend);
        });

        group.MapGet("/details", async (DateTime? from, DateTime? to, string? feedCode, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.FeedConsumptionRecords.AsNoTracking();
            if (from.HasValue) query = query.Where(x => x.ConsumedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.ConsumedAt <= to.Value);
            if (!string.IsNullOrWhiteSpace(feedCode)) query = query.Where(x => x.FeedCode == feedCode);
            var details = await query.OrderByDescending(x => x.ConsumedAt).Select(x => new { x.Id, x.ConsumedAt, x.FeedCode, x.FeedTitle, x.MealCode, x.ActualAmountKg, WasteKg = x.WasteAmountKg ?? 0m, x.NetConsumedKg, x.SheepId, x.Notes }).ToListAsync(cancellationToken);
            return Results.Ok(details);
        });

        return group;
    }
}
