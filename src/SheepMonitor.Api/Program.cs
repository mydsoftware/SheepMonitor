using Microsoft.EntityFrameworkCore;
using SheepMonitor.Data;
using SheepMonitor.Api;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<SheepMonitorDbContext>(options =>
        options.UseInMemoryDatabase("SheepMonitor-DailyMealConsumption-Tests"));
}
else
{
    builder.Services.AddDbContext<SheepMonitorDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=SheepMonitor;Trusted_Connection=True;TrustServerCertificate=True"));
}

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapFeedConsumptionEndpoints();
app.MapMealConsumptionEndpoints();

app.Run();
