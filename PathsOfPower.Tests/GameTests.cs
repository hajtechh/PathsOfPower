namespace PathsOfPower.Tests;

public class GameTests
{
    [Fact]
    public void GetQuestsShouldNotReturnNull()
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object);

        // Act
        var actual = sut.GetQuests(1);

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public void CreateCharacterNameShouldNotReturnEmpty()
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        mock.SetupSequence(x => x.GetInput(It.IsAny<string>()))
            .Returns("")
            .Returns("Ron");
        var sut = new Game(mock.Object);

        // Act
        var actual = sut.CreateCharacter();

        // Assert
        Assert.NotNull(actual.Name);
        mock.Verify(x => x.GetInput(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void SerializeSavedGameShouldSeralizeObjectAndReturnExpected()
    {
        // Arrange
        var expected = @"{""Character"":{""Name"":""Haj"",""MoralitySpectrum"":0},""QuestIndex"":""1.2""}";
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object)
        {
            Player = new Player()
            {
                Name = "Haj",
                MoralitySpectrum = 0
            }
        };

        // Act
        var actual = sut.SerializeSavedGame("1.2");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeserializeSavedGameReturnsASavedGame()
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object);
        var jsonString = @"{""Character"":{""Name"":""Haj""},""QuestIndex"":""1.2""}";

        // Act
        var actual = sut.DeserializeSavedGame(jsonString);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.Character);
        Assert.NotNull(actual.Character.Name);
        Assert.NotNull(actual.QuestIndex);
    }

    [Theory]
    // Check out valid slotnumbers in directory SavedGameFiles
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ReadFromFileShouldReturnNotNull(int slotNumber)
    {
        // Arrange
        var path = $"../../../../PathsOfPower/SavedGameFiles/slot{slotNumber}.json";
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object);

        // Act
        var actual = sut.ReadFromFile(path);

        // Assert
        Assert.NotNull(actual);
    }

    [Theory]
    // Check for out of bounds slotnumbers in directory SavedGameFiles
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void ReadFromFileShouldThrowFileNotFoundExceptionWhenSlotNumberIsOutOfBounds(int slotNumber)
    {
        // Arrange
        var path = $"../../../../PathsOfPower/SavedGameFiles/slot{slotNumber}.json";
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object);

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => sut.ReadFromFile(path));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(13)]
    public void ApplyMoralityScoreAppliesExpectedValueToCharactersMoralitySpectrum(int expected)
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object);
        var mockCharacter = new Mock<Player>();
        mockCharacter.SetupAllProperties();
        mockCharacter.Object.MoralitySpectrum = 0;
        sut.Player = mockCharacter.Object;

        // Act
        sut.ApplyMoralityScore(expected);
        var actual = sut.Player.MoralitySpectrum;

        // Assert
       Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddInventoryItemShouldAddExpectedItemToInventory()
    {
        // Arrange
        var mockCharacter = new Mock<Player>();
        mockCharacter.SetupAllProperties();
        mockCharacter.Object.InventoryItems = new List<InventoryItem>();

        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object)
        {
            Player = mockCharacter.Object
        };

        var expected = new InventoryItem()
        {
            Name = "The Elder Wand"
        };

        // Act
        sut.AddInventoryItem(expected);
        var actual = sut.Player.InventoryItems.FirstOrDefault();

        // Assert
        Assert.Equal(expected, actual);
    }

    #region AskDaniel
    //TODO: Fråga Daniel
    //[Fact]
    //public void SavedGameShouldWriteToTextFile()
    //{
    //    // Arrange
    //    var expected = '1';
    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    mockUserInteraction.Setup(x => x.GetChar())
    //        .Returns(expected);

    //    var mockGame = new Mock<Game>();
    //    //mockGame.Setup(x => x.SaveGame("1.2"));

    //    var character = new Character()
    //    {
    //        Name = "Haj"
    //    };

    //    mockGame.SetupProperty(x => x.Character, character);
    //    mockGame.SetupGet(x => x.Character).Returns(character);
    //    var sut = mockGame.Object;

    //    // Act
    //    sut.SaveGame("1.2");

    //    // Assert
    //    mockGame.Verify(x => x.WriteToFile(It.IsAny<char>(), It.IsAny<string>()), Times.Once());
    //}
    #endregion
}
