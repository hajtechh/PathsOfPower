namespace PathsOfPower.Tests;

public class StringHelperTests
{
    [Theory]
    [InlineData("...Test...", "Test")]
    [InlineData("   test   ", "test")]
    [InlineData("!.*test!.*", "test")]
    [InlineData("      ", "")]
    public void TrimInputStringShouldReturnExpected(string input, string expected)
    {
        // Arrange
        var sut = new StringHelper();

        // Act
        var actual = sut.TrimInput(input);
        // Assert
        Assert.Equal(expected, actual);
    }
}
