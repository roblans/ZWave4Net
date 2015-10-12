using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorMultiLevel : CommandClassBase
    {
        public event AsyncEventHandler<ReportEventArgs<SensorMultiLevelReport>> Changed;

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

        protected internal override async Task HandleEvent(Command command)
        {
            await base.HandleEvent(command);

            var report = new SensorMultiLevelReport(Node, command.Payload);
            await OnChanged(new ReportEventArgs<SensorMultiLevelReport>(report));
        }

        protected virtual async Task OnChanged(ReportEventArgs<SensorMultiLevelReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                await handler.Invoke(this, e);
            }
        }

    }
}
