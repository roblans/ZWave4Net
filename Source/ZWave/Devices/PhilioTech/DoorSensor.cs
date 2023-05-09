using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Devices.Fibaro;

namespace ZWave.Devices.PhilioTech
{
    public class DoorSensor : Device
    {
        public event EventHandler<EventArgs> MotionDetected;
        public event EventHandler<EventArgs> MotionCancelled;
        public event EventHandler<EventArgs> TamperDetected;
        public event EventHandler<EventArgs> TamperCancelled;
        public event EventHandler<EventArgs> ContactOpen;
        public event EventHandler<EventArgs> ContactClosed;
        public event EventHandler<MeasureEventArgs> TemperatureMeasured;
        public event EventHandler<MeasureEventArgs> LuminanceMeasured;

        public DoorSensor(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SwitchBinary>().Changed += Contact_Changed;
            node.GetCommandClass<SensorMultiLevel>().Changed += SensorMultiLevel_Changed;
            node.GetCommandClass<Notification>().Changed += Notification_Changed;
        }

        private void SensorMultiLevel_Changed(object sender, ReportEventArgs<SensorMultiLevelReport> e)
        {
            if (e.Report.Type == SensorType.Temperature)
            {
                OnTemperatureMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Celsius)));
            }
            if (e.Report.Type == SensorType.Luminance)
            {
                OnLuminanceMeasured(new MeasureEventArgs(new Measure(e.Report.Value, Unit.Lux)));
            }
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.TargetValue == 0x00)
            {
                OnMotionCancelled(EventArgs.Empty);
                return;
            }
            if (e.Report.TargetValue == 0xFF)
            {
                OnMotionDetected(EventArgs.Empty);
                return;
            }
        }
        private void Contact_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.TargetValue == true)
            {
                OnContactOpen(EventArgs.Empty);
            }
            else
            {
                OnContactClosed(EventArgs.Empty);
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

        protected virtual void OnMotionDetected(EventArgs e)
        {
            MotionDetected?.Invoke(this, e);
        }

        protected virtual void OnMotionCancelled(EventArgs e)
        {
            MotionCancelled?.Invoke(this, e);
        }
        
        protected virtual void OnTemperatureMeasured(MeasureEventArgs e)
        {
            TemperatureMeasured?.Invoke(this, e);
        }

        protected virtual void OnLuminanceMeasured(MeasureEventArgs e)
        {
            LuminanceMeasured?.Invoke(this, e);
        }

        private void Notification_Changed(object sender, ReportEventArgs<NotificationReport> e)
        {
            if (e.Report.Type == NotificationType.AccessControl)
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
        }

        protected virtual void OnContactOpen(EventArgs e)
        {
            ContactOpen?.Invoke(this, e);
        }

        protected virtual void OnContactClosed(EventArgs e)
        {
            ContactClosed?.Invoke(this, e);
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
