using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// سرویس تخصیص برنامه غذایی به گوسفندان در SQL Server.
/// </summary>
public sealed class SheepFeedPlanService(SheepMonitorDbContext db) : ISheepFeedPlanService
{
    public async Task<IReadOnlyList<SheepFeedPlanAssignment>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default) =>
        await db.SheepFeedPlanAssignments.AsNoTracking()
            .Where(x => x.SheepId == sheepId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);

    public Task<SheepFeedPlanAssignment?> GetActiveAsync(int sheepId, CancellationToken cancellationToken = default) =>
        db.SheepFeedPlanAssignments.AsNoTracking()
            .Where(x => x.SheepId == sheepId && x.IsActive && (!x.EndDate.HasValue || x.EndDate.Value >= DateTime.Today))
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<SheepFeedPlanAssignment> AssignAsync(SheepFeedPlanAssignment assignment, CancellationToken cancellationToken = default)
    {
        if (!await db.Sheep.AnyAsync(x => x.Id == assignment.SheepId, cancellationToken))
            throw new InvalidOperationException("گوسفند انتخاب‌شده وجود ندارد.");
        if (!await db.FeedPlans.AnyAsync(x => x.Id == assignment.FeedPlanId, cancellationToken))
            throw new InvalidOperationException("برنامه غذایی انتخاب‌شده وجود ندارد.");
        if (assignment.EndDate.HasValue && assignment.EndDate.Value < assignment.StartDate)
            throw new ArgumentException("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد.");

        var overlapping = await db.SheepFeedPlanAssignments.AnyAsync(x =>
            x.SheepId == assignment.SheepId &&
            x.IsActive &&
            (!x.EndDate.HasValue || x.EndDate.Value >= assignment.StartDate) &&
            x.StartDate <= (assignment.EndDate ?? DateTime.MaxValue), cancellationToken);
        if (overlapping) throw new InvalidOperationException("برای این گوسفند در این بازه زمانی یک برنامه غذایی فعال وجود دارد.");

        db.SheepFeedPlanAssignments.Add(assignment);
        await db.SaveChangesAsync(cancellationToken);
        return assignment;
    }

    public async Task EndAsync(int assignmentId, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var assignment = await db.SheepFeedPlanAssignments.FirstOrDefaultAsync(x => x.Id == assignmentId, cancellationToken)
            ?? throw new InvalidOperationException("تخصیص برنامه غذایی پیدا نشد.");
        if (endDate < assignment.StartDate) throw new ArgumentException("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد.");
        assignment.EndDate = endDate;
        assignment.IsActive = false;
        await db.SaveChangesAsync(cancellationToken);
    }
}
