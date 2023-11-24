namespace PathsOfPower.Tests;

public class GameTests
{
    [Fact]
    public void GetQuestsShouldReturnListOfQuests()
    {
        // Arrange
        var mock = new Mock<IUserInteraction>();
        var sut = new Game(mock.Object);

        // Act
        var actual = sut.GetQuests(1);

        // Assert
        Assert.NotNull(actual);
    }
}
