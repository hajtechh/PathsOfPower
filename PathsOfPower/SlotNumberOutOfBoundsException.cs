namespace PathsOfPower;

public class SlotNumberOutOfBoundsException : Exception
{
    public SlotNumberOutOfBoundsException()
    {
    }

    public SlotNumberOutOfBoundsException(string message)
        : base(message)
    {
    }

    public SlotNumberOutOfBoundsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}