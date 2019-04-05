using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class WallPlug : Device
    {
        public readonly FirmwareVersion Version;

        public event EventHandler<MeasureEventArgs> EnergyConsumptionMeasured;
        public event EventHandler<MeasureEventArgs> PowerLoadMeasured;
        public event EventHandler<EventArgs> SwitchedOn;
        public event EventHandler<EventArgs> SwitchedOff;

        public WallPlug(Node node, FirmwareVersion version = FirmwareVersion.Latest)
            : base(node)
        {
            Version = version;
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
            var value = (await Node.GetCommandClass<SensorMultiLevel>().Get(SensorType.Power)).Value;
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
            var parameterID = default(byte);
            switch(Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 61;
                    break;
                default:
                    parameterID = 41;
                    break;
            }
            var value = (byte)EnumConverter.GetConfigurationValue(colorOn, Version);
            await Node.GetCommandClass<Configuration>().Set(parameterID, value);
        }

        public async Task<LedRingColorOn> GetLedRingColorOn()
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 61;
                    break;
                default:
                    parameterID = 41;
                    break;
            }

            var value = Convert.ToByte((await Node.GetCommandClass<Configuration>().Get(parameterID)).Value);
            return EnumConverter.ParseConfigurationValue<LedRingColorOn>(value, Version);
        }

        public async Task<LedRingColorOff> GetLedRingColorOff()
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 62;
                    break;
                default:
                    parameterID = 42;
                    break;
            }

            var value = Convert.ToByte((await Node.GetCommandClass<Configuration>().Get(parameterID)).Value);
            return EnumConverter.ParseConfigurationValue<LedRingColorOff>(value, Version);
        }

        public async Task SetLedRingColorOff(LedRingColorOff colorOff)
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 62;
                    break;
                default:
                    parameterID = 42;
                    break;
            }

            var value = (byte)EnumConverter.GetConfigurationValue(colorOff, Version);
            await Node.GetCommandClass<Configuration>().Set(parameterID, value);
        }

        public async Task<TimeSpan> GetMeasureInterval()
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 47;
                    break;
                default:
                    parameterID = 14;
                    break;
            }

            var value = Convert.ToUInt16((await Node.GetCommandClass<Configuration>().Get(parameterID)).Value);
            return TimeSpan.FromSeconds(value);
        }

        public async Task SetMeasureInterval(TimeSpan value)
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 47;
                    break;
                default:
                    parameterID = 14;
                    break;
            }
            await Node.GetCommandClass<Configuration>().Set(parameterID, (ushort)value.TotalSeconds);
        }

        public async Task<sbyte> GetPowerLoadChangeReporting()
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 42;
                    break;
                default:
                    parameterID = 11;
                    break;
            }
            return (sbyte)(await Node.GetCommandClass<Configuration>().Get(parameterID)).Value;
        }

        public async Task SetPowerLoadChangeReporting(sbyte percentage)
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 42;
                    break;
                default:
                    parameterID = 11;
                    break;
            }
            await Node.GetCommandClass<Configuration>().Set(parameterID, percentage);
        }

        /// <summary>
        /// V2: Lowest value is 0.01 kWh. Highest is 2.54 kWh. If 0 is specified, it will disable reporting.
        /// V3: Lowest value is 0.01 kWh. Highest is 5 kWh. If 0 is specified, it will disable reporting.
        /// </summary>
        /// <param name="everykWh">Report everytime this threshold is reached</param>
        /// <returns></returns>
        public async Task SetEnergyReportingThreshold(decimal everykWh)
        {
            if (Version == FirmwareVersion.V2)
            {
                if (everykWh < 0.01M)
                    everykWh = 0M;
                if (everykWh > 2.54M)
                    everykWh = 2.54M;

                var value = (byte)(everykWh * 100);
                await Node.GetCommandClass<Configuration>().Set(45, value);
            }
            else
            {
                if (everykWh < 0.01M)
                    everykWh = 0M;
                if (everykWh > 5M)
                    everykWh = 5M;

                var value = (ushort)(everykWh * 100);
                await Node.GetCommandClass<Configuration>().Set(13, value);
            }
        }


        public async Task<decimal> GetEnergyReportingThreshold()
        {
            var parameterID = default(byte);
            switch (Version)
            {
                case FirmwareVersion.V2:
                    parameterID = 45;
                    break;
                default:
                    parameterID = 13;
                    break;
            }

            var value = (await Node.GetCommandClass<Configuration>().Get(parameterID)).Value;
            return (Convert.ToDecimal(value) / 100M);
        }

        /// <summary>
        /// V2: Max 6553, Min 1. Specificity: one decimal. Higher than 3200 will disable switch.
        /// V3: Max 3000, Min 1. Specificity: one decimal. Less than 1 will disable switch.
        /// </summary>
        public async Task SetOverloadSafetySwitch(decimal maxWatt)
        {
            if (Version == FirmwareVersion.V2)
            {
                if (maxWatt > 6553.5M)
                    maxWatt = 6553.5M;
                if (maxWatt < 1)
                    maxWatt = 0;
                ushort value = (ushort)(maxWatt);
                await Node.GetCommandClass<Configuration>().Set(70, value);
            }
            else
            {
                if (maxWatt > 3000)
                    maxWatt = 3000;
                if (maxWatt < 1)
                    maxWatt = 0;
                ushort value = (ushort)(maxWatt * 10);
                await Node.GetCommandClass<Configuration>().Set(3, value);
            }
        }

        public async Task<decimal> GetOverloadSafetySwitch()
        {
            if (Version == FirmwareVersion.V2)
            {
                var value = (await Node.GetCommandClass<Configuration>().Get(70)).Value;
                return Convert.ToDecimal(value);
            }
            else
            {
                var value = (await Node.GetCommandClass<Configuration>().Get(3)).Value;
                return Convert.ToDecimal(value) / 10;
            }
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

        public enum FirmwareVersion : byte
        {
            Latest = 0,
            V2 = 2,
            V3 = 3,
        }

        public enum LedRingColorOn : byte
        {
            [ConfigurationValue(0x00)]
            [ConfigurationValue(0x09, FirmwareVersion.V2)]
            Off,

            [ConfigurationValue(0x01)]
            [ConfigurationValue(0x01, FirmwareVersion.V2)]
            PowerLoadContinuously,

            [ConfigurationValue(0x02)]
            [ConfigurationValue(0x00, FirmwareVersion.V2)]
            PowerLoadStep,

            [ConfigurationValue(0x03)]
            [ConfigurationValue(0x02, FirmwareVersion.V2)]
            White,

            [ConfigurationValue(0x04)]
            [ConfigurationValue(0x03, FirmwareVersion.V2)]
            Red,

            [ConfigurationValue(0x05)]
            [ConfigurationValue(0x04, FirmwareVersion.V2)]
            Green,

            [ConfigurationValue(0x06)]
            [ConfigurationValue(0x05, FirmwareVersion.V2)]
            Blue,

            [ConfigurationValue(0x07)]
            [ConfigurationValue(0x06, FirmwareVersion.V2)]
            Yellow,

            [ConfigurationValue(0x08)]
            [ConfigurationValue(0x07, FirmwareVersion.V2)]
            Cyan,

            [ConfigurationValue(0x09)]
            [ConfigurationValue(0x08, FirmwareVersion.V2)]
            Magenta,
        }

        public enum LedRingColorOff : byte
        {
            [ConfigurationValue(0x00)]
            [ConfigurationValue(0x08, FirmwareVersion.V2)]
            Off,

            [ConfigurationValue(0x01)]
            [ConfigurationValue(0x00, FirmwareVersion.V2)]
            NoChange,

            [ConfigurationValue(0x03)]
            [ConfigurationValue(0x01, FirmwareVersion.V2)]
            White,

            [ConfigurationValue(0x04)]
            [ConfigurationValue(0x02, FirmwareVersion.V2)]
            Red,

            [ConfigurationValue(0x05)]
            [ConfigurationValue(0x03, FirmwareVersion.V2)]
            Green,

            [ConfigurationValue(0x06)]
            [ConfigurationValue(0x04, FirmwareVersion.V2)]
            Blue,

            [ConfigurationValue(0x07)]
            [ConfigurationValue(0x05, FirmwareVersion.V2)]
            Yellow,

            [ConfigurationValue(0x08)]
            [ConfigurationValue(0x06, FirmwareVersion.V2)]
            Cyan,

            [ConfigurationValue(0x09)]
            [ConfigurationValue(0x07, FirmwareVersion.V2)]
            Magenta,
        }

    }
}
