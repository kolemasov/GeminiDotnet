using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable MEAI001 // Type is for evaluation purposes only

namespace GeminiDotnet.Extensions.AI;

public static class GeminiClientExtensions
{
    /// <summary>
    /// Gets an <see cref="IChatClient"/> using this <see cref="IGeminiClient"/> instance.
    /// </summary>
    public static IChatClient AsChatClient(this IGeminiClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return new GeminiChatClient(client);
    }

    /// <summary>
    /// Gets an <see cref="IEmbeddingGenerator"/> using this <see cref="IGeminiClient"/> instance.
    /// </summary>
    public static IEmbeddingGenerator AsEmbeddingGenerator(this IGeminiClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return new GeminiEmbeddingGenerator(client);
    }

    /// <summary>
    /// Gets an <see cref="IHostedFileClient"/> using this <see cref="IGeminiClient"/> instance.
    /// </summary>
    [Experimental("MEAI001")]
    public static IHostedFileClient AsHostedFileClient(this IGeminiClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return new GeminiHostedFileClient(client);
    }
}
