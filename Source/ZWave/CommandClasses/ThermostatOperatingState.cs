using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatOperatingState : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<ThermostatOperatingStateReport>> Changed;

        public enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public ThermostatOperatingState(Node node)
            : base(node, CommandClass.ThermostatOperatingState)
        { }

        public Task<ThermostatOperatingStateReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ThermostatOperatingStateReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new ThermostatOperatingStateReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new ThermostatOperatingStateReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<ThermostatOperatingStateReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<ThermostatOperatingStateReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
