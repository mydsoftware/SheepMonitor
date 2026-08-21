using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Data;
using SheepMonitor.Data.Services;
using Xunit;

namespace SheepMonitor.Tests;

/// <summary>
/// تست گزارش رشد بر اساس وزن اولیه و رکوردهای وزن‌گیری ذخیره‌شده.
/// </summary>
public sealed class GrowthReportServiceTests
{
    [Fact]
    public async Task گزارش_رشد_باید_آخرین_وزن_و_تغییرات_را_محاسبه_کند()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"GrowthReport-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 1,
            Number = "001",
            Gender = "نر",
            HealthStatus = "سالم",
            InitialWeighingDate = new DateTime(2026, 8, 1),
            InitialWeightKg = 40m
        });
        db.WeightRecords.AddRange(
            new WeightRecord { Id = 1, SheepId = 1, WeighedAt = new DateTime(2026, 8, 10), WeightKg = 44m },
            new WeightRecord { Id = 2, SheepId = 1, WeighedAt = new DateTime(2026, 8, 20), WeightKg = 49m });

        await db.SaveChangesAsync();

        var service = new GrowthReportService(db);
        var report = await service.GetAsync(1);

        Assert.NotNull(report);
        Assert.Equal(40m, report!.InitialWeightKg);
        Assert.Equal(49m, report.LatestWeightKg);
        Assert.Equal(9m, report.TotalWeightChangeKg);
        Assert.Equal(4.5m, report.AverageWeightChangeKg);
        Assert.Equal(2, report.Points.Count);
        Assert.Equal(4m, report.Points[0].ChangeFromPreviousKg);
        Assert.Equal(5m, report.Points[1].ChangeFromPreviousKg);
    }

    [Fact]
    public async Task گوسفند_بدون_وزن_دوره_ای_باید_وزن_اولیه_را_آخرین_وزن_درنظر_بگیرد()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"GrowthReport-Empty-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 2,
            Number = "002",
            Gender = "ماده",
            HealthStatus = "سالم",
            InitialWeighingDate = DateTime.Today,
            InitialWeightKg = 45m
        });
        await db.SaveChangesAsync();

        var report = await new GrowthReportService(db).GetAsync(2);

        Assert.NotNull(report);
        Assert.Equal(45m, report!.LatestWeightKg);
        Assert.Equal(0m, report.TotalWeightChangeKg);
        Assert.Empty(report.Points);
    }
}
