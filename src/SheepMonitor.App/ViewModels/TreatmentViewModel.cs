using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت ثبت و نمایش درمان‌های مرتبط با سوابق بیماری.
/// نوع درمان، دارو، واحد دوز و نتیجه از اطلاعات پایه SQL Server خوانده می‌شوند.
/// </summary>
public sealed class TreatmentViewModel : INotifyPropertyChanged
{
    private readonly IHealthService _healthService;
    private readonly ITreatmentService _treatmentService;
    private readonly IReferenceDataService _referenceDataService;
    private int? _selectedHealthRecordId;

    public TreatmentViewModel(IHealthService healthService, ITreatmentService treatmentService, IReferenceDataService referenceDataService)
    {
        _healthService = healthService;
        _treatmentService = treatmentService;
        _referenceDataService = referenceDataService;
        Model = new SheepTreatmentRecord { StartedAt = DateTime.Today };
    }

    public ObservableCollection<SheepHealthRecord> HealthRecords { get; } = [];
    public ObservableCollection<SheepTreatmentRecord> Treatments { get; } = [];
    public IReadOnlyList<ReferenceData> TreatmentTypes { get; private set; } = [];
    public IReadOnlyList<ReferenceData> Medicines { get; private set; } = [];
    public IReadOnlyList<ReferenceData> DoseUnits { get; private set; } = [];
    public IReadOnlyList<ReferenceData> TreatmentResults { get; private set; } = [];

    public int? SelectedHealthRecordId
    {
        get => _selectedHealthRecordId;
        set
        {
            if (_selectedHealthRecordId == value) return;
            _selectedHealthRecordId = value;
            OnPropertyChanged();
        }
    }

    public SheepTreatmentRecord Model { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadAsync(int sheepId, CancellationToken cancellationToken = default)
    {
        HealthRecords.Clear();
        foreach (var item in await _healthService.GetBySheepAsync(sheepId, cancellationToken))
            HealthRecords.Add(item);

        TreatmentTypes = await _referenceDataService.GetAsync(ReferenceDataCategories.TreatmentType, cancellationToken);
        Medicines = await _referenceDataService.GetAsync(ReferenceDataCategories.Medication, cancellationToken);
        DoseUnits = await _referenceDataService.GetAsync(ReferenceDataCategories.DoseUnit, cancellationToken);
        TreatmentResults = await _referenceDataService.GetAsync(ReferenceDataCategories.TreatmentResult, cancellationToken);

        OnPropertyChanged(nameof(TreatmentTypes));
        OnPropertyChanged(nameof(Medicines));
        OnPropertyChanged(nameof(DoseUnits));
        OnPropertyChanged(nameof(TreatmentResults));
    }

    public async Task LoadTreatmentsAsync(CancellationToken cancellationToken = default)
    {
        if (!SelectedHealthRecordId.HasValue)
            throw new InvalidOperationException("ابتدا یک سابقه بیماری را انتخاب کنید.");

        Treatments.Clear();
        foreach (var item in await _treatmentService.GetByHealthRecordAsync(SelectedHealthRecordId.Value, cancellationToken))
            Treatments.Add(item);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!SelectedHealthRecordId.HasValue)
            throw new InvalidOperationException("ابتدا یک سابقه بیماری را انتخاب کنید.");

        if (string.IsNullOrWhiteSpace(Model.TreatmentCode))
            throw new InvalidOperationException("انتخاب نوع درمان الزامی است.");

        Model.HealthRecordId = SelectedHealthRecordId.Value;
        await _treatmentService.AddAsync(Model, cancellationToken);

        Model.TreatmentCode = string.Empty;
        Model.MedicineCode = string.Empty;
        Model.Dose = null;
        Model.DoseUnitCode = null;
        Model.DailyFrequency = null;
        Model.EndedAt = null;
        Model.ResultCode = null;
        Model.Notes = null;
        Model.StartedAt = DateTime.Today;

        await LoadTreatmentsAsync(cancellationToken);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
