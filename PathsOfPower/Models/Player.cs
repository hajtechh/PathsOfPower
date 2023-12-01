using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;

public class Player : ICharacter
{
    public string Name { get; set; }
    public int MoralitySpectrum { get; set; }
    public int MaxHealthPoints { get; set; } = 100;
    public int CurrentHealthPoints { get; set; } = 100;
    public int Power { get; set; } = 10;
    public IList<InventoryItem>? InventoryItems { get; set; }
}
