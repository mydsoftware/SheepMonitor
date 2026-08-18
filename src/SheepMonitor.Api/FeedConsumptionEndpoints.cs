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
            var query = db.FeedConsumptionRecords.AsNoTracking();
            var records = await query.SelectMany(x => db.FeedConsumptionItems.Where(i => i.FeedConsumptionRecordId == x.Id))
                .ToListAsync(cancellationToken);

            var animalCount = await db.Sheep.AsNoTracking().CountAsync(cancellationToken);
            var summary = new FeedConsumptionSummary(
                records.Sum(x => x.PlannedKg),
                records.Sum(x => x.ActualKg),
                records.Sum(x => x.WasteKg),
                animalCount);

            return Results.Ok(summary);
        });

        return group;
    }
}
