namespace PathsOfPower.Core.Models;
public class Quest
{
    public string Index { get; set; }
    public string Description { get; set; }
    public int PowerUpScore { get; set; }
    public InventoryItem? Item { get; set; }
    public Enemy? Enemy { get; set; }
    public IEnumerable<Option>? Options { get; set; }

    public Quest()
    {
        Index = string.Empty;
        Description = string.Empty;
    }
}
