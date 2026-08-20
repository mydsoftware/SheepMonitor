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
            var validation = Validate(request);
            if (validation is not null)
                return Results.BadRequest(validation);

            request.FeedCode = request.FeedCode.Trim();
            db.FeedConsumptionThresholds.Add(request);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/feed-consumption-thresholds/{request.FeedCode}", request);
        });

        group.MapPut("/{id:int}", async (int id, FeedConsumptionThreshold request, SheepMonitorDbContext db, CancellationToken cancellationToken) =>
        {
            var validation = Validate(request);
            if (validation is not null)
                return Results.BadRequest(validation);

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

    private static string? Validate(FeedConsumptionThreshold request)
    {
        if (string.IsNullOrWhiteSpace(request.FeedCode))
            return "کد خوراک الزامی است.";
        if (request.LowDeviationPercent < 0m)
            return "آستانه کم‌مصرف نمی‌تواند منفی باشد.";
        if (request.HighDeviationPercent <= request.LowDeviationPercent)
            return "آستانه پرمصرف باید بزرگ‌تر از آستانه کم‌مصرف باشد.";
        return null;
    }
}
