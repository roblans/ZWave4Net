using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorBinary : EndpointSupportedCommandClassBase
    {
        private const int SensorTypeMinimalProtocolVersion = 2;
        public event EventHandler<ReportEventArgs<SensorBinaryReport>> Changed;

        enum command
        {
            SupportedGet = 0x1,
            Get = 0x02,
            Report = 0x03,
            SupportedReport = 0x4
        }

        public SensorBinary(Node node)
            : base(node, CommandClass.SensorBinary)
        { }

        internal SensorBinary(Node node, byte endpointId)
            : base(node, CommandClass.SensorBinary, endpointId)
        { }

        public async Task<bool> IsV2(CancellationToken cancellationToken)
        {
            var report = await Node.GetCommandClassVersionReport(Class, cancellationToken);
            return report.Version >= SensorTypeMinimalProtocolVersion;
        }

        public Task<SensorBinaryReport> Get()
        {
            return Get(BinarySensorType.FirstSupported, CancellationToken.None);
        }

        public Task<SensorBinaryReport> Get(BinarySensorType sensorType)
        {
            return Get(sensorType, CancellationToken.None);
        }

        public Task<SensorBinaryReport> Get(CancellationToken cancellationToken)
        {
            return Get(BinarySensorType.FirstSupported, cancellationToken);
        }

        public async Task<SensorBinaryReport> Get(BinarySensorType sensorType, CancellationToken cancellationToken)
        {
            if (await IsV2(cancellationToken))
            {
                var response = await Send(new Command(Class, command.Get, (byte)sensorType), command.Report, cancellationToken);
                return new SensorBinaryReport(Node, response);
            }
            else
            {
                var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
                return new SensorBinaryReport(Node, response);
            }
        }

        public async Task<BinarySensorType[]> GetSensorType(CancellationToken cancellationToken)
        {
            if (!await IsV2(cancellationToken))
                throw new VersionNotSupportedException($"Sensor Type works with class type {Class} greater or equal to {SensorTypeMinimalProtocolVersion}.");

            List<BinarySensorType> types = new List<BinarySensorType>();
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            BitArray supported = new BitArray(response);
            for (int i = 0; i < supported.Length; i++)
            {
                if (supported[i])
                    types.Add((BinarySensorType)i);
            }
            return types.ToArray();
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorBinaryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SensorBinaryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SensorBinaryReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
