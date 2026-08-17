using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

public sealed class ReferenceDataService(SheepMonitorDbContext db) : IReferenceDataService
{
    public async Task<IReadOnlyList<ReferenceData>> GetAsync(string category, CancellationToken cancellationToken = default)
    {
        return await db.ReferenceData
            .AsNoTracking()
            .Where(x => x.Category == category && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReferenceData> AddAsync(ReferenceData item, CancellationToken cancellationToken = default)
    {
        db.ReferenceData.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(ReferenceData item, CancellationToken cancellationToken = default)
    {
        db.ReferenceData.Update(item);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DisableAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await db.ReferenceData.SingleAsync(x => x.Id == id, cancellationToken);
        item.IsActive = false;
        await db.SaveChangesAsync(cancellationToken);
    }
}
