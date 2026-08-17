using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت سوابق سلامت و بیماری گوسفندان.
/// </summary>
public interface IHealthService
{
    Task<IReadOnlyList<SheepHealthRecord>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default);
    Task<SheepHealthRecord> AddAsync(SheepHealthRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(SheepHealthRecord record, CancellationToken cancellationToken = default);
}
