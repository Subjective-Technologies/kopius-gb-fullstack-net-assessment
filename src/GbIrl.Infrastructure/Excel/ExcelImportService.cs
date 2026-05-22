using ClosedXML.Excel;
using GbIrl.Core.Entities;
using GbIrl.Core.Enums;

namespace GbIrl.Infrastructure.Excel;

/// <summary>
/// ClosedXML-based import from uploaded .xlsx (Phase 3).
/// </summary>
public class ExcelImportService
{
    public ImportResult Import(Stream xlsxStream)
    {
        using var workbook = new XLWorkbook(xlsxStream);
        var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("IRL", StringComparison.OrdinalIgnoreCase))
            ?? workbook.Worksheets.First();

        var headerMap = BuildHeaderMap(worksheet);
        var loaded = new List<IrlItem>();
        var rejected = new List<ImportRowError>();
        var seenSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var row = 2; row <= lastRow; row++)
        {
            if (worksheet.Row(row).IsEmpty())
                continue;

            var errors = new List<string>();
            var sku = ReadText(worksheet, row, headerMap, "SKU");
            var description = ReadText(worksheet, row, headerMap, "Description");
            var category = ReadText(worksheet, row, headerMap, "Category");
            var conditionText = ReadText(worksheet, row, headerMap, "Condition");

            if (string.IsNullOrWhiteSpace(sku))
                errors.Add("SKU is required.");
            else if (!seenSkus.Add(sku))
                errors.Add($"Duplicate SKU '{sku}'.");

            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description is required.");
            if (string.IsNullOrWhiteSpace(category))
                errors.Add("Category is required.");

            if (!TryReadInt(worksheet, row, headerMap, "Quantity", out var quantity) || quantity < 0)
                errors.Add("Quantity must be a non-negative whole number.");

            if (!TryReadDecimal(worksheet, row, headerMap, "Unit Cost", out var unitCost) || unitCost < 0)
                errors.Add("Unit Cost must be a non-negative number.");

            if (!Enum.TryParse<ItemCondition>(conditionText, true, out var condition))
                errors.Add("Condition must be New, Used, or Damaged.");

            if (errors.Count > 0)
            {
                rejected.Add(new ImportRowError { RowNumber = row, Reason = string.Join(" ", errors) });
                continue;
            }

            loaded.Add(new IrlItem
            {
                Sku = sku,
                Description = description,
                Category = category,
                Quantity = quantity,
                UnitCost = unitCost,
                Condition = condition
            });
        }

        return new ImportResult { LoadedItems = loaded, RejectedRows = rejected };
    }

    private static Dictionary<string, int> BuildHeaderMap(IXLWorksheet worksheet)
    {
        var lastColumn = worksheet.Row(1).LastCellUsed()?.Address.ColumnNumber ?? 0;
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var column = 1; column <= lastColumn; column++)
        {
            var header = worksheet.Cell(1, column).GetString().Trim();
            if (!string.IsNullOrWhiteSpace(header))
                map[header] = column;
        }

        foreach (var required in new[] { "SKU", "Description", "Category", "Quantity", "Unit Cost", "Condition" })
        {
            if (!map.ContainsKey(required))
                throw new InvalidOperationException($"Missing required column '{required}'.");
        }

        return map;
    }

    private static string ReadText(
        IXLWorksheet worksheet,
        int row,
        IReadOnlyDictionary<string, int> headerMap,
        string header) =>
        worksheet.Cell(row, headerMap[header]).GetFormattedString().Trim();

    private static bool TryReadInt(
        IXLWorksheet worksheet,
        int row,
        IReadOnlyDictionary<string, int> headerMap,
        string header,
        out int value)
    {
        var cell = worksheet.Cell(row, headerMap[header]);
        return cell.TryGetValue(out value) ||
               int.TryParse(cell.GetFormattedString().Trim(), out value);
    }

    private static bool TryReadDecimal(
        IXLWorksheet worksheet,
        int row,
        IReadOnlyDictionary<string, int> headerMap,
        string header,
        out decimal value)
    {
        var cell = worksheet.Cell(row, headerMap[header]);
        return cell.TryGetValue(out value) ||
               decimal.TryParse(cell.GetFormattedString().Trim(), out value);
    }
}
