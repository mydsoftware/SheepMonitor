using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

public sealed class SheepService(SheepMonitorDbContext db) : ISheepService
{
    public async Task<IReadOnlyList<Sheep>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Sheep.AsNoTracking().OrderBy(x => x.Number).ToListAsync(cancellationToken);

    public Task<Sheep?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        db.Sheep.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Sheep> AddAsync(Sheep sheep, CancellationToken cancellationToken = default)
    {
        db.Sheep.Add(sheep);
        await db.SaveChangesAsync(cancellationToken);
        return sheep;
    }

    public async Task UpdateAsync(Sheep sheep, CancellationToken cancellationToken = default)
    {
        db.Sheep.Update(sheep);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sheep = await db.Sheep.SingleAsync(x => x.Id == id, cancellationToken);
        db.Sheep.Remove(sheep);
        await db.SaveChangesAsync(cancellationToken);
    }
}
