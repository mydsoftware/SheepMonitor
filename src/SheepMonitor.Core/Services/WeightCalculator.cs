namespace SheepMonitor.Core.Services;

/// <summary>
/// محاسبات مربوط به وزن‌گیری گروهی.
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
