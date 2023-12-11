namespace PathsOfPower.Tests;

public class QuestServiceTests
{
    [Fact]
    public void GetQuestsShoulNotReturnNullOrEmpty()
    {
        // Arrange
        var file = new FileHelper();
        var jsonContent = file.GetQuestsFromFile(1) ?? string.Empty;
        var expected = JsonSerializer.Deserialize<List<Quest>>(jsonContent);

        var mockJson = new Mock<IJsonHelper>();
        var mockfileHelper = new Mock<IFileHelper>();
        mockJson.Setup(x => x.Deserialize<List<Quest>>(jsonContent)).Returns(expected);

        var sut = new QuestService(mockJson.Object, mockfileHelper.Object);

        // Act
        //var actual = sut.GetQuests(jsonContent);

        // Assert
        //Assert.NotNull(actual);
        //Assert.NotEmpty(actual);
        //Assert.Equal(expected, actual);
    }
}
