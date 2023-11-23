using Xunit;

namespace PathsOfPower.Tests;

public class UserInputTests
{
    [Fact]
    public void GetInputShouldReturnOne()
    {
        //Arrange
        //var expected = ConsoleKey.D1;
        var expected = "1";
        var mock = new Mock<IConsoleWrapper>();
        mock.Setup(x => x.ReadKey())
            .Returns(ConsoleKey.D1);

        var sut = new UserInteraction(mock.Object);
        
        //Act
        var actual = sut.GetKeyChar().ToString();
        
        //Assert
        Assert.Equal(expected, actual);
        mock.Verify(x => x.ReadKey(), Times.Once());
    }
}