namespace GeminiDotnet.V1Beta.Files;

/// <summary>
/// Wraps the raw HTTP response from a file download, providing streaming access
/// to the file content along with metadata from the response headers.
/// </summary>
public sealed class FileDownloadResult : IDisposable, IAsyncDisposable
{
    private readonly HttpResponseMessage _response;
    private Stream? _content;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDownloadResult"/> class.
    /// </summary>
    /// <param name="response">The HTTP response message containing the file content.</param>
    public FileDownloadResult(HttpResponseMessage response)
    {
        _response = response;
    }

    /// <summary>
    /// Gets the file content as a stream. The stream is lazily obtained from the
    /// underlying HTTP response on first access.
    /// </summary>
    public Stream Content => _content ??= _response.Content.ReadAsStream();

    /// <summary>
    /// Asynchronously gets the file content as a stream. Prefer this over <see cref="Content"/>
    /// in async code paths to avoid sync-over-async if the response was not fully buffered.
    /// </summary>
    public async Task<Stream> GetContentStreamAsync(CancellationToken cancellationToken = default)
    {
        return _content ??= await _response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the media type (MIME type) from the Content-Type response header, if present.
    /// </summary>
    public string? MediaType => _response.Content.Headers.ContentType?.MediaType;

    /// <summary>
    /// Gets the content length from the Content-Length response header, if present.
    /// </summary>
    public long? ContentLength => _response.Content.Headers.ContentLength;

    /// <inheritdoc />
    public void Dispose()
    {
        _content?.Dispose();
        _response.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_content is not null)
        {
            await _content.DisposeAsync().ConfigureAwait(false);
        }

        _response.Dispose();
    }
}
