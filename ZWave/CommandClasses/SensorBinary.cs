using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorBinary : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<SensorBinaryReport>> Changed;

        enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public SensorBinary(Node node)
            : base(node, CommandClass.SensorBinary)
        { }

        internal SensorBinary(Node node, byte endpointId)
            : base(node, CommandClass.SensorBinary, endpointId)
        { }

        public async Task<SensorBinaryReport> Get()
        {
            var response = await Send(new Command(Class, command.Get), command.Report);
            return new SensorBinaryReport(Node, response);
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
