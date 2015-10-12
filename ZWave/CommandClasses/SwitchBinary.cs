using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchBinary : CommandClassBase
    {
        public event AsyncEventHandler<ReportEventArgs<SwitchBinaryReport>> Changed;

        enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public SwitchBinary(Node node) : base(node, CommandClass.SwitchBinary)
        {
        }

        public async Task<SwitchBinaryReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new SwitchBinaryReport(Node, response);
        }

        public async Task Set(bool value)
        {
            await Channel.Send(Node, new Command(Class, command.Set, value ? (byte)0xFF : (byte)0x00));
        }

        protected internal override async Task HandleEvent(Command command)
        {
            await base.HandleEvent(command);

            var report = new SwitchBinaryReport(Node, command.Payload);
            await OnChanged(new ReportEventArgs<SwitchBinaryReport>(report));
        }

        protected virtual async Task OnChanged(ReportEventArgs<SwitchBinaryReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                await handler.Invoke(this, e);
            }
        }

    }
}
