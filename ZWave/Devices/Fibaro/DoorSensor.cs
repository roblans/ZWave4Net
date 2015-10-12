using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class DoorSensor : Device
    {
        public event AsyncEventHandler<EventArgs> TamperDetected;
        public event AsyncEventHandler<EventArgs> TamperCancelled;
        public event AsyncEventHandler<EventArgs> ContactOpen;
        public event AsyncEventHandler<EventArgs> ContactClosed;

        public DoorSensor(Node node)
            : base(node)
        {
            node.GetCommandClass<Basic>().Changed += Basic_Changed;
            node.GetCommandClass<SwitchBinary>().Changed += Contact_Changed;
            node.GetCommandClass<Alarm>().Changed += Alarm_Changed;
        }

        private async Task Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value == 0x00)
            {
                await OnContactOpen(EventArgs.Empty);
                return;
            }
            if (e.Report.Value == 0xFF)
            {
                await OnContactClosed(EventArgs.Empty);
                return;
            }
        }
        private async Task Contact_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
            {
                await OnContactOpen(EventArgs.Empty);
            }
            else
            {
                await OnContactClosed(EventArgs.Empty);
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
        

        private async Task Alarm_Changed(object sender, ReportEventArgs<AlarmReport> e)
        {
            if (e.Report.Type == AlarmType.General)
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
        }

        protected virtual async Task OnContactOpen(EventArgs e)
        {
            await ContactOpen?.Invoke(this, e);
        }

        protected virtual async Task OnContactClosed(EventArgs e)
        {
            await ContactClosed?.Invoke(this, e);
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
