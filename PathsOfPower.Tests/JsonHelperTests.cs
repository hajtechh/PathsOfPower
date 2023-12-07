
namespace PathsOfPower.Tests;

public class JsonHelperTests
{
    [Fact]
    public void DeserializeObjectShouldReturnNotNull()
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
    public void DeserializeListOfObjectsShouldReturnNotNull()
    {
        // Arrange
        var sut = new JsonHelper();
        var fileHelper = new FileHelper();
        var quests = fileHelper.GetQuestsFromFile(1);

        // Act
        var actual = sut.Deserialize<List<Quest>>(quests ?? string.Empty);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
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
