using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;

public class Player
{
    public string Name { get; set; }
    public int MoralitySpectrum { get; set; }
    public IList<InventoryItem>? InventoryItems { get; set; }
}
