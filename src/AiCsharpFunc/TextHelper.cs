namespace AiCsharpFunc;

/// <summary>
/// Provides helper functions for text processing used in AI workflows.
/// </summary>
public static class TextHelper
{
    /// <summary>
    /// Truncates text to a maximum number of characters (including the ellipsis), appending "..." if truncated.
    /// </summary>
    public static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return text ?? string.Empty;

        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength must be greater than zero.");

        if (text.Length <= maxLength)
            return text;

        const string ellipsis = "...";
        int truncateAt = maxLength > ellipsis.Length ? maxLength - ellipsis.Length : 0;
        return text[..truncateAt] + ellipsis;
    }

    /// <summary>
    /// Splits text into chunks of a given size, useful for sending to AI APIs with token limits.
    /// </summary>
    public static IEnumerable<string> ChunkText(string text, int chunkSize)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        if (chunkSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "chunkSize must be greater than zero.");

        for (int i = 0; i < text.Length; i += chunkSize)
        {
            yield return text.Substring(i, Math.Min(chunkSize, text.Length - i));
        }
    }

    /// <summary>
    /// Counts the approximate number of tokens in a text string using a rough heuristic of 1 token ≈ 4 characters.
    /// This approximation is based on GPT-style tokenization for typical English text.
    /// Actual token counts may vary significantly depending on the model and content type.
    /// </summary>
    public static int EstimateTokenCount(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        return (int)Math.Ceiling(text.Length / 4.0);
    }
}
