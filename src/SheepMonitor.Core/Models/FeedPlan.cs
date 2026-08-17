namespace SheepMonitor.Core.Models;

/// <summary>
/// برنامه غذایی قابل مدیریت برای یک گروه یا شرایط مشخص از گوسفندان.
/// </summary>
public sealed class FeedPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TargetGroup { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// یک ماده غذایی و مقدار مصرف روزانه در برنامه غذایی.
/// </summary>
public sealed class FeedPlanItem
{
    public int Id { get; set; }
    public int FeedPlanId { get; set; }
    public string FeedCode { get; set; } = string.Empty;
    public decimal AmountPerDay { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public int MealsPerDay { get; set; }
    public string? Notes { get; set; }
}
