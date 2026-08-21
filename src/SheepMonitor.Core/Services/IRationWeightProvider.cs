namespace SheepMonitor.Core.Services;

/// <summary>
/// تأمین وزن مبنا از اطلاعات ثبت‌شده دام‌ها.
/// </summary>
public interface IRationWeightProvider
{
    Task<decimal> GetSheepWeightAsync(int sheepId, CancellationToken cancellationToken = default);
    Task<decimal> GetAverageHerdWeightAsync(CancellationToken cancellationToken = default);
}
