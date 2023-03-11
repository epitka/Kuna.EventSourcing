namespace Kuna.EventSourcing.Core.Exceptions;

public class InvalidExpectedVersionException : Exception
{
    public InvalidExpectedVersionException(string message) : base(message)
    {

    }

    public InvalidExpectedVersionException (string message, Exception innerException) : base(message, innerException)
    {
    }
}