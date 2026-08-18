using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

/// <summary>
/// موتور محاسبه جیره که قوانین، وعده‌ها، مواد غذایی و وزن را از SQL Server دریافت می‌کند.
/// </summary>
public sealed class RationService(SheepMonitorDbContext db, IRationFormulaEvaluator formulaEvaluator) : IRationService
{
    public async Task<IReadOnlyList<RationCalculationRule>> GetRulesAsync(CancellationToken cancellationToken = default) => await db.RationCalculationRules.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public async Task<RationCalculationRule> AddRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default) { Validate(rule); db.RationCalculationRules.Add(rule); await db.SaveChangesAsync(cancellationToken); return rule; }
    public async Task<RationCalculationRule> UpdateRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default) { Validate(rule); db.RationCalculationRules.Update(rule); await db.SaveChangesAsync(cancellationToken); return rule; }
    public async Task<IReadOnlyList<RationMealRule>> GetMealRulesAsync(int ruleId, CancellationToken cancellationToken = default) => await db.RationMealRules.AsNoTracking().Where(x => x.RationCalculationRuleId == ruleId && x.IsActive).OrderBy(x => x.MealCode).ToListAsync(cancellationToken);
    public async Task<RationMealRule> SaveMealRuleAsync(RationMealRule rule, CancellationToken cancellationToken = default) { if (rule.PercentOfDailyAmount < 0 || rule.PercentOfDailyAmount > 100) throw new ArgumentOutOfRangeException(nameof(rule.PercentOfDailyAmount)); var total = await db.RationMealRules.Where(x => x.RationCalculationRuleId == rule.RationCalculationRuleId && x.Id != rule.Id && x.IsActive).SumAsync(x => x.PercentOfDailyAmount, cancellationToken); if (total + rule.PercentOfDailyAmount > 100m) throw new InvalidOperationException("مجموع درصد وعده‌ها نمی‌تواند بیشتر از ۱۰۰ درصد باشد."); if (rule.Id == 0) db.RationMealRules.Add(rule); else db.RationMealRules.Update(rule); await db.SaveChangesAsync(cancellationToken); return rule; }
    public async Task<IReadOnlyList<RationPeriod>> GetPeriodsAsync(CancellationToken cancellationToken = default) => await db.RationPeriods.AsNoTracking().OrderByDescending(x => x.IsActive).ThenByDescending(x => x.StartDate).ToListAsync(cancellationToken);
    public async Task<RationPeriod> SavePeriodAsync(RationPeriod period, CancellationToken cancellationToken = default) { if (period.DurationDays <= 0) throw new ArgumentOutOfRangeException(nameof(period.DurationDays)); if (period.Id == 0) db.RationPeriods.Add(period); else db.RationPeriods.Update(period); await db.SaveChangesAsync(cancellationToken); return period; }

    public async Task<RationDayResult> CalculateDayAsync(RationCalculationRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        var weight = await ResolveWeightAsync(request, cancellationToken);
        var rules = await GetRulesAsync(cancellationToken);
        return await CalculateAsync(request.DayNumber, request.PeriodStartDate.AddDays(request.DayNumber - 1), weight, rules, cancellationToken);
    }

    public async Task<IReadOnlyList<RationDayResult>> CalculatePeriodAsync(RationCalculationRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        var weight = await ResolveWeightAsync(request, cancellationToken);
        var rules = await GetRulesAsync(cancellationToken);
        var result = new List<RationDayResult>(request.PeriodDurationDays);
        for (var day = 1; day <= request.PeriodDurationDays; day++)
            result.Add(await CalculateAsync(day, request.PeriodStartDate.AddDays(day - 1), weight, rules, cancellationToken));
        return result;
    }

    private async Task<decimal> ResolveWeightAsync(RationCalculationRequest request, CancellationToken cancellationToken)
    {
        if (request.WeightKg is > 0) return request.WeightKg.Value;
        if (request.SheepId is int sheepId && !request.UseAllSheepAverage)
        {
            var latest = await db.WeightRecords.AsNoTracking().Where(x => x.SheepId == sheepId).OrderByDescending(x => x.WeighedAt).Select(x => (decimal?)x.WeightKg).FirstOrDefaultAsync(cancellationToken);
            if (latest is > 0) return latest.Value;
            var sheep = await db.Sheep.AsNoTracking().SingleOrDefaultAsync(x => x.Id == sheepId, cancellationToken) ?? throw new InvalidOperationException("گوسفند انتخاب‌شده وجود ندارد.");
            if (sheep.InitialWeightKg <= 0) throw new InvalidOperationException("وزن معتبر برای گوسفند انتخاب‌شده وجود ندارد.");
            return sheep.InitialWeightKg;
        }

        var sheepIds = await db.Sheep.AsNoTracking().Select(x => x.Id).ToListAsync(cancellationToken);
        var weights = new List<decimal>();
        foreach (var id in sheepIds)
        {
            var latest = await db.WeightRecords.AsNoTracking().Where(x => x.SheepId == id).OrderByDescending(x => x.WeighedAt).Select(x => (decimal?)x.WeightKg).FirstOrDefaultAsync(cancellationToken);
            if (latest is > 0) weights.Add(latest.Value);
            else
            {
                var initial = await db.Sheep.AsNoTracking().Where(x => x.Id == id).Select(x => (decimal?)x.InitialWeightKg).FirstOrDefaultAsync(cancellationToken);
                if (initial is > 0) weights.Add(initial.Value);
            }
        }
        if (weights.Count == 0) throw new InvalidOperationException("برای محاسبه میانگین، وزن معتبر در دیتابیس وجود ندارد.");
        return weights.Average();
    }

    private async Task<RationDayResult> CalculateAsync(int day, DateTime date, decimal weight, IReadOnlyList<RationCalculationRule> rules, CancellationToken cancellationToken)
    {
        var mealRules = await db.RationMealRules.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);
        var mealCodes = mealRules.Select(x => x.MealCode).Distinct().ToList();
        var feedCodes = rules.Select(x => x.FeedCode).Distinct().ToList();
        var references = await db.ReferenceData.AsNoTracking()
            .Where(x => x.IsActive && ((x.Category == "Meal" && mealCodes.Contains(x.Code)) || (x.Category == "Feed" && feedCodes.Contains(x.Code))))
            .ToListAsync(cancellationToken);
        var result = new RationDayResult { DayNumber = day, Date = date };
        foreach (var rule in rules)
        {
            var dailyAmount = formulaEvaluator.Evaluate(rule.Formula, weight, rule.BasePercent, rule.WeightCoefficient);
            dailyAmount = Math.Clamp(dailyAmount, rule.MinimumKg, rule.MaximumKg);
            var feedTitle = references.FirstOrDefault(x => x.Category == "Feed" && x.Code == rule.FeedCode)?.Title ?? rule.FeedCode;
            foreach (var mealRule in mealRules.Where(x => x.RationCalculationRuleId == rule.Id))
            {
                var mealTitle = references.FirstOrDefault(x => x.Category == "Meal" && x.Code == mealRule.MealCode)?.Title ?? mealRule.MealCode;
                result.Meals.Add(new RationMealResult { FeedCode = rule.FeedCode, FeedTitle = feedTitle, MealCode = mealRule.MealCode, MealTitle = mealTitle, AmountKg = dailyAmount * mealRule.PercentOfDailyAmount / 100m });
            }
        }
        return result;
    }

    private static void ValidateRequest(RationCalculationRequest request)
    {
        if (request.PeriodDurationDays <= 0) throw new ArgumentOutOfRangeException(nameof(request.PeriodDurationDays));
        if (request.DayNumber < 1 || request.DayNumber > request.PeriodDurationDays) throw new ArgumentOutOfRangeException(nameof(request.DayNumber));
        if (!request.UseAllSheepAverage && request.SheepId is null && request.WeightKg is not > 0) throw new InvalidOperationException("یک گوسفند یا میانگین گله را انتخاب کنید.");
    }

    private static void Validate(RationCalculationRule rule) { if (string.IsNullOrWhiteSpace(rule.Code)) throw new ArgumentException("کد قانون الزامی است."); if (string.IsNullOrWhiteSpace(rule.FeedCode)) throw new ArgumentException("کد ماده غذایی الزامی است."); if (rule.MinimumKg < 0 || rule.MaximumKg < rule.MinimumKg) throw new ArgumentException("حداقل و حداکثر مقدار قانون نامعتبر است."); }
}
