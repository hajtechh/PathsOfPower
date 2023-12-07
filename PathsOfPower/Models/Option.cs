namespace PathsOfPower.Models;

public class Option
{
    public int Index { get; set; }
    public string Name { get; set; }
    public int MoralityScore { get; set; }

    public Option()
    {
        Name = string.Empty;
    }
  
}
