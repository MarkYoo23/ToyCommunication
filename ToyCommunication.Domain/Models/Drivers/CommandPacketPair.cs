using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Models.Drivers
{
    public class CommandPacketPair
    {
        public CommandPacketPair(IRequestPacket requestPacket)
        {
            Command = requestPacket.GetCommand();
            RequestPacket = requestPacket;
        }

        public string Command { get; private set; }
        public IRequestPacket RequestPacket { get; private set; }
        public IResponsePacket? ResponsePacket { get; set; }
    }
}
