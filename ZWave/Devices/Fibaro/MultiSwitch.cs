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
            Node.GetCommandClass<MultiChannel>().Changed += Switch_Changed;
        }

        private void Switch_Changed(object sender, ReportEventArgs<MultiChannelReport> e)
        {
            var endpointReport = (SwitchBinaryReport) e.Report.Report;
            switch (e.Report.EndPointID)
            {
                case 1:
                    if (endpointReport.Value)
                    {
                        OnSwitchedOn1(EventArgs.Empty);
                    }
                    else
                    {
                        OnSwitchedOff1(EventArgs.Empty);
                    }
                    break;
                case 2:
                    if (endpointReport.Value)
                    {
                        OnSwitchedOn2(EventArgs.Empty);
                    }
                    else
                    {
                        OnSwitchedOff2(EventArgs.Empty);
                    }
                    break;
                default:
                    //don't care: this device only ever has 2 endpoints.
                    break;
            }
        }

        public async Task SwitchOn1()
        {
            await Node.GetCommandClass<MultiChannel>().BinarySwitchSet(1, true);
        }

        public async Task SwitchOff1()
        {
            await Node.GetCommandClass<MultiChannel>().BinarySwitchSet(1, false);
        }

        public async Task SwitchOn2()
        {
            await Node.GetCommandClass<MultiChannel>().BinarySwitchSet(2, true);
        }

        public async Task SwitchOff2()
        {
            await Node.GetCommandClass<MultiChannel>().BinarySwitchSet(2, false);
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
