using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorMultiLevel : EndpointSupportedCommandClassBase
    {
        private const int GetSupportedSensorsMinimalProtocolVersion = 5;

        public event EventHandler<ReportEventArgs<SensorMultiLevelReport>> Changed;

        enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Get = 0x04,
            Report = 0x05
        }

        public SensorMultiLevel(Node node)
            : base(node, CommandClass.SensorMultiLevel)
        { }

        internal SensorMultiLevel(Node node, byte endpointId)
            : base(node, CommandClass.SensorMultiLevel, endpointId)
        { }

        public Task<bool> IsSupportGetSupportedSensors()
        {
            return IsSupportGetSupportedSensors(CancellationToken.None);
        }

        public async Task<bool> IsSupportGetSupportedSensors(CancellationToken cancellationToken)
        {
            var report = await Node.GetCommandClassVersionReport(Class, cancellationToken);
            return report.Version >= GetSupportedSensorsMinimalProtocolVersion;
        }

        public Task<SensorMultilevelSupportedSensorReport> GetSupportedSensors()
        {
            return GetSupportedSensors(CancellationToken.None);
        }

        public async Task<SensorMultilevelSupportedSensorReport> GetSupportedSensors(CancellationToken cancellationToken)
        {
            if (!await IsSupportGetSupportedSensors(cancellationToken))
            {
                throw new VersionNotSupportedException($"GetSupportedSensors works with class type {Class} greater or equal to {GetSupportedSensorsMinimalProtocolVersion}.");
            }

            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new SensorMultilevelSupportedSensorReport(Node, response);
        }

        public Task<SensorMultiLevelReport> Get(SensorType type)
        {
            return Get(type, CancellationToken.None);
        }

        public async Task<SensorMultiLevelReport> Get(SensorType type, CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get, (byte)type), command.Report, cancellationToken);
            return new SensorMultiLevelReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorMultiLevelReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SensorMultiLevelReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SensorMultiLevelReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
