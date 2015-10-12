using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Vision
{
    public class GarageDoorDetector : BatteryDevice
    {
        public event AsyncEventHandler<EventArgs> DoorOpened;
        public event AsyncEventHandler<EventArgs> DoorClosed;
        public event AsyncEventHandler<EventArgs> TamperDetected;
        public event AsyncEventHandler<EventArgs> TamperCancelled;

        public GarageDoorDetector(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private async Task Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                await OnDoorClosed(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                await OnDoorOpened(EventArgs.Empty);
                return;
            }
        }

        protected virtual async Task OnDoorOpened(EventArgs e)
        {
            await DoorOpened?.Invoke(this, e);
        }

        protected virtual async Task OnDoorClosed(EventArgs e)
        {
            await DoorClosed?.Invoke(this, e);
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
                    await OnDoorClosed(EventArgs.Empty);
                    return;
                }
                if (e.Report.Level == 0xFF)
                {
                    await OnDoorOpened(EventArgs.Empty);
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

        public async Task<bool> IsDoorOpen()
        {
            var basic = await Node.GetCommandClass<Basic>().Get();
            if (basic.Value == 0xFF)
            {
                return true;
            }

            var alarm = (await Node.GetCommandClass<Alarm>().Get());
            if (alarm.Type == AlarmType.Burglar && alarm.Level == 0xFF)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsDoorClosed()
        {
            return !(await IsDoorOpen());
        }
    }
}
