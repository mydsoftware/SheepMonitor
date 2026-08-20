using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Data;

namespace SheepMonitor.Api;

public static class FeedConsumptionThresholdEndpoints
{
    public static RouteGroupBuilder MapFeedConsumptionThresholdEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/feed-consumption-thresholds");

        group.MapGet("/", async (SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var items = await db.FeedConsumptionThresholds.AsNoTracking()
                .OrderBy(x => x.FeedCode)
                .ToListAsync(cancellationToken);
            return Results.Ok(items);
        });

        group.MapGet("/{feedCode}", async (string feedCode, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var item = await db.FeedConsumptionThresholds.AsNoTracking()
                .Where(x => x.FeedCode == feedCode && x.IsActive)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            return item is null ? Results.NotFound() : Results.Ok(item);
        });

        group.MapPost("/", async (FeedConsumptionThreshold request, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.FeedCode) || request.LowDeviationPercent < 0m || request.HighDeviationPercent < request.LowDeviationPercent)
                return Results.BadRequest("مقادیر آستانه مصرف نامعتبر است.");

            var item = new FeedConsumptionThreshold
            {
                FeedCode = request.FeedCode.Trim(),
                LowDeviationPercent = request.LowDeviationPercent,
                HighDeviationPercent = request.HighDeviationPercent,
                IsActive = request.IsActive,
                Notes = request.Notes
            };

            db.FeedConsumptionThresholds.Add(item);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/feed-consumption-thresholds/{item.FeedCode}", item);
        });

        group.MapPut("/{id:int}", async (int id, FeedConsumptionThreshold request, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.FeedCode) || request.LowDeviationPercent < 0m || request.HighDeviationPercent < request.LowDeviationPercent)
                return Results.BadRequest("مقادیر آستانه مصرف نامعتبر است.");

            var item = await db.FeedConsumptionThresholds.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item is null)
                return Results.NotFound();

            item.FeedCode = request.FeedCode.Trim();
            item.LowDeviationPercent = request.LowDeviationPercent;
            item.HighDeviationPercent = request.HighDeviationPercent;
            item.IsActive = request.IsActive;
            item.Notes = request.Notes;
            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(item);
        });

        group.MapDelete("/{id:int}", async (int id, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var item = await db.FeedConsumptionThresholds.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item is null)
                return Results.NotFound();

            db.FeedConsumptionThresholds.Remove(item);
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });

        return group;
    }
}
