namespace PathsOfPower.Core.Exceptions;

public class FileHelperUnableToDeserialize : Exception
{
    public FileHelperUnableToDeserialize()
    {
    }

    public FileHelperUnableToDeserialize(string message)
        : base(message)
    {
    }

    public FileHelperUnableToDeserialize(string message, Exception inner)
        : base(message, inner)
    {
    }
}
