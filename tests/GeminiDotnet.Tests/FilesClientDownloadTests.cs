using GeminiDotnet.V1Beta;
using System.Net;
using System.Text;

namespace GeminiDotnet;

public sealed class FilesClientDownloadTests
{
    [Fact]
    public async Task DownloadFileStreamAsync_ContentIsReadableAfterReturn()
    {
        // Arrange — the method uses `using var message` which disposes the
        // HttpRequestMessage when the method returns. This test verifies that
        // the response content is fully buffered (via ResponseContentRead) and
        // remains readable after the request message has been disposed.
        var expectedContent = "CSV row 1\nCSV row 2\n";
        var handler = new DownloadHandler(expectedContent, "text/csv");

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://generativelanguage.googleapis.com") };
        var requester = new GeminiRequester(httpClient, V1BetaJsonContext.Default);
        var client = new FilesClient(requester);

        // Act — after this returns, the HttpRequestMessage created inside
        // DownloadFileStreamAsync has already been disposed.
        using var result = await client.DownloadFileStreamAsync("test-file");

        // Assert — reading the content stream must still succeed because the
        // response body was buffered by HttpClient (ResponseContentRead).
        var stream = await result.GetContentStreamAsync();
        using var reader = new StreamReader(stream);
        var actualContent = await reader.ReadToEndAsync();

        Assert.Equal(expectedContent, actualContent);
        Assert.Equal("text/csv", result.MediaType);
    }

    [Fact]
    public async Task DownloadFileStreamAsync_BuildsCorrectRequestUrl()
    {
        // Arrange
        var handler = new DownloadHandler("data", "application/octet-stream");

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://generativelanguage.googleapis.com") };
        var requester = new GeminiRequester(httpClient, V1BetaJsonContext.Default);
        var client = new FilesClient(requester);

        // Act
        using var result = await client.DownloadFileStreamAsync("my-file-id");

        // Assert — verify the URL includes the :download action and alt=media param
        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("/v1beta/files/my-file-id:download", handler.LastRequestUri.PathAndQuery);
        Assert.Contains("alt=media", handler.LastRequestUri.Query);
    }

    /// <summary>
    /// A handler that returns a configurable byte payload, simulating the
    /// Gemini file download endpoint.
    /// </summary>
    private sealed class DownloadHandler(string content, string mediaType) : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes(content)),
            };
            response.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);

            return Task.FromResult(response);
        }
    }
}
