using GbIrl.Core.Entities;
using GbIrl.Core.Enums;

namespace GbIrl.Spec.Tests.Support;

public static class TestData
{
    public static string SampleExcelPath =>
        Path.Combine(AppContext.BaseDirectory, "sample_irl.xlsx");

    public static IrlItem Item(
        string sku = "SKU-TEST",
        string description = "Test item",
        string category = "Electronics",
        int quantity = 10,
        decimal unitCost = 100m,
        ItemCondition condition = ItemCondition.New,
        int id = 1) => new()
    {
        Id = id,
        Sku = sku,
        Description = description,
        Category = category,
        Quantity = quantity,
        UnitCost = unitCost,
        Condition = condition
    };

    public static IReadOnlyList<IrlItem> SubsetForFilterTests() =>
    [
        Item("A1", category: "Electronics", quantity: 10, unitCost: 100m, condition: ItemCondition.New, id: 1),
        Item("A2", category: "Electronics", quantity: 2, unitCost: 200m, condition: ItemCondition.Used, id: 2),
        Item("B1", category: "Furniture", quantity: 4, unitCost: 50m, condition: ItemCondition.Damaged, id: 3),
        Item("B2", category: "Furniture", quantity: 1, unitCost: 300m, condition: ItemCondition.New, id: 4),
    ];
}
