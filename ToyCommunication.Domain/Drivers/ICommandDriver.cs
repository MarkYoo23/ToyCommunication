using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Communicaions
{
    public interface ICommandDriver
    {
        Task SendAsync(IRequestPacket requestPacket, int waitMilies = 3000);
        Task<IResponsePacket> ReceiveAsync(IRequestPacket requestPacket, int waitMilies = 3000);
    }
}
