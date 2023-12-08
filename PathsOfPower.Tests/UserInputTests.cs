namespace PathsOfPower.Tests;

public class UserInputTests
{
    [Fact]
    public void GetCharShouldReturnExpectedAndRunOnce()
    {
        //Arrange
        var expected = new ConsoleKeyInfo((char)ConsoleKey.D1, ConsoleKey.D1, false, false, false);
        var mock = new Mock<IConsoleWrapper>();
        mock.Setup(x => x.ReadChar())
            .Returns(expected);

        var sut = new UserInteraction(mock.Object);

        //Act
        var actual = sut.GetChar();
        
        //Assert
        Assert.Equal(expected, actual);
        mock.Verify(x => x.ReadChar(), Times.Once());
    }

    [Fact]
    public void PrintShouldCallWriteLine()
    {
        //Arrange
        var mock = new Mock<IConsoleWrapper>();
        mock.Setup(x => x.WriteLine(It.IsAny<string>()));

        var sut = new UserInteraction(mock.Object);

        //Act
        sut.Print("Hello");    

        //Assert
        mock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public void ClearConsoleShouldCallClear()
    {
        //Arrange
        var mock = new Mock<IConsoleWrapper>();
        var sut = new UserInteraction(mock.Object);

        //Act
        sut.ClearConsole();

        //Assert
        mock.Verify(x => x.Clear(), Times.Once());
    }
}