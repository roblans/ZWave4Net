using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Alarm : CommandClassBase
    {
        private const byte FIRST_AVAILABLE = 0xFF;
        public event EventHandler<ReportEventArgs<AlarmReport>> Changed;

        enum command
        {
            Get = 0x04,
            Report = 0x05,
            Set = 0x06,
            SupportedGet = 0x07,
            SupportedReport = 0x09,
        }

        public Alarm(Node node) : base(node, CommandClass.Alarm)
        {
        }

        public Task<AlarmReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<AlarmReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, FIRST_AVAILABLE), command.Report, cancellationToken);
            return new AlarmReport(Node, response);
        }

        public Task<AlarmReport> Set(NotificationType type, bool activate)
        {
            return Set(type, activate, CancellationToken.None);
        }

        public async Task<AlarmReport> Set(NotificationType type, bool activate, CancellationToken cancellationToken)
        {
            byte status = activate ? (byte)0xFF : (byte)0x00;
            var response = await Channel.Send(Node, new Command(Class, command.Get, (byte)type, status), command.Report, cancellationToken);
            return new AlarmReport(Node, response);
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
