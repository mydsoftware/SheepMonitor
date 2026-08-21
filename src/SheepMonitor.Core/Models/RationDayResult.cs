using System.Globalization;

namespace SheepMonitor.Core.Models;

/// <summary>
/// نتیجه محاسبه جیره برای یک روز و وعده‌های تعریف‌شده در دیتابیس.
/// </summary>
public sealed class RationDayResult
{
    public int DayNumber { get; set; }
    public DateTime Date { get; set; }
    public string PersianDate { get { var calendar = new PersianCalendar(); return $"{calendar.GetYear(Date):0000}/{calendar.GetMonth(Date):00}/{calendar.GetDayOfMonth(Date):00}"; } }
    public List<RationMealResult> Meals { get; set; } = [];
    public decimal TotalKg => Meals.Sum(x => x.AmountKg);
}

/// <summary>
/// مقدار یک ماده غذایی در یک وعده.
/// </summary>
public sealed class RationMealResult
{
    public string FeedCode { get; set; } = string.Empty;
    public string FeedTitle { get; set; } = string.Empty;
    public string MealCode { get; set; } = string.Empty;
    public string MealTitle { get; set; } = string.Empty;
    public decimal AmountKg { get; set; }
}
