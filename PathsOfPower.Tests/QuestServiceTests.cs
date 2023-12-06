using PathsOfPower.Services;
using System.Text.Json;

namespace PathsOfPower.Tests;

public class QuestServiceTests
{
    [Fact]
    public void GetQuestsShoulNotReturnNullOrEmpty()
    {
        // Arrange
        var mockJson = new Mock<IJsonHelper>();
        var file = new FileHelper();
        var jsonContent = file.GetQuestsFromFile(1);
        var expected = JsonSerializer.Deserialize<List<Quest>>(jsonContent);
        mockJson.Setup(x => x.Deserialize<List<Quest>>(jsonContent)).Returns(expected);
        var sut = new QuestService(mockJson.Object);

        // Act
        var actual = sut.GetQuests(jsonContent);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(expected, actual);
    }
}
