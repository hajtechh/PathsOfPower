using Moq;
using PathsOfPower.Helpers;
using PathsOfPower.Interfaces;

namespace PathsOfPower.Tests;

public class GameTests
{
    [Fact]
    public void GetQuestsShouldNotReturnNull()
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mock.Object, mockFileHelper.Object);

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
        var mockFileHelper = new Mock<IFileHelper>();
        mock.SetupSequence(x => x.GetInput(It.IsAny<string>()))
            .Returns("")
            .Returns("Ron");
        var sut = new Game(mock.Object, mockFileHelper.Object);

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
        var expected = @"{""Character"":{""Name"":""Haj"",""MoralitySpectrum"":0,""InventoryItems"":null},""QuestIndex"":""1.2""}";
        var mock = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mock.Object, mockFileHelper.Object)
        {
            Player = new Player("Haj")
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
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mock.Object, mockFileHelper.Object);
        var jsonString = @"{""Character"":{""Name"":""Haj""},""QuestIndex"":""1.2""}";

        // Act
        var actual = sut.DeserializeSavedGame(jsonString);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.Player);
        Assert.NotNull(actual.Player.Name);
        Assert.NotNull(actual.QuestIndex);
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
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mock.Object, mockFileHelper.Object);
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

    [Theory]
    [InlineData(-10, 0)]
    [InlineData(0, 10)]
    [InlineData(10, 20)]
    [InlineData(123, 133)]
    public void ApplyPowerUpScoreToPlayerAppliesExpectedValueToPlayersPower(int powerUpScore, int expected)
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mock.Object, mockFileHelper.Object);

        var mockCharacter = new Mock<Player>();
        mockCharacter.SetupAllProperties();
        mockCharacter.Object.Power = 10;
        sut.Player = mockCharacter.Object;

        // Act
        sut.ApplyPowerUpScoreToPlayer(powerUpScore);
        var actual = sut.Player.Power;

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
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mock.Object, mockFileHelper.Object)
        {
            Player = mockCharacter.Object
        };

        var expected = new InventoryItem()
        {
            Name = "The Elder Wand"
        };

        // Act
        sut.Player.AddInventoryItem(expected);
        var actual = sut.Player.InventoryItems.FirstOrDefault();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FightEnemyShouldreturnTrueWhenEnemyCurrentHealthIsZeroOrLess()
    {
        //Arrange
        var mockPlayer = new Mock<Player>();
        mockPlayer.SetupAllProperties();
        mockPlayer.Object.Power = 10;
        mockPlayer.Object.CurrentHealthPoints = 10;

        var mockQuest = new Mock<Quest>();
        mockQuest.SetupAllProperties();
        mockQuest.Object.Enemy = new Enemy()
        {
            CurrentHealthPoints = 10,
            Power = 1
        };

        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mockUserInteraction.Object, mockFileHelper.Object);
        sut.Player = mockPlayer.Object;

        //Act 
        var actual = sut.FightEnemy(mockQuest.Object.Enemy, It.IsAny<string>());

        //Assert
        Assert.True(actual);
    }

    [Fact]
    public void WriteToFileShouldThrowIndexOutOfBoundsException()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
        var slotNumber = '9';
        mockFileHelper.Setup(x => x.WriteAllText(jsonContent, slotNumber));

        var sut = new Game(mockUserInteraction.Object, mockFileHelper.Object);

        // Act
        // Assert
        Assert.Throws<SlotNumberOutOfBoundsException>(() => sut.WriteToFile(slotNumber, jsonContent));
    }

    [Fact]
    public void WriteToFileShouldReturnTrue()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
        var slotNumber = '1';
        mockFileHelper.Setup(x => x.WriteAllText(jsonContent, slotNumber));

        var sut = new Game(mockUserInteraction.Object, mockFileHelper.Object);

        // Act
        var actual = sut.WriteToFile(slotNumber, jsonContent);

        // Assert
        Assert.True(actual);
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
