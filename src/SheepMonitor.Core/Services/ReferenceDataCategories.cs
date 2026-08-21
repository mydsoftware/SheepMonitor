namespace SheepMonitor.Core.Services;

/// <summary>
/// دسته‌بندی‌های اطلاعات پایه قابل تنظیم از SQL Server.
/// مقادیر واقعی (عنوان و کد) فقط از دیتابیس خوانده می‌شوند و هاردکد نیستند.
/// </summary>
public static class ReferenceDataCategories
{
    public const string Gender = "Gender";
    public const string HealthStatus = "HealthStatus";
    public const string Symptom = "Symptom";
    public const string Disease = "Disease";
    public const string Severity = "Severity";
    public const string FeedType = "FeedType";
    public const string Medication = "Medication";
    public const string Unit = "Unit";
    public const string AnimalGroup = "AnimalGroup";
    public const string TreatmentType = "TreatmentType";
    public const string TreatmentResult = "TreatmentResult";
    public const string DoseUnit = "DoseUnit";
}
