namespace SheepMonitor.Core.Reports;

public static class FeedCostCalculator
{
    public static decimal Calculate(decimal kilograms, decimal pricePerKg)
        => decimal.Round(kilograms * pricePerKg, 2, MidpointRounding.AwayFromZero);
}
