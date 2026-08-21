using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// نمایش خروجی جیره روزانه و کل دوره.
/// </summary>
public sealed class RationResultViewModel
{
    private readonly RationPeriodCalculator calculator;

    public ObservableCollection<RationCalculationResult> Results { get; } = [];
    public int SelectedDayNumber { get; set; } = 1;
    public int PeriodDurationDays { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public decimal WeightKg { get; set; }

    public RationResultViewModel(RationPeriodCalculator calculator)
    {
        this.calculator = calculator;
    }

    public void CalculateDay(RationCalculationRequest request, IEnumerable<RationCalculationRule> rules, IEnumerable<RationMealRule> meals)
    {
        Results.Clear();
        foreach (var result in calculator.CalculateDay(request, WeightKg, rules, meals))
            Results.Add(result);
    }

    public void CalculatePeriod(RationCalculationRequest request, IEnumerable<RationCalculationRule> rules, IEnumerable<RationMealRule> meals)
    {
        Results.Clear();
        foreach (var result in calculator.CalculatePeriod(request, WeightKg, rules, meals))
            Results.Add(result);
    }
}
