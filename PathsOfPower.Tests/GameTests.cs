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
}
