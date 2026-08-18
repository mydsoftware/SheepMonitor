using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// ویرایش قوانین، فرمول‌ها و درصد وعده‌های جیره از داخل برنامه.
/// </summary>
public sealed class RationRuleEditorViewModel(IRationRuleService service)
{
    public ObservableCollection<RationCalculationRule> Rules { get; } = [];
    public ObservableCollection<RationMealRule> Meals { get; } = [];
    public RationCalculationRule? SelectedRule { get; set; }
    public string? Message { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Rules.Clear();
        foreach (var rule in await service.GetRulesAsync(cancellationToken)) Rules.Add(rule);
        if (SelectedRule is not null) await LoadMealsAsync(cancellationToken);
    }

    public async Task LoadMealsAsync(CancellationToken cancellationToken = default)
    {
        Meals.Clear();
        if (SelectedRule is null) return;
        foreach (var meal in await service.GetMealsAsync(SelectedRule.Id, cancellationToken)) Meals.Add(meal);
    }

    public async Task SaveRuleAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedRule is null) throw new InvalidOperationException("ابتدا یک قانون جیره را انتخاب کنید.");
        await service.SaveRuleAsync(SelectedRule, cancellationToken);
        Message = "قانون جیره ذخیره شد.";
        await LoadAsync(cancellationToken);
    }

    public async Task SaveMealAsync(RationMealRule meal, CancellationToken cancellationToken = default)
    {
        if (SelectedRule is null) throw new InvalidOperationException("ابتدا قانون جیره را انتخاب کنید.");
        meal.RationCalculationRuleId = SelectedRule.Id;
        await service.SaveMealAsync(meal, cancellationToken);
        Message = "درصد وعده ذخیره شد.";
        await LoadMealsAsync(cancellationToken);
    }
}
