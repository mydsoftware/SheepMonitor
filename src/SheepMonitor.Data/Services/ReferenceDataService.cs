using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// پیاده‌سازی مدیریت اطلاعات پایه در SQL Server.
/// </summary>
public sealed class ReferenceDataService(SheepMonitorDbContext db) : IReferenceDataService
{
    public async Task<IReadOnlyList<ReferenceData>> GetAsync(string category, CancellationToken cancellationToken = default) =>
        await db.ReferenceData.AsNoTracking().Where(x => x.Category == category && x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.Title).ToListAsync(cancellationToken);

    public async Task<ReferenceData> SaveAsync(ReferenceData item, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(item.Category)) throw new ArgumentException("دسته اطلاعات پایه الزامی است.");
        if (string.IsNullOrWhiteSpace(item.Code)) throw new ArgumentException("کد اطلاعات پایه الزامی است.");
        if (string.IsNullOrWhiteSpace(item.Title)) throw new ArgumentException("عنوان فارسی الزامی است.");
        item.Code = item.Code.Trim().ToUpperInvariant();
        item.Title = item.Title.Trim();
        if (item.Id == 0) db.ReferenceData.Add(item); else db.ReferenceData.Update(item);
        await db.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task DisableAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await db.ReferenceData.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new InvalidOperationException("رکورد اطلاعات پایه پیدا نشد.");
        item.IsActive = false;
        await db.SaveChangesAsync(cancellationToken);
    }
}
