using GbIrl.Core.Entities;
using GbIrl.Core.Enums;

namespace GbIrl.Core.Services;

public static class IrlItemFilters
{
    public static IQueryable<IrlItem> Apply(
        IQueryable<IrlItem> query,
        string? category,
        ItemCondition? condition)
    {
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(i => i.Category == category);

        if (condition is not null)
            query = query.Where(i => i.Condition == condition.Value);

        return query;
    }
}
