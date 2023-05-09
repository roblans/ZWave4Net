using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class SmokeSensor : Device
    {
        public event EventHandler<MeasureEventArgs> TemperatureMeasured;
        public event EventHandler<MeasureEventArgs> SmokeMeasured;

        public SmokeSensor(Node node)
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
            if (e.Report.Type == SensorType.CO2) // Todo: Check
            {
                OnSmokeMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Smoke)));
            }
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.TargetValue == 0x00)
            {
                //OnMotionCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.TargetValue == 0xFF)
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

        protected virtual void OnTemperatureMeasured(MeasureEventArgs e)
        {
            TemperatureMeasured?.Invoke(this, e);
        }

        protected virtual void OnSmokeMeasured(MeasureEventArgs e)
        {
            SmokeMeasured?.Invoke(this, e);
        }
        
    }
}
