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

        // مقایسه مصرف واقعی با جیره برنامه‌ریزی‌شده در سطح کل گله و هر قلم خوراک.
        group.MapGet("/comparison", async (DateTime? from, DateTime? to, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var query = from item in db.FeedConsumptionItems.AsNoTracking()
                        join record in db.FeedConsumptionRecords.AsNoTracking()
                            on item.FeedConsumptionRecordId equals record.Id
                        select new { item, record.ConsumedAt };

            if (from.HasValue)
                query = query.Where(x => x.ConsumedAt >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.ConsumedAt <= to.Value);

            var rows = await query.ToListAsync(cancellationToken);
            var byFeed = rows
                .GroupBy(x => x.item.FeedCode)
                .Select(g =>
                {
                    var planned = g.Sum(x => x.item.PlannedKg);
                    var actual = g.Sum(x => x.item.ActualKg);
                    var waste = g.Sum(x => x.item.WasteKg);
                    var variance = actual - planned;
                    var variancePercent = planned == 0m ? 0m : decimal.Round(variance / planned * 100m, 2, MidpointRounding.AwayFromZero);
                    return new
                    {
                        FeedCode = g.Key,
                        PlannedKg = planned,
                        ActualKg = actual,
                        WasteKg = waste,
                        NetActualKg = actual - waste,
                        VarianceKg = variance,
                        VariancePercent = variancePercent
                    };
                })
                .OrderBy(x => x.FeedCode)
                .ToList();

            var totalPlanned = byFeed.Sum(x => x.PlannedKg);
            var totalActual = byFeed.Sum(x => x.ActualKg);
            var totalWaste = byFeed.Sum(x => x.WasteKg);
            var totalVariance = totalActual - totalPlanned;
            var totalVariancePercent = totalPlanned == 0m ? 0m : decimal.Round(totalVariance / totalPlanned * 100m, 2, MidpointRounding.AwayFromZero);
            var animalCount = await db.Sheep.AsNoTracking().CountAsync(cancellationToken);
            var netActual = totalActual - totalWaste;

            return Results.Ok(new
            {
                From = from,
                To = to,
                AnimalCount = animalCount,
                TotalPlannedKg = totalPlanned,
                TotalActualKg = totalActual,
                TotalWasteKg = totalWaste,
                TotalNetActualKg = netActual,
                TotalVarianceKg = totalVariance,
                TotalVariancePercent = totalVariancePercent,
                NetActualPerAnimalKg = animalCount == 0 ? 0m : decimal.Round(netActual / animalCount, 3, MidpointRounding.AwayFromZero),
                ByFeed = byFeed,
                GeneratedAtUtc = DateTime.UtcNow
            });
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

            var byFeed = records.GroupBy(x => new { x.FeedCode, x.FeedTitle }).Select(g =>
            {
                var netKg = g.Sum(x => x.NetConsumedKg);
                var cost = g.Sum(x => FeedCostCalculator.Calculate(x.NetConsumedKg, ResolvePrice(x.FeedCode, x.ConsumedAt)));
                return new { g.Key.FeedCode, g.Key.FeedTitle, NetConsumedKg = netKg, TotalCost = cost };
            }).OrderByDescending(x => x.TotalCost).ToList();

            var totalCost = byFeed.Sum(x => x.TotalCost);
            var netKgTotal = byFeed.Sum(x => x.NetConsumedKg);
            var costPerAnimal = animalCount > 0 ? decimal.Round(totalCost / animalCount, 2, MidpointRounding.AwayFromZero) : 0m;
            return Results.Ok(new { TotalCost = totalCost, CostPerAnimal = costPerAnimal, NetConsumptionKg = netKgTotal, AnimalCount = animalCount, Currency = currency, ByFeed = byFeed, GeneratedAtUtc = DateTime.UtcNow });
        });

        group.MapGet("/cost-trend", async (DateTime? from, DateTime? to, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.FeedConsumptionRecords.AsNoTracking();
            if (from.HasValue) query = query.Where(x => x.ConsumedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.ConsumedAt <= to.Value);

            var records = await query.Select(x => new { x.ConsumedAt, x.FeedCode, x.NetConsumedKg }).ToListAsync(cancellationToken);
            var prices = await db.FeedPrices.AsNoTracking().ToListAsync(cancellationToken);

            decimal ResolvePrice(string feedCode, DateTime consumedAt) => prices
                .Where(p => p.FeedCode == feedCode && p.EffectiveFrom <= consumedAt && (p.EffectiveTo == null || p.EffectiveTo >= consumedAt))
                .OrderByDescending(p => p.EffectiveFrom)
                .Select(p => (decimal?)p.PricePerKg).FirstOrDefault() ?? 0m;

            var trend = records.GroupBy(x => x.ConsumedAt.Date).Select(g => new
            {
                Date = g.Key,
                TotalCost = g.Sum(x => FeedCostCalculator.Calculate(x.NetConsumedKg, ResolvePrice(x.FeedCode, x.ConsumedAt))),
                NetConsumptionKg = g.Sum(x => x.NetConsumedKg)
            }).OrderBy(x => x.Date).ToList();

            return Results.Ok(trend);
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
