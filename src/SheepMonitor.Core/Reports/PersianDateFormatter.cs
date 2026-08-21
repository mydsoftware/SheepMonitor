using System.Globalization;

namespace SheepMonitor.Core.Reports;

public static class PersianDateFormatter
{
    public static string Format(DateTime date)
    {
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}";
    }
}
