using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class WallPlug : Device
    {
        public event EventHandler<MeasureEventArgs> EnergyConsumptionMeasured;
        public event EventHandler<MeasureEventArgs> PowerLoadMeasured;
        public event EventHandler<EventArgs> SwitchedOn;
        public event EventHandler<EventArgs> SwitchedOff;

        public WallPlug(Node node)
            : base(node)
        {
            Node.GetCommandClass<SwitchBinary>().Changed += SwitchBinary_Changed;
            Node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
            Node.GetCommandClass<Meter>().Changed += Meter_Changed;
        }

        private void Meter_Changed(object sender, ReportEventArgs<MeterReport> e)
        {
            OnEnergyConsumptionMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.KiloWattHour)));
        }

        private void SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            OnPowerLoadMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Watt)));
        }

        private void SwitchBinary_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
            {
                OnSwitchedOn(EventArgs.Empty);
            }
            else
            {
                OnSwitchedOff(EventArgs.Empty);
            }
        }

        public async Task<Measure> GetPowerLoad()
        {
            var value = (await Node.GetCommandClass<SensorMultiLevel>().Get()).Value;
            return new Measure(value, Unit.Watt);
        }

        public async Task<Measure> GetEnergyConsumption()
        {
            var value = (await Node.GetCommandClass<Meter>().Get()).Value;
            return new Measure(value, Unit.KiloWattHour);
        }

        public async Task SwitchOn()
        {
            await Node.GetCommandClass<SwitchBinary>().Set(true);
        }

        public async Task SwitchOff()
        {
            await Node.GetCommandClass<SwitchBinary>().Set(false);
        }

        public async Task<bool> IsSwitchOn()
        {
            return (await Node.GetCommandClass<SwitchBinary>().Get()).Value;
        }

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add((byte)group, node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove((byte)group, node.NodeID);
        }

        public async Task SetLedRingColorOn(LedRingColorOn colorOn)
        {
            await Node.GetCommandClass<Configuration>().Set(61, Convert.ToByte(colorOn));
        }

        public async Task<LedRingColorOn> GetLedRingColorOn()
        {
            var value = (await Node.GetCommandClass<Configuration>().Get(61)).Value;
            return (LedRingColorOn)Enum.ToObject(typeof(LedRingColorOn), value);
        }

        public async Task<LedRingColorOff> GetLedRingColorOff()
        {
            var value = (await Node.GetCommandClass<Configuration>().Get(62)).Value;
            return (LedRingColorOff)Enum.ToObject(typeof(LedRingColorOn), value);
        }

        public async Task SetLedRingColorOff(LedRingColorOff colorOff)
        {
            await Node.GetCommandClass<Configuration>().Set(62, Convert.ToByte(colorOff));
        }

        public async Task<TimeSpan> GetMeasureInterval()
        {
            var value = (ushort)(await Node.GetCommandClass<Configuration>().Get(47)).Value;
            return TimeSpan.FromSeconds(value);
        }

        public async Task SetMeasureInterval(TimeSpan value)
        {
            await Node.GetCommandClass<Configuration>().Set(47, (ushort)value.TotalSeconds);
        }

        protected virtual void OnSwitchedOn(EventArgs e)
        {
            SwitchedOn?.Invoke(this, e);
        }

        protected virtual void OnSwitchedOff(EventArgs e)
        {
            SwitchedOff?.Invoke(this, e);
        }

        protected virtual void OnPowerLoadMeasured(MeasureEventArgs e)
        {
            PowerLoadMeasured?.Invoke(this, e);
        }

        protected virtual void OnEnergyConsumptionMeasured(MeasureEventArgs e)
        {
            EnergyConsumptionMeasured?.Invoke(this, e);
        }

        public enum LedRingColorOn : byte
        {
            PowerLoadStep = 0x00,
            PowerLoadContinuously = 0x01,
            White = 0x02,
            Red = 0x03,
            Green = 0x04,
            Blue = 0x05,
            Yellow = 0x06,
            Cyan = 0x07,
            Magenta = 0x08,
            Off = 0x09,
        }

        public enum LedRingColorOff : byte
        {
            NoChange = 0x00,
            White = 0x01,
            Red = 0x02,
            Green = 0x03,
            Blue = 0x04,
            Yellow = 0x05,
            Cyan = 0x06,
            Magenta = 0x07,
            Off = 0x08,
        }

    }
}
