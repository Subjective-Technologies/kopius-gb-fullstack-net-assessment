using FluentAssertions;
using GbIrl.Infrastructure.Excel;
using GbIrl.Spec.Tests.Support;

namespace GbIrl.Spec.Tests.Infrastructure;

/// <summary>S20–S25: Excel import and validation.</summary>
public class ExcelImportServiceTests
{
    private readonly ExcelImportService _sut = new();

    [Fact]
    public void S20_SampleExcelFile_IsAvailableToTests()
    {
        File.Exists(TestData.SampleExcelPath).Should().BeTrue(
            "sample_irl.xlsx must be copied to test output");
    }

    [Fact]
    public void S21_ImportSampleFile_LoadsAbout30Rows()
    {
        using var stream = File.OpenRead(TestData.SampleExcelPath);
        var result = _sut.Import(stream);

        result.LoadedCount.Should().BeInRange(28, 32);
        result.RejectedCount.Should().Be(0);
    }

    [Fact]
    public void S22_ImportSampleFile_ParsesRequiredColumns()
    {
        using var stream = File.OpenRead(TestData.SampleExcelPath);
        var result = _sut.Import(stream);

        result.LoadedItems.Should().Contain(i =>
            i.Sku == "SKU-1001" &&
            i.Description == "Dell Latitude 5540 Laptop" &&
            i.Category == "Electronics" &&
            i.Quantity == 12 &&
            i.UnitCost == 899m);
    }

    [Fact]
    public void S23_ImportInvalidRow_ReportsRejectionWithReason()
    {
        using var stream = InvalidExcelBuilder.CreateWithBadRow();
        var result = _sut.Import(stream);

        result.RejectedCount.Should().BeGreaterThan(0);
        result.RejectedRows.Should().Contain(e =>
            e.RowNumber >= 2 && !string.IsNullOrWhiteSpace(e.Reason));
    }

    [Fact]
    public void S24_ImportDuplicateSku_RejectsOrSkipsPerDocumentedPolicy()
    {
        using var stream = InvalidExcelBuilder.CreateWithDuplicateSku();
        var result = _sut.Import(stream);

        result.RejectedCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void S25_ImportSummary_ExposesLoadedAndRejectedCounts()
    {
        using var stream = File.OpenRead(TestData.SampleExcelPath);
        var result = _sut.Import(stream);

        (result.LoadedCount + result.RejectedCount).Should().BeGreaterThan(0);
    }
}
