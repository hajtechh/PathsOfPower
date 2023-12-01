using PathsOfPower.Helpers;

namespace PathsOfPower.Tests;

public class FileHelperTests
{
    [Theory]
    // Check out valid slotnumbers in directory SavedGameFiles
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ReadFromFileShouldReturnNotNull(int slotNumber)
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
    public void ReadFromFileShouldThrowFileNotFoundExceptionWhenSlotNumberIsOutOfBounds(int slotNumber)
    {
        // Arrange
        var sut = new FileHelper();

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => sut.GetSavedGameFromFile(slotNumber));
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
    [InlineData(4)]
    [InlineData(5)]
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
}
