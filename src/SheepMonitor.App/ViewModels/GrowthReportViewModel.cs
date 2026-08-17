using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت نمایش گزارش رشد یک گوسفند.
/// </summary>
public sealed class GrowthReportViewModel(ISheepService sheepService, IGrowthReportService reportService)
{
    public ObservableCollection<Sheep> Sheep { get; } = [];
    public Sheep? SelectedSheep { get; set; }
    public SheepGrowthReport? Report { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Sheep.Clear();
        foreach (var item in await sheepService.GetAllAsync(cancellationToken)) Sheep.Add(item);
    }

    public async Task LoadReportAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null) throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");
        Report = await reportService.GetAsync(SelectedSheep.Id, cancellationToken)
            ?? throw new InvalidOperationException("گزارش رشد این گوسفند پیدا نشد.");
    }
}
