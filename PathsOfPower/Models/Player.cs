using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;

public class Player : ICharacter
{
    public string Name { get; set; }
    public int MoralitySpectrum { get; set; }
    public int MaxHealthPoints { get; set; }
    public int CurrentHealthPoints { get; set; }
    public int Power { get; set; }
    public IList<InventoryItem>? InventoryItems { get; set; }

    public Player(string name)
    {
        Name = name;
        MaxHealthPoints = 100;
        CurrentHealthPoints = 100;
        Power = 10;
    }

    public void AddInventoryItem(InventoryItem item)
    {
        InventoryItems ??= new List<InventoryItem>();
        InventoryItems.Add(item);
    }

    public void ApplyMoralityScore(int? moralityScore)
    {
        MoralitySpectrum += moralityScore ?? 0;
    }
}
