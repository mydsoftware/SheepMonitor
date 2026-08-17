using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت برنامه‌های غذایی.
/// </summary>
public interface IFeedPlanService
{
    Task<IReadOnlyList<FeedPlan>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeedPlanItem>> GetItemsAsync(int feedPlanId, CancellationToken cancellationToken = default);
    Task<FeedPlan> AddAsync(FeedPlan plan, CancellationToken cancellationToken = default);
    Task<FeedPlanItem> AddItemAsync(FeedPlanItem item, CancellationToken cancellationToken = default);
}
