namespace PathsOfPower.Tests;

public class GameTests
{
    private readonly Game _sut;
    private Mock<IUserInteraction> _mockUserInteraction = new();
    private Mock<IStringHelper> _mockStringHelper = new();
    private Mock<IFileHelper> _mockFileHelper = new();
    private Mock<IQuestService> _mockQuestService = new();
    private Mock<ISavedGameService> _mockSavedGameService = new();
    private Mock<IFactory> _mockFactory = new();

    public GameTests()
    {
        _sut = new Game(
            new List<Quest>(),
            new Player("Haj"),
            new Quest(),
            _mockFactory.Object,
            _mockUserInteraction.Object,
            _mockStringHelper.Object,
            _mockFileHelper.Object,
            _mockQuestService.Object,
            _mockSavedGameService.Object);
    }

    [Fact]
    public void SetupKeyActionsGoToGameMenuShouldReturnExpected()
    {
        // Arrange
        var expected = new Dictionary<ConsoleKey, Action>()
        {
            { ConsoleKey.D2, _sut.SaveGame },
            { ConsoleKey.NumPad2, _sut.SaveGame },
            { ConsoleKey.D3, _sut.QuitToMainMenu },
            { ConsoleKey.NumPad3, _sut.QuitToMainMenu },
            { ConsoleKey.D4, _sut.QuitGame },
            { ConsoleKey.NumPad4, _sut.QuitGame }
        };

        // Act
        var actual = _sut.SetupkeyActionsGoToGameMenu();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ConsoleKey.D2, nameof(Game.SaveGame))]
    [InlineData(ConsoleKey.NumPad2, nameof(Game.SaveGame))]
    [InlineData(ConsoleKey.D3, nameof(Game.QuitToMainMenu))]
    [InlineData(ConsoleKey.NumPad3, nameof(Game.QuitToMainMenu))]
    [InlineData(ConsoleKey.D4, nameof(Game.QuitGame))]
    [InlineData(ConsoleKey.NumPad4, nameof(Game.QuitGame))]
    public void GetActionShouldReturnExpectedActionAndBeEqualToExpected(ConsoleKey consoleKey, string expected)
    {
        // Arrange
        var keyActions = new Dictionary<ConsoleKey, Action>()
        {
            { ConsoleKey.D2, _sut.SaveGame },
            { ConsoleKey.NumPad2, _sut.SaveGame },
            { ConsoleKey.D3, _sut.QuitToMainMenu },
            { ConsoleKey.NumPad3, _sut.QuitToMainMenu },
            { ConsoleKey.D4, _sut.QuitGame },
            { ConsoleKey.NumPad4, _sut.QuitGame }
        };
        var consoleKeyInfo = new ConsoleKeyInfo((char)consoleKey, consoleKey, false, false, false);

        // Act
        // Assert
        var action = _sut.GetAction(keyActions, consoleKeyInfo);
        Assert.NotNull(action);
        var actual = action.Method.Name;
        Assert.Equal(expected, actual);
    }

    #region PathsOfPowerAppTests
    // TODO: Create PathsOfPowerAppTests
    //[Fact]
    //public void CreateCharacterNameShouldNotReturnEmpty()
    //{
    //    // Arrange
    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    var mockFileHelper = new Mock<IFileHelper>();
    //    var mockJsonHelper = new Mock<IJsonHelper>();
    //    var mockGraphics = new Mock<StringHelper>();
    //    var mockQuestService = new Mock<IQuestService>();
    //    var mockSavedGameService = new Mock<ISavedGameService>();
    //    mockUserInteraction.SetupSequence(x => x.GetInput(It.IsAny<string>()))
    //        .Returns("")
    //        .Returns("Ron");
    //    var sut = new Game(mockUserInteraction.Object,
    //        mockGraphics.Object,
    //        mockFileHelper.Object,
    //        mockQuestService.Object,
    //        mockSavedGameService.Object);
    //    // Act
    //    var actual = sut.CreatePlayer();

    //    // Assert
    //    Assert.NotNull(actual.Name);
    //    mockUserInteraction.Verify(x => x.GetInput(It.IsAny<string>()), Times.Exactly(2));
    //}

    #endregion
}
