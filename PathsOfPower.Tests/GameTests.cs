using Moq;
using PathsOfPower.Cli.Interfaces;
using PathsOfPower.Exceptions;
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
        var actual = sut.CreatePlayer();

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

    [Fact]
    public void FightEnemyShouldreturnTrueWhenEnemyCurrentHealthIsZeroOrLess()
    {
        //Arrange
        var mockPlayer = new Mock<Player>();
        mockPlayer.SetupAllProperties();
        mockPlayer.Object.Power = 10;
        mockPlayer.Object.HealthPoints = 10;

        var mockQuest = new Mock<Quest>();
        mockQuest.SetupAllProperties();
        mockQuest.Object.Enemy = new Enemy("Haj")
        {
            HealthPoints = 10,
            Power = 1
        };

        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var sut = new Game(mockUserInteraction.Object, mockFileHelper.Object)
        {
            Player = mockPlayer.Object
        };

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
