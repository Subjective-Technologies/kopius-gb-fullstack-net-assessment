using ClosedXML.Excel;

namespace GbIrl.Spec.Tests.Infrastructure;

/// <summary>Builds minimal .xlsx streams for negative import tests.</summary>
internal static class InvalidExcelBuilder
{
    private static readonly string[] Headers =
        ["SKU", "Description", "Category", "Quantity", "Unit Cost", "Condition"];

    public static Stream CreateWithBadRow()
    {
        var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("IRL");
        WriteHeader(ws);
        ws.Cell(2, 1).Value = "BAD-1";
        ws.Cell(2, 2).Value = "Missing qty";
        ws.Cell(2, 3).Value = "Electronics";
        ws.Cell(2, 4).Value = "not-a-number";
        ws.Cell(2, 5).Value = 10;
        ws.Cell(2, 6).Value = "New";
        return ToStream(wb);
    }

    public static Stream CreateWithDuplicateSku()
    {
        var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("IRL");
        WriteHeader(ws);
        WriteValidRow(ws, 2, "DUP-1");
        WriteValidRow(ws, 3, "DUP-1");
        return ToStream(wb);
    }

    private static void WriteHeader(IXLWorksheet ws)
    {
        for (var c = 0; c < Headers.Length; c++)
            ws.Cell(1, c + 1).Value = Headers[c];
    }

    private static void WriteValidRow(IXLWorksheet ws, int row, string sku)
    {
        ws.Cell(row, 1).Value = sku;
        ws.Cell(row, 2).Value = "Item";
        ws.Cell(row, 3).Value = "Electronics";
        ws.Cell(row, 4).Value = 1;
        ws.Cell(row, 5).Value = 100;
        ws.Cell(row, 6).Value = "New";
    }

    private static Stream ToStream(XLWorkbook wb)
    {
        var ms = new MemoryStream();
        wb.SaveAs(ms);
        ms.Position = 0;
        return ms;
    }
}
