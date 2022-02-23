using System.Net.Sockets;
using System.Text;
using ToyCommunication.Domain.Communicaions;
using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public class SocketCommandDriver : CommandDriver, ISocketCommunicator
    {
        private Socket _socket;

        public SocketCommandDriver(
            ICommandManager commandManager,
            IPacketParser packetParsers) :
            base(commandManager, packetParsers)
            
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        #region CommandDriver
        protected override Task SendAction(IRequestPacket requestPacket)
        {
            var message = requestPacket.ToMessage() + "\r\n";
            var bytes = Encoding.UTF8.GetBytes(message);
            _socket.Send(bytes);
            return Task.CompletedTask;
        }
        #endregion

        #region ISocketCommunicator
        public Task ChangeConnectionAsync(string host, int port)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
