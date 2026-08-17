using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// پیاده‌سازی مدیریت سوابق سلامت و بیماری در SQL Server.
/// </summary>
public sealed class HealthService(SheepMonitorDbContext db) : IHealthService
{
    public async Task<IReadOnlyList<HealthRecord>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default) =>
        await db.HealthRecords.AsNoTracking()
            .Where(x => x.SheepId == sheepId)
            .OrderByDescending(x => x.RecordedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<HealthRecord> AddAsync(HealthRecord record, CancellationToken cancellationToken = default)
    {
        if (!await db.Sheep.AnyAsync(x => x.Id == record.SheepId, cancellationToken))
            throw new InvalidOperationException("گوسفند انتخاب‌شده در پایگاه داده وجود ندارد.");
        if (record.RecordedAt == default)
            throw new InvalidOperationException("تاریخ ثبت وضعیت سلامت الزامی است.");

        db.HealthRecords.Add(record);
        await db.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task UpdateAsync(HealthRecord record, CancellationToken cancellationToken = default)
    {
        var existing = await db.HealthRecords.SingleOrDefaultAsync(x => x.Id == record.Id, cancellationToken)
            ?? throw new InvalidOperationException("سابقه سلامت پیدا نشد.");

        existing.RecordedAt = record.RecordedAt;
        existing.IsSick = record.IsSick;
        existing.Status = record.Status;
        existing.DiseaseName = record.DiseaseName;
        existing.Symptoms = record.Symptoms;
        existing.Treatment = record.Treatment;
        existing.Notes = record.Notes;
        await db.SaveChangesAsync(cancellationToken);
    }
}
