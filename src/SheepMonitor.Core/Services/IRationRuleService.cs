using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت قوانین و وعده‌های جیره پویا.
/// </summary>
public interface IRationRuleService
{
    Task<IReadOnlyList<RationCalculationRule>> GetRulesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RationMealRule>> GetMealsAsync(int ruleId, CancellationToken cancellationToken = default);
    Task SaveRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default);
    Task SaveMealAsync(RationMealRule meal, CancellationToken cancellationToken = default);
    Task DeleteRuleAsync(int ruleId, CancellationToken cancellationToken = default);
    Task DeleteMealAsync(int mealId, CancellationToken cancellationToken = default);
}
