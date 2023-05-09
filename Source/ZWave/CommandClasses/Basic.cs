using System;
using System.Threading;
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

        public event EventHandler<ReportEventArgs<BasicReport>> Changed;

        public Basic(Node node) : base(node, CommandClass.Basic)
        {
        }

        public Task<BasicReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<BasicReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new BasicReport(Node, response);
        }

        public Task Set(byte value)
        {
            return Set(value, CancellationToken.None);
        }

        public async Task Set(byte value, CancellationToken cancellationToken)
        {
            await Channel.Send(Node, new Command(Class, command.Set, value), cancellationToken);
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
