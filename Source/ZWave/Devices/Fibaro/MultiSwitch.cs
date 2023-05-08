using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Fibaro
{
    public class MultiSwitch : Device
    {

        public event EventHandler<EventArgs> SwitchedOn1;
        public event EventHandler<EventArgs> SwitchedOff1;
        public event EventHandler<EventArgs> SwitchedOn2;
        public event EventHandler<EventArgs> SwitchedOff2;

        public MultiSwitch(Node node)
            : base(node)
        {
            MultiChannel multiChannel = Node.GetCommandClass<MultiChannel>();
            multiChannel.GetEndPointCommandClass<SwitchBinary>(1).Changed += Switch1Changed;
            multiChannel.GetEndPointCommandClass<SwitchBinary>(2).Changed += Switch2Changed;
        }

        private void Switch1Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.TargetValue == true)
            {
                OnSwitchedOn1(EventArgs.Empty);
            }
            else
            {
                OnSwitchedOff1(EventArgs.Empty);
            }
        }

        private void Switch2Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.TargetValue == true)
            {
                OnSwitchedOn2(EventArgs.Empty);
            }
            else
            {
                OnSwitchedOff2(EventArgs.Empty);
            }
        }

        public async Task SwitchOn1()
        {
            await Node.GetCommandClass<MultiChannel>().GetEndPointCommandClass<SwitchBinary>(1).Set(true);
        }

        public async Task SwitchOff1()
        {
            await Node.GetCommandClass<MultiChannel>().GetEndPointCommandClass<SwitchBinary>(1).Set(false);
        }

        public async Task SwitchOn2()
        {
            await Node.GetCommandClass<MultiChannel>().GetEndPointCommandClass<SwitchBinary>(2).Set(true);
        }

        public async Task SwitchOff2()
        {
            await Node.GetCommandClass<MultiChannel>().GetEndPointCommandClass<SwitchBinary>(2).Set(false);
        }

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add((byte)group, node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove((byte)group, node.NodeID);
        }


        protected virtual void OnSwitchedOn1(EventArgs e)
        {
            SwitchedOn1?.Invoke(this, e);
        }

        protected virtual void OnSwitchedOff1(EventArgs e)
        {
            SwitchedOff1?.Invoke(this, e);
        }

        protected virtual void OnSwitchedOn2(EventArgs e)
        {
            SwitchedOn2?.Invoke(this, e);
        }

        protected virtual void OnSwitchedOff2(EventArgs e)
        {
            SwitchedOff2?.Invoke(this, e);
        }

    }
}
