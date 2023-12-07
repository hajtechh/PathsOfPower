namespace PathsOfPower.Core.Interfaces;

public interface ICharacter
{
    public string Name { get; set; }
    public int HealthPoints { get; set; }
    public int Power { get; set; }

    void PerformAttack(ICharacter target);
}
