using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<bool> IsSupportGetSupportedSensors()
        {
            var report = await Node.GetCommandClassVersionReport(Class);
            return report.Version >= GetSupportedSensorsMinimalProtocolVersion;
        }

        public async Task<SensorMultilevelSupportedSensorReport> GetSupportedSensors()
        {
            if (!await IsSupportGetSupportedSensors())
            {
                throw new VesrionNotSupportedException($"GetSupportedSensors works with class type {Class} greater or equal to {GetSupportedSensorsMinimalProtocolVersion}.");
            }

            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport);
            return new SensorMultilevelSupportedSensorReport(Node, response);
        }

        public async Task<SensorMultiLevelReport> Get(SensorType type)
        {
            var response = await Send(new Command(Class, command.Get, (byte)type), command.Report);
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
