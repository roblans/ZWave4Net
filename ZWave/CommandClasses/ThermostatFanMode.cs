using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatFanMode : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<ThermostatFanModeReport>> Changed;

        public enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            SupportedGet = 0x04,
            SupportedReport = 0x05
        }

        public ThermostatFanMode(Node node)
            : base(node, CommandClass.ThermostatFanMode)
        { }

        internal ThermostatFanMode(Node node, byte endpointId)
            : base(node, CommandClass.ThermostatFanMode, endpointId)
        { }

        public Task<ThermostatFanModeReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ThermostatFanModeReport> Get(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new ThermostatFanModeReport(Node, response);
        }

        public Task Set(ThermostatFanModeValue value)
        {
            return Set(value, CancellationToken.None);
        }

        public async Task Set(ThermostatFanModeValue value, CancellationToken cancellationToken)
        {
            await Send(new Command(Class, command.Set, (byte)value), cancellationToken);
        }

        public Task<ThermostatFanModeSupportedValuesReport> GetSupportedValues()
        {
            return GetSupportedValues(CancellationToken.None);
        }

        public async Task<ThermostatFanModeSupportedValuesReport> GetSupportedValues(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new ThermostatFanModeSupportedValuesReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new ThermostatFanModeReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<ThermostatFanModeReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<ThermostatFanModeReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
