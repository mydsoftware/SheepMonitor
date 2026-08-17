using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد تهیه گزارش روند رشد گوسفندان.
/// </summary>
public interface IGrowthReportService
{
    Task<SheepGrowthReport?> GetAsync(int sheepId, CancellationToken cancellationToken = default);
}
