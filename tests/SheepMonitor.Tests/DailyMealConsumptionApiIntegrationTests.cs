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

public sealed class DailyMealConsumptionApiIntegrationTests : IClassFixture<DailyMealConsumptionApiFactory>
{
    private readonly HttpClient _client;

    public DailyMealConsumptionApiIntegrationTests(DailyMealConsumptionApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Report_WithoutFilter_ShouldReturnAllDays()
    {
        var response = await _client.GetAsync("/api/feed-consumption/daily/report");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        Assert.Contains("1405/05/27", body);
        Assert.Contains("1405/05/28", body);
    }

    [Fact]
    public async Task Report_WithFromAndTo_ShouldFilterPersianDateRange()
    {
        var response = await _client.GetAsync("/api/feed-consumption/daily/report?from=1405/05/27&to=1405/05/27");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        Assert.Contains("1405/05/27", body);
        Assert.DoesNotContain("1405/05/28", body);
    }

    [Fact]
    public async Task Report_WithInvalidDate_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/api/feed-consumption/daily/report?from=1405/13/01");
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Report_WithReversedRange_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/api/feed-consumption/daily/report?from=1405/05/28&to=1405/05/27");
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}

public sealed class DailyMealConsumptionApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"SheepMonitor-DailyMealConsumption-Tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<SheepMonitorDbContext>>();
            services.RemoveAll<SheepMonitorDbContext>();
            services.AddDbContext<SheepMonitorDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    protected override Microsoft.Extensions.Hosting.IHost CreateHost(Microsoft.Extensions.Hosting.IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SheepMonitorDbContext>();
        db.Database.EnsureCreated();

        if (!db.FeedConsumptionRecords.Any())
        {
            db.FeedPrices.Add(new FeedPrice { Id = 1, FeedCode = "CONCENTRATE", PricePerKg = 100m, Currency = "IRR", EffectiveFrom = new DateTime(2026, 1, 1) });
            db.Sheep.AddRange(
                new Sheep { Id = 1, Number = "001", Gender = "نر", InitialWeighingDate = new DateTime(2026, 8, 18), InitialWeightKg = 50m },
                new Sheep { Id = 2, Number = "002", Gender = "ماده", InitialWeighingDate = new DateTime(2026, 8, 18), InitialWeightKg = 45m });
            db.FeedConsumptionRecords.AddRange(
                new FeedConsumptionRecord { Id = 1, ConsumedAt = new DateTime(2026, 8, 18, 7, 0, 0), FeedCode = "CONCENTRATE", FeedTitle = "کنسانتره", MealCode = "صبح", ActualAmountKg = 10m, WasteAmountKg = 1m },
                new FeedConsumptionRecord { Id = 2, ConsumedAt = new DateTime(2026, 8, 19, 7, 0, 0), FeedCode = "CONCENTRATE", FeedTitle = "کنسانتره", MealCode = "صبح", ActualAmountKg = 12m, WasteAmountKg = 1m });
            db.SaveChanges();
        }
        return host;
    }
}
