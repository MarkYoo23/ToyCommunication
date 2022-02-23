using ToyCommunication.Domain.Communicaions;
using ToyCommunication.Domain.Exceptions.Networks;
using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public abstract class CommandDriver : ICommandDriver
    {
        protected readonly ICommandManager _commandManager;
        protected readonly IPacketParser _packetParsers;

        public CommandDriver(
            ICommandManager commandManager,
            IPacketParser packetParsers)
        {
            _commandManager = commandManager;
            _packetParsers = packetParsers;
        }

        public async Task SendAsync(IRequestPacket requestPacket, int waitMilies = 3000)
        {
            if (_commandManager.CanRegister(requestPacket))
            {
                // TODO : (dh) Add Exception
                throw new Exception();
            }

            var cts = new CancellationTokenSource();

            try
            {
                var job = Task.Run(() => SendAction(requestPacket), cts.Token);
                if (job != await Task.WhenAny(job, Task.Delay(waitMilies)))
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

        protected abstract Task SendAction(IRequestPacket requestPacket);

        public async Task<IResponsePacket> ReceiveAsync(IRequestPacket requestPacket, int waitMilies = 3000)
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
    }
}
