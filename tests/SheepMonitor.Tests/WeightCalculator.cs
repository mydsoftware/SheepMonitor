namespace SheepMonitor.Core.Services;

/// <summary>
/// پیاده‌سازی موقت محاسبه میانگین برای تست پایه؛ در مرحله بعد تست‌ها مستقیماً به پروژه Core متصل می‌شوند.
/// </summary>
public static class WeightCalculator
{
    public static decimal CalculateAverage(IEnumerable<decimal> weights)
    {
        var values = weights.ToArray();
        if (values.Length == 0)
            throw new ArgumentException("حداقل یک وزن لازم است.", nameof(weights));

        return values.Average();
    }
}
