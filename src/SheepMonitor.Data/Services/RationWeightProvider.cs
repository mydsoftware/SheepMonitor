using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// تامین وزن جیره از آخرین رکورد وزن‌گیری یا وزن اولیه ثبت‌شده دام.
/// </summary>
public sealed class RationWeightProvider(SheepMonitorDbContext db) : IRationWeightProvider
{
    public async Task<decimal> GetSheepWeightAsync(int sheepId, CancellationToken cancellationToken = default)
    {
        var sheep = await db.Sheep.AsNoTracking().SingleOrDefaultAsync(x => x.Id == sheepId, cancellationToken)
            ?? throw new InvalidOperationException("گوسفند انتخاب‌شده پیدا نشد.");

        var latest = await db.WeightRecords.AsNoTracking()
            .Where(x => x.SheepId == sheepId)
            .OrderByDescending(x => x.WeighedAt)
            .Select(x => (decimal?)x.WeightKg)
            .FirstOrDefaultAsync(cancellationToken);

        return latest ?? sheep.InitialWeightKg;
    }

    public async Task<decimal> GetAverageHerdWeightAsync(CancellationToken cancellationToken = default)
    {
        var weights = await db.Sheep.AsNoTracking()
            .Select(s => new
            {
                s.Id,
                Initial = s.InitialWeightKg
            })
            .ToListAsync(cancellationToken);

        if (weights.Count == 0) throw new InvalidOperationException("هیچ گوسفندی برای محاسبه میانگین وجود ندارد.");

        var latest = await db.WeightRecords.AsNoTracking()
            .GroupBy(x => x.SheepId)
            .Select(g => new { SheepId = g.Key, Weight = g.OrderByDescending(x => x.WeighedAt).Select(x => x.WeightKg).First() })
            .ToListAsync(cancellationToken);

        var latestMap = latest.ToDictionary(x => x.SheepId, x => x.Weight);
        return weights.Average(x => latestMap.TryGetValue(x.Id, out var weight) ? weight : x.Initial);
    }
}
