using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Meter : CommandClassBase
    {
        public event AsyncEventHandler<ReportEventArgs<MeterReport>> Changed;

        enum command
        {
            Get = 0x01,
            Report = 0x02,
            
            // Version 2
            SupportedGet = 0x03,
            SupportedReport = 0x04,
            Reset = 0x05
        }

        public Meter(Node node) : base(node, CommandClass.Meter)
        {
        }

        public async Task<MeterReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new MeterReport(Node, response);
        }

        public async Task<MeterSupportedReport> GetSupported()
        {
            var response = await Channel.Send(Node, new Command(Class, command.SupportedGet), command.SupportedReport);
            return new MeterSupportedReport(Node, response);
        }

        protected internal override async Task HandleEvent(Command command)
        {
            await base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(Meter.command.Report))
            {
                var report = new MeterReport(Node, command.Payload);
                await OnChanged(new ReportEventArgs<MeterReport>(report));
            }
        }

        protected virtual async Task OnChanged(ReportEventArgs<MeterReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                await handler.Invoke(this, e);
            }
        }

    }
}
