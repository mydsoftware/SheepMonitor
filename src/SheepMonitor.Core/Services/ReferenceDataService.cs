using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

public interface IReferenceDataService
{
    Task<IReadOnlyList<ReferenceData>> GetAsync(string category, CancellationToken cancellationToken = default);
    Task<ReferenceData> AddAsync(ReferenceData item, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReferenceData item, CancellationToken cancellationToken = default);
    Task DisableAsync(int id, CancellationToken cancellationToken = default);
}
