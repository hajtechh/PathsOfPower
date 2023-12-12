namespace PathsOfPower.Tests;

public class SavedGameServiceTests
{
    [Fact]
    public void GetSavedGamesShouldNotReturnNullOrEmpty()
    {
        // Arrange
        var fileHelper = new FileHelper();

        var mockFactory = new Mock<IFactory>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        mockJsonHelper
            .Setup(x => x.Deserialize<List<SavedGame>>(It.IsAny<string>()))
            .Returns(It.IsAny<List<SavedGame>>());

        var sut = new SavedGameService(mockJsonHelper.Object, fileHelper, mockFactory.Object);

        // Act
        var actual = sut.GetSavedGames();

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void GetSavedGameShouldNotReturnNullAndBeEqualToExpected()
    {
        // Arrange
        var fileHelper = new FileHelper();
        var slotNumber = 3;
        var jsonContent = fileHelper.GetSavedGameFromFile(slotNumber);
        var expected = JsonSerializer.Deserialize<SavedGame>(jsonContent);

        var mockFactory = new Mock<IFactory>();
        var mockJsonHelper = new Mock<IJsonHelper>();
        mockJsonHelper.Setup(x => x.Deserialize<SavedGame>(It.IsAny<string>())).Returns(expected);

        var sut = new SavedGameService(mockJsonHelper.Object, fileHelper, mockFactory.Object);

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
        //var fileHelper = new FileHelper();
        //var expected = fileHelper.GetSavedGameFromFile(1) ?? string.Empty;

        //var mockJsonHelper = new Mock<IJsonHelper>();
        //mockJsonHelper.Setup(x => x.Serialize(It.IsAny<SavedGame>())).Returns(expected);

        //var sut = new SavedGameService(mockJsonHelper.Object, fileHelper);

        //// Act
        //var actual = sut.SaveGame(It.IsAny<SavedGame>());

        //// Assert
        //Assert.NotNull(actual);
        //Assert.NotEmpty(actual);
        //Assert.Equal(expected, actual);
    }

    //[Fact]
    //public void SerializeSavedGameShouldSeralizeObjectAndReturnExpected()
    //{
    //    // Arrange
    //    var expected = @"{""Player"":{""Name"":""Haj"",""MoralitySpectrum"":0,""InventoryItems"":null},""QuestIndex"":""1.2""}";

    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    var mockFileHelper = new Mock<IFileHelper>();
    //    var mockQuestService = new Mock<IQuestService>();
    //    var mockGraphics = new Mock<StringHelper>();
    //    var mockSavedGameService = new Mock<ISavedGameService>();

    //    var sut = new Game(mockUserInteraction.Object,
    //        mockGraphics.Object,
    //        mockFileHelper.Object,
    //        mockQuestService.Object,
    //        mockSavedGameService.Object)
    //    {
    //        Player = new Player("Haj")
    //    };

    //    mockSavedGameService.Setup(x => x.CreateSavedGame(It.IsAny<SavedGame>())).Returns(expected);

    //    // Act
    //    var actual = sut.SerializeSavedGame("1.2");

    //    // Assert
    //    Assert.Equal(expected, actual);
    //}


    //[Fact]
    //public void DeserializeSavedGameReturnsASavedGame()
    //{
    //    // Arrange
    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    var mockFileHelper = new Mock<IFileHelper>();
    //    var mockQuestService = new Mock<IQuestService>();
    //    var mockGraphics = new Mock<StringHelper>();
    //    var mockSavedGameService = new Mock<ISavedGameService>();
    //    var sut = new Game(mockUserInteraction.Object,
    //        mockGraphics.Object,
    //        mockFileHelper.Object,
    //        mockQuestService.Object,
    //        mockSavedGameService.Object);
    //    var jsonString = @"{""Player"":{""Name"":""Haj""},""QuestIndex"":""1.2""}";

    //    var expected = new SavedGame(new Player("Haj"), "1.2");

    //    mockSavedGameService.Setup(x => x.GetSavedGame(jsonString)).Returns(expected);

    //    // Act
    //    var actual = sut.DeserializeSavedGame(jsonString);

    //    // Assert
    //    Assert.NotNull(actual);
    //    Assert.NotNull(actual.Player);
    //    Assert.NotNull(actual.Player.Name);
    //    Assert.NotNull(actual.QuestIndex);
    //}

    //[Fact]
    //public void WriteToFileShouldThrowIndexOutOfBoundsException()
    //{
    //    // Arrange
    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    var mockFileHelper = new Mock<IFileHelper>();
    //    var mockJsonHelper = new Mock<IJsonHelper>();
    //    var mockGraphics = new Mock<StringHelper>();
    //    var mockQuestService = new Mock<IQuestService>();
    //    var mockSavedGameService = new Mock<ISavedGameService>();
    //    var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
    //    var slotNumber = '9';
    //    mockFileHelper.Setup(x => x.WriteAllText(jsonContent, slotNumber));

    //    var sut = new Game(mockUserInteraction.Object,
    //        mockGraphics.Object,
    //        mockFileHelper.Object,
    //        mockQuestService.Object,
    //        mockSavedGameService.Object);

    //    // Act
    //    // Assert
    //    Assert.Throws<SlotNumberOutOfBoundsException>(() => sut.WriteToFile(slotNumber, jsonContent));
    //}



    //[Fact]
    //public void WriteToFileShouldReturnTrue()
    //{
    //    // Arrange
    //    var mockUserInteraction = new Mock<IUserInteraction>();
    //    var mockFileHelper = new Mock<IFileHelper>();
    //    var mockQuestService = new Mock<IQuestService>();
    //    var mockGraphics = new Mock<StringHelper>();
    //    var mockSavedGameService = new Mock<ISavedGameService>();
    //    var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
    //    var slotNumber = '1';
    //    mockFileHelper.Setup(x => x.WriteAllText(jsonContent, slotNumber));

    //    var sut = new Game(mockUserInteraction.Object,
    //        mockGraphics.Object,
    //        mockFileHelper.Object,
    //        mockQuestService.Object,
    //        mockSavedGameService.Object);

    //    // Act
    //    var actual = sut.WriteToFile(slotNumber, jsonContent);

    //    // Assert
    //    Assert.True(actual);
    //}
}
