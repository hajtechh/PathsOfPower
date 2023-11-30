using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;

public class Character
{
    public string Name { get; set; }
    public int MoralitySpectrum { get; set; }
    public IEnumerable<IInventoryItem> InventoryItems { get; set; }
}
