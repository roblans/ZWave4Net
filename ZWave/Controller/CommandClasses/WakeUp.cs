using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class WakeUp : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<WakeUpReport>> Changed;
        public event EventHandler<ReportEventArgs<WakeUpNotificationReport>> Notification;

        enum command
        {
            IntervalSet = 0x04,
            IntervalGet = 0x05,
            IntervalReport = 0x06,
            Notification = 0x07,
            NoMoreInformation = 0x08
        }

        public WakeUp(Node node) : base(node, CommandClass.WakeUp)
        {
        }

        public async Task<WakeUpReport> GetInterval()
        {
            var response = await Channel.Send(Node, new Command(Class, command.IntervalGet), command.IntervalReport);
            return new WakeUpReport(Node, response);
        }

        public async Task SetInterval(TimeSpan interval, byte targetNodeID)
        {
            var seconds = PayloadConverter.GetBytes((uint)interval.TotalSeconds);
            await Channel.Send(Node, new Command(Class, command.IntervalSet, seconds[1], seconds[2], seconds[3], targetNodeID));
        }

        public async Task NoMoreInformation()
        {
            await Channel.Send(Node, new Command(Class, command.NoMoreInformation));
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(WakeUp.command.IntervalReport))
            {
                var report = new WakeUpReport(Node, command.Payload);
                OnChanged(new ReportEventArgs<WakeUpReport>(report));
                return;
            }

            if (command.CommandID == Convert.ToByte(WakeUp.command.Notification))
            {
                var report = new WakeUpNotificationReport(Node);
                OnNotification(new ReportEventArgs<WakeUpNotificationReport>(report));
                return;
            }
        }

        protected virtual void OnChanged(ReportEventArgs<WakeUpReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnNotification(ReportEventArgs<WakeUpNotificationReport> e)
        {
            var handler = Notification;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
