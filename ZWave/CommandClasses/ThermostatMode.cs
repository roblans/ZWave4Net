using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatMode : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public event EventHandler<ReportEventArgs<ThermostatModeReport>> Changed;

        public ThermostatMode(Node node)
            : base(node, CommandClass.ThermostatSetpoint)
        {
        }

        public async Task<ThermostatModeReport> Get(ThermostatSetpointType type)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, Convert.ToByte(type)), command.Report);
            return new ThermostatModeReport(Node, response);
        }

        public async Task Set(ThermostatSetpointType type, float value)
        {
            await Channel.Send(Node, new Command(Class, command.Set, Convert.ToByte(type)));
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
