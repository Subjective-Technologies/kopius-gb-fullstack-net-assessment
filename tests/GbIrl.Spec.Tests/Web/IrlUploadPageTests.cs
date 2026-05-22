using System.Net;
using FluentAssertions;
using GbIrl.Spec.Tests.Support;

namespace GbIrl.Spec.Tests.Web;

/// <summary>S30–S32: Upload screen and post-upload summary.</summary>
public class IrlUploadPageTests : IClassFixture<GbIrlWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IrlUploadPageTests(GbIrlWebApplicationFactory factory) =>
        _client = factory.CreateClient(new() { AllowAutoRedirect = false });

    [Fact]
    public async Task S30_UploadPage_IsReachable()
    {
        var response = await _client.GetAsync(AppRoutes.Upload);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task S31_UploadSampleExcel_ReturnsSuccessAndSummary()
    {
        await using var stream = File.OpenRead(TestData.SampleExcelPath);
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "file", "sample_irl.xlsx");

        var response = await _client.PostAsync(AppRoutes.Upload, content);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().MatchRegex("loaded|Loaded", "post-upload summary must show loaded count");
        body.Should().MatchRegex("rejected|Rejected", "post-upload summary must show rejected count");
    }

    [Fact]
    public async Task S32_AfterUpload_ItemsArePersistedAndListed()
    {
        await UploadSampleAsync();
        var response = await _client.GetAsync(AppRoutes.Items);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("SKU-1001");
    }

    private async Task UploadSampleAsync()
    {
        await using var stream = File.OpenRead(TestData.SampleExcelPath);
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "file", "sample_irl.xlsx");
        await _client.PostAsync(AppRoutes.Upload, content);
    }
}
