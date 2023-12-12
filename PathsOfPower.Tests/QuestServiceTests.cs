using PathsOfPower.Core.Models;

namespace PathsOfPower.Tests;

public class QuestServiceTests
{
    [Fact]
    public void GetQuestsShouldNotReturnNullOrEmpty()
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
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void GetQuestsShouldReturnNull(int chapter)
    {
        // Arrange
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockfileHelper = new Mock<IFileHelper>();
        mockfileHelper.Setup(x => x.GetQuestsFromFile(It.IsAny<int>())).Returns(string.Empty);

        var sut = new QuestService(mockJsonHelper.Object, mockfileHelper.Object);

        // Act
        var actual = sut.GetQuestsFromChapter(chapter);

        // Assert
        Assert.Null(actual);
    }

    [Theory]
    [InlineData('1')]
    [InlineData('2')]
    [InlineData('3')]
    public void CheckIfOptionExistsShouldReturnTrue(char input)
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
        Assert.True(actual);
    }

    [Theory]
    [InlineData('4')]
    [InlineData('0')]
    [InlineData('a')]
    public void CheckIfOptionExistsShouldReturnFalse(char input)
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
        Assert.False(actual);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.1")]
    [InlineData("1.1.1")]
    public void GetQuestFromIndexShouldReturnQuest(string index)
    {
        // Arrange
        var quests = new List<Quest>
        {
            new() { Index = "1" },
            new() { Index = "1.1" },
            new() { Index = "1.1.1" },
        };

        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new QuestService(mockJsonHelper.Object, mockFileHelper.Object);

        // Act
        var actual = sut.GetQuestFromIndex(index, quests);

        // Assert
        Assert.IsType<Quest>(actual);
        Assert.NotNull(actual);
        Assert.Equal(index, actual.Index);
    }
}
