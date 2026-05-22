using GbIrl.Core.Enums;

namespace GbIrl.Core.Entities;

public class IrlItem
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public ItemCondition Condition { get; set; }
}
