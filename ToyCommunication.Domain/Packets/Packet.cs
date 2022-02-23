namespace ToyCommunication.Domain.Packets
{
    public abstract class Packet
    {
        public string Command { get; private set; }

        public Packet(string command)
        {
            Command = command;
        }

        public override string ToString()
        {
            return $"{Command}";
        }

        public virtual object ToPacket(byte[] bytes) 
        {
            return new object();
        }
    }
}
