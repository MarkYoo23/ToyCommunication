using System.IO.Ports;
using System.Text;
using ToyCommunication.Domain.Communicaions;
using ToyCommunication.Domain.Exceptions.Networks;
using ToyCommunication.Domain.Models.Serials;
using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public abstract class SerialCommandDriver : ICommandDriver, ISerialCommunicator
    {
        private readonly ICommandManager _commandManager;
        private readonly IPacketParser _packetParsers;
        private SerialPort _serialPort;

        public SerialCommandDriver(
            ICommandManager commandManager,
            IPacketParser packetParsers)
        {
            _commandManager = commandManager;
            _packetParsers = packetParsers;
            _serialPort = new SerialPort();
        }

        #region ICommandCommunicator
        public async Task SendAsync(IRequestPacket requestPacket)
        {
            if (_commandManager.CanRegister(requestPacket))
            {
                // TODO : (dh) Add Exception
                throw new Exception();
            }

            // TODO : (dh) Need Refactoring
            var message = requestPacket.ToMessage() + "\r\n";
            var cts = new CancellationTokenSource();

            try
            {
                var job = Task.Run(() => _serialPort.Write(message), cts.Token);
                if (job != await Task.WhenAny(job, Task.Delay(3000)))
                {
                    cts.Cancel();
                    job.Wait();
                }
            }
            catch (InvalidOperationException e)
            {
                throw new PortNotOpenException(e.Message);
            }
            catch (TimeoutException e)
            {
                throw new SendTimeoutException(e.Message);
            }
            catch
            {
                throw;
            }
            finally
            {
                cts.Dispose();
            }

            _commandManager.Register(requestPacket);
        }

        public async Task<IResponsePacket> ReceiveAsync(
            IRequestPacket requestPacket,
            int waitMilies = 3000)
        {
            IResponsePacket? responsePacket = null;
            var cts = new CancellationTokenSource();
            var job = Task.Run(async () =>
            {
                while (true)
                {
                    if (_commandManager.TryUnregister(requestPacket, out responsePacket))
                    {
                        return;
                    }

                    await Task.Delay(10);
                }
            }, cts.Token);

            if (job != await Task.WhenAny(job, Task.Delay(waitMilies)))
            {
                cts.Cancel();
                job.Wait();
            }

            if (responsePacket == null) 
            {
                throw new NullReferenceException();
            }

            return responsePacket;
        }

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
