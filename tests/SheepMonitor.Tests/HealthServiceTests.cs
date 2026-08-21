using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Data;
using SheepMonitor.Data.Services;
using Xunit;

namespace SheepMonitor.Tests;

/// <summary>
/// تست ثبت و به‌روزرسانی سوابق بیماری و وضعیت سلامت گوسفند.
/// </summary>
public sealed class HealthServiceTests
{
    [Fact]
    public async Task ثبت_بیماری_باید_وضعیت_گوسفند_را_بیمار_کند()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"Health-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 1,
            Number = "H001",
            Gender = "نر",
            HealthStatus = "سالم",
            InitialWeighingDate = DateTime.Today,
            InitialWeightKg = 40m,
            IsSick = false
        });
        await db.SaveChangesAsync();

        var service = new HealthService(db);
        var record = await service.AddAsync(new SheepHealthRecord
        {
            SheepId = 1,
            DiseaseCode = "FOOTROT",
            SymptomsCode = "LIMP",
            SeverityCode = "MODERATE",
            StartedAt = DateTime.Today,
            VeterinaryNotes = "نیاز به درمان"
        });

        Assert.True(record.Id > 0);

        var sheep = await db.Sheep.SingleAsync(x => x.Id == 1);
        Assert.True(sheep.IsSick);
        Assert.Equal("بیمار", sheep.HealthStatus);

        var list = await service.GetBySheepAsync(1);
        Assert.Single(list);
        Assert.Equal("FOOTROT", list[0].DiseaseCode);
    }

    [Fact]
    public async Task ثبت_بهبودی_باید_وضعیت_گوسفند_را_سالم_کند()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"Health-Recover-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 2,
            Number = "H002",
            Gender = "ماده",
            HealthStatus = "بیمار",
            InitialWeighingDate = DateTime.Today.AddDays(-10),
            InitialWeightKg = 38m,
            IsSick = true
        });
        db.SheepHealthRecords.Add(new SheepHealthRecord
        {
            Id = 10,
            SheepId = 2,
            DiseaseCode = "MASTITIS",
            SymptomsCode = "SWELL",
            SeverityCode = "HIGH",
            StartedAt = DateTime.Today.AddDays(-5)
        });
        await db.SaveChangesAsync();

        var service = new HealthService(db);
        await service.MarkRecoveredAsync(10, DateTime.Today);

        var sheep = await db.Sheep.SingleAsync(x => x.Id == 2);
        Assert.False(sheep.IsSick);
        Assert.Equal("سالم", sheep.HealthStatus);

        var record = await db.SheepHealthRecords.SingleAsync(x => x.Id == 10);
        Assert.NotNull(record.RecoveredAt);
    }

    [Fact]
    public async Task بدون_کد_بیماری_باید_خطا_دهد()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"Health-Invalid-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 3,
            Number = "H003",
            Gender = "نر",
            HealthStatus = "سالم",
            InitialWeighingDate = DateTime.Today,
            InitialWeightKg = 42m
        });
        await db.SaveChangesAsync();

        var service = new HealthService(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(new SheepHealthRecord
        {
            SheepId = 3,
            DiseaseCode = "",
            StartedAt = DateTime.Today
        }));
    }
}
