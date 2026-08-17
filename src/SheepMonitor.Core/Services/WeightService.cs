using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت وزن‌گیری‌های دوره‌ای گوسفندان.
/// </summary>
public interface IWeightService
{
    Task<IReadOnlyList<WeightRecord>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default);
    Task<WeightRecord> AddAsync(WeightRecord record, CancellationToken cancellationToken = default);
    Task<decimal> CalculateAverageAsync(IEnumerable<decimal> weights);
}
