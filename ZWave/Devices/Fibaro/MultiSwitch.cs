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
            Node.GetCommandClass<Basic>().Changed += Basic_Changed;
            Node.GetCommandClass<SwitchBinary>().Changed += SwitchBinary_Changed;
            Node.GetCommandClass<MultiChannel>().Changed += MultiChannel_Changed;
        }


        private void Basic_Changed(object sender, ReportEventArgs<BasicReport> e)
        {
            if (e.Report.Value)
            {
                OnSwitchedOn1(EventArgs.Empty);
            }
            else
            {
                OnSwitchedOff1(EventArgs.Empty);
            }
        }
        private void SwitchBinary_Changed(object sender, ReportEventArgs<SwitchBinaryReport> e)
        {
            if (e.Report.Value)
            {
                OnSwitchedOn1(EventArgs.Empty);
            }
            else
            {
                OnSwitchedOff1(EventArgs.Empty);
            }
        }
        private void MultiChannel_Changed(object sender, ReportEventArgs<MultiChannelReport> e)
        {
            if (e.Report.Report is SwitchBinaryReport)
            {
                var switchReport = e.Report.Report as SwitchBinaryReport;
                if (byte.Equals(e.Report.EndPointID, 1))
                {
                    if (switchReport.Value)
                    {
                        OnSwitchedOn1(EventArgs.Empty);
                    }
                    else
                    {
                        OnSwitchedOff1(EventArgs.Empty);
                    }
                }
                else if (byte.Equals(e.Report.EndPointID, 2))
                {
                    if (switchReport.Value)
                    {
                        OnSwitchedOn2(EventArgs.Empty);
                    }
                    else
                    {
                        OnSwitchedOff2(EventArgs.Empty);
                    }
                }
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
