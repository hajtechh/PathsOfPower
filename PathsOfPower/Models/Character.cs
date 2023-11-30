using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;

public class Character
{
    public string Name { get; set; }
    public int MoralitySpectrum { get; set; }
    public List<IInventoryItem> InventoryItems { get; set; }
}
