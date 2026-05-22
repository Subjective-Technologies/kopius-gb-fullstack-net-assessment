using System.Net;
using FluentAssertions;
using GbIrl.Core.Enums;
using GbIrl.Core.Services;
using GbIrl.Spec.Tests.Support;

namespace GbIrl.Spec.Tests.Web;

/// <summary>S33–S35: Item table, OLV column, and aggregate stats on page.</summary>
public class IrlItemsPageTests : IClassFixture<GbIrlWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IrlItemsPageTests(GbIrlWebApplicationFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task S33_ItemsPage_ListsAllRequiredColumns()
    {
        await SeedViaUploadAsync();
        var html = await _client.GetStringAsync(AppRoutes.Items);

        foreach (var column in new[]
                 {
                     "SKU", "Description", "Category", "Quantity", "Unit Cost", "Condition", "OLV"
                 })
        {
            html.Should().ContainEquivalentOf(column, $"table must include column {column}");
        }
    }

    [Fact]
    public async Task S34_ItemsPage_ShowsOlvPerRow_MatchingFormula()
    {
        await SeedViaUploadAsync();
        var html = await _client.GetStringAsync(AppRoutes.Items);

        var expectedOlv = OlvCalculator.Calculate(899m, 12, ItemCondition.New);
        html.Should().Contain(expectedOlv.ToString("F2").TrimEnd('0').TrimEnd('.'));
    }

    [Fact]
    public async Task S35_ItemsPage_ShowsTotalOlvWeightedAvgMedianStdDev()
    {
        await SeedViaUploadAsync();
        var html = await _client.GetStringAsync(AppRoutes.Items);

        html.Should().MatchRegex("Total\\s*OLV|total.*olv", "page shows total OLV");
        html.Should().MatchRegex("Weighted|weighted", "page shows weighted average unit cost");
        html.Should().MatchRegex("Median|median", "page shows median unit cost");
        html.Should().MatchRegex("Standard\\s*Deviation|Std\\.?\\s*Dev", "page shows standard deviation");
    }

    private async Task SeedViaUploadAsync()
    {
        await using var stream = File.OpenRead(TestData.SampleExcelPath);
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "file", "sample_irl.xlsx");
        await _client.PostAsync(AppRoutes.Upload, content);
    }
}
