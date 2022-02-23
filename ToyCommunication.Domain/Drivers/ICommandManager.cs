using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public interface ICommandManager
    {
        bool CanRegister(IRequestPacket packet);
        void Register(IRequestPacket packet);
        void Register(IResponsePacket packet);
        bool TryUnregister(IRequestPacket requestPacket, out IResponsePacket packet);
    }
}
