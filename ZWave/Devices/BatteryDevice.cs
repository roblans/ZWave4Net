using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices
{
    public class BatteryDevice : Device
    {
        private byte? _controllerID;
        public event EventHandler<EventArgs> WakeUp;

        public BatteryDevice(Node node)
            : base(node)
        {
            node.GetCommandClass<WakeUp>().Changed += WakeUp_Changed;

        }

        private async Task<byte> GetControllerID()
        {
            return (_controllerID ?? (_controllerID = await Node.Controller.GetNodeID())).Value;
        }

        private void WakeUp_Changed(object sender, ReportEventArgs<WakeUpReport> e)
        {
            if (e.Report.Awake)
            {
                OnWakeUp(EventArgs.Empty);
                return;
            }
        }

        protected virtual void OnWakeUp(EventArgs e)
        {
            WakeUp?.Invoke(this, e);
        }

        public async Task<TimeSpan> GetWakeUpInterval()
        {
            return (await Node.GetCommandClass<WakeUp>().GetInterval()).Interval;
        }

        public async Task SetWakeUpInterval(TimeSpan value)
        {
            var controllerNodeID = await Node.Controller.GetNodeID();
            await Node.GetCommandClass<WakeUp>().SetInterval(value, controllerNodeID);
        }

        public async Task<Measure> GetBatteryLevel()
        {
            var value = (await Node.GetCommandClass<Battery>().Get()).Value;
            return new Measure(value, Unit.Percentage);
        }

        public async Task Sleep()
        {
            await Node.GetCommandClass<WakeUp>().NoMoreInformation();
        }
    }
}
