using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatSetpoint : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public event EventHandler<ReportEventArgs<ThermostatSetpointReport>> Changed;

        public ThermostatSetpoint(Node node)
            : base(node, CommandClass.ThermostatSetpoint)
        {
        }

        public async Task<ThermostatSetpointReport> Get(ThermostatSetpointType type)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, Convert.ToByte(type)), command.Report);
            return new ThermostatSetpointReport(Node, response);
        }

        public async Task Set(ThermostatSetpointType type, float value)
        {
            // encode value, decimals = 1, scale = 0 (°C) 
            var encoded = PayloadConverter.GetBytes(value, decimals: 1, scale: 0);

            var payload = new byte[] { Convert.ToByte(type) }.Concat(encoded).ToArray();
            await Channel.Send(Node, new Command(Class, command.Set, payload));
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new ThermostatSetpointReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<ThermostatSetpointReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<ThermostatSetpointReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
