using System;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Vision
{
    public class GarageDoorDetector : BatteryDevice
    {
        public event EventHandler<EventArgs> DoorOpened;
        public event EventHandler<EventArgs> DoorClosed;
        public event EventHandler<EventArgs> TamperDetected;
        public event EventHandler<EventArgs> TamperCancelled;

        public GarageDoorDetector(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.TargetValue == 0x00)
            {
                OnDoorClosed(EventArgs.Empty);
                return;
            }
            if (e.Report.TargetValue == 0xFF)
            {
                OnDoorOpened(EventArgs.Empty);
                return;
            }
        }

        protected virtual void OnDoorOpened(EventArgs e)
        {
            DoorOpened?.Invoke(this, e);
        }

        protected virtual void OnDoorClosed(EventArgs e)
        {
            DoorClosed?.Invoke(this, e);
        }

        private void Alarm_Changed(object sender, ReportEventArgs<AlarmReport> e)
        {
            if (e.Report.Event == NotificationState.TamperingProductCoverRemoved)
            {
                if (e.Report.Level == 0x00)
                {
                    OnTamperCancelled(EventArgs.Empty);
                    return;
                }
                if (e.Report.Level == 0xFF)
                {
                    OnTamperDetected(EventArgs.Empty);
                    return;
                }
            }
            if (e.Report.Type == NotificationType.HomeSecurity)
            {
                if (e.Report.Level == 0x00)
                {
                    OnDoorClosed(EventArgs.Empty);
                    return;
                }
                if (e.Report.Level == 0xFF)
                {
                    OnDoorOpened(EventArgs.Empty);
                    return;
                }
            }
        }

        protected virtual void OnTamperDetected(EventArgs e)
        {
            TamperDetected?.Invoke(this, e);
        }

        protected virtual void OnTamperCancelled(EventArgs e)
        {
            TamperCancelled?.Invoke(this, e);
        }

        public async Task<bool> IsDoorOpen()
        {
            var basic = await Node.GetCommandClass<Basic>().Get();
            if (basic.TargetValue == 0xFF)
            {
                return true;
            }

            var alarm = (await Node.GetCommandClass<Alarm>().Get());
            if (alarm.Type == NotificationType.HomeSecurity && alarm.Level == 0xFF)
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
