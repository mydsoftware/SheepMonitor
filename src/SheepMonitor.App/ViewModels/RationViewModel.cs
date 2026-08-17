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
    public ObservableCollection<Sheep> Sheep { get; } = [];
    public ObservableCollection<ReferenceData> Feeds { get; } = [];
    public ObservableCollection<ReferenceData> Meals { get; } = [];
    public ObservableCollection<RationDayResult> Results { get; } = [];
    public RationCalculationRule RuleModel { get; } = new() { IsActive = true, MinimumKg = 0, MaximumKg = 1000 };
    public Sheep? SelectedSheep { get; set; }
    public bool UseAllSheepAverage { get; set; }
    public int DayNumber { get; set; } = 1;
    public DateTime PeriodStartDate { get; set; } = DateTime.Today;
    public int PeriodDurationDays { get; set; } = 30;
    public decimal? WeightKg { get; set; }
    public string PersianDate => ToPersianDate(PeriodStartDate.AddDays(Math.Max(0, DayNumber - 1)));

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Rules.Clear(); Sheep.Clear(); Feeds.Clear(); Meals.Clear();
        foreach (var item in await rationService.GetRulesAsync(cancellationToken)) Rules.Add(item);
        foreach (var item in await sheepService.GetAllAsync(cancellationToken)) Sheep.Add(item);
        foreach (var item in await referenceDataService.GetAsync("ماده غذایی", cancellationToken)) Feeds.Add(item);
        foreach (var item in await referenceDataService.GetAsync("وعده غذایی", cancellationToken)) Meals.Add(item);
    }

    public async Task SaveRuleAsync(CancellationToken cancellationToken = default)
    {
        if (RuleModel.Id == 0) Rules.Add(await rationService.AddRuleAsync(RuleModel, cancellationToken));
        else { var saved = await rationService.UpdateRuleAsync(RuleModel, cancellationToken); var old = Rules.FirstOrDefault(x => x.Id == saved.Id); if (old is not null) Rules[Rules.IndexOf(old)] = saved; }
    }

    public async Task CalculateDayAsync(CancellationToken cancellationToken = default)
    {
        Results.Clear();
        Results.Add(await rationService.CalculateDayAsync(BuildRequest(), cancellationToken));
    }

    public async Task CalculatePeriodAsync(CancellationToken cancellationToken = default)
    {
        Results.Clear();
        foreach (var item in await rationService.CalculatePeriodAsync(BuildRequest(), cancellationToken)) Results.Add(item);
    }

    private RationCalculationRequest BuildRequest() => new()
    {
        SheepId = SelectedSheep?.Id,
        UseAllSheepAverage = UseAllSheepAverage,
        WeightKg = WeightKg,
        DayNumber = DayNumber,
        PeriodStartDate = PeriodStartDate,
        PeriodDurationDays = PeriodDurationDays
    };

    private static string ToPersianDate(DateTime date)
    {
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}";
    }
}
