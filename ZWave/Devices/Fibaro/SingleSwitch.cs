using Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class SingleSwitch : Device
    {
        public event AsyncEventHandler<EventArgs> SwitchedOn;
        public event AsyncEventHandler<EventArgs> SwitchedOff;

        public SingleSwitch(Node node)
            : base(node)
        {
            Node.GetCommandClass<SwitchBinary>().Changed += SwitchBinary_Changed;
        }


        private async Task SwitchBinary_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
            {
                await OnSwitchedOn(EventArgs.Empty);
            }
            else
            {
                await OnSwitchedOff(EventArgs.Empty);
            }
        }

        public async Task SwitchOn()
        {
            await Node.GetCommandClass<SwitchBinary>().Set(true);
        }

        public async Task SwitchOff()
        {
            await Node.GetCommandClass<SwitchBinary>().Set(false);
        }

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add((byte)group, node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove((byte)group, node.NodeID);
        }


        protected virtual async Task OnSwitchedOn(EventArgs e)
        {
            await SwitchedOn?.Invoke(this, e);
        }

        protected virtual async Task OnSwitchedOff(EventArgs e)
        {
            await SwitchedOff?.Invoke(this, e);
        }

    }
}
