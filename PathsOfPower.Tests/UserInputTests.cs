namespace PathsOfPower.Tests;

public class UserInputTests
{
    [Fact]
    public void GetInputShouldReturnOne()
    {
        //Arrange
        var expected = "1";
        var mock = new Mock<IConsoleWrapper>();
        mock.Setup(x => x.ReadLine()).Returns(expected);

        var sut = new UserInteraction(mock.Object);
        
        //Act
        var actual = sut.GetInput("Starta spelet");
        
        //Assert
    }
}