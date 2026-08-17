using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت قوانین و محاسبه پویای جیره.
/// </summary>
public interface IRationService
{
    Task<IReadOnlyList<RationCalculationRule>> GetRulesAsync(CancellationToken cancellationToken = default);
    Task<RationCalculationRule> AddRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default);
    Task<RationCalculationRule> UpdateRuleAsync(RationCalculationRule rule, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RationMealRule>> GetMealRulesAsync(int ruleId, CancellationToken cancellationToken = default);
    Task<RationMealRule> SaveMealRuleAsync(RationMealRule rule, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RationPeriod>> GetPeriodsAsync(CancellationToken cancellationToken = default);
    Task<RationPeriod> SavePeriodAsync(RationPeriod period, CancellationToken cancellationToken = default);
    Task<RationDayResult> CalculateDayAsync(RationCalculationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RationDayResult>> CalculatePeriodAsync(RationCalculationRequest request, CancellationToken cancellationToken = default);
}
