using Framework.Threading.Tasks;
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
        public event AsyncEventHandler<EventArgs> MotionDetected;
        public event AsyncEventHandler<EventArgs> MotionCancelled;
        public event AsyncEventHandler<MeasureEventArgs> TemperatureMeasured;
        public event AsyncEventHandler<MeasureEventArgs> LuminanceMeasured;
        public event AsyncEventHandler<MeasureEventArgs> HumidityMeasured;
        public event AsyncEventHandler<MeasureEventArgs> UltravioletMeasured;
        public event AsyncEventHandler<MeasureEventArgs> SeismicIntensityMeasured; // ToDo: Check

        public MultiSensor6(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
        }

        private async Task SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            if (e.Report.Type == SensorType.Temperature)
            {
                await OnTemperatureMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Celsius)));
            }
            if (e.Report.Type == SensorType.Luminance)
            {
                await OnLuminanceMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Lux)));
            }
            if (e.Report.Type == SensorType.RelativeHumidity)
            {
                await OnHumidityMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Humidity)));
            }
            if (e.Report.Type == SensorType.Ultraviolet)
            {
                await OnUltravioletMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Ultraviolet)));
            }
            if (e.Report.Type == SensorType.SeismicIntensity) // ToDo: Check
            {
                await OnSeismicIntensityMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.SeismicIntensity)));
            }
        }

        private async Task Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                await OnMotionCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                await OnMotionDetected(EventArgs.Empty);
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

        protected virtual async Task OnMotionDetected(EventArgs e)
        {
            await MotionDetected?.Invoke(this, e);
        }

        protected virtual async Task OnMotionCancelled(EventArgs e)
        {
            await MotionCancelled?.Invoke(this, e);
        }

        protected virtual async Task OnTemperatureMeasured(MeasureEventArgs e)
        {
            await TemperatureMeasured?.Invoke(this, e);
        }

        protected virtual async Task OnLuminanceMeasured(MeasureEventArgs e)
        {
            await LuminanceMeasured?.Invoke(this, e);
        }

        protected virtual async Task OnHumidityMeasured(MeasureEventArgs e)
        {
            await HumidityMeasured?.Invoke(this, e);
        }

        protected virtual async Task OnUltravioletMeasured(MeasureEventArgs e)
        {
            await UltravioletMeasured?.Invoke(this, e);
        }

        protected virtual async Task OnSeismicIntensityMeasured(MeasureEventArgs e)
        {
            await SeismicIntensityMeasured?.Invoke(this, e);
        }
    }
}
