using System;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class WallPlug_V3 : Device
    {
        public event EventHandler<MeasureEventArgs> EnergyConsumptionMeasured;
        public event EventHandler<MeasureEventArgs> PowerLoadMeasured;
        public event EventHandler<bool> PowerStateChanged;

        public WallPlug_V3(Node node)
            : base(node)
        {
            Node.GetCommandClass<SwitchBinary>().Changed     += SwitchBinary_Changed;
            Node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
            Node.GetCommandClass<Meter>().Changed            += Meter_Changed;
        }
        private void SwitchBinary_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            PowerStateChanged?.Invoke(this, e.Report.Value);
        }
        public async Task<bool> IsSwitchOn()
        {
            return (await Node.GetCommandClass<SwitchBinary>().Get()).Value;
        }
        public async Task Switch(bool state)
        {
            await Node.GetCommandClass<SwitchBinary>().Set(state);
        }

        private void Meter_Changed(object sender, ReportEventArgs<MeterReport> e)
        {
            OnEnergyConsumptionMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.KiloWattHour)));
        }

        private void SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            OnPowerLoadMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Watt)));
        }

        public async Task<Measure> GetPowerLoad()
        {
            var value = (await Node.GetCommandClass<SensorMultiLevel>().Get(SensorType.Power)).Value;
            return new Measure(value, Unit.Watt);
        }

        public async Task<Measure> GetEnergyConsumption()
        {
            var value = (await Node.GetCommandClass<Meter>().Get()).Value;
            return new Measure(value, Unit.KiloWattHour);
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
            await Node.GetCommandClass<Configuration>().Set(41, Convert.ToByte(colorOn));
        }

        public async Task<LedRingColorOn> GetLedRingColorOn()
        {
            var value = (await Node.GetCommandClass<Configuration>().Get(41)).Value;
            return (LedRingColorOn)Enum.ToObject(typeof(LedRingColorOn), value);
        }

        public async Task<LedRingColorOff> GetLedRingColorOff()
        {
            var value = (await Node.GetCommandClass<Configuration>().Get(42)).Value;
            return (LedRingColorOff)Enum.ToObject(typeof(LedRingColorOn), value);
        }

        public async Task SetLedRingColorOff(LedRingColorOff colorOff)
        {
            await Node.GetCommandClass<Configuration>().Set(42, Convert.ToByte(colorOff));
        }

        public async Task<TimeSpan> GetMeasureInterval()
        {
            var value = Convert.ToUInt16((await Node.GetCommandClass<Configuration>().Get(14)).Value);
            return TimeSpan.FromSeconds(value);
        }

        public async Task SetMeasureInterval(TimeSpan value)
        {
            await Node.GetCommandClass<Configuration>().Set(14, (ushort)value.TotalSeconds);
        }

        public async Task<sbyte> GetPowerLoadChangeReporting()
        {
            return (sbyte)(await Node.GetCommandClass<Configuration>().Get(11)).Value;
        }

        public async Task SetPowerLoadChangeReporting(sbyte percentage)
        {
            await Node.GetCommandClass<Configuration>().Set(11, percentage);
        }
        /// <summary>
        /// Lowest value is 0.01 kWh. Highest is 5 kWh. If 0 is specified, it will disable reporting.
        /// </summary>
        /// <param name="everykWh">Report everytime this threshold is reached</param>
        /// <returns></returns>
        public async Task SetEnergyReportingThreshold(decimal everykWh)
        {
            if (everykWh < 0.01M)
                everykWh = 0M;
            if (everykWh > 5M)
                everykWh = 5M;

            var value = (ushort)(everykWh * 100);
            await Node.GetCommandClass<Configuration>().Set(13, value);
        }

        public async Task<decimal> GetEnergyReportingThreshold()
        {
            var value = (await Node.GetCommandClass<Configuration>().Get(13)).Value;
            return (Convert.ToDecimal(value) / 100M);
        }

        /// <summary>
        /// Max 3000, Min 1. Specificity: one decimal. Less than 1 will disable switch.
        /// </summary>
        /// <param name="maxWatt"></param>
        /// <returns></returns>
        public async Task SetOverloadSafetySwitch(decimal maxWatt)
        {
            if (maxWatt > 3000)
                maxWatt = 3000;
            if (maxWatt < 1)
                maxWatt = 0;

            ushort value = (ushort)(maxWatt * 10);
            await Node.GetCommandClass<Configuration>().Set(3, value);
        }

        public async Task<decimal> GetOverloadSafetySwitch()
        {
            var value = (await Node.GetCommandClass<Configuration>().Get(3)).Value;
            return Convert.ToDecimal(value)/10;
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
            Off = 0x00,
            PowerLoadContinuously = 0x01,
            PowerLoadStep = 0x02,
            White = 0x03,
            Red = 0x04,
            Green = 0x05,
            Blue = 0x06,
            Yellow = 0x07,
            Cyan = 0x08,
            Magenta = 0x09,
        }

        public enum LedRingColorOff : byte
        {
            Off = 0x00,
            NoChange = 0x01,
            White = 0x03,
            Red = 0x04,
            Green = 0x05,
            Blue = 0x06,
            Yellow = 0x07,
            Cyan = 0x08,
            Magenta = 0x09,
        }
    }
}
