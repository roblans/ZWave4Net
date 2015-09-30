using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class WallPlug : Device
    {
        public event EventHandler<MeasureEventArgs> EnergyConsumptionChanged;
        public event EventHandler<MeasureEventArgs> PowerLoadChanged;
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
            OnEnergyConsumptionChanged(new MeasureEventArgs(e.Report.Value));
        }

        private void SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            OnPowerLoadChanged(new MeasureEventArgs(e.Report.Value));
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

        public async Task<float> GetPowerLoad()
        {
            return (await Node.GetCommandClass<SensorMultiLevel>().Get()).Value;
        }

        public async Task<LedRingColorOff> GetLedRingColorOff()
        {
            return (LedRingColorOff)(await Node.GetCommandClass<Configuration>().Get(61)).Value;
        }

        public async Task SetLedRingColorOff(LedRingColorOff colorOff)
        {
            await Node.GetCommandClass<Configuration>().Set(61, (byte)colorOff);
        }

        public async Task SetLedRingColorOn(LedRingColorOn colorOn)
        {
            await Node.GetCommandClass<Configuration>().Set(62, (byte)colorOn);
        }

        public async Task<LedRingColorOn> GetLedRingColorOn()
        {
            return (LedRingColorOn)(await Node.GetCommandClass<Configuration>().Get(62)).Value;
        }

        public async Task SwitchOn()
        {
            await Node.GetCommandClass<SwitchBinary>().Set(true);
        }

        public async Task SwitchOff()
        {
            await Node.GetCommandClass<SwitchBinary>().Set(false);
        }

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add((byte)group, node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove((byte)group, node.NodeID);
        }

        protected virtual void OnSwitchedOn(EventArgs e)
        {
            SwitchedOn?.Invoke(this, e);
        }

        protected virtual void OnSwitchedOff(EventArgs e)
        {
            SwitchedOff?.Invoke(this, e);
        }

        protected virtual void OnPowerLoadChanged(MeasureEventArgs e)
        {
            PowerLoadChanged?.Invoke(this, e);
        }

        protected virtual void OnEnergyConsumptionChanged(MeasureEventArgs e)
        {
            EnergyConsumptionChanged?.Invoke(this, e);
        }
    }
}
