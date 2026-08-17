using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

public interface ISheepService
{
    Task<IReadOnlyList<Sheep>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sheep?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Sheep> AddAsync(Sheep sheep, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sheep sheep, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
