using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت ثبت و نمایش درمان‌های مرتبط با سوابق بیماری.
/// </summary>
public sealed class TreatmentViewModel(IHealthService healthService, ITreatmentService treatmentService, IReferenceDataService referenceDataService)
{
    public ObservableCollection<HealthRecord> HealthRecords { get; } = [];
    public ObservableCollection<SheepTreatmentRecord> Treatments { get; } = [];
    public IReadOnlyList<ReferenceData> TreatmentTypes { get; private set; } = [];
    public IReadOnlyList<ReferenceData> Medicines { get; private set; } = [];
    public IReadOnlyList<ReferenceData> DoseUnits { get; private set; } = [];
    public IReadOnlyList<ReferenceData> TreatmentResults { get; private set; } = [];
    public int? SelectedHealthRecordId { get; set; }
    public SheepTreatmentRecord Model { get; } = new() { StartedAt = DateTime.Now };

    public async Task LoadAsync(int sheepId, CancellationToken cancellationToken = default)
    {
        HealthRecords.Clear();
        foreach (var item in await healthService.GetBySheepAsync(sheepId, cancellationToken)) HealthRecords.Add(item);
        TreatmentTypes = await referenceDataService.GetAsync("نوع درمان", cancellationToken);
        Medicines = await referenceDataService.GetAsync("دارو", cancellationToken);
        DoseUnits = await referenceDataService.GetAsync("واحد دوز", cancellationToken);
        TreatmentResults = await referenceDataService.GetAsync("نتیجه درمان", cancellationToken);
    }

    public async Task LoadTreatmentsAsync(CancellationToken cancellationToken = default)
    {
        if (!SelectedHealthRecordId.HasValue) throw new InvalidOperationException("ابتدا یک سابقه بیماری را انتخاب کنید.");
        Treatments.Clear();
        foreach (var item in await treatmentService.GetByHealthRecordAsync(SelectedHealthRecordId.Value, cancellationToken)) Treatments.Add(item);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!SelectedHealthRecordId.HasValue) throw new InvalidOperationException("ابتدا یک سابقه بیماری را انتخاب کنید.");
        Model.HealthRecordId = SelectedHealthRecordId.Value;
        await treatmentService.AddAsync(Model, cancellationToken);
        await LoadTreatmentsAsync(cancellationToken);
    }
}
