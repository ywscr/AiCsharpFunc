namespace AiCsharpFunc;

/// <summary>
/// Represents a message in an AI conversation.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// The role of the message author (e.g. "user", "assistant", "system").
    /// </summary>
    public string Role { get; init; }

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string Content { get; init; }

    public ChatMessage(string role, string content)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        Role = role;
        Content = content;
    }
}

/// <summary>
/// Manages a conversation history for use with AI chat APIs.
/// </summary>
public class ChatHistory
{
    private readonly List<ChatMessage> _messages = new();

    /// <summary>
    /// Read-only view of the current messages.
    /// </summary>
    public IReadOnlyList<ChatMessage> Messages => _messages.AsReadOnly();

    /// <summary>
    /// Adds a message from the user.
    /// </summary>
    public void AddUserMessage(string content) =>
        _messages.Add(new ChatMessage("user", content));

    /// <summary>
    /// Adds a message from the assistant.
    /// </summary>
    public void AddAssistantMessage(string content) =>
        _messages.Add(new ChatMessage("assistant", content));

    /// <summary>
    /// Adds a system prompt message.
    /// </summary>
    public void AddSystemMessage(string content) =>
        _messages.Add(new ChatMessage("system", content));

    /// <summary>
    /// Clears all messages from the history.
    /// </summary>
    public void Clear() => _messages.Clear();
}
