using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Meter : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<MeterReport>> Changed;

        enum Command
        {
            Get = 0x01,
            Report = 0x02,

            // Version 2
            SupportedGet = 0x03,
            SupportedReport = 0x04,
            Reset = 0x05
        }

        public Meter(Node node) : base(node, CommandClass.Meter) { }

        public async Task<MeterReport> Get(CancellationToken? cancellationToken = null)
        {
            byte[] response;
            if (cancellationToken.HasValue)
                response = await Channel.Send(Node, new Channel.Command(Class, Command.Get), Command.Report, cancellationToken.Value);
            else
                response = await Channel.Send(Node, new Channel.Command(Class, Command.Get), Command.Report, CancellationToken.None);

            return new MeterReport(Node, response);
        }

        public Task<MeterReport> Get(ElectricMeterScale scale, CancellationToken? cancellationToken = null)
        {
            if (cancellationToken.HasValue)
                return Get((Enum)scale, cancellationToken.Value);

            return Get((Enum)scale, CancellationToken.None);
        }

        public Task<MeterReport> Get(GasMeterScale scale, CancellationToken? cancellationToken = null)
        {
            if (cancellationToken.HasValue)
                return Get((Enum)scale, cancellationToken.Value);

            return Get((Enum)scale, CancellationToken.None);
        }

        public Task<MeterReport> Get(WaterMeterScale scale, CancellationToken? cancellationToken = null)
        {
            if (cancellationToken.HasValue)
                return Get((Enum)scale, cancellationToken.Value);

            return Get((Enum)scale, CancellationToken.None);
        }

        private async Task<MeterReport> Get(Enum scale, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Channel.Command(Class, Command.Get, (byte)(Convert.ToByte(scale) << 3)), Command.Report, cancellationToken);
            return new MeterReport(Node, response);
        }

        public async Task<MeterSupportedReport> GetSupported(CancellationToken? cancellationToken = null)
        {
            byte[] response;
            if (cancellationToken.HasValue)
                response = await Channel.Send(Node, new Channel.Command(Class, Command.SupportedGet), Command.SupportedReport, cancellationToken.Value);
            else
                response = await Channel.Send(Node, new Channel.Command(Class, Command.SupportedGet), Command.SupportedReport, CancellationToken.None);

            return new MeterSupportedReport(Node, response);
        }

        protected internal override void HandleEvent(Channel.Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID != Convert.ToByte(Command.Report))
                return;

            var report = new MeterReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<MeterReport>(report));
        }
        protected virtual void OnChanged(ReportEventArgs<MeterReport> e)
        {
            Changed?.Invoke(this, e);
        }
    }
}
