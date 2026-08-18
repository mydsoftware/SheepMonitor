namespace SheepMonitor.Core.Services;

/// <summary>
/// ارزیابی فرمول ذخیره‌شده در دیتابیس با متغیرهای محاسباتی.
/// </summary>
public interface IRationFormulaEvaluator
{
    decimal Evaluate(string? formula, decimal weightKg, decimal basePercent, decimal weightCoefficient);
}
