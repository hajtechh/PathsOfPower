
namespace PathsOfPower.Tests;

public class GameTests
{
    [Fact]
    public void GetQuestsShouldNotReturnNull()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockGraphics = new Mock<Graphics>();
        var mockQuestService = new Mock<IQuestService>();
        var sut = new Game(mockUserInteraction.Object,
            mockGraphics.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockJsonHelper.Object);

        var jsonContent = @"{""Index"":""1"",""Description"":""You are in a classroom at Hogwarts. What do you want to teach your students today?""}";

        mockFileHelper
            .Setup(x => x.GetQuestsFromFile(1))
            .Returns(jsonContent);

        mockQuestService
            .Setup(x => x.GetQuests(jsonContent))
            .Returns(new List<Quest>() { new Quest() });

        // Act
        var actual = sut.GetQuests(1);

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public void CreateCharacterNameShouldNotReturnEmpty()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockGraphics = new Mock<Graphics>();
        var mockQuestService = new Mock<IQuestService>();
        mockUserInteraction.SetupSequence(x => x.GetInput(It.IsAny<string>()))
            .Returns("")
            .Returns("Ron");
        var sut = new Game(mockUserInteraction.Object,
            mockGraphics.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockJsonHelper.Object);
        // Act
        var actual = sut.CreatePlayer();

        // Assert
        Assert.NotNull(actual.Name);
        mockUserInteraction.Verify(x => x.GetInput(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void SerializeSavedGameShouldSeralizeObjectAndReturnExpected()
    {
        // Arrange
        var expected = @"{""Player"":{""Name"":""Haj"",""MoralitySpectrum"":0,""InventoryItems"":null},""QuestIndex"":""1.2""}";

        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockQuestService = new Mock<IQuestService>();
        var mockGraphics = new Mock<Graphics>();

        var sut = new Game(mockUserInteraction.Object,
            mockGraphics.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockJsonHelper.Object)
        {
            Player = new Player("Haj")
        };

        mockJsonHelper.Setup(x => x.Serialize(It.IsAny<SavedGame>())).Returns(expected);

        // Act
        var actual = sut.SerializeSavedGame("1.2");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeserializeSavedGameReturnsASavedGame()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockQuestService = new Mock<IQuestService>();
        var mockGraphics = new Mock<Graphics>();
        var sut = new Game(mockUserInteraction.Object,
            mockGraphics.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockJsonHelper.Object);
        var jsonString = @"{""Player"":{""Name"":""Haj""},""QuestIndex"":""1.2""}";

        var expected = new SavedGame(new Player("Haj"), "1.2");

        mockJsonHelper.Setup(x => x.Deserialize<SavedGame>(jsonString)).Returns(expected);

        // Act
        var actual = sut.DeserializeSavedGame(jsonString);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.Player);
        Assert.NotNull(actual.Player.Name);
        Assert.NotNull(actual.QuestIndex);
    }

    //[Fact]
    //public void FightEnemyShouldreturnTrueWhenEnemyCurrentHealthIsZeroOrLess()
    //{
    //    //Arrange
    //    var mockPlayer = new Mock<Player>();
    //    mockPlayer.SetupAllProperties();
    //    mockPlayer.Object.Power = 10;
    //    mockPlayer.Object.HealthPoints = 10;

    //    var mockQuest = new Mock<Quest>();
    //    mockQuest.SetupAllProperties();
    //    mockQuest.Object.Enemy = new Enemy("Haj")
    //    {
    //        HealthPoints = 10,
    //        Power = 1
    //    };

    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    var mockFileHelper = new Mock<IFileHelper>();
    //    var mockJsonHelper = new Mock<IJsonHelper>();
    //    var mockGraphics = new Mock<Graphics>();
    //    var mockQuestService = new Mock<IQuestService>();
    //    var sut = new Game(mockUserInteraction.Object,
    //        mockGraphics.Object,
    //        mockFileHelper.Object,
    //        mockQuestService.Object,
    //        mockJsonHelper.Object)
    //    {
    //        Player = mockPlayer.Object
    //    };

    //    //Act 
    //    var actual = sut.FightEnemy(mockQuest.Object.Enemy, It.IsAny<string>());

    //    //Assert
    //    Assert.True(actual);
    //}

    [Fact]
    public void WriteToFileShouldThrowIndexOutOfBoundsException()
    {
        // Arrange
        var mockUserInteraction = new Mock<IUserInteraction>();
        var mockFileHelper = new Mock<IFileHelper>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockGraphics = new Mock<Graphics>();
        var mockQuestService = new Mock<IQuestService>();
        var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
        var slotNumber = '9';
        mockFileHelper.Setup(x => x.WriteAllText(jsonContent, slotNumber));

        var sut = new Game(mockUserInteraction.Object,
            mockGraphics.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockJsonHelper.Object);

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
        var mockJsonHelper = new Mock<IJsonHelper>();
        var mockQuestService = new Mock<IQuestService>();
        var mockGraphics = new Mock<Graphics>();
        var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
        var slotNumber = '1';
        mockFileHelper.Setup(x => x.WriteAllText(jsonContent, slotNumber));

        var sut = new Game(mockUserInteraction.Object,
            mockGraphics.Object,
            mockFileHelper.Object,
            mockQuestService.Object,
            mockJsonHelper.Object);

        // Act
        var actual = sut.WriteToFile(slotNumber, jsonContent);

        // Assert
        Assert.True(actual);
    }
}
