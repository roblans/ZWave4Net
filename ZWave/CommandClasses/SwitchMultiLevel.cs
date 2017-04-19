using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchMultiLevel : CommandClassBase
    {
        enum command : byte
        {
            //SupportedGet = 0x01,
            //SupportedReport = 0x02,
            Get = 0x04,
            Report = 0x05
        }

        public event EventHandler<ReportEventArgs<SwitchMultiLevelReport>> Changed;

        public SwitchMultiLevel(Node node) : base(node, CommandClass.SwitchMultiLevel)
        {
        }

        public async Task<SwitchMultiLevelReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new SwitchMultiLevelReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SwitchMultiLevelReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchMultiLevelReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SwitchMultiLevelReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
