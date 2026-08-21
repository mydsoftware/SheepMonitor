using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد تخصیص و مدیریت برنامه غذایی گوسفندان.
/// </summary>
public interface ISheepFeedPlanService
{
    Task<IReadOnlyList<SheepFeedPlanAssignment>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default);
    Task<SheepFeedPlanAssignment?> GetActiveAsync(int sheepId, CancellationToken cancellationToken = default);
    Task<SheepFeedPlanAssignment> AssignAsync(SheepFeedPlanAssignment assignment, CancellationToken cancellationToken = default);
    Task EndAsync(int assignmentId, DateTime endDate, CancellationToken cancellationToken = default);
}
