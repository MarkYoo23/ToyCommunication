namespace ToyCommunication.Domain.Exceptions.Networks
{
    public class SendTimeoutException : Exception
    {
        public SendTimeoutException(string message) :base(message)
        {
        }
    }
}
