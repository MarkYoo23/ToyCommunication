namespace ToyCommunication.Domain.Packets
{
    public class GetStatusPacket : Packet
    {
        public const string command = "GSTA";

        public GetStatusPacket() : base(command)
        {
        }

        public override string ToString()
        {
            return $"{Command}";
        }

        public override GetStatusPacket ToPacket(byte[] bytes)
        {
            return new GetStatusPacket();
        }
    }
}
