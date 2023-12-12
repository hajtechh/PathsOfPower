namespace PathsOfPower.Tests;

public class FactoryTests
{
    [Fact]
    public void CreateGameShouldReturnGame()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var mockStringHelper = new Mock<IStringHelper>();
        var mockQuestService = new Mock<IQuestService>();
        var mockSavedGameService = new Mock<ISavedGameService>();

        var sut = new Factory();

        // Act
        var actual = sut.CreateGame(
            new List<Quest>(),
            new Player("Haj"),
            new Quest(),
            mockUserInteraction.Object,
            mockStringHelper.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockSavedGameService.Object);

        // Assert
        Assert.IsType<Game>(actual);
        Assert.NotNull(actual);
    }

    [Fact]
    public void CreateInventoryItemsShouldReturnListOfInventoryItem()
    {
        // Arrange
        var sut = new Factory();

        // Act
        var actual = sut.CreateInventoryItems();

        // Assert
        Assert.IsType<List<InventoryItem>>(actual);
        Assert.NotNull(actual);
    }

    [Theory]
    [InlineData("Hanna")]
    [InlineData("Anna")]
    [InlineData("Jonna")]
    public void CreatePlayerShouldReturnPlayerWithExpectedName(string expected)
    {
        // Arrange
        var sut = new Factory();

        // Act
        var actual = sut.CreatePlayer(expected);

        // Assert
        Assert.IsType<Player>(actual);
        Assert.NotNull(actual.Name);
        Assert.NotEmpty(actual.Name);
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public void CreatePlayerShouldReturnPlayerWithEmptyName()
    {
        // Arrange
        var sut = new Factory();
        var name = string.Empty;

        // Act
        var actual = sut.CreatePlayer(name);

        // Assert
        Assert.IsType<Player>(actual);
        Assert.NotNull(actual.Name);
        Assert.Empty(actual.Name);
        Assert.Equal(name, actual.Name);
    }

    [Fact]
    public void CreateSavedGameShouldReturnSavedGame()
    {
        // Arrange
        var sut = new Factory();

        // Act
        var actual = sut.CreateSavedGame();

        // Assert
        Assert.IsType<SavedGame>(actual);
        Assert.NotNull(actual);
    }

    [Fact]
    public void CreateSavedGamesShouldReturnListOfSavedGame()
    {
        // Arrange
        var sut = new Factory();

        // Act
        var actual = sut.CreateSavedGames();

        // Assert
        Assert.IsType<List<SavedGame>>(actual);
        Assert.NotNull(actual);
    }
}