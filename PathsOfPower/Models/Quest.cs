using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;
public class Quest
{
    public string Index { get; set; }
    public string Description { get; set; }
    public IEnumerable<Option>? Options { get; set; }
    public IInventoryItem Item { get; set; }
}
