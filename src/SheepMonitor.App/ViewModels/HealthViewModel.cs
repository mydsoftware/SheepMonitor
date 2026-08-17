using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت ثبت و نمایش سوابق سلامت گوسفندان.
/// </summary>
public sealed class HealthViewModel(ISheepService sheepService, IHealthService healthService, IReferenceDataService referenceDataService)
{
    public ObservableCollection<Sheep> Sheep { get; } = [];
    public ObservableCollection<SheepHealthRecord> Records { get; } = [];
    public IReadOnlyList<ReferenceData> Diseases { get; private set; } = [];
    public IReadOnlyList<ReferenceData> Symptoms { get; private set; } = [];
    public IReadOnlyList<ReferenceData> Severities { get; private set; } = [];
    public Sheep? SelectedSheep { get; set; }
    public SheepHealthRecord Model { get; } = new() { StartedAt = DateTime.Now };

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Sheep.Clear();
        foreach (var item in await sheepService.GetAllAsync(cancellationToken)) Sheep.Add(item);
        Diseases = await referenceDataService.GetAsync("بیماری", cancellationToken);
        Symptoms = await referenceDataService.GetAsync("علائم بیماری", cancellationToken);
        Severities = await referenceDataService.GetAsync("شدت بیماری", cancellationToken);
    }

    public async Task LoadRecordsAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null) throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");
        Records.Clear();
        foreach (var item in await healthService.GetBySheepAsync(SelectedSheep.Id, cancellationToken)) Records.Add(item);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null) throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");
        Model.SheepId = SelectedSheep.Id;
        await healthService.AddAsync(Model, cancellationToken);
        await LoadRecordsAsync(cancellationToken);
    }
}
