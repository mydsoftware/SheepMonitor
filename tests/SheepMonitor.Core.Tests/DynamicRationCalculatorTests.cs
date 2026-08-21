using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;
using Xunit;

namespace SheepMonitor.Core.Tests;

public sealed class DynamicRationCalculatorTests
{
    [Fact]
    public void محاسبه_روزانه_و_وعده_ها_باید_بر_اساس_قانون_دیتابیس_باشد()
    {
        var evaluator = new RationFormulaEvaluator();
        var calculator = new DynamicRationCalculator(evaluator);
        var rule = new RationCalculationRule
        {
            Id = 1,
            Name = "قانون آزمایشی",
            Code = "TEST",
            FeedCode = "HAY",
            Formula = "وزن * درصدپایه / 100",
            BasePercent = 2,
            MinimumKg = 0,
            MaximumKg = 100
        };
        var meals = new[]
        {
            new RationMealRule { Id = 1, RationCalculationRuleId = 1, MealCode = "صبح", PercentOfDailyAmount = 50 },
            new RationMealRule { Id = 2, RationCalculationRuleId = 1, MealCode = "شب", PercentOfDailyAmount = 50 }
        };

        var result = calculator.Calculate(50, new DateTime(2026, 1, 1), 10, [rule], meals);

        Assert.Equal(1, result.Select(x => x.DayNumber).Distinct().Single());
        Assert.Equal(1m, result.Sum(x => x.MealAmountKg));
        Assert.Equal(0.5m, result.Single(x => x.MealCode == "صبح").MealAmountKg);
        Assert.Equal(0.5m, result.Single(x => x.MealCode == "شب").MealAmountKg);
    }

    [Fact]
    public void حداقل_و_حداکثر_باید_اعمال_شود()
    {
        var calculator = new DynamicRationCalculator(new RationFormulaEvaluator());
        var rule = new RationCalculationRule { Id = 1, Name = "قانون", Code = "T", FeedCode = "HAY", Formula = "وزن", MinimumKg = 2, MaximumKg = 3 };
        var result = calculator.Calculate(50, DateTime.Today, 1, [rule], []);
        Assert.Equal(3m, result.Single().DailyAmountKg);
    }
}
