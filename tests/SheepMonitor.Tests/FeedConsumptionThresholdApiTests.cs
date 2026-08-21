using System.Net;
using System.Net.Http.Json;
using SheepMonitor.Core.Models;
using Xunit;

namespace SheepMonitor.Tests;

public sealed class FeedConsumptionThresholdApiTests : IClassFixture<DailyMealConsumptionApiFactory>
{
    private readonly HttpClient _client;

    public FeedConsumptionThresholdApiTests(DailyMealConsumptionApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task ThresholdCrud_ShouldCreateReadUpdateAndDelete()
    {
        var code = "TEST-THRESHOLD-CRUD";
        var create = new FeedConsumptionThreshold
        {
            FeedCode = code,
            LowDeviationPercent = 10m,
            HighDeviationPercent = 20m,
            IsActive = true,
            Notes = "تست CRUD"
        };

        var post = await _client.PostAsJsonAsync("/api/feed-consumption-thresholds/", create);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var created = await post.Content.ReadFromJsonAsync<FeedConsumptionThreshold>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);

        var get = await _client.GetAsync($"/api/feed-consumption-thresholds/{code}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var loaded = await get.Content.ReadFromJsonAsync<FeedConsumptionThreshold>();
        Assert.Equal(10m, loaded!.LowDeviationPercent);

        var update = new FeedConsumptionThreshold
        {
            FeedCode = code,
            LowDeviationPercent = 12m,
            HighDeviationPercent = 25m,
            IsActive = true,
            Notes = "ویرایش شده"
        };
        var put = await _client.PutAsJsonAsync($"/api/feed-consumption-thresholds/{created.Id}", update);
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        var invalid = new FeedConsumptionThreshold
        {
            FeedCode = code,
            LowDeviationPercent = 30m,
            HighDeviationPercent = 20m,
            IsActive = true
        };
        var invalidPost = await _client.PostAsJsonAsync("/api/feed-consumption-thresholds/", invalid);
        Assert.Equal(HttpStatusCode.BadRequest, invalidPost.StatusCode);

        var delete = await _client.DeleteAsync($"/api/feed-consumption-thresholds/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var missing = await _client.GetAsync($"/api/feed-consumption-thresholds/{code}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
}
