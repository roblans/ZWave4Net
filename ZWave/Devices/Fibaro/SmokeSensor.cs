using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class SmokeSensor : Device
    {
        public event AsyncEventHandler<MeasureEventArgs> TemperatureMeasured;
        public event AsyncEventHandler<MeasureEventArgs> SmokeMeasured;

        public SmokeSensor(Node node)
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
            if (e.Report.Type == SensorType.CO2) // Todo: Check
            {
                await OnSmokeMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Smoke)));
            }
        }

        private async Task Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                //OnMotionCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                //OnMotionDetected(EventArgs.Empty);
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

        protected virtual async Task OnTemperatureMeasured(MeasureEventArgs e)
        {
            await TemperatureMeasured?.Invoke(this, e);
        }

        protected virtual async Task OnSmokeMeasured(MeasureEventArgs e)
        {
            await SmokeMeasured?.Invoke(this, e);
        }
        
    }
}
