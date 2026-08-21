using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// فرم ثبت و ویرایش اطلاعات گوسفند.
/// </summary>
public sealed class SheepEditorViewModel(ISheepService sheepService, IReferenceDataService referenceDataService)
{
    public Sheep Model { get; private set; } = new() { InitialWeighingDate = DateTime.Today };
    public IReadOnlyList<ReferenceData> Genders { get; private set; } = [];
    public IReadOnlyList<ReferenceData> HealthStatuses { get; private set; } = [];

    /// <summary>
    /// اطلاعات دام انتخاب‌شده را برای ویرایش در فرم قرار می‌دهد.
    /// </summary>
    public void Load(Sheep sheep)
    {
        Model = new Sheep
        {
            Id = sheep.Id,
            Number = sheep.Number,
            ImagePath = sheep.ImagePath,
            BirthDate = sheep.BirthDate,
            Gender = sheep.Gender,
            InitialWeightKg = sheep.InitialWeightKg,
            InitialWeighingDate = sheep.InitialWeighingDate,
            IsSick = sheep.IsSick,
            HealthStatus = sheep.HealthStatus,
            HealthNotes = sheep.HealthNotes
        };
    }

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        Genders = await referenceDataService.GetAsync(ReferenceDataCategories.Gender, cancellationToken);
        HealthStatuses = await referenceDataService.GetAsync(ReferenceDataCategories.HealthStatus, cancellationToken);
    }

    /// <summary>
    /// در صورت دام جدید، ثبت و در صورت دام موجود، ویرایش را انجام می‌دهد.
    /// </summary>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(Model.Number))
            throw new InvalidOperationException("شماره دام الزامی است.");
        if (string.IsNullOrWhiteSpace(Model.Gender))
            throw new InvalidOperationException("جنسیت دام الزامی است.");
        if (Model.InitialWeightKg <= 0)
            throw new InvalidOperationException("وزن اولیه باید بیشتر از صفر باشد.");

        if (Model.Id == 0)
            Model = await sheepService.AddAsync(Model, cancellationToken);
        else
            await sheepService.UpdateAsync(Model, cancellationToken);
    }
}
