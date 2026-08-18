namespace SheepMonitor.App.Views;

/// <summary>
/// ردیف تخت‌شده برای نمایش ماده غذایی و وعده در جدول جیره.
/// </summary>
public sealed class RationMealDisplayRow
{
    public int DayNumber { get; init; }
    public string PersianDate { get; init; } = string.Empty;
    public string MealTitle { get; init; } = string.Empty;
    public string FeedTitle { get; init; } = string.Empty;
    public decimal AmountKg { get; init; }
}
