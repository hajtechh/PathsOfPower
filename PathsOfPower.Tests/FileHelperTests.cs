﻿namespace PathsOfPower.Tests;

public class FileHelperTests
{
    [Theory]
    // Check out valid slotnumbers in directory SavedGameFiles
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetSavedGameFromFileShouldReturnNotNull(int slotNumber)
    {
        // Arrange
        var sut = new FileHelper();

        // Act
        var actual = sut.GetSavedGameFromFile(slotNumber);

        // Assert
        Assert.NotNull(actual);
    }

    [Theory]
    // Check for out of bounds slotnumbers in directory SavedGameFiles
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void GetSavedGameFromFileShouldThrowFileNotFoundExceptionWhenSlotNumberIsOutOfBounds(int slotNumber)
    {
        // Arrange
        var sut = new FileHelper();

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => sut.GetSavedGameFromFile(slotNumber));
    }

    [Fact]
    public void GetSavedGameFromFileFullPathShouldNotReturnNullOrEmpty()
    {
        // Arrange
        var sut = new FileHelper();
        var path = "../../../../PathsOfPower/SavedGameFiles/slot2.json";

        // Act
        var actual = sut.GetSavedGameFromFile(path);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CheckIfNextChapterExistsShouldReturnTrue(int currentChapter)
    {
        // Arrange
        var sut = new FileHelper();

        // Act
        var actual = sut.IsNextChapterExisting(currentChapter);
        // Assert
        Assert.True(actual);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(66)]
    [InlineData(-1)]
    public void CheckIfNextChapterExistsShouldReturnFalse(int currentChapter)
    {
        // Arrange
        var sut = new FileHelper();

        // Act
        var actual = sut.IsNextChapterExisting(currentChapter);
        // Assert
        Assert.False(actual);
    }
    
    [Fact]
    public void GetAllSavedGameFilesFromDirectoryShouldNotReturnNullOrEmpty()
    {
        // Arrange
        var sut = new FileHelper();

        // Act
        var actual = sut.GetAllSavedGameFilesFromDirectory();

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void WriteAllTextShouldRunAtleastOnce()
    {
        // Arrange
        var sutMockFileHelper = new Mock<IFileHelper>();
        var jsonContent = @"{""Player"":{""Name"":""Test Save"",""MoralitySpectrum"":-4,""MaxHealthPoints"":100,""CurrentHealthPoints"":100,""Power"":20,""InventoryItems"":[]},""QuestIndex"":""2""}";
        var slotNumber = '1';

        // Act
        sutMockFileHelper.Object.WriteAllText(jsonContent, slotNumber);

        // Assert
        sutMockFileHelper.Verify(x => x.WriteAllText(jsonContent, slotNumber), Times.Once);
    }


    [Fact]
    public void GetQuestsFromFileShouldNotReturnNullOrEmpty()
    {
        // Arrange
        var sut = new FileHelper();
        var chapterNumber = 1;

        // Act
        var actual = sut.GetQuestsFromFile(chapterNumber);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }
}
