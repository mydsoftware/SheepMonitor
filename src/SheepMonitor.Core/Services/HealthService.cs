using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت سوابق سلامت و بیماری گوسفندان.
/// </summary>
public interface IHealthService
{
    Task<IReadOnlyList<HealthRecord>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default);
    Task<HealthRecord> AddAsync(HealthRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(HealthRecord record, CancellationToken cancellationToken = default);
}
