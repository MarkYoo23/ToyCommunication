using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public interface IPacketParser
    {
        IResponsePacket Parse(string message);
    }
}
