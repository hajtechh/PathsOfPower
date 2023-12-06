namespace PathsOfPower.Tests;

public class JsonHelperTests
{
    [Fact]
    public void DeSerializeObjectShouldReturnNotNull()
    {
        // Arrange
        var sut = new JsonHelper();
        var quest = @"{""Index"":""1"",""Description"":""You are in a classroom at Hogwarts. What do you want to teach your students today?""}";

        // Act
        var actual = sut.Deserialize<Quest>(quest);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.Description);
    }

    [Fact]
    public void SerializeObjectShouldReturnNotNullOrEmpty()
    {
        // Arrange
        var sut = new JsonHelper();
        var quest = new Quest()
        {
            Index = "1",
            Description = "You are in a classroom at Hogwarts. What do you want to teach your students today?"
        };

        // Act
        var actual = sut.Serialize(quest);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }
}
