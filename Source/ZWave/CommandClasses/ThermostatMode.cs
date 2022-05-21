using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatMode : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<ThermostatModeReport>> Changed;

        public enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            SupportedGet = 0x04,
            SupportedReport = 0x05
        }

        public ThermostatMode(Node node)
            : base(node, CommandClass.ThermostatMode)
        { }

        internal ThermostatMode(Node node, byte endpointId)
            : base(node, CommandClass.ThermostatMode, endpointId)
        { }

        public Task<ThermostatModeReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ThermostatModeReport> Get(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new ThermostatModeReport(Node, response);
        }

        public Task Set(ThermostatModeValue value)
        {
            return Set(value, CancellationToken.None);
        }

        public async Task Set(ThermostatModeValue value, CancellationToken cancellationToken)
        {
            await Send(new Command(Class, command.Set, (byte)value), cancellationToken);
        }

        public Task<ThermostatModeSupportedValuesReport> GetSupportedValues()
        {
            return GetSupportedValues(CancellationToken.None);
        }

        public async Task<ThermostatModeSupportedValuesReport> GetSupportedValues(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new ThermostatModeSupportedValuesReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new ThermostatModeReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<ThermostatModeReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<ThermostatModeReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
