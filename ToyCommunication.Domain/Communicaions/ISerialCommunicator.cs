using ToyCommunication.Domain.Models.Serials;

namespace ToyCommunication.Domain.Communicaions
{
    public interface ISerialCommunicator : ICommunicator
    {
        Task ChangePortAsync(int comPortNum, SystemBaudRateTypes baudRateType);
    }
}
