namespace ToyCommunication.Domain.Packets
{
    public interface IRequestPacket : ICommandPacket
    {
        string ToMessage();
    }
}
