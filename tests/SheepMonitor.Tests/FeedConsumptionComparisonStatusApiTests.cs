using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SheepMonitor.Api;
using SheepMonitor.Core.Models;
using SheepMonitor.Data;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedConsumptionComparisonStatusApiTests : IClassFixture<FeedComparisonApiFactory>
{
    private readonly HttpClient _client;

    public FeedConsumptionComparisonStatusApiTests(FeedComparisonApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Comparison_ShouldResolveLowNormalAndHighStatusesFromDatabase()
    {
        var response = await _client.GetAsync("/api/feed-consumption/comparison");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("کم‌مصرف", body);
        Assert.Contains("نرمال", body);
        Assert.Contains("پرمصرف", body);
    }
}

public sealed class FeedComparisonApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"SheepMonitor-FeedComparison-Tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<SheepMonitorDbContext>>();
            services.RemoveAll<SheepMonitorDbContext>();
            services.AddDbContext<SheepMonitorDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }

    protected override Microsoft.Extensions.Hosting.IHost CreateHost(Microsoft.Extensions.Hosting.IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SheepMonitorDbContext>();
        db.Database.EnsureCreated();

        db.Sheep.Add(new Sheep
        {
            Id = 1,
            Number = "001",
            Gender = "نر",
            InitialWeighingDate = new DateTime(2026, 8, 18),
            InitialWeightKg = 50m
        });

        db.FeedConsumptionRecords.AddRange(
            new FeedConsumptionRecord { Id = 1, ConsumedAt = new DateTime(2026, 8, 18, 7, 0, 0), FeedCode = "LOW", FeedTitle = "کم", MealCode = "صبح", ActualAmountKg = 8m },
            new FeedConsumptionRecord { Id = 2, ConsumedAt = new DateTime(2026, 8, 18, 8, 0, 0), FeedCode = "NORMAL", FeedTitle = "نرمال", MealCode = "صبح", ActualAmountKg = 10m },
            new FeedConsumptionRecord { Id = 3, ConsumedAt = new DateTime(2026, 8, 18, 9, 0, 0), FeedCode = "HIGH", FeedTitle = "زیاد", MealCode = "صبح", ActualAmountKg = 12m });

        db.FeedConsumptionItems.AddRange(
            new FeedConsumptionItem { Id = 1, FeedConsumptionRecordId = 1, FeedCode = "LOW", PlannedKg = 10m, ActualKg = 8m, WasteKg = 0m },
            new FeedConsumptionItem { Id = 2, FeedConsumptionRecordId = 2, FeedCode = "NORMAL", PlannedKg = 10m, ActualKg = 10m, WasteKg = 0m },
            new FeedConsumptionItem { Id = 3, FeedConsumptionRecordId = 3, FeedCode = "HIGH", PlannedKg = 10m, ActualKg = 12m, WasteKg = 0m });

        db.FeedConsumptionThresholds.AddRange(
            new FeedConsumptionThreshold { Id = 1, FeedCode = "LOW", LowDeviationPercent = 10m, HighDeviationPercent = 20m, IsActive = true },
            new FeedConsumptionThreshold { Id = 2, FeedCode = "NORMAL", LowDeviationPercent = 10m, HighDeviationPercent = 20m, IsActive = true },
            new FeedConsumptionThreshold { Id = 3, FeedCode = "HIGH", LowDeviationPercent = 10m, HighDeviationPercent = 20m, IsActive = true });

        db.SaveChanges();
        return host;
    }
}
