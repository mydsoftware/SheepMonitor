using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت ثبت و نمایش سوابق سلامت و بیماری گوسفندان.
/// بیماری، علائم و شدت از اطلاعات پایه SQL Server خوانده می‌شوند.
/// </summary>
public sealed class HealthViewModel : INotifyPropertyChanged
{
    private readonly ISheepService _sheepService;
    private readonly IHealthService _healthService;
    private readonly IReferenceDataService _referenceDataService;
    private Sheep? _selectedSheep;
    private SheepHealthRecord? _selectedRecord;

    public HealthViewModel(ISheepService sheepService, IHealthService healthService, IReferenceDataService referenceDataService)
    {
        _sheepService = sheepService;
        _healthService = healthService;
        _referenceDataService = referenceDataService;
        Model = new SheepHealthRecord { StartedAt = DateTime.Today };
    }

    public ObservableCollection<Sheep> Sheep { get; } = [];
    public ObservableCollection<SheepHealthRecord> Records { get; } = [];
    public IReadOnlyList<ReferenceData> Diseases { get; private set; } = [];
    public IReadOnlyList<ReferenceData> Symptoms { get; private set; } = [];
    public IReadOnlyList<ReferenceData> Severities { get; private set; } = [];

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

    public SheepHealthRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            _selectedRecord = value;
            OnPropertyChanged();
        }
    }

    public SheepHealthRecord Model { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Sheep.Clear();
        foreach (var item in await _sheepService.GetAllAsync(cancellationToken))
            Sheep.Add(item);

        Diseases = await _referenceDataService.GetAsync(ReferenceDataCategories.Disease, cancellationToken);
        Symptoms = await _referenceDataService.GetAsync(ReferenceDataCategories.Symptom, cancellationToken);
        Severities = await _referenceDataService.GetAsync(ReferenceDataCategories.Severity, cancellationToken);
        OnPropertyChanged(nameof(Diseases));
        OnPropertyChanged(nameof(Symptoms));
        OnPropertyChanged(nameof(Severities));
    }

    public async Task LoadRecordsAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null)
            throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");

        Records.Clear();
        foreach (var item in await _healthService.GetBySheepAsync(SelectedSheep.Id, cancellationToken))
            Records.Add(item);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null)
            throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");

        if (string.IsNullOrWhiteSpace(Model.DiseaseCode))
            throw new InvalidOperationException("انتخاب بیماری الزامی است.");

        Model.SheepId = SelectedSheep.Id;
        await _healthService.AddAsync(Model, cancellationToken);

        // آماده‌سازی فرم برای ثبت بعدی
        Model.DiseaseCode = string.Empty;
        Model.SymptomsCode = string.Empty;
        Model.SeverityCode = string.Empty;
        Model.VeterinaryNotes = null;
        Model.StartedAt = DateTime.Today;
        Model.RecoveredAt = null;

        await LoadRecordsAsync(cancellationToken);
    }

    public async Task MarkSelectedRecoveredAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedRecord is null)
            throw new InvalidOperationException("ابتدا یک سابقه بیماری را از جدول انتخاب کنید.");

        if (SelectedRecord.RecoveredAt.HasValue)
            throw new InvalidOperationException("این سابقه قبلاً به عنوان بهبودیافته ثبت شده است.");

        await _healthService.MarkRecoveredAsync(SelectedRecord.Id, DateTime.Today, cancellationToken);
        await LoadRecordsAsync(cancellationToken);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
