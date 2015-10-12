using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices
{
    public class BatteryDevice : Device
    {
        public event AsyncEventHandler<EventArgs> WakeUp;

        public BatteryDevice(Node node)
            : base(node)
        {
            node.GetCommandClass<WakeUp>().Changed += WakeUp_Changed;

        }

        private async Task WakeUp_Changed(object sender, ReportEventArgs<WakeUpReport> e)
        {
            if (e.Report.Awake)
            {
                await OnWakeUp(EventArgs.Empty);
                return;
            }
        }

        protected virtual async Task OnWakeUp(EventArgs e)
        {
            await WakeUp?.Invoke(this, e);
        }

        public async Task<TimeSpan> GetWakeUpInterval()
        {
            return (await Node.GetCommandClass<WakeUp>().GetInterval()).Interval;
        }

        public async Task SetWakeUpInterval(TimeSpan value)
        {
            await Node.GetCommandClass<WakeUp>().SetInterval(value, 0xFF);
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
