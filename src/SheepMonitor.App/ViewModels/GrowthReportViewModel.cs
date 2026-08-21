using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت نمایش گزارش رشد یک گوسفند با پشتیبانی از به‌روزرسانی رابط کاربری.
/// </summary>
public sealed class GrowthReportViewModel : INotifyPropertyChanged
{
    private readonly ISheepService _sheepService;
    private readonly IGrowthReportService _reportService;
    private Sheep? _selectedSheep;
    private SheepGrowthReport? _report;

    public GrowthReportViewModel(ISheepService sheepService, IGrowthReportService reportService)
    {
        _sheepService = sheepService;
        _reportService = reportService;
    }

    public ObservableCollection<Sheep> Sheep { get; } = [];

    public Sheep? SelectedSheep
    {
        get => _selectedSheep;
        set
        {
            if (_selectedSheep == value) return;
            _selectedSheep = value;
            OnPropertyChanged();
        }
    }

    public SheepGrowthReport? Report
    {
        get => _report;
        private set
        {
            _report = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasReport));
        }
    }

    public bool HasReport => Report is not null;

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Sheep.Clear();
        foreach (var item in await _sheepService.GetAllAsync(cancellationToken))
            Sheep.Add(item);
    }

    public async Task LoadReportAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null)
            throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");

        Report = await _reportService.GetAsync(SelectedSheep.Id, cancellationToken)
            ?? throw new InvalidOperationException("گزارش رشد این گوسفند پیدا نشد.");
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
