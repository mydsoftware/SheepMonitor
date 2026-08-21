using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// سرویس مدیریت برنامه‌های غذایی در SQL Server.
/// </summary>
public sealed class FeedPlanService(SheepMonitorDbContext db) : IFeedPlanService
{
    public async Task<IReadOnlyList<FeedPlan>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.FeedPlans.AsNoTracking().OrderByDescending(x => x.IsActive).ThenBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<FeedPlanItem>> GetItemsAsync(int feedPlanId, CancellationToken cancellationToken = default) =>
        await db.FeedPlanItems.AsNoTracking().Where(x => x.FeedPlanId == feedPlanId).OrderBy(x => x.FeedName).ToListAsync(cancellationToken);

    public async Task<FeedPlan> AddAsync(FeedPlan plan, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(plan.Name)) throw new ArgumentException("نام برنامه غذایی الزامی است.");
        if (plan.EndDate.HasValue && plan.EndDate.Value < plan.StartDate)
            throw new ArgumentException("تاریخ پایان برنامه نمی‌تواند قبل از تاریخ شروع باشد.");
        db.FeedPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken);
        return plan;
    }

    public async Task<FeedPlanItem> AddItemAsync(FeedPlanItem item, CancellationToken cancellationToken = default)
    {
        if (!await db.FeedPlans.AnyAsync(x => x.Id == item.FeedPlanId, cancellationToken))
            throw new InvalidOperationException("برنامه غذایی انتخاب‌شده وجود ندارد.");
        if (string.IsNullOrWhiteSpace(item.FeedName)) throw new ArgumentException("ماده غذایی الزامی است.");
        if (item.AmountKgPerDay <= 0) throw new ArgumentOutOfRangeException(nameof(item.AmountKgPerDay), "مقدار روزانه باید بیشتر از صفر باشد.");
        db.FeedPlanItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return item;
    }
}
