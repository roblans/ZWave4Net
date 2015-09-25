using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Communication;

namespace ZWave.Controller.CommandClasses
{
    public class Basic : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public event EventHandler<ReportEventArgs<BasicReport>> Changed;

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

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new BasicReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<BasicReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<BasicReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
