using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ManufacturerSpecific : CommandClassBase
    {
        enum command
        {
            Get = 0x04,
            Report = 0x05,
            DeviceSpecificGet = 0x06,
            DeviceSpecificReport = 0x07
        }

        public ManufacturerSpecific(Node node) : base(node, CommandClass.ManufacturerSpecific)
        {
        }

        public Task<ManufacturerSpecificReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ManufacturerSpecificReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new ManufacturerSpecificReport(Node, response);
        }

        public Task<ManufacturerSpecificDeviceReport> SpecificGet(DeviceSpecificType type)
        {
            return SpecificGet(type, CancellationToken.None);
        }

        public async Task<ManufacturerSpecificDeviceReport> SpecificGet(DeviceSpecificType type, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.DeviceSpecificGet, (byte)type), command.DeviceSpecificReport, cancellationToken);
            return new ManufacturerSpecificDeviceReport(Node, response);
        }
    }
}
