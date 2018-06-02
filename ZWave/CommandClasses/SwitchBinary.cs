using System;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchBinary : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<SwitchBinaryReport>> Changed;

        public enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public SwitchBinary(Node node)
            : base(node, CommandClass.SwitchBinary)
        { }

        internal SwitchBinary(Node node, byte endpointId)
            : base(node, CommandClass.SwitchBinary, endpointId)
        { }

        public async Task<SwitchBinaryReport> Get()
        {
            var response = await Send(new Command(Class, command.Get), command.Report);
            return new SwitchBinaryReport(Node, response);
        }

        public async Task Set(bool value)
        {
            await Send(new Command(Class, command.Set, value ? (byte)0xFF : (byte)0x00));
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SwitchBinaryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchBinaryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SwitchBinaryReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
