using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Notification : CommandClassBase
    {
        private const byte FIRST_AVAILABLE = 0xFF;
        public event EventHandler<ReportEventArgs<NotificationReport>> Changed;

        enum command
        {
            EventSupportedGet = 0x01,
            EventSupportedReport = 0x02,
            Get = 0x04,
            Report = 0x05,
            Set = 0x06,
            SupportedGet = 0x07,
            SupportedReport = 0x08
        }

        public Notification(Node node) : base(node, CommandClass.Notification)
        {
        }

        public Task<NotificationReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<NotificationReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, (byte)0x0, FIRST_AVAILABLE, (byte)0x0), command.Report, cancellationToken);
            return new NotificationReport(Node, response);
        }

        public Task Set(NotificationType type, bool enabled)
        {
            return Set(type, enabled, CancellationToken.None);
        }

        public async Task Set(NotificationType type, bool enabled, CancellationToken cancellationToken)
        {
            byte status = enabled ? (byte)0xFF : (byte)0x00;
            await Channel.Send(Node, new Command(Class, command.Set, (byte)type, status), cancellationToken);
        }

        public Task<AlarmSupportedReport> SupportedGet()
        {
            return SupportedGet(CancellationToken.None);
        }

        public async Task<AlarmSupportedReport> SupportedGet(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new AlarmSupportedReport(Node, response);
        }

        public Task<NotificationReport> EventSupportedGet(NotificationType type)
        {
            return Get(CancellationToken.None);
        }

        public async Task<NotificationStateReport> EventSupportedGet(NotificationType type, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.EventSupportedGet, (byte)type), command.EventSupportedReport, cancellationToken);
            return new NotificationStateReport(Node, type, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new NotificationReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<NotificationReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<NotificationReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
