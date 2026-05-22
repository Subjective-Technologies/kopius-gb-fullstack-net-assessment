using GbIrl.Core.Entities;

namespace GbIrl.Core.Services;

/// <summary>
/// Aggregates for OLV and unit-cost statistics (see TESTS.md for formulas).
/// </summary>
public static class IrlStatisticsService
{
    public static decimal TotalOlv(IEnumerable<IrlItem> items) =>
        items.Sum(i => OlvCalculator.Calculate(i.UnitCost, i.Quantity, i.Condition));

    public static decimal WeightedAverageUnitCost(IEnumerable<IrlItem> items)
    {
        var itemList = items.ToList();
        var totalQuantity = itemList.Sum(i => i.Quantity);
        var weightedCost = itemList.Sum(i => i.UnitCost * i.Quantity);

        return totalQuantity == 0 ? 0m : weightedCost / totalQuantity;
    }

    public static decimal MedianUnitCost(IEnumerable<IrlItem> items)
    {
        var costs = items.Select(i => i.UnitCost).Order().ToList();
        if (costs.Count == 0)
            return 0m;

        var middle = costs.Count / 2;
        return costs.Count % 2 == 1
            ? costs[middle]
            : (costs[middle - 1] + costs[middle]) / 2m;
    }

    public static decimal StandardDeviationUnitCost(IEnumerable<IrlItem> items)
    {
        var costs = items.Select(i => i.UnitCost).ToList();
        if (costs.Count <= 1)
            return 0m;

        var average = costs.Average();
        var sumSquaredDiffs = costs.Sum(cost => Math.Pow((double)(cost - average), 2));

        return (decimal)Math.Sqrt(sumSquaredDiffs / (costs.Count - 1));
    }
}
