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
}
