using GeminiDotnet.V1Beta.Files;

namespace GeminiDotnet.V1Beta;

public partial interface IFilesClient
{
    /// <summary>
    /// Downloads a file's content as a stream, preserving the original binary data.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="DownloadFileAsync"/>, which attempts JSON deserialization
    /// (unsuitable for binary content), this method returns the raw response wrapped
    /// in a <see cref="FileDownloadResult"/> that provides streaming access.
    /// The caller is responsible for disposing the returned result.
    /// </remarks>
    /// <param name="file">Resource ID segment making up resource <c>name</c>.</param>
    /// <param name="cancellationToken"></param>
    Task<FileDownloadResult> DownloadFileStreamAsync(
        string file,
        CancellationToken cancellationToken = default);
}
