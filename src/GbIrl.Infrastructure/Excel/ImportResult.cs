using GbIrl.Core.Entities;

namespace GbIrl.Infrastructure.Excel;

public sealed class ImportRowError
{
    public int RowNumber { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed class ImportResult
{
    public IReadOnlyList<IrlItem> LoadedItems { get; init; } = Array.Empty<IrlItem>();
    public IReadOnlyList<ImportRowError> RejectedRows { get; init; } = Array.Empty<ImportRowError>();

    public int LoadedCount => LoadedItems.Count;
    public int RejectedCount => RejectedRows.Count;
}
