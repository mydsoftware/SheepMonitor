using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// سرویس مدیریت اطلاعات پایه مواد غذایی و وعده‌ها.
/// </summary>
public interface IReferenceDataService
{
    Task<IReadOnlyList<ReferenceData>> GetAsync(string category, CancellationToken cancellationToken = default);
    Task<ReferenceData> SaveAsync(ReferenceData item, CancellationToken cancellationToken = default);
    Task DisableAsync(int id, CancellationToken cancellationToken = default);
}
