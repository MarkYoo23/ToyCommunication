using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyCommunication.Domain.Models.Drivers;
using ToyCommunication.Domain.Packets;

namespace ToyCommunication.Domain.Drivers
{
    public class CommandManager : ICommandManager
    {
        private ConcurrentDictionary<string, CommandPacketPair> pairs = new ConcurrentDictionary<string, CommandPacketPair>();

        public bool CanRegister(IRequestPacket packet)
        {
            var command = packet.GetCommand();
            var isGetValue = pairs.TryGetValue(command, out var _);
            return  isGetValue == false;
        }

        public void Register(IRequestPacket packet)
        {
            var command = packet.GetCommand();
            var pair = new CommandPacketPair(packet);

            var isSuccess = pairs.TryAdd(command, pair);
            if (isSuccess == false)
            {
                // TODO : (dh) Add Register Exception
                throw new Exception();
            }
        }

        public void Register(IResponsePacket packet)
        {
            var command = packet.GetCommand();
            if (pairs.TryGetValue(command, out var pair))
            {
                pair.ResponsePacket = packet;
            }
            else
            {
                // TODO : (dh) Add Register Exception
                throw new Exception();
            }
        }

        public bool TryUnregister(string command, out IResponsePacket packet)
        {
            if (pairs.TryGetValue(command, out var pair))
            {
                if (pair.ResponsePacket != null)
                {
                    packet = pair.ResponsePacket;
                    return true;
                }
            }

            packet = null;
            return false;
        }
    }
}
