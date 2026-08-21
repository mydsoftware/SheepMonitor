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
        Assert.Equal(3, report.Points.Count); // وزن اولیه + ۲ رکورد
        Assert.True(report.Points[0].IsInitial);
        Assert.Equal(4m, report.Points[1].ChangeFromPreviousKg);
        Assert.Equal(5m, report.Points[2].ChangeFromPreviousKg);
        Assert.Equal(40m, report.MinWeightKg);
        Assert.Equal(49m, report.MaxWeightKg);
        Assert.Equal(2, report.WeighingCount);
        Assert.Equal(19, report.PeriodDays); // از ۱ تا ۲۰ مرداد
        Assert.Equal(Math.Round(9m / 19m, 3), report.AverageDailyGainKg);
        Assert.Equal("رشد مثبت", report.GrowthStatus);
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
        Assert.Single(report.Points);
        Assert.True(report.Points[0].IsInitial);
        Assert.Equal(0, report.WeighingCount);
        Assert.Equal("داده وزن‌گیری کافی نیست", report.GrowthStatus);
        Assert.Equal(45m, report.MinWeightKg);
        Assert.Equal(45m, report.MaxWeightKg);
    }

    [Fact]
    public async Task کاهش_وزن_باید_وضعیت_کاهش_وزن_را_نشان_دهد()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"GrowthReport-Loss-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 3,
            Number = "003",
            Gender = "نر",
            HealthStatus = "سالم",
            InitialWeighingDate = new DateTime(2026, 7, 1),
            InitialWeightKg = 50m
        });
        db.WeightRecords.Add(new WeightRecord
        {
            Id = 1,
            SheepId = 3,
            WeighedAt = new DateTime(2026, 7, 15),
            WeightKg = 47m
        });
        await db.SaveChangesAsync();

        var report = await new GrowthReportService(db).GetAsync(3);

        Assert.NotNull(report);
        Assert.Equal(-3m, report!.TotalWeightChangeKg);
        Assert.Equal("کاهش وزن", report.GrowthStatus);
        Assert.Equal(47m, report.MinWeightKg);
        Assert.Equal(50m, report.MaxWeightKg);
    }
}
