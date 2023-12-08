namespace PathsOfPower.Core.Interfaces;

public interface IJsonHelper
{
    T? Deserialize<T>(string jsonContent)
        where T : class;
    string? Serialize<T>(T model);
}
