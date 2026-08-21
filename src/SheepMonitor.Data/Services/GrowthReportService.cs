using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// سرویس تهیه گزارش رشد از اطلاعات ذخیره‌شده در SQL Server.
/// تمام محاسبات بر اساس رکوردهای واقعی وزن‌گیری و وزن اولیه انجام می‌شود.
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

        // نقطه شروع: وزن اولیه
        var points = new List<WeightGrowthPoint>
        {
            new()
            {
                Date = sheep.InitialWeighingDate,
                WeightKg = sheep.InitialWeightKg,
                ChangeFromPreviousKg = 0m,
                IsInitial = true
            }
        };

        var previousWeight = sheep.InitialWeightKg;
        foreach (var record in records)
        {
            points.Add(new WeightGrowthPoint
            {
                Date = record.WeighedAt,
                WeightKg = record.WeightKg,
                ChangeFromPreviousKg = Math.Round(record.WeightKg - previousWeight, 2),
                IsInitial = false
            });
            previousWeight = record.WeightKg;
        }

        var latest = records.Count == 0 ? sheep.InitialWeightKg : records[^1].WeightKg;
        var totalChange = Math.Round(latest - sheep.InitialWeightKg, 2);

        // تغییرات بین وزن‌گیری‌ها (بدون نقطه اولیه)
        var sequentialChanges = points.Where(p => !p.IsInitial).Select(x => x.ChangeFromPreviousKg).ToList();
        var averageChange = sequentialChanges.Count == 0
            ? 0m
            : Math.Round(sequentialChanges.Average(), 2);

        // محاسبه میانگین افزایش روزانه بر اساس بازه زمانی واقعی
        var firstDate = points[0].Date.Date;
        var lastDate = points[^1].Date.Date;
        var periodDays = Math.Max(0, (lastDate - firstDate).Days);
        var averageDailyGain = periodDays > 0
            ? Math.Round(totalChange / periodDays, 3)
            : 0m;

        var allWeights = points.Select(p => p.WeightKg).ToList();
        var minWeight = allWeights.Min();
        var maxWeight = allWeights.Max();

        var growthStatus = DetermineGrowthStatus(totalChange, records.Count);

        return new SheepGrowthReport
        {
            SheepId = sheep.Id,
            SheepNumber = sheep.Number,
            InitialWeightKg = sheep.InitialWeightKg,
            LatestWeightKg = latest,
            MinWeightKg = minWeight,
            MaxWeightKg = maxWeight,
            TotalWeightChangeKg = totalChange,
            AverageWeightChangeKg = averageChange,
            AverageDailyGainKg = averageDailyGain,
            PeriodDays = periodDays,
            WeighingCount = records.Count,
            GrowthStatus = growthStatus,
            Points = points
        };
    }

    /// <summary>
    /// تعیین وضعیت رشد صرفاً بر اساس جهت تغییر وزن و وجود داده.
    /// آستانه‌های عددی از دیتابیس خوانده نمی‌شوند تا نیاز به جدول جدید نباشد؛
    /// در صورت نیاز بعدی می‌توان از ReferenceData یا جدول آستانه استفاده کرد.
    /// </summary>
    private static string DetermineGrowthStatus(decimal totalChange, int weighingCount)
    {
        if (weighingCount == 0)
            return "داده وزن‌گیری کافی نیست";

        if (totalChange > 0)
            return "رشد مثبت";

        if (totalChange < 0)
            return "کاهش وزن";

        return "بدون تغییر وزن";
    }
}
