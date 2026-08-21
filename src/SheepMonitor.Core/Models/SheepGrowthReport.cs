namespace SheepMonitor.Core.Models;

/// <summary>
/// خلاصه روند رشد و تغییرات وزن یک گوسفند بر اساس سوابق وزن‌گیری.
/// </summary>
public sealed class SheepGrowthReport
{
    public int SheepId { get; set; }
    public string SheepNumber { get; set; } = string.Empty;

    /// <summary>وزن اولیه ثبت‌شده برای گوسفند.</summary>
    public decimal InitialWeightKg { get; set; }

    /// <summary>آخرین وزن ثبت‌شده (یا وزن اولیه در صورت نبود رکورد).</summary>
    public decimal LatestWeightKg { get; set; }

    /// <summary>کمینه وزن در بازه (شامل وزن اولیه).</summary>
    public decimal MinWeightKg { get; set; }

    /// <summary>بیشینه وزن در بازه (شامل وزن اولیه).</summary>
    public decimal MaxWeightKg { get; set; }

    /// <summary>تغییر کل وزن از وزن اولیه تا آخرین وزن.</summary>
    public decimal TotalWeightChangeKg { get; set; }

    /// <summary>میانگین تغییر وزن بین هر دو وزن‌گیری متوالی.</summary>
    public decimal AverageWeightChangeKg { get; set; }

    /// <summary>میانگین افزایش وزن روزانه (کیلوگرم در روز).</summary>
    public decimal AverageDailyGainKg { get; set; }

    /// <summary>تعداد روز بین اولین و آخرین تاریخ وزن‌گیری.</summary>
    public int PeriodDays { get; set; }

    /// <summary>تعداد دفعات وزن‌گیری دوره‌ای (بدون احتساب وزن اولیه).</summary>
    public int WeighingCount { get; set; }

    /// <summary>وضعیت رشد بر اساس جهت تغییر وزن (بدون آستانه هاردکد).</summary>
    public string GrowthStatus { get; set; } = string.Empty;

    /// <summary>نقاط روند وزن به ترتیب تاریخ.</summary>
    public IReadOnlyList<WeightGrowthPoint> Points { get; set; } = [];
}

/// <summary>
/// یک نقطه از روند تغییر وزن در گزارش رشد.
/// </summary>
public sealed class WeightGrowthPoint
{
    public DateTime Date { get; set; }
    public decimal WeightKg { get; set; }
    public decimal ChangeFromPreviousKg { get; set; }

    /// <summary>آیا این نقطه وزن اولیه است؟</summary>
    public bool IsInitial { get; set; }
}
