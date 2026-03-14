using AiCsharpFunc;

namespace AiCsharpFunc.Tests;

public class TextHelperTests
{
    [Theory]
    [InlineData("Hello, world!", 8, "Hello...")]
    [InlineData("Hi", 10, "Hi")]
    [InlineData("Exactly", 7, "Exactly")]
    [InlineData("LongText", 3, "...")]
    public void Truncate_ReturnsExpectedResult(string input, int maxLength, string expected)
    {
        var result = TextHelper.Truncate(input, maxLength);
        Assert.Equal(expected, result);
        Assert.True(result.Length <= maxLength);
    }

    [Fact]
    public void Truncate_EmptyString_ReturnsEmpty()
    {
        var result = TextHelper.Truncate("", 5);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Truncate_NullString_ReturnsEmpty()
    {
        var result = TextHelper.Truncate(null!, 5);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Truncate_InvalidMaxLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TextHelper.Truncate("text", 0));
    }

    [Fact]
    public void ChunkText_SplitsCorrectly()
    {
        var chunks = TextHelper.ChunkText("abcdefgh", 3).ToList();
        Assert.Equal(3, chunks.Count);
        Assert.Equal("abc", chunks[0]);
        Assert.Equal("def", chunks[1]);
        Assert.Equal("gh", chunks[2]);
    }

    [Fact]
    public void ChunkText_EmptyString_ReturnsNoChunks()
    {
        var chunks = TextHelper.ChunkText("", 3).ToList();
        Assert.Empty(chunks);
    }

    [Fact]
    public void ChunkText_InvalidChunkSize_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TextHelper.ChunkText("text", 0).ToList());
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("abcd", 1)]
    [InlineData("abcde", 2)]
    [InlineData("Hello, World!", 4)]
    public void EstimateTokenCount_ReturnsExpectedCount(string input, int expected)
    {
        var result = TextHelper.EstimateTokenCount(input);
        Assert.Equal(expected, result);
    }
}
