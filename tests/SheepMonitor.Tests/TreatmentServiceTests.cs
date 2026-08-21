using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Data;
using SheepMonitor.Data.Services;
using Xunit;

namespace SheepMonitor.Tests;

/// <summary>
/// تست ثبت و بازیابی سوابق درمان وابسته به سابقه بیماری.
/// </summary>
public sealed class TreatmentServiceTests
{
    [Fact]
    public async Task ثبت_درمان_باید_به_سابقه_بیماری_وصل_شود()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"Treatment-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 1,
            Number = "T001",
            Gender = "نر",
            HealthStatus = "بیمار",
            InitialWeighingDate = DateTime.Today.AddDays(-20),
            InitialWeightKg = 40m,
            IsSick = true
        });
        db.SheepHealthRecords.Add(new SheepHealthRecord
        {
            Id = 5,
            SheepId = 1,
            DiseaseCode = "FOOTROT",
            SymptomsCode = "LIMP",
            SeverityCode = "MODERATE",
            StartedAt = DateTime.Today.AddDays(-3)
        });
        await db.SaveChangesAsync();

        var service = new TreatmentService(db);
        var treatment = await service.AddAsync(new SheepTreatmentRecord
        {
            HealthRecordId = 5,
            TreatmentCode = "ANTIBIOTIC",
            MedicineCode = "OXYTET",
            Dose = 5.5m,
            DoseUnitCode = "ML",
            DailyFrequency = 2,
            StartedAt = DateTime.Today,
            Notes = "تزریق عضلانی"
        });

        Assert.True(treatment.Id > 0);

        var list = await service.GetByHealthRecordAsync(5);
        Assert.Single(list);
        Assert.Equal("ANTIBIOTIC", list[0].TreatmentCode);
        Assert.Equal(5.5m, list[0].Dose);
    }

    [Fact]
    public async Task درمان_بدون_سابقه_بیماری_باید_خطا_دهد()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"Treatment-NoHealth-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        var service = new TreatmentService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(new SheepTreatmentRecord
        {
            HealthRecordId = 999,
            TreatmentCode = "ANTIBIOTIC",
            StartedAt = DateTime.Today
        }));
    }

    [Fact]
    public async Task دوز_منفی_باید_خطا_دهد()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase($"Treatment-Dose-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.Add(new Sheep
        {
            Id = 2,
            Number = "T002",
            Gender = "ماده",
            HealthStatus = "بیمار",
            InitialWeighingDate = DateTime.Today,
            InitialWeightKg = 35m,
            IsSick = true
        });
        db.SheepHealthRecords.Add(new SheepHealthRecord
        {
            Id = 7,
            SheepId = 2,
            DiseaseCode = "MASTITIS",
            StartedAt = DateTime.Today
        });
        await db.SaveChangesAsync();

        var service = new TreatmentService(db);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddAsync(new SheepTreatmentRecord
        {
            HealthRecordId = 7,
            TreatmentCode = "ANTIBIOTIC",
            Dose = -1m,
            StartedAt = DateTime.Today
        }));
    }
}
