using System.Collections.ObjectModel;
using System.ComponentModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت فرم وزن‌گیری گروهی و محاسبه میانگین وزن‌ها.
/// </summary>
public sealed class WeightEntryViewModel(ISheepService sheepService, IWeightService weightService) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Sheep> Sheep { get; } = [];
    public ObservableCollection<WeightInput> Entries { get; } = [];
    public DateTime WeighedAt { get; set; } = DateTime.Now;

    private decimal _averageWeight;
    public decimal AverageWeight
    {
        get => _averageWeight;
        private set
        {
            if (_averageWeight == value) return;
            _averageWeight = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AverageWeight)));
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Sheep.Clear();
        foreach (var item in await sheepService.GetAllAsync(cancellationToken))
            Sheep.Add(item);
        Entries.Clear();
        foreach (var item in Sheep)
            Entries.Add(new WeightInput(item));
        AverageWeight = 0;
    }

    /// <summary>
    /// میانگین وزن‌های معتبر واردشده را محاسبه می‌کند.
    /// </summary>
    public async Task CalculateAsync()
    {
        AverageWeight = await weightService.CalculateAverageAsync(Entries.Select(x => x.WeightKg));
    }

    /// <summary>
    /// تمام وزن‌های معتبر فرم را در پایگاه داده ثبت می‌کند.
    /// </summary>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await CalculateAsync();
        foreach (var entry in Entries.Where(x => x.WeightKg > 0))
        {
            await weightService.AddAsync(new WeightRecord
            {
                SheepId = entry.Sheep.Id,
                WeighedAt = WeighedAt,
                WeightKg = entry.WeightKg
            }, cancellationToken);
        }
    }
}

public sealed class WeightInput(Sheep sheep)
{
    public Sheep Sheep { get; } = sheep;
    public decimal WeightKg { get; set; }
}
