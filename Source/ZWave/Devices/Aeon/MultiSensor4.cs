using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Devices.Fibaro;

namespace ZWave.Devices.Aeon
{
    public class MultiSensor4 : BatteryDevice
    {
        public event EventHandler<EventArgs> MotionDetected;
        public event EventHandler<EventArgs> MotionCancelled;
        public event EventHandler<MeasureEventArgs> TemperatureMeasured;
        public event EventHandler<MeasureEventArgs> LuminanceMeasured;
        public event EventHandler<MeasureEventArgs> HumidityMeasured;

        public MultiSensor4(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
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
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.TargetValue == 0x00)
            {
                OnMotionCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.TargetValue == 0xFF)
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
    }
}
