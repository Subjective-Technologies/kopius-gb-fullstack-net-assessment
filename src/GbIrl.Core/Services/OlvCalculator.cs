using GbIrl.Core.Enums;

namespace GbIrl.Core.Services;

public static class OlvCalculator
{
    public static decimal GetConditionFactor(ItemCondition condition) => condition switch
    {
        ItemCondition.New => 0.70m,
        ItemCondition.Used => 0.50m,
        ItemCondition.Damaged => 0.20m,
        _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
    };

    public static decimal Calculate(decimal unitCost, int quantity, ItemCondition condition) =>
        unitCost * quantity * GetConditionFactor(condition);
}
