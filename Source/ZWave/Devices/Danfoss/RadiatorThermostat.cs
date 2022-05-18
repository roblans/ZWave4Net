using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Danfoss
{
    public class RadiatorThermostat : BatteryDevice
    {
        public event EventHandler<SetpointEventArgs> SetPointChanged;

        public RadiatorThermostat(Node node)
            : base(node)
        {
            node.GetCommandClass<ThermostatSetpoint>().Changed += ThermostatSetpoint_Changed;

        }

        public async Task<Setpoint> GetSetpoint()
        {
            var report = await Node.GetCommandClass<ThermostatSetpoint>().Get(ThermostatSetpointType.Heating);
            return new Setpoint(report.Value, report.Scale == 0 ? Unit.Celsius : Unit.Fahrenheit);
        }

        public async Task SetSetpoint(float value)
        {
            await Node.GetCommandClass<ThermostatSetpoint>().Set(ThermostatSetpointType.Heating, value);
        }

        private void ThermostatSetpoint_Changed(object sender, ReportEventArgs<ThermostatSetpointReport> e)
        {
            var setpoint = new Setpoint(e.Report.Value, e.Report.Scale == 0 ? Unit.Celsius : Unit.Fahrenheit);
            OnSetPointChanged(new SetpointEventArgs(setpoint));
        }

        protected virtual void OnSetPointChanged(SetpointEventArgs e)
        {
            SetPointChanged?.Invoke(this, e);
        }

        public async Task<Clock> GetClock()
        {
            var report = await Node.GetCommandClass<CommandClasses.Clock>().Get();
            return new Clock(report.DayOfWeek, report.Hour, report.Minute);
        }

        public async Task SetClock(Clock value)
        {
            await Node.GetCommandClass<CommandClasses.Clock>().Set((DayOfWeek)value.DayOfWeek, (byte)value.Hour, (byte)value.Minute);
        }
    }
}
