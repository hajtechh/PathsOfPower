namespace PathsOfPower.Core.Models;

public class Player : ICharacter
{
    public string Name { get; set; }
    public int MoralitySpectrum { get; set; }
    public int HealthPoints { get; set; }
    public int Power { get; set; }
    public IList<InventoryItem>? InventoryItems { get; set; }

    public Player(string name)
    {
        Name = name;
        HealthPoints = 100;
        Power = 10;
        InventoryItems = new List<InventoryItem>();
    }

    public void AddInventoryItem(InventoryItem item)
    {
        InventoryItems ??= new List<InventoryItem>();
        InventoryItems.Add(item);
    }

    public void ApplyMoralityScore(int moralityScore) =>
        MoralitySpectrum += moralityScore;

    public void ApplyPowerUpScore(int powerUpScore) =>
        Power += powerUpScore;

    public void PerformAttack(ICharacter target) =>
        target.HealthPoints -= Power;
}
