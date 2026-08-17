using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// سرویس ذخیره و مدیریت سوابق درمان در SQL Server.
/// </summary>
public sealed class TreatmentService(SheepMonitorDbContext db) : ITreatmentService
{
    public async Task<IReadOnlyList<SheepTreatmentRecord>> GetByHealthRecordAsync(int healthRecordId, CancellationToken cancellationToken = default) =>
        await db.SheepTreatmentRecords.AsNoTracking()
            .Where(x => x.HealthRecordId == healthRecordId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(cancellationToken);

    public async Task<SheepTreatmentRecord> AddAsync(SheepTreatmentRecord record, CancellationToken cancellationToken = default)
    {
        if (!await db.SheepHealthRecords.AnyAsync(x => x.Id == record.HealthRecordId, cancellationToken))
            throw new InvalidOperationException("سابقه بیماری انتخاب‌شده وجود ندارد.");
        if (record.Dose is <= 0) throw new ArgumentOutOfRangeException(nameof(record.Dose), "دوز باید بیشتر از صفر باشد.");
        if (record.EndedAt.HasValue && record.EndedAt.Value < record.StartedAt)
            throw new ArgumentException("تاریخ پایان درمان نمی‌تواند قبل از تاریخ شروع باشد.");

        db.SheepTreatmentRecords.Add(record);
        await db.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task UpdateAsync(SheepTreatmentRecord record, CancellationToken cancellationToken = default)
    {
        if (record.Dose is <= 0) throw new ArgumentOutOfRangeException(nameof(record.Dose), "دوز باید بیشتر از صفر باشد.");
        if (record.EndedAt.HasValue && record.EndedAt.Value < record.StartedAt)
            throw new ArgumentException("تاریخ پایان درمان نمی‌تواند قبل از تاریخ شروع باشد.");
        db.SheepTreatmentRecords.Update(record);
        await db.SaveChangesAsync(cancellationToken);
    }
}
