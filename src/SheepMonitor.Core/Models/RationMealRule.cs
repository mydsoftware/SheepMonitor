namespace SheepMonitor.Core.Models;

/// <summary>
/// سهم هر وعده از مقدار روزانه یک ماده غذایی.
/// </summary>
public sealed class RationMealRule
{
    public int Id { get; set; }
    public int RationCalculationRuleId { get; set; }
    public string MealCode { get; set; } = string.Empty;
    public decimal PercentOfDailyAmount { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
