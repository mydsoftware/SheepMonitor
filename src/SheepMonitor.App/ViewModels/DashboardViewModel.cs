using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using SheepMonitor.Data;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// اطلاعات خلاصه داشبورد را مستقیماً از دیتابیس بارگذاری می‌کند.
/// </summary>
public sealed class DashboardViewModel(SheepMonitorDbContext db)
{
    public int TotalSheep { get; private set; }
    public int SickSheep { get; private set; }
    public decimal PlannedKg { get; private set; }
    public decimal ActualKg { get; private set; }
    public decimal WasteKg { get; private set; }
    public decimal NetKg => Math.Max(0m, ActualKg - WasteKg);
    public ObservableCollection<FeedDashboardRow> FeedSummary { get; } = [];
    public ObservableCollection<DailyConsumptionRow> DailyConsumption { get; } = [];

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        TotalSheep = await db.Sheep.CountAsync(cancellationToken);
        SickSheep = await db.Sheep.CountAsync(x => x.IsSick, cancellationToken);

        PlannedKg = await db.FeedConsumptionItems.SumAsync(x => (decimal?)x.PlannedKg, cancellationToken) ?? 0m;
        ActualKg = await db.FeedConsumptionRecords.SumAsync(x => (decimal?)x.ActualAmountKg, cancellationToken) ?? 0m;
        WasteKg = await db.FeedConsumptionRecords.SumAsync(x => (decimal?)x.WasteAmountKg, cancellationToken) ?? 0m;

        FeedSummary.Clear();
        var rows = await db.FeedConsumptionItems
            .GroupBy(x => x.FeedCode)
            .Select(g => new FeedDashboardRow
            {
                FeedCode = g.Key,
                PlannedKg = g.Sum(x => x.PlannedKg),
                ActualKg = g.Sum(x => x.ActualKg),
                WasteKg = g.Sum(x => x.WasteKg)
            })
            .OrderBy(x => x.FeedCode)
            .ToListAsync(cancellationToken);

        foreach (var row in rows)
            FeedSummary.Add(row);

        DailyConsumption.Clear();
        var dailyRows = await db.FeedConsumptionRecords
            .GroupBy(x => x.ConsumedAt.Date)
            .Select(g => new DailyConsumptionRow
            {
                Date = g.Key,
                ActualKg = g.Sum(x => x.ActualAmountKg),
                WasteKg = g.Sum(x => x.WasteAmountKg ?? 0m)
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        var max = dailyRows.Count == 0 ? 0m : dailyRows.Max(x => x.NetKg);
        foreach (var row in dailyRows)
        {
            row.BarWidth = max == 0m ? 0m : Math.Min(1m, row.NetKg / max);
            DailyConsumption.Add(row);
        }
    }
}

public sealed class FeedDashboardRow
{
    public string FeedCode { get; init; } = string.Empty;
    public decimal PlannedKg { get; init; }
    public decimal ActualKg { get; init; }
    public decimal WasteKg { get; init; }
    public decimal NetKg => Math.Max(0m, ActualKg - WasteKg);
    public decimal DeviationPercent => PlannedKg == 0m ? 0m : ((NetKg - PlannedKg) / PlannedKg) * 100m;
}

public sealed class DailyConsumptionRow
{
    public DateTime Date { get; init; }
    public decimal ActualKg { get; init; }
    public decimal WasteKg { get; init; }
    public decimal NetKg => Math.Max(0m, ActualKg - WasteKg);
    public decimal BarWidth { get; set; }
}
