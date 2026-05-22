using System.Net;
using FluentAssertions;
using GbIrl.Spec.Tests.Support;

namespace GbIrl.Spec.Tests.Web;

/// <summary>S36–S37: Filters recalc stats; edit recalculates OLV.</summary>
public class IrlFilterAndEditTests : IClassFixture<GbIrlWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IrlFilterAndEditTests(GbIrlWebApplicationFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task S36_FilterByCategory_RecalculatesSubsetStatistics()
    {
        await SeedViaUploadAsync();

        var unfiltered = await _client.GetStringAsync(AppRoutes.Items);
        var filtered = await _client.GetStringAsync($"{AppRoutes.Items}?category=Electronics");

        filtered.Should().Contain("SKU-1001", "Electronics filter keeps electronics SKU");
        filtered.Should().NotContain("SKU-1007", "Furniture SKU must be hidden when filtering Electronics");
        unfiltered.Should().Contain("SKU-1007", "unfiltered list includes other categories");
        filtered.Should().NotBe(unfiltered);
    }

    [Fact]
    public async Task S37_EditItem_QuantityUnitCostCondition_RecalculatesOlv()
    {
        await SeedViaUploadAsync();

        var editPage = await _client.GetAsync(AppRoutes.Edit(1));
        editPage.StatusCode.Should().Be(HttpStatusCode.OK);

        var form = new Dictionary<string, string>
        {
            ["Quantity"] = "99",
            ["UnitCost"] = "10.00",
            ["Condition"] = "Damaged"
        };
        var post = await _client.PostAsync(AppRoutes.Edit(1), new FormUrlEncodedContent(form));
        post.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);

        var listHtml = await _client.GetStringAsync(AppRoutes.Items);
        listHtml.Should().Contain("198", "OLV = 10 * 99 * 0.20 after edit");
    }

    private async Task SeedViaUploadAsync()
    {
        await using var stream = File.OpenRead(TestData.SampleExcelPath);
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "file", "sample_irl.xlsx");
        await _client.PostAsync(AppRoutes.Upload, content);
    }
}
