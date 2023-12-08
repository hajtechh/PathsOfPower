namespace PathsOfPower.Tests;

public class PlayerTests
{
    [Fact]
    public void InventoryItemsListShouldNotBeNullOrEmptyWhenAddInventoryItemIsCalled()
    {
        // Arrange FirstItemInInventoryListShouldBeEqualExpected
        var sut = new Player("Haj");
        var inventoryItem = new InventoryItem()
        {
            Name = "The Elder Wand"
        };

        // Act
        sut.AddInventoryItem(inventoryItem);

        // Assert
        Assert.NotNull(sut.InventoryItems);
        Assert.NotEmpty(sut.InventoryItems);
    }

    [Theory]
    [InlineData("The Elder Wand")]
    [InlineData("Veritserum")]
    public void FirstItemInInventoryListShouldBeEqualExpected(string expected)
    {
        // Arrange
        var sut = new Player("Haj");
        var inventoryItem = new InventoryItem()
        {
            Name = expected
        };

        // Act
        sut.AddInventoryItem(inventoryItem);

        // Assert
        Assert.NotNull(sut.InventoryItems);
        Assert.NotEmpty(sut.InventoryItems);
        Assert.Equal(sut.InventoryItems.First().Name, expected);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(13)]
    [InlineData(100)]
    public void ApplyMoralityScoreShouldAddExpected(int expected)
    {
        // Arrange
        var sut = new Player("Haj");

        // Act
        sut.ApplyMoralityScore(expected);
        var actual = sut.MoralitySpectrum;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    // A player starts with 10 in Power
    [InlineData(-100, -90)]
    [InlineData(-10, 0)]
    [InlineData(0, 10)]
    [InlineData(10, 20)]
    [InlineData(123, 133)]
    public void ApplyPowerUpScoreShouldBeEqualToExpected(int powerScore, int expected)
    {
        // Arrange
        var sut = new Player("Haj");

        // Act
        sut.ApplyPowerUpScore(powerScore);
        var actual = sut.Power;

        // Assert
        Assert.Equal(expected, actual);
    }

    // A target (ICharacter) starts with 100 in HealthPoints
    [Theory]
    [InlineData(-5, 57, 62)]
    [InlineData(0, 10, 10)]
    [InlineData(5, 1, -4)]
    [InlineData(10, 0, -10)]
    [InlineData(22, -10, -32)]
    [InlineData(68, 100, 32)]
    public void WhenPlayerPerformAttackRunsThenTargetsHealthPointsShouldBeEqualToExpected(int power, int healthPoints, int expected)
    {
        // Arrange
        var sut = new Player("Haj")
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
