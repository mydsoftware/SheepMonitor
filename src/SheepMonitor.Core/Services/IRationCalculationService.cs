using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// موتور محاسبه جیره بر اساس قوانین ذخیره‌شده در دیتابیس.
/// </summary>
public interface IRationCalculationService
{
    Task<IReadOnlyList<RationCalculationResult>> CalculateForSheepAsync(int sheepId, int periodId, int dayNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RationCalculationResult>> CalculateForAverageWeightAsync(int periodId, int dayNumber, CancellationToken cancellationToken = default);
}
