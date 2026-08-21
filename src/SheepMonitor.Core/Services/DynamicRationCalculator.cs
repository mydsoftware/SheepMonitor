using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// محاسبه مقدار روزانه و مقدار هر وعده با قوانین قابل تنظیم دیتابیس.
/// </summary>
public sealed class DynamicRationCalculator(IRationFormulaEvaluator formulaEvaluator)
{
    public IReadOnlyList<RationCalculationResult> Calculate(
        decimal weightKg,
        DateTime startDate,
        int dayNumber,
        IEnumerable<RationCalculationRule> rules,
        IEnumerable<RationMealRule> meals)
    {
        if (weightKg <= 0) throw new ArgumentException("وزن باید بیشتر از صفر باشد.");
        if (dayNumber <= 0) throw new ArgumentException("شماره روز باید بیشتر از صفر باشد.");

        var date = startDate.Date.AddDays(dayNumber - 1);
        var results = new List<RationCalculationResult>();
        foreach (var rule in rules.Where(x => x.IsActive))
        {
            var daily = formulaEvaluator.Evaluate(rule.Formula, weightKg, rule.BasePercent, rule.WeightCoefficient);
            daily = Math.Clamp(daily, rule.MinimumKg, rule.MaximumKg);
            var ruleMeals = meals.Where(x => x.RationCalculationRuleId == rule.Id).ToList();
            if (ruleMeals.Count == 0)
            {
                results.Add(new RationCalculationResult { DayNumber = dayNumber, Date = date, FeedCode = rule.FeedCode, WeightKg = weightKg, DailyAmountKg = daily, MealAmountKg = daily });
                continue;
            }
            foreach (var meal in ruleMeals)
            {
                results.Add(new RationCalculationResult
                {
                    DayNumber = dayNumber,
                    Date = date,
                    MealCode = meal.MealCode,
                    FeedCode = rule.FeedCode,
                    WeightKg = weightKg,
                    DailyAmountKg = daily,
                    MealAmountKg = daily * meal.PercentOfDailyAmount / 100m
                });
            }
        }
        return results;
    }
}
