using PathsOfPower.Interfaces;

namespace PathsOfPower.Models;

public class Enemy : ICharacter
{
    public string Name { get; set; }
    public int HealthPoints { get; set; }
    public int Power { get; set; }

    public Enemy(string name)
    {
        Name = name;
    }

    public void PerformAttack(ICharacter target) =>
        target.HealthPoints -= Power;
}
