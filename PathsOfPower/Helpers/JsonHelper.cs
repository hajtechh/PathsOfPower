using PathsOfPower.Core.Interfaces;

namespace PathsOfPower.Core.Helpers;

public class JsonHelper : IJsonHelper
{
    public T? Deserialize<T>(string jsonContent) where T : class =>
        JsonSerializer.Deserialize<T>(jsonContent);

    public string? Serialize<T>(T model) =>
        JsonSerializer.Serialize(model);
}
