using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class SensorBinary : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<SensorBinaryReport>> Changed;

        enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public SensorBinary(Node node) : base(node, CommandClass.SensorBinary)
        {
        }

        public async Task<SensorBinaryReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new SensorBinaryReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorBinaryReport(Node, command.Payload);
            OnReportReceived(new ReportEventArgs<SensorBinaryReport>(report));
        }

        protected virtual void OnReportReceived(ReportEventArgs<SensorBinaryReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
