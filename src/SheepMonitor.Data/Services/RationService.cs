using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// موتور محاسبه جیره که قوانین، مواد و وعده‌ها را از SQL Server دریافت می‌کند.
/// </summary>
public sealed class RationService(SheepMonitorDbContext db) : IRationService
{
    public async Task<IReadOnlyList<RationCalculationRule>> GetRulesAsync(CancellationToken cancellationToken = default) => await db.RationCalculationRules.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<RationCalculationRule> AddRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default) { Validate(rule); db.RationCalculationRules.Add(rule); await db.SaveChangesAsync(cancellationToken); return rule; }

    public async Task<RationCalculationRule> UpdateRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default) { Validate(rule); db.RationCalculationRules.Update(rule); await db.SaveChangesAsync(cancellationToken); return rule; }

    public async Task<RationDayResult> CalculateDayAsync(RationCalculationRequest request, CancellationToken cancellationToken = default)
    {
        var weight = await ResolveWeightAsync(request, cancellationToken);
        var rules = await GetRulesAsync(cancellationToken);
        return await CalculateAsync(request.DayNumber, request.PeriodStartDate.AddDays(request.DayNumber - 1), weight, rules, cancellationToken);
    }

    public async Task<IReadOnlyList<RationDayResult>> CalculatePeriodAsync(RationCalculationRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PeriodDurationDays <= 0) throw new ArgumentOutOfRangeException(nameof(request.PeriodDurationDays));
        var weight = await ResolveWeightAsync(request, cancellationToken);
        var rules = await GetRulesAsync(cancellationToken);
        var result = new List<RationDayResult>(request.PeriodDurationDays);
        for (var day = 1; day <= request.PeriodDurationDays; day++) result.Add(await CalculateAsync(day, request.PeriodStartDate.AddDays(day - 1), weight, rules, cancellationToken));
        return result;
    }

    private async Task<decimal> ResolveWeightAsync(RationCalculationRequest request, CancellationToken cancellationToken)
    {
        if (request.WeightKg is > 0) return request.WeightKg.Value;
        if (request.SheepId is int sheepId && !request.UseAllSheepAverage)
        {
            var sheep = await db.Sheep.FindAsync([sheepId], cancellationToken) ?? throw new InvalidOperationException("گوسفند انتخاب‌شده وجود ندارد.");
            return sheep.InitialWeightKg;
        }
        var weights = await db.Sheep.AsNoTracking().Select(x => x.InitialWeightKg).Where(x => x > 0).ToListAsync(cancellationToken);
        if (weights.Count == 0) throw new InvalidOperationException("برای محاسبه، وزن معتبر در دیتابیس وجود ندارد.");
        return weights.Average();
    }

    private async Task<RationDayResult> CalculateAsync(int day, DateTime date, decimal weight, IReadOnlyList<RationCalculationRule> rules, CancellationToken cancellationToken)
    {
        var mealRules = await db.RationMealRules.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);
        var mealCodes = mealRules.Select(x => x.MealCode).Distinct().ToList();
        var meals = await db.ReferenceData.AsNoTracking().Where(x => mealCodes.Contains(x.Code) && x.IsActive).ToListAsync(cancellationToken);
        var result = new RationDayResult { DayNumber = day, Date = date };
        foreach (var rule in rules)
        {
            var amount = Math.Clamp(rule.BasePercent / 100m * weight + rule.WeightCoefficient * weight, rule.MinimumKg, rule.MaximumKg);
            foreach (var mealRule in mealRules.Where(x => x.RationCalculationRuleId == rule.Id))
            {
                var meal = meals.FirstOrDefault(x => x.Code == mealRule.MealCode);
                result.Meals.Add(new RationMealResult { FeedCode = rule.FeedCode, FeedTitle = rule.FeedCode, MealCode = mealRule.MealCode, MealTitle = meal?.Title ?? mealRule.MealCode, AmountKg = amount * mealRule.PercentOfDailyAmount / 100m });
            }
        }
        return result;
    }

    private static void Validate(RationCalculationRule rule)
    {
        if (string.IsNullOrWhiteSpace(rule.Code)) throw new ArgumentException("کد قانون الزامی است.");
        if (string.IsNullOrWhiteSpace(rule.FeedCode)) throw new ArgumentException("کد ماده غذایی الزامی است.");
        if (rule.MinimumKg < 0 || rule.MaximumKg < rule.MinimumKg) throw new ArgumentException("حداقل و حداکثر مقدار قانون نامعتبر است.");
    }
}
