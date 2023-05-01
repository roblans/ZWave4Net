using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Alarm : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<AlarmReport>> Changed;

        enum command
        {
            Get = 0x04,
            Report = 0x05,
            Set = 0x06,
            SupportedGet = 0x07,
            SupportedReport = 0x08
        }

        public Alarm(Node node) : base(node, CommandClass.Alarm)
        {
        }

        public async Task<bool> IsV2(CancellationToken cancellationToken)
        {
            var report = await Node.GetCommandClassVersionReport(Class, cancellationToken);
            return report.Version >= 2;
        }

        public Task<AlarmReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<AlarmReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new AlarmReport(Node, response);
        }

        public Task Set(NotificationType type, bool activate)
        {
            return Set(type, activate, CancellationToken.None);
        }

        public async Task Set(NotificationType type, bool activate, CancellationToken cancellationToken)
        {
            if (!await IsV2(cancellationToken))
                throw new VersionNotSupportedException($"Set works with class type {Class} version >= 2.");
            byte status = activate ? (byte)0xFF : (byte)0x00;
            await Channel.Send(Node, new Command(Class, command.Set, (byte)type, status), cancellationToken);
        }

        public Task<AlarmSupportedReport> SupportedGet()
        {
            return SupportedGet(CancellationToken.None);
        }

        public async Task<AlarmSupportedReport> SupportedGet(CancellationToken cancellationToken)
        {
            if (!await IsV2(cancellationToken))
                throw new VersionNotSupportedException($"SupportedGet works with class type {Class} version >= 2.");
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
