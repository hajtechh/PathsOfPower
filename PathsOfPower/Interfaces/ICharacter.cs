namespace PathsOfPower.Interfaces;

public interface ICharacter
{
    public string Name { get; set; }
    public int MaxHealthPoints { get; set; }
    public int CurrentHealthPoints { get; set; }
    public int Power {  get; set; }

    void PerformAttack(ICharacter target);
}
