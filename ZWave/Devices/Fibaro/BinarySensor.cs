using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class BinarySensor : Device
    {

        public event AsyncEventHandler<EventArgs> SwitchedOn1;
        public event AsyncEventHandler<EventArgs> SwitchedOff1;
        public event AsyncEventHandler<EventArgs> SwitchedOn2;
        public event AsyncEventHandler<EventArgs> SwitchedOff2;

        public BinarySensor(Node node)
            : base(node)
        {
            Node.GetCommandClass<SwitchBinary>().Changed += SwitchBinary1_Changed;
            Node.GetCommandClass<SwitchBinary>().Changed += SwitchBinary2_Changed;
        }


        private async Task SwitchBinary1_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
            {
                await OnSwitchedOn1(EventArgs.Empty);
            }
            else
            {
                await OnSwitchedOff1(EventArgs.Empty);
            }
        }

        private async Task SwitchBinary2_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
            {
                await OnSwitchedOn2(EventArgs.Empty);
            }
            else
            {
                await OnSwitchedOff2(EventArgs.Empty);
            }
        }
        

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add((byte)group, node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove((byte)group, node.NodeID);
        }


        protected virtual async Task OnSwitchedOn1(EventArgs e)
        {
            await SwitchedOn1?.Invoke(this, e);
        }

        protected virtual async Task OnSwitchedOff1(EventArgs e)
        {
            await SwitchedOff1?.Invoke(this, e);
        }

        protected virtual async Task OnSwitchedOn2(EventArgs e)
        {
            await SwitchedOn2?.Invoke(this, e);
        }

        protected virtual async Task OnSwitchedOff2(EventArgs e)
        {
            await SwitchedOff2?.Invoke(this, e);
        }

    }
}
