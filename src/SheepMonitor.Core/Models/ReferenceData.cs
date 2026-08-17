namespace SheepMonitor.Core.Models;

/// <summary>
/// داده مرجع قابل مدیریت از طریق SQL Server و رابط مدیریت اطلاعات پایه.
/// </summary>
public sealed class ReferenceData
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public string? Notes { get; set; }
}
