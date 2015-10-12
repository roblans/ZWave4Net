using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Vision
{
    public class ShockSensor : BatteryDevice
    {
        public event AsyncEventHandler<EventArgs> ShockDetected;
        public event AsyncEventHandler<EventArgs> ShockCancelled;
        public event AsyncEventHandler<EventArgs> TamperDetected;
        public event AsyncEventHandler<EventArgs> TamperCancelled;

        public ShockSensor(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private async Task Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                await OnShockCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                await OnShockDetected(EventArgs.Empty);
                return;
            }
        }

        protected virtual async Task OnShockDetected(EventArgs e)
        {
            await ShockDetected?.Invoke(this, e);
        }

        protected virtual async Task OnShockCancelled(EventArgs e)
        {
            await ShockCancelled?.Invoke(this, e);
        }

        private async Task Alarm_Changed(object sender, ReportEventArgs<AlarmReport> e)
        {
            if (e.Report.Detail == AlarmDetailType.TamperingProductCoveringRemoved)
            {
                if (e.Report.Level == 0x00)
                {
                    await OnTamperCancelled(EventArgs.Empty);
                    return;
                }
                if (e.Report.Level == 0xFF)
                {
                    await OnTamperDetected(EventArgs.Empty);
                    return;
                }
            }
            if (e.Report.Type == AlarmType.Burglar)
            {
                if (e.Report.Level == 0x00)
                {
                    await OnShockCancelled(EventArgs.Empty);
                    return;
                }
                if (e.Report.Level == 0xFF)
                {
                    await OnShockDetected(EventArgs.Empty);
                    return;
                }
            }
        }

        protected virtual async Task OnTamperDetected(EventArgs e)
        {
            await TamperDetected?.Invoke(this, e);
        }

        protected virtual async Task OnTamperCancelled(EventArgs e)
        {
            await TamperCancelled?.Invoke(this, e);
        }
    }
}
