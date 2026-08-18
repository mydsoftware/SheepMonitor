using System.Windows;
using System.Windows.Controls;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.Views;

/// <summary>
/// اجرای واقعی محاسبه جیره و تبدیل نتیجه به ردیف‌های قابل نمایش.
/// </summary>
public partial class RationResultView : UserControl
{
    private readonly IRationService rationService;
    private readonly List<RationDayResult> results = [];
    private readonly List<RationMealDisplayRow> displayResults = [];

    public RationResultView()
    {
        InitializeComponent();
        rationService = ((App)Application.Current).GetRequiredService<IRationService>();
        DataContext = this;
    }

    public int SelectedDayNumber { get; set; } = 1;
    public int SheepId { get; set; }
    public bool UseAllSheepAverage { get; set; }
    public decimal? WeightKg { get; set; }
    public DateTime PeriodStartDate { get; set; } = DateTime.Today;
    public int PeriodDurationDays { get; set; } = 30;
    public string CalculationTarget { get; set; } = "SingleSheep";
    public IEnumerable<RationMealDisplayRow> DisplayResults => displayResults;

    private async void CalculateDay_Click(object sender, RoutedEventArgs e) => await CalculateAsync(false);

    private async void CalculatePeriod_Click(object sender, RoutedEventArgs e) => await CalculateAsync(true);

    private async Task CalculateAsync(bool wholePeriod)
    {
        try
        {
            UseAllSheepAverage = CalculationTarget == "AverageHerdWeight";
            var request = new RationCalculationRequest
            {
                SheepId = UseAllSheepAverage ? null : SheepId,
                UseAllSheepAverage = UseAllSheepAverage,
                WeightKg = WeightKg,
                DayNumber = SelectedDayNumber,
                PeriodStartDate = PeriodStartDate,
                PeriodDurationDays = PeriodDurationDays
            };

            var calculated = wholePeriod
                ? await rationService.CalculatePeriodAsync(request)
                : [await rationService.CalculateDayAsync(request)];

            results.Clear();
            results.AddRange(calculated);
            displayResults.Clear();
            foreach (var day in results)
                foreach (var meal in day.Meals)
                    displayResults.Add(new RationMealDisplayRow
                    {
                        DayNumber = day.DayNumber,
                        PersianDate = day.PersianDate,
                        MealTitle = meal.MealTitle,
                        FeedTitle = meal.FeedTitle,
                        AmountKg = meal.AmountKg
                    });
            ResultsGrid.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا در محاسبه جیره", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
