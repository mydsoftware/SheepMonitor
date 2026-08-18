using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Tests;

public sealed class DynamicRationCalculatorTests
{
    [Fact]
    public void Calculate_ForOneSheep_UsesWeightAndSplitsDailyAmountAcrossMeals()
    {
        var calculator = new DynamicRationCalculator(new FixedFormulaEvaluator());
        var rules = new[]
        {
            new RationCalculationRule
            {
                Id = 1, Name = "یونجه", Code = "HAY", FeedCode = "HAY", IsActive = true,
                BasePercent = 2, WeightCoefficient = 0, MinimumKg = 0, MaximumKg = 10,
                Formula = "weight * basePercent / 100"
            }
        };
        var meals = new[]
        {
            new RationMealRule { Id = 1, RationCalculationRuleId = 1, MealCode = "MORNING", PercentOfDailyAmount = 40, IsActive = true },
            new RationMealRule { Id = 2, RationCalculationRuleId = 1, MealCode = "NOON", PercentOfDailyAmount = 30, IsActive = true },
            new RationMealRule { Id = 3, RationCalculationRuleId = 1, MealCode = "NIGHT", PercentOfDailyAmount = 30, IsActive = true }
        };

        var result = calculator.Calculate(50m, new DateTime(2026, 8, 1), 10, rules, meals);

        Assert.Equal(3, result.Count);
        Assert.All(result, x => Assert.Equal(10, x.DayNumber));
        Assert.All(result, x => Assert.Equal(new DateTime(2026, 8, 10), x.Date));
        Assert.Equal(1m, result[0].DailyAmountKg);
        Assert.Equal(0.4m, result.Single(x => x.MealCode == "MORNING").MealAmountKg);
        Assert.Equal(0.3m, result.Single(x => x.MealCode == "NOON").MealAmountKg);
        Assert.Equal(0.3m, result.Single(x => x.MealCode == "NIGHT").MealAmountKg);
        Assert.Equal(1m, result.Sum(x => x.MealAmountKg));
    }

    [Fact]
    public void Calculate_AppliesMinimumAndMaximumLimits()
    {
        var calculator = new DynamicRationCalculator(new FixedFormulaEvaluator(25m));
        var rule = new RationCalculationRule
        {
            Id = 2, FeedCode = "CONCENTRATE", IsActive = true,
            MinimumKg = 1.5m, MaximumKg = 2m, BasePercent = 0, WeightCoefficient = 0,
            Formula = "fixed"
        };

        var result = calculator.Calculate(60m, new DateTime(2026, 8, 1), 1, new[] { rule }, Array.Empty<RationMealRule>());

        Assert.Single(result);
        Assert.Equal(2m, result[0].DailyAmountKg);
        Assert.Equal(2m, result[0].MealAmountKg);
    }

    [Fact]
    public void Calculate_WithoutMealRules_ReturnsTheWholeDailyAmountAsOneMeal()
    {
        var calculator = new DynamicRationCalculator(new FixedFormulaEvaluator(1.25m));
        var rule = new RationCalculationRule
        {
            Id = 3, FeedCode = "STRAW", IsActive = true,
            MinimumKg = 0, MaximumKg = 10, BasePercent = 0, WeightCoefficient = 0
        };

        var result = calculator.Calculate(50m, new DateTime(2026, 8, 1), 5, new[] { rule }, Array.Empty<RationMealRule>());

        Assert.Single(result);
        Assert.Equal(1.25m, result[0].DailyAmountKg);
        Assert.Equal(1.25m, result[0].MealAmountKg);
        Assert.Equal(string.Empty, result[0].MealCode);
    }

    private sealed class FixedFormulaEvaluator(decimal? value = null) : IRationFormulaEvaluator
    {
        private readonly decimal? _value = value;

        public decimal Evaluate(string? formula, decimal weightKg, decimal basePercent, decimal weightCoefficient)
            => _value ?? weightKg * basePercent / 100m + weightKg * weightCoefficient;
    }
}
