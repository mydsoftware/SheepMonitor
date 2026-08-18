using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// پیاده‌سازی مدیریت قوانین جیره در SQL Server.
/// </summary>
public sealed class RationRuleService(SheepMonitorDbContext db) : IRationRuleService
{
    public async Task<IReadOnlyList<RationCalculationRule>> GetRulesAsync(CancellationToken cancellationToken = default) =>
        await db.RationCalculationRules.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RationMealRule>> GetMealsAsync(int ruleId, CancellationToken cancellationToken = default) =>
        await db.RationMealRules.AsNoTracking().Where(x => x.RationCalculationRuleId == ruleId).OrderBy(x => x.MealCode).ToListAsync(cancellationToken);

    public async Task SaveRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rule.Name) || string.IsNullOrWhiteSpace(rule.Code) || string.IsNullOrWhiteSpace(rule.FeedCode))
            throw new ArgumentException("نام، کد قانون و کد ماده غذایی الزامی هستند.");
        if (rule.MinimumKg < 0 || rule.MaximumKg < rule.MinimumKg)
            throw new ArgumentException("حداقل و حداکثر مقدار جیره نامعتبر است.");

        if (rule.Id == 0) db.RationCalculationRules.Add(rule);
        else db.RationCalculationRules.Update(rule);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveMealAsync(RationMealRule meal, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(meal.MealCode)) throw new ArgumentException("کد وعده الزامی است.");
        if (meal.PercentOfDailyAmount < 0) throw new ArgumentException("درصد وعده نمی‌تواند منفی باشد.");
        if (!await db.RationCalculationRules.AnyAsync(x => x.Id == meal.RationCalculationRuleId, cancellationToken))
            throw new InvalidOperationException("قانون جیره پیدا نشد.");

        if (meal.Id == 0) db.RationMealRules.Add(meal);
        else db.RationMealRules.Update(meal);

        var total = await db.RationMealRules
            .Where(x => x.RationCalculationRuleId == meal.RationCalculationRuleId && x.Id != meal.Id)
            .SumAsync(x => (decimal?)x.PercentOfDailyAmount, cancellationToken) ?? 0;
        total += meal.PercentOfDailyAmount;
        if (total > 100.000m) throw new ArgumentException("مجموع درصد وعده‌ها نمی‌تواند بیشتر از ۱۰۰ درصد باشد.");

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRuleAsync(int ruleId, CancellationToken cancellationToken = default)
    {
        var rule = await db.RationCalculationRules.FindAsync([ruleId], cancellationToken)
            ?? throw new InvalidOperationException("قانون جیره پیدا نشد.");
        db.RationCalculationRules.Remove(rule);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMealAsync(int mealId, CancellationToken cancellationToken = default)
    {
        var meal = await db.RationMealRules.FindAsync([mealId], cancellationToken)
            ?? throw new InvalidOperationException("وعده جیره پیدا نشد.");
        db.RationMealRules.Remove(meal);
        await db.SaveChangesAsync(cancellationToken);
    }
}
