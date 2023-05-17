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
            SupportedGetScale = 0x03,
            Get = 0x04,
            Report = 0x05,
            SupportedScaleReport = 0x06
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

        public async Task<SensorMultilevelSupportedSensorReport> GetSupportedSensors(CancellationToken cancellationToken = default)
        {
            if (!await IsSupportGetSupportedSensors(cancellationToken))
            {
                throw new VersionNotSupportedException($"GetSupportedSensors works with class type {Class} greater or equal to {GetSupportedSensorsMinimalProtocolVersion}.");
            }

            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new SensorMultilevelSupportedSensorReport(Node, response);
        }

        /// <summary>
        /// Get default sensor value (Version 1+)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SensorMultiLevelReport> Get(CancellationToken cancellationToken = default)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new SensorMultiLevelReport(Node, response);
        }

        /// <summary>
        /// Get sensor value for type (Version 5+)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SensorMultiLevelReport> Get(SensorType type, byte scale, CancellationToken cancellationToken = default)
        {
            if (!await IsSupportGetSupportedSensors(cancellationToken))
            {
                throw new VersionNotSupportedException($"Get(SensorType) works with class type {Class} greater or equal to {GetSupportedSensorsMinimalProtocolVersion}.");
            }
            scale = (byte)((scale & 0x03) << 3);
            var response = await Send(new Command(Class, command.Get, (byte)type, scale), command.Report, cancellationToken);
            return new SensorMultiLevelReport(Node, response);
        }

        /// <summary>
        /// Get sensor scale for type (Version 5+)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SensorMultilevelSupportedScaleReport> GetScale(SensorType type, CancellationToken cancellationToken = default)
        {
            if (!await IsSupportGetSupportedSensors(cancellationToken))
            {
                throw new VersionNotSupportedException($"GetScale works with class type {Class} greater or equal to {GetSupportedSensorsMinimalProtocolVersion}.");
            }
            var response = await Send(new Command(Class, command.SupportedGetScale, (byte)type), command.SupportedScaleReport, cancellationToken);
            return new SensorMultilevelSupportedScaleReport(Node, response);
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
