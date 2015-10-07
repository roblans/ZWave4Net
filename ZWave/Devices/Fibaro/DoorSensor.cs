using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class DoorSensor : Device
    {
        public event EventHandler<EventArgs> TamperDetected;
        public event EventHandler<EventArgs> TamperCancelled;
        public event EventHandler<EventArgs> ContactOpen;
        public event EventHandler<EventArgs> ContactClosed;

        public DoorSensor(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SwitchBinary>().Changed += Contact_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                OnContactOpen(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                OnContactClosed(EventArgs.Empty);
                return;
            }
        }
        private void Contact_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
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
        

        private void Alarm_Changed(object sender, ReportEventArgs<AlarmReport> e)
        {
            if (e.Report.Type == AlarmType.General)
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
