namespace SheepMonitor.Core.Models;

/// <summary>
/// تخصیص یک برنامه غذایی به یک گوسفند در یک بازه زمانی مشخص.
/// </summary>
public sealed class SheepFeedPlanAssignment
{
    public int Id { get; set; }
    public int SheepId { get; set; }
    public int FeedPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
