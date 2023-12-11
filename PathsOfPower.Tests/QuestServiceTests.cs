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
        mockfileHelper.Setup(x => x.GetQuestsFromFile(1)).Returns(jsonContent);

        var sut = new QuestService(mockJson.Object, mockfileHelper.Object);

        // Act
        var actual = sut.GetQuestsFromChapter(1);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData('1', true)]
    [InlineData('2', true)]
    [InlineData('3', true)]
    [InlineData('4', false)]
    [InlineData('0', false)]
    [InlineData('a', false)]
    public void CheckIfOptionExistsShouldReturnEqualToExpected(char input, bool expected)
    {
        // Arrange
        var quest = new Quest
        {
            Options = new List<Option>
            {
                new() { Index = 1 }, 
                new() { Index = 2 }, 
                new() { Index = 3 }
            }
        };
        //var expected = true;
        var mockFileHelper = new Mock<IFileHelper>();
        var mockJsonHelper = new Mock<IJsonHelper>();

        var sut = new QuestService(mockJsonHelper.Object, mockFileHelper.Object);

        // Act
        var actual = sut.CheckIfOptionExists(input, quest);

        // Assert
        Assert.Equal(expected, actual);
    }

}
