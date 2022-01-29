namespace ToyCommunication.Domain.Communicaions
{
    public interface ICommunicator
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        bool IsConnected();
    }
}
