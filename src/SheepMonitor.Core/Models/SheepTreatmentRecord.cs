namespace SheepMonitor.Core.Models;

/// <summary>
/// یک سابقه درمان برای بیماری ثبت‌شده گوسفند.
/// </summary>
public sealed class SheepTreatmentRecord
{
    public int Id { get; set; }
    public int HealthRecordId { get; set; }
    public string TreatmentCode { get; set; } = string.Empty;
    public string MedicineCode { get; set; } = string.Empty;
    public decimal? Dose { get; set; }
    public string? DoseUnitCode { get; set; }
    public int? DailyFrequency { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? ResultCode { get; set; }
    public string? Notes { get; set; }
}
