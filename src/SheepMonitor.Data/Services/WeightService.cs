using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// سرویس ذخیره و محاسبه اطلاعات وزن‌گیری.
/// </summary>
public sealed class WeightService(SheepMonitorDbContext db) : IWeightService
{
    public async Task<IReadOnlyList<WeightRecord>> GetBySheepAsync(int sheepId, CancellationToken cancellationToken = default) =>
        await db.WeightRecords.AsNoTracking()
            .Where(x => x.SheepId == sheepId)
            .OrderByDescending(x => x.WeighedAt)
            .ToListAsync(cancellationToken);

    public async Task<WeightRecord> AddAsync(WeightRecord record, CancellationToken cancellationToken = default)
    {
        if (record.WeightKg <= 0) throw new ArgumentOutOfRangeException(nameof(record.WeightKg), "وزن باید بیشتر از صفر باشد.");
        if (!await db.Sheep.AnyAsync(x => x.Id == record.SheepId, cancellationToken))
            throw new InvalidOperationException("گوسفند انتخاب‌شده در پایگاه داده وجود ندارد.");

        db.WeightRecords.Add(record);
        await db.SaveChangesAsync(cancellationToken);
        return record;
    }

    public Task<decimal> CalculateAverageAsync(IEnumerable<decimal> weights)
    {
        var values = weights.Where(x => x > 0).ToList();
        if (values.Count == 0) throw new InvalidOperationException("حداقل یک وزن معتبر لازم است.");
        return Task.FromResult(Math.Round(values.Average(), 2));
    }
}
