using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;

namespace SheepMonitor.Data.Seed;

/// <summary>
/// داده‌های اولیه مورد نیاز برنامه را وارد می‌کند.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(SheepMonitorDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);

        if (await db.FeedPlans.AnyAsync(cancellationToken))
            return;

        var plans = new[]
        {
            new FeedPlan { Name = "جیره پایه رشد", TargetGroup = "بره‌های در حال رشد", Notes = "مقادیر اولیه قابل تنظیم توسط دامدار یا دامپزشک." },
            new FeedPlan { Name = "جیره نگهداری", TargetGroup = "دام بالغ", Notes = "مقادیر اولیه قابل تنظیم بر اساس شرایط دام." },
            new FeedPlan { Name = "جیره پرواری", TargetGroup = "دام پرواری", Notes = "برای شروع طراحی جیره؛ مقدار نهایی باید با شرایط دام تنظیم شود." }
        };

        db.FeedPlans.AddRange(plans);
        await db.SaveChangesAsync(cancellationToken);

        var items = new[]
        {
            new FeedPlanItem { FeedPlanId = plans[0].Id, FeedName = "یونجه", AmountKgPerDay = 0.40m },
            new FeedPlanItem { FeedPlanId = plans[0].Id, FeedName = "کاه", AmountKgPerDay = 0.15m },
            new FeedPlanItem { FeedPlanId = plans[0].Id, FeedName = "کنسانتره", AmountKgPerDay = 0.25m },
            new FeedPlanItem { FeedPlanId = plans[1].Id, FeedName = "یونجه", AmountKgPerDay = 0.50m },
            new FeedPlanItem { FeedPlanId = plans[1].Id, FeedName = "کاه", AmountKgPerDay = 0.30m },
            new FeedPlanItem { FeedPlanId = plans[1].Id, FeedName = "جو", AmountKgPerDay = 0.15m },
            new FeedPlanItem { FeedPlanId = plans[2].Id, FeedName = "یونجه", AmountKgPerDay = 0.35m },
            new FeedPlanItem { FeedPlanId = plans[2].Id, FeedName = "کنسانتره", AmountKgPerDay = 0.45m },
            new FeedPlanItem { FeedPlanId = plans[2].Id, FeedName = "جو", AmountKgPerDay = 0.20m }
        };

        db.FeedPlanItems.AddRange(items);
        await db.SaveChangesAsync(cancellationToken);
    }
}
