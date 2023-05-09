using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class DoorSensor : BatteryDevice
    {
        public event EventHandler<EventArgs> TamperDetected;
        public event EventHandler<EventArgs> TamperCancelled;
        public event EventHandler<EventArgs> DoorOpened;
        public event EventHandler<EventArgs> DoorClosed;

        public DoorSensor(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SensorBinary>().Changed += SensorBinary_Changed;
            node.GetCommandClass<SensorAlarm>().Changed += SensorAlarm_Changed;
        }

        public async Task<bool> IsDoorOpen()
        {
            var basic = await Node.GetCommandClass<Basic>().Get();
            if (basic.TargetValue == 0xFF)
            {
                return true;
            }

            var sensorBinary = (await Node.GetCommandClass<SensorBinary>().Get());
            if (sensorBinary.Value)
            {
                return true;
            }

            return false;
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.TargetValue == 0xFF)
            {
                OnDoorOpened(EventArgs.Empty);
                return;
            }
            if (e.Report.TargetValue == 0x00)
            {
                OnDoorClosed(EventArgs.Empty);
                return;
            }
        }
        private void SensorBinary_Changed(object sender, ReportEventArgs<SensorBinaryReport> e)
        {
            if (e.Report.Value)
            {
                OnDoorOpened(EventArgs.Empty);
            }
            else
            {
                OnDoorClosed(EventArgs.Empty);
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
        

        private void SensorAlarm_Changed(object sender, ReportEventArgs<SensorAlarmReport> e)
        {
            if (e.Report.Type == NotificationType.General)
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

        protected virtual void OnDoorOpened(EventArgs e)
        {
            DoorOpened?.Invoke(this, e);
        }

        protected virtual void OnDoorClosed(EventArgs e)
        {
            DoorClosed?.Invoke(this, e);
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
