namespace PathsOfPower.Tests;

public class SavedGameServiceTests
{
    [Fact]
    public void GetSavedGamesShouldNotReturnNullOrEmptyAndBeEqualToExpected()
    {
        // Arrange
        var fileHelper = new FileHelper();
        var expected = new List<SavedGame>()
        {
            new SavedGame(),
            new SavedGame(),
        };

        var mockJsonHelper = new Mock<IJsonHelper>();
        mockJsonHelper.Setup(x => x.Deserialize<List<SavedGame>>(It.IsAny<string>())).Returns(expected);

        var sut = new SavedGameService(mockJsonHelper.Object);

        // Act
        var actual = sut.GetSavedGames(It.IsAny<string>());

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetSavedGameShouldNotReturnNullAndBeEqualToExpected()
    {
        // Arrange
        var fileHelper = new FileHelper();
        var jsonContent = fileHelper.GetSavedGameFromFile(1) ?? string.Empty;
        var expected = JsonSerializer.Deserialize<SavedGame>(jsonContent);

        var mockJsonHelper = new Mock<IJsonHelper>();
        mockJsonHelper.Setup(x => x.Deserialize<SavedGame>(It.IsAny<string>())).Returns(expected);

        var sut = new SavedGameService(mockJsonHelper.Object);

        // Act
        var actual = sut.GetSavedGame(jsonContent);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CreateSavedGameShouldNotReturnNullOrEmptyAndBeEqualToExpected()
    {
        // Arrange
        var fileHelper = new FileHelper();
        var expected = fileHelper.GetSavedGameFromFile(1) ?? string.Empty;

        var mockJsonHelper = new Mock<IJsonHelper>();
        mockJsonHelper.Setup(x => x.Serialize(It.IsAny<SavedGame>())).Returns(expected);

        var sut = new SavedGameService(mockJsonHelper.Object);

        // Act
        var actual = sut.CreateSavedGame(It.IsAny<SavedGame>());

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(expected, actual);
    }
}
