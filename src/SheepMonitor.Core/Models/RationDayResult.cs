namespace SheepMonitor.Core.Models;

/// <summary>
/// نتیجه محاسبه جیره برای یک روز و سه وعده غذایی.
/// </summary>
public sealed class RationDayResult
{
    public int DayNumber { get; set; }
    public DateTime Date { get; set; }
    public decimal HayKg { get; set; }
    public decimal StrawKg { get; set; }
    public decimal ConcentrateKg { get; set; }
    public decimal TotalKg => HayKg + StrawKg + ConcentrateKg;
}
