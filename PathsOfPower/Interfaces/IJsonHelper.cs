namespace PathsOfPower.Interfaces;

public interface IJsonHelper
{
    string? Serialize<T>(T model);
    string? Deserialize<T>(T model);
}
