using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// پیاده‌سازی مدیریت سوابق سلامت و بیماری در SQL Server.
/// </summary>
public sealed class HealthService(SheepMonitorDbContext db) : IHealthService
{
    public async Task<IReadOnlyList<SheepHealthRecord>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default) =>
        await db.SheepHealthRecords.AsNoTracking()
            .Where(x => x.SheepId == sheepId)
            .OrderByDescending(x => x.StartedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<SheepHealthRecord> AddAsync(SheepHealthRecord record, CancellationToken cancellationToken = default)
    {
        if (!await db.Sheep.AnyAsync(x => x.Id == record.SheepId, cancellationToken))
            throw new InvalidOperationException("گوسفند انتخاب‌شده در پایگاه داده وجود ندارد.");

        if (string.IsNullOrWhiteSpace(record.DiseaseCode))
            throw new InvalidOperationException("کد بیماری الزامی است.");

        if (record.StartedAt == default)
            throw new InvalidOperationException("تاریخ شروع بیماری الزامی است.");

        if (record.RecoveredAt.HasValue && record.RecoveredAt.Value < record.StartedAt)
            throw new ArgumentException("تاریخ بهبودی نمی‌تواند قبل از تاریخ شروع باشد.");

        db.SheepHealthRecords.Add(record);

        // به‌روزرسانی وضعیت سلامت گوسفند در صورت بیماری فعال
        var sheep = await db.Sheep.SingleAsync(x => x.Id == record.SheepId, cancellationToken);
        if (!record.RecoveredAt.HasValue)
        {
            sheep.IsSick = true;
            sheep.HealthStatus = "بیمار";
        }

        await db.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task UpdateAsync(SheepHealthRecord record, CancellationToken cancellationToken = default)
    {
        var existing = await db.SheepHealthRecords.SingleOrDefaultAsync(x => x.Id == record.Id, cancellationToken)
            ?? throw new InvalidOperationException("سابقه سلامت پیدا نشد.");

        if (record.RecoveredAt.HasValue && record.RecoveredAt.Value < record.StartedAt)
            throw new ArgumentException("تاریخ بهبودی نمی‌تواند قبل از تاریخ شروع باشد.");

        existing.DiseaseCode = record.DiseaseCode;
        existing.SymptomsCode = record.SymptomsCode;
        existing.SeverityCode = record.SeverityCode;
        existing.StartedAt = record.StartedAt;
        existing.RecoveredAt = record.RecoveredAt;
        existing.VeterinaryNotes = record.VeterinaryNotes;

        await RefreshSheepHealthStatusAsync(existing.SheepId, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkRecoveredAsync(int recordId, DateTime recoveredAt, CancellationToken cancellationToken = default)
    {
        var existing = await db.SheepHealthRecords.SingleOrDefaultAsync(x => x.Id == recordId, cancellationToken)
            ?? throw new InvalidOperationException("سابقه سلامت پیدا نشد.");

        if (recoveredAt < existing.StartedAt)
            throw new ArgumentException("تاریخ بهبودی نمی‌تواند قبل از تاریخ شروع باشد.");

        existing.RecoveredAt = recoveredAt;
        await RefreshSheepHealthStatusAsync(existing.SheepId, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// اگر هیچ بیماری فعالی باقی نمانده باشد، وضعیت گوسفند به سالم برمی‌گردد.
    /// از موجودیت‌های tracked استفاده می‌کند تا تغییرات قبل از SaveChanges دیده شوند.
    /// </summary>
    private async Task RefreshSheepHealthStatusAsync(int sheepId, CancellationToken cancellationToken)
    {
        // بارگذاری با tracking تا تغییرات محلی (مثل RecoveredAt) در محاسبه لحاظ شوند
        var records = await db.SheepHealthRecords
            .Where(x => x.SheepId == sheepId)
            .ToListAsync(cancellationToken);

        var hasActiveDisease = records.Any(x => x.RecoveredAt == null);

        var sheep = await db.Sheep.SingleAsync(x => x.Id == sheepId, cancellationToken);
        sheep.IsSick = hasActiveDisease;
        sheep.HealthStatus = hasActiveDisease ? "بیمار" : "سالم";
    }
}
