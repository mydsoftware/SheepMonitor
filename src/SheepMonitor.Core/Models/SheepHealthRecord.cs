namespace SheepMonitor.Core.Models;

/// <summary>
/// یک سابقه وضعیت سلامت و بیماری برای گوسفند.
/// </summary>
public sealed class SheepHealthRecord
{
    public int Id { get; set; }
    public int SheepId { get; set; }
    public string DiseaseCode { get; set; } = string.Empty;
    public string SymptomsCode { get; set; } = string.Empty;
    public string SeverityCode { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public string? VeterinaryNotes { get; set; }
}
