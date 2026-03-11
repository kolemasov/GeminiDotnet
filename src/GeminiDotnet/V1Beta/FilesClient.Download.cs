using GeminiDotnet.V1Beta.Files;

namespace GeminiDotnet.V1Beta;

internal sealed partial class FilesClient
{
    /// <inheritdoc />
    public async Task<FileDownloadResult> DownloadFileStreamAsync(
        string file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        // The alt=media query parameter tells the API to return the raw file bytes
        // instead of a JSON-wrapped response.
        using var message = new HttpRequestMessage(HttpMethod.Get, $"/v1beta/files/{file}:download?alt=media");

        var response = await _requester.SendAsync(message, cancellationToken).ConfigureAwait(false);

        return new FileDownloadResult(response);
    }
}
