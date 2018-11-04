using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Devices.Fibaro;

namespace ZWave.Devices.Aeon
{
    public class MultiSensor6 : BatteryDevice
    {
        public event EventHandler<EventArgs> MotionDetected;
        public event EventHandler<EventArgs> MotionCancelled;
        public event EventHandler<MeasureEventArgs> TemperatureMeasured;
        public event EventHandler<MeasureEventArgs> LuminanceMeasured;
        public event EventHandler<MeasureEventArgs> HumidityMeasured;
        public event EventHandler<MeasureEventArgs> UltravioletMeasured;
        public event EventHandler<EventArgs> VibrationDetected;

        public MultiSensor6(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private void Alarm_Changed(object sender, ReportEventArgs<AlarmReport> e)
        {
            if (e.Report.Type == AlarmType.General && e.Report.Detail == AlarmDetailType.TamperingProductCoveringRemoved)
            {
                OnVibrationDetected(EventArgs.Empty);
            }
        }

        private void SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            if (e.Report.Type == SensorType.Temperature)
            {
                OnTemperatureMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Celsius)));
            }
            if (e.Report.Type == SensorType.Luminance)
            {
                OnLuminanceMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Lux)));
            }
            if (e.Report.Type == SensorType.RelativeHumidity)
            {
                OnHumidityMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Humidity)));
            }
            if (e.Report.Type == SensorType.Ultraviolet)
            {
                OnUltravioletMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Ultraviolet)));
            }
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                OnMotionCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                OnMotionDetected(EventArgs.Empty);
                return;
            }
        }

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add(Convert.ToByte(group), node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove(Convert.ToByte(group), node.NodeID);
        }

        public async Task<Measure> GetTemperature()
        {
            var report = await Node.GetCommandClass<SensorMultiLevel>().Get(SensorType.Temperature);
            return new Measure(report.Value, Unit.Celsius);
        }

        public async Task<Measure> GetLuminance()
        {
            var report = await Node.GetCommandClass<SensorMultiLevel>().Get(SensorType.Luminance);
            return new Measure(report.Value, Unit.Lux);
        }

        public async Task<Measure> GetRelativeHumidity()
        {
            var report = await Node.GetCommandClass<SensorMultiLevel>().Get(SensorType.RelativeHumidity);
            return new Measure(report.Value, Unit.Humidity);
        }

        public async Task<Measure> GetUltraviolet()
        {
            var report = await Node.GetCommandClass<SensorMultiLevel>().Get(SensorType.Ultraviolet);
            return new Measure(report.Value, Unit.Ultraviolet);
        }

        protected virtual void OnMotionDetected(EventArgs e)
        {
            MotionDetected?.Invoke(this, e);
        }

        protected virtual void OnMotionCancelled(EventArgs e)
        {
            MotionCancelled?.Invoke(this, e);
        }

        protected virtual void OnTemperatureMeasured(MeasureEventArgs e)
        {
            TemperatureMeasured?.Invoke(this, e);
        }

        protected virtual void OnLuminanceMeasured(MeasureEventArgs e)
        {
            LuminanceMeasured?.Invoke(this, e);
        }

        protected virtual void OnHumidityMeasured(MeasureEventArgs e)
        {
            HumidityMeasured?.Invoke(this, e);
        }

        protected virtual void OnUltravioletMeasured(MeasureEventArgs e)
        {
            UltravioletMeasured?.Invoke(this, e);
        }

        protected virtual void OnVibrationDetected(EventArgs e)
        {
            VibrationDetected?.Invoke(this, e);
        }
    }
}
