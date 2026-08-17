using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

public sealed class SheepEditorViewModel(ISheepService sheepService, IReferenceDataService referenceDataService)
{
    public Sheep Model { get; } = new() { InitialWeighingDate = DateTime.Now };
    public IReadOnlyList<ReferenceData> Genders { get; private set; } = [];
    public IReadOnlyList<ReferenceData> HealthStatuses { get; private set; } = [];

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        Genders = await referenceDataService.GetAsync(ReferenceDataCategories.Gender, cancellationToken);
        HealthStatuses = await referenceDataService.GetAsync(ReferenceDataCategories.HealthStatus, cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(Model.Number)) throw new InvalidOperationException("شماره دام الزامی است.");
        if (Model.InitialWeightKg <= 0) throw new InvalidOperationException("وزن اولیه باید بیشتر از صفر باشد.");
        await sheepService.AddAsync(Model, cancellationToken);
    }
}
