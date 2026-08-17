using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SheepMonitor.App.ViewModels;
using SheepMonitor.Core.Services;
using SheepMonitor.Data;
using SheepMonitor.Data.Services;

namespace SheepMonitor.App;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();
        services.AddDbContext<SheepMonitorDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SheepMonitor")));
        services.AddScoped<IReferenceDataService, ReferenceDataService>();
        services.AddScoped<ISheepService, SheepService>();
        services.AddTransient<ReferenceDataViewModel>();
        services.AddTransient<SheepViewModel>();
        _services = services.BuildServiceProvider();
    }

    public T GetRequiredService<T>() where T : notnull => _services!.GetRequiredService<T>();

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}
