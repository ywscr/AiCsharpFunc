using AiCsharpFunc;

namespace AiCsharpFunc.Tests;

public class ChatHistoryTests
{
    [Fact]
    public void AddUserMessage_AddsMessageWithUserRole()
    {
        var history = new ChatHistory();
        history.AddUserMessage("Hello");

        Assert.Single(history.Messages);
        Assert.Equal("user", history.Messages[0].Role);
        Assert.Equal("Hello", history.Messages[0].Content);
    }

    [Fact]
    public void AddAssistantMessage_AddsMessageWithAssistantRole()
    {
        var history = new ChatHistory();
        history.AddAssistantMessage("Hi there!");

        Assert.Single(history.Messages);
        Assert.Equal("assistant", history.Messages[0].Role);
        Assert.Equal("Hi there!", history.Messages[0].Content);
    }

    [Fact]
    public void AddSystemMessage_AddsMessageWithSystemRole()
    {
        var history = new ChatHistory();
        history.AddSystemMessage("You are a helpful assistant.");

        Assert.Single(history.Messages);
        Assert.Equal("system", history.Messages[0].Role);
        Assert.Equal("You are a helpful assistant.", history.Messages[0].Content);
    }

    [Fact]
    public void Clear_RemovesAllMessages()
    {
        var history = new ChatHistory();
        history.AddUserMessage("Hello");
        history.AddAssistantMessage("Hi!");
        history.Clear();

        Assert.Empty(history.Messages);
    }

    [Fact]
    public void Messages_ReturnsMessagesInOrder()
    {
        var history = new ChatHistory();
        history.AddSystemMessage("Be concise.");
        history.AddUserMessage("What is AI?");
        history.AddAssistantMessage("AI stands for Artificial Intelligence.");

        Assert.Equal(3, history.Messages.Count);
        Assert.Equal("system", history.Messages[0].Role);
        Assert.Equal("user", history.Messages[1].Role);
        Assert.Equal("assistant", history.Messages[2].Role);
    }

    [Fact]
    public void ChatMessage_EmptyRole_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ChatMessage("", "content"));
    }

    [Fact]
    public void ChatMessage_NullContent_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ChatMessage("user", null!));
    }
}
