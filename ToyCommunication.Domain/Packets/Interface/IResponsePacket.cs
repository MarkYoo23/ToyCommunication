namespace ToyCommunication.Domain.Packets
{
    public interface IResponsePacket : ICommandPacket
    {
        void Append(string message);
    }
}
