using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class SensorMultiLevel : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<SensorMultiLevelReport>> Changed;

        enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Get = 0x04,
            Report = 0x05
        }

        public SensorMultiLevel(Node node) : base(node, CommandClass.SensorMultiLevel)
        {
        }

        public async Task<SensorMultiLevelReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
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
