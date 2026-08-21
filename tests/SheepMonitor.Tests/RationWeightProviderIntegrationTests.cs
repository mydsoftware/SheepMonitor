using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Data;
using SheepMonitor.Data.Services;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class RationWeightProviderIntegrationTests
{
    [Fact]
    public async Task AverageHerdWeight_UsesLatestWeight_AndInitialWeightWhenMissing()
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new SheepMonitorDbContext(options);
        db.Sheep.AddRange(
            new Sheep { Id = 1, Number = "001", Gender = "نر", HealthStatus = "سالم", InitialWeightKg = 40 },
            new Sheep { Id = 2, Number = "002", Gender = "نر", HealthStatus = "سالم", InitialWeightKg = 50 },
            new Sheep { Id = 3, Number = "003", Gender = "ماده", HealthStatus = "سالم", InitialWeightKg = 60 });

        db.WeightRecords.AddRange(
            new WeightRecord { SheepId = 1, WeighedAt = new DateTime(2026, 1, 1), WeightKg = 42 },
            new WeightRecord { SheepId = 1, WeighedAt = new DateTime(2026, 1, 10), WeightKg = 46 },
            new WeightRecord { SheepId = 2, WeighedAt = new DateTime(2026, 1, 5), WeightKg = 54 });

        await db.SaveChangesAsync();

        var provider = new RationWeightProvider(db);
        var average = await provider.GetAverageHerdWeightAsync();

        Assert.Equal(160m / 3m, average);
        Assert.Equal(46m, await provider.GetSheepWeightAsync(1));
        Assert.Equal(54m, await provider.GetSheepWeightAsync(2));
        Assert.Equal(60m, await provider.GetSheepWeightAsync(3));
    }
}
