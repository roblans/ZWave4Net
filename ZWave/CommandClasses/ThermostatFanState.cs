using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatFanState : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<ThermostatFanStateReport>> Changed;

        public enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public ThermostatFanState(Node node)
            : base(node, CommandClass.ThermostatFanState)
        { }

        public Task<ThermostatFanStateReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ThermostatFanStateReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new ThermostatFanStateReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new ThermostatFanStateReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<ThermostatFanStateReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<ThermostatFanStateReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
