using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class Alarm : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<AlarmReport>> Changed;

        enum command
        {
            Get = 0x04,
            Report = 0x05,
            SupportedGet = 0x07,
            SupportedReport = 0x09,
        }

        public Alarm(Node node) : base(node, CommandClass.Alarm)
        {
        }

        public async Task<AlarmReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new AlarmReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new AlarmReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<AlarmReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<AlarmReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
