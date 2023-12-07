
using PathsOfPower.Core.Interfaces;

namespace PathsOfPower.Tests;

public class EnemyTests
{    // A target (ICharacter) starts with 100 in HealthPoints
    [Theory]
    [InlineData(-5, 57, 62)]
    [InlineData(0, 10, 10)]
    [InlineData(5, 1, -4)]
    [InlineData(10, 0, -10)]
    [InlineData(22, -10, -32)]
    [InlineData(68, 100, 32)]
    public void WhenEnemyPerformAttackRunsThenTargetsHealthPointsShouldBeEqualToExpected(int power, int healthPoints, int expected)
    {
        // Arrange
        var sut = new Enemy("Haj")
        {
            Power = power,
        };

        var mockTarget = new Mock<ICharacter>();
        mockTarget.SetupAllProperties();
        mockTarget.Object.HealthPoints = healthPoints;

        // Act
        sut.PerformAttack(mockTarget.Object);
        var actual = mockTarget.Object.HealthPoints;

        // Assert
        Assert.Equal(expected, actual);
    }
}
