using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchMultiLevel : EndpointSupportedCommandClassBase
    {
        enum command : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            StartLevelChange = 0x04,
            StopLevelChange = 0x05,
        }

        public event EventHandler<ReportEventArgs<SwitchMultiLevelReport>> Changed;

        public SwitchMultiLevel(Node node)
            : base(node, CommandClass.SwitchMultiLevel)
        { }

        internal SwitchMultiLevel(Node node, byte endpointId)
            : base(node, CommandClass.SwitchMultiLevel, endpointId)
        { }

        public Task<SwitchMultiLevelReport> Get()
        {
            return Get(CancellationToken.None);
        }


        public async Task<SwitchMultiLevelReport> Get(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new SwitchMultiLevelReport(Node, response);
        }

        public Task Set(byte value)
        {
            return Set(value, CancellationToken.None);
        }

        public async Task Set(byte value, CancellationToken cancellationToken)
        {
            await Channel.Send(Node, new Command(Class, command.Set, value), cancellationToken);
        }

        public Task StartLevelChange(bool increase, byte duration)
        {
            return StartLevelChange(increase, duration, CancellationToken.None);
        }

        public async Task StartLevelChange(bool increase, byte duration, CancellationToken cancellationToken)
        {
            var payload = new byte[]
            {
                increase ? (byte)0x20 : (byte)0x60,
                0, // Start level - ignored (for now!)
                duration,
            };
            await Channel.Send(Node, new Command(Class, command.StartLevelChange, payload), cancellationToken);
        }

        public Task StopLevelChange()
        {
            return StopLevelChange(CancellationToken.None);
        }

        public async Task StopLevelChange(CancellationToken cancellationToken)
        {
            await Channel.Send(Node, new Command(Class, command.StopLevelChange), cancellationToken);
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
