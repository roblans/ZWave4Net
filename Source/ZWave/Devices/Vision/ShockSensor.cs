using System;
using ZWave.CommandClasses;

namespace ZWave.Devices.Vision
{
    public class ShockSensor : BatteryDevice
    {
        public event EventHandler<EventArgs> ShockDetected;
        public event EventHandler<EventArgs> ShockCancelled;
        public event EventHandler<EventArgs> TamperDetected;
        public event EventHandler<EventArgs> TamperCancelled;

        public ShockSensor(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.TargetValue == 0x00)
            {
                OnShockCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.TargetValue == 0xFF)
            {
                OnShockDetected(EventArgs.Empty);
                return;
            }
        }

        protected virtual void OnShockDetected(EventArgs e)
        {
            ShockDetected?.Invoke(this, e);
        }

        protected virtual void OnShockCancelled(EventArgs e)
        {
            ShockCancelled?.Invoke(this, e);
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
                    OnShockCancelled(EventArgs.Empty);
                    return;
                }
                if (e.Report.Level == 0xFF)
                {
                    OnShockDetected(EventArgs.Empty);
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
    }
}
