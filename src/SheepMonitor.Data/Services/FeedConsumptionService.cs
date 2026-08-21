using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;

namespace SheepMonitor.Data.Services;

public sealed class FeedConsumptionService(SheepMonitorDbContext db)
{
    public async Task<FeedConsumptionRecord> AddAsync(FeedConsumptionRecord record, CancellationToken cancellationToken = default)
    {
        if (record.ActualAmountKg < 0) throw new ArgumentOutOfRangeException(nameof(record.ActualAmountKg));
        if (record.WasteAmountKg is < 0) throw new ArgumentOutOfRangeException(nameof(record.WasteAmountKg));
        if (record.WasteAmountKg is not null && record.WasteAmountKg > record.ActualAmountKg)
            throw new ArgumentException("مقدار ضایعات نمی‌تواند بیشتر از مقدار خوراک باشد.", nameof(record.WasteAmountKg));

        db.Set<FeedConsumptionRecord>().Add(record);
        await db.SaveChangesAsync(cancellationToken);
        return record;
    }

    public Task<List<FeedConsumptionRecord>> GetDayAsync(DateTime date, long? sheepId = null, CancellationToken cancellationToken = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return db.Set<FeedConsumptionRecord>()
            .AsNoTracking()
            .Where(x => x.ConsumedAt >= start && x.ConsumedAt < end && (!sheepId.HasValue || x.SheepId == sheepId))
            .OrderBy(x => x.ConsumedAt)
            .ToListAsync(cancellationToken);
    }
}
