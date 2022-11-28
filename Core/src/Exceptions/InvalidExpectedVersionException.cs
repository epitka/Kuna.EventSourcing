namespace Senf.EventSourcing.Core.Exceptions
{
    public class InvalidExpectedVersionException : Exception
    {
        public InvalidExpectedVersionException (string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
