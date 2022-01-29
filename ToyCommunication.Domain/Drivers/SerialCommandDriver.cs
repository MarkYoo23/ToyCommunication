using System.IO.Ports;
using System.Text;
using ToyCommunication.Domain.Communicaions;
using ToyCommunication.Domain.Models.Serials;
using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public abstract class SerialCommandDriver : CommandDriver, ISerialCommunicator
    {
        private SerialPort _serialPort;

        public SerialCommandDriver(
            ICommandManager commandManager,
            IPacketParser packetParsers) :
            base(commandManager, packetParsers)
        {
            _serialPort = new SerialPort();
        }

        #region CommandDriver
        protected override Task SendAction(IRequestPacket requestPacket) 
        {
            var message = requestPacket.ToMessage() + "\r\n";
            _serialPort.Write(message);
            return Task.CompletedTask;
        }
        #endregion

        #region ISerialCommunicator
        public async Task ChangePortAsync(int comPortNum, SystemBaudRateTypes baudRateType)
        {
            if (_serialPort.IsOpen)
            {
                await DisconnectAsync();
            }

            _serialPort = new SerialPort($"COM{comPortNum}", (int)baudRateType);
            _serialPort.Encoding = Encoding.GetEncoding(28591);
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;
        }

        public Task ConnectAsync()
        {
            if (_serialPort == null)
            {
                // TODO : (dh) Add Exception
                return Task.FromException(new Exception());
            }

            _serialPort.Open();
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            if (_serialPort == null)
            {
                // TODO : (dh) Add Exception
                return Task.FromException(new Exception());
            }

            _serialPort.DataReceived -= HandleDataReceived;
            _serialPort.Close();
            _serialPort.Dispose();

            return Task.CompletedTask;
        }

        public bool IsConnected()
        {
            if (_serialPort == null)
            {
                return false;
            }

            return _serialPort.IsOpen;
        }

        private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var receivedData = new List<byte>();

            while (_serialPort.BytesToRead > 0)
            {
                receivedData.Add((byte)_serialPort.ReadByte());
            }

            var message = BitConverter.ToString(receivedData.ToArray());
            var packet = _packetParsers.Parse(message);
            _commandManager.Register(packet);
        }
        #endregion
    }
}
