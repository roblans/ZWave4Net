using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Battery : CommandClassBase
    {
        enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public event EventHandler<ReportEventArgs<BatteryReport>> Changed;

        public Battery(Node node) : base(node, CommandClass.Battery)
        {
        }

        public Task<BatteryReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<BatteryReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new BatteryReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new BatteryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<BatteryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<BatteryReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
