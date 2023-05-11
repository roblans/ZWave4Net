using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchAll : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<SwitchAllReport>> Changed;

        public enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            On = 0x04,
            Off = 0x05
        }

        public SwitchAll(Node node)
            : base(node, CommandClass.SwitchAll)
        { }

        internal SwitchAll(Node node, byte endpointId)
            : base(node, CommandClass.SwitchAll, endpointId)
        { }

        public Task<SwitchAllReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<SwitchAllReport> Get(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new SwitchAllReport(Node, response);
        }

        public Task Set(SwitchAllMode value)
        {
            return Set(value, CancellationToken.None);
        }

        public async Task Set(SwitchAllMode value, CancellationToken cancellationToken)
        {
            await Send(new Command(Class, command.Set, (byte)value), cancellationToken);
        }

        public async Task On(bool value)
        {
            await Send(new Command(Class, command.On), CancellationToken.None);
        }

        public async Task Off(bool value)
        {
            await Send(new Command(Class, command.Off), CancellationToken.None);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SwitchAllReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchAllReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SwitchAllReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
