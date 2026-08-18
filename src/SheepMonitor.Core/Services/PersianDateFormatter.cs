namespace SheepMonitor.Core.Services;

/// <summary>
/// تبدیل تاریخ میلادی به نمایش تاریخ شمسی بدون وابستگی به رابط کاربری.
/// </summary>
public sealed class PersianDateFormatter
{
    public string Format(DateTime date)
    {
        var calendar = new System.Globalization.PersianCalendar();
        return $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}";
    }
}
