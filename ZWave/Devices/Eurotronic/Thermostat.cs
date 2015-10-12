using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Vision
{
    public class Thermostat : Device
    {

        public event AsyncEventHandler<MeasureEventArgs> TemperatureMeasured;

        public Thermostat(Node node)
            : base(node)
        {
            node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
        }

        private async Task SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            if (e.Report.Type == SensorType.Temperature)
            {
                await OnTemperatureMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Celsius)));
            }
        }

        public async Task SetTemperature(double temperature)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task OnTemperatureMeasured(MeasureEventArgs e)
        {
            await TemperatureMeasured?.Invoke(this, e);
        }

    }
}
