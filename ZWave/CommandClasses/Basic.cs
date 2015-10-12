using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Basic : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public event AsyncEventHandler<ReportEventArgs<BasicReport>> Changed;

        public Basic(Node node) : base(node, CommandClass.Basic)
        {
        }

        public async Task<BasicReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new BasicReport(Node, response);
        }

        public async Task Set(byte value)
        {
            await Channel.Send(Node, new Command(Class, command.Set, value));
        }

        protected internal override async Task HandleEvent(Command command)
        {
            await base.HandleEvent(command);

            var report = new BasicReport(Node, command.Payload);
            await OnChanged(new ReportEventArgs<BasicReport>(report));
        }

        protected virtual async Task OnChanged(ReportEventArgs<BasicReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                await handler.Invoke(this, e);
            }
        }
    }
}
