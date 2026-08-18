using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// تولید خروجی جیره برای روز انتخابی یا همه روزهای دوره.
/// </summary>
public sealed class RationPeriodCalculator(DynamicRationCalculator calculator)
{
    public IReadOnlyList<RationCalculationResult> CalculateDay(RationCalculationRequest request, decimal weightKg, IEnumerable<RationCalculationRule> rules, IEnumerable<RationMealRule> meals)
    {
        Validate(request);
        return calculator.Calculate(weightKg, request.PeriodStartDate, request.DayNumber, rules, meals);
    }

    public IReadOnlyList<RationCalculationResult> CalculatePeriod(RationCalculationRequest request, decimal weightKg, IEnumerable<RationCalculationRule> rules, IEnumerable<RationMealRule> meals)
    {
        Validate(request);
        var result = new List<RationCalculationResult>();
        for (var day = 1; day <= request.PeriodDurationDays; day++)
            result.AddRange(calculator.Calculate(weightKg, request.PeriodStartDate, day, rules, meals));
        return result;
    }

    private static void Validate(RationCalculationRequest request)
    {
        if (request.PeriodDurationDays <= 0) throw new ArgumentException("تعداد روزهای دوره باید بیشتر از صفر باشد.");
        if (request.DayNumber < 1 || request.DayNumber > request.PeriodDurationDays)
            throw new ArgumentException("روز انتخاب‌شده خارج از محدوده دوره است.");
    }
}
