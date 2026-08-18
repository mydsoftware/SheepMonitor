using System.Collections.ObjectModel;
using System.Globalization;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// فرم تنظیم قوانین و محاسبه جیره روزانه و دوره‌ای.
/// </summary>
public sealed class RationViewModel(IRationService rationService, ISheepService sheepService, IReferenceDataService referenceDataService)
{
    public ObservableCollection<RationCalculationRule> Rules { get; } = [];
    public ObservableCollection<RationMealRule> MealRules { get; } = [];
    public ObservableCollection<RationPeriod> Periods { get; } = [];
    public ObservableCollection<Sheep> Sheep { get; } = [];
    public ObservableCollection<ReferenceData> Feeds { get; } = [];
    public ObservableCollection<ReferenceData> Meals { get; } = [];
    public ObservableCollection<RationDayResult> Results { get; } = [];
    public RationCalculationRule RuleModel { get; } = new() { IsActive = true, MinimumKg = 0, MaximumKg = 1000 };
    public RationPeriod PeriodModel { get; } = new() { StartDate = DateTime.Today, DurationDays = 30, IsActive = true };
    public Sheep? SelectedSheep { get; set; }
    public RationCalculationRule? SelectedRule { get; set; }
    public RationDayResult? SelectedResult { get; set; }
    public bool UseAllSheepAverage { get; set; }
    public int DayNumber { get; set; } = 1;
    public DateTime PeriodStartDate { get; set; } = DateTime.Today;
    public int PeriodDurationDays { get; set; } = 30;
    public decimal? WeightKg { get; set; }
    public string PersianDate => ToPersianDate(PeriodStartDate.AddDays(Math.Max(0, DayNumber - 1)));

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Rules.Clear(); Sheep.Clear(); Feeds.Clear(); Meals.Clear(); Periods.Clear();
        foreach (var item in await rationService.GetRulesAsync(cancellationToken)) Rules.Add(item);
        foreach (var item in await rationService.GetPeriodsAsync(cancellationToken)) Periods.Add(item);
        foreach (var item in await sheepService.GetAllAsync(cancellationToken)) Sheep.Add(item);
        foreach (var item in await referenceDataService.GetAsync("Feed", cancellationToken)) Feeds.Add(item);
        foreach (var item in await referenceDataService.GetAsync("Meal", cancellationToken)) Meals.Add(item);
        var activePeriod = Periods.FirstOrDefault(x => x.IsActive); if (activePeriod is not null) { PeriodStartDate = activePeriod.StartDate; PeriodDurationDays = activePeriod.DurationDays; }
    }
    public async Task LoadMealRulesAsync(CancellationToken cancellationToken = default) { MealRules.Clear(); if (SelectedRule is null) return; foreach (var item in await rationService.GetMealRulesAsync(SelectedRule.Id, cancellationToken)) MealRules.Add(item); }
    public async Task SaveRuleAsync(CancellationToken cancellationToken = default) { var saved = RuleModel.Id == 0 ? await rationService.AddRuleAsync(RuleModel, cancellationToken) : await rationService.UpdateRuleAsync(RuleModel, cancellationToken); var old = Rules.FirstOrDefault(x => x.Id == saved.Id); if (old is not null) Rules[Rules.IndexOf(old)] = saved; else Rules.Add(saved); SelectedRule = saved; }
    public async Task SaveMealRulesAsync(CancellationToken cancellationToken = default) { foreach (var rule in MealRules) await rationService.SaveMealRuleAsync(rule, cancellationToken); }
    public async Task SavePeriodAsync(CancellationToken cancellationToken = default) { PeriodModel.StartDate = PeriodStartDate; PeriodModel.DurationDays = PeriodDurationDays; PeriodModel.IsActive = true; var saved = await rationService.SavePeriodAsync(PeriodModel, cancellationToken); Periods.Add(saved); }
    public async Task CalculateDayAsync(CancellationToken cancellationToken = default) { Results.Clear(); Results.Add(await rationService.CalculateDayAsync(BuildRequest(), cancellationToken)); SelectedResult = Results.FirstOrDefault(); }
    public async Task CalculatePeriodAsync(CancellationToken cancellationToken = default) { Results.Clear(); foreach (var item in await rationService.CalculatePeriodAsync(BuildRequest(), cancellationToken)) Results.Add(item); SelectedResult = Results.FirstOrDefault(); }
    private RationCalculationRequest BuildRequest() => new() { SheepId = SelectedSheep?.Id, UseAllSheepAverage = UseAllSheepAverage, WeightKg = WeightKg, DayNumber = DayNumber, PeriodStartDate = PeriodStartDate, PeriodDurationDays = PeriodDurationDays };
    private static string ToPersianDate(DateTime date) { var calendar = new PersianCalendar(); return $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}"; }
}
