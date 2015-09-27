using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchBinary : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<SwitchBinaryReport>> Changed;

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

        public async Task Set(byte value)
        {
            await Channel.Send(Node, new Command(Class, command.Set, value));
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SwitchBinaryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchBinaryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SwitchBinaryReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
