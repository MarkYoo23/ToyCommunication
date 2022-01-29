namespace ToyCommunication.Domain.Communicaions
{
    public interface ISocketCommunicator
    {
        Task ChangeConnectionAsync(string host, int port);
    }
}
