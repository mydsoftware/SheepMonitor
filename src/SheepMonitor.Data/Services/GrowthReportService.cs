using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// سرویس تهیه گزارش رشد از اطلاعات ذخیره‌شده در SQL Server.
/// </summary>
public sealed class GrowthReportService(SheepMonitorDbContext db) : IGrowthReportService
{
    public async Task<SheepGrowthReport?> GetAsync(int sheepId, CancellationToken cancellationToken = default)
    {
        var sheep = await db.Sheep.AsNoTracking().SingleOrDefaultAsync(x => x.Id == sheepId, cancellationToken);
        if (sheep is null) return null;

        var records = await db.WeightRecords.AsNoTracking()
            .Where(x => x.SheepId == sheepId)
            .OrderBy(x => x.WeighedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var points = new List<WeightGrowthPoint>();
        var previousWeight = sheep.InitialWeightKg;
        foreach (var record in records)
        {
            points.Add(new WeightGrowthPoint
            {
                Date = record.WeighedAt,
                WeightKg = record.WeightKg,
                ChangeFromPreviousKg = Math.Round(record.WeightKg - previousWeight, 2)
            });
            previousWeight = record.WeightKg;
        }

        var latest = records.Count == 0 ? sheep.InitialWeightKg : records[^1].WeightKg;
        var changes = points.Select(x => x.ChangeFromPreviousKg).ToList();

        return new SheepGrowthReport
        {
            SheepId = sheep.Id,
            SheepNumber = sheep.Number,
            InitialWeightKg = sheep.InitialWeightKg,
            LatestWeightKg = latest,
            TotalWeightChangeKg = Math.Round(latest - sheep.InitialWeightKg, 2),
            AverageWeightChangeKg = changes.Count == 0 ? 0 : Math.Round(changes.Average(), 2),
            Points = points
        };
    }
}
