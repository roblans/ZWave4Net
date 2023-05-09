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

        public Task Set(byte value, TimeSpan duration)
        {
            return Set(value, duration, CancellationToken.None);
        }

        public async Task Set(byte value, TimeSpan duration, CancellationToken cancellationToken)
        {
            byte time = 0;
            if (duration.TotalSeconds >= 1)
                time = PayloadConverter.GetByte(duration);
            await Send(new Command(Class, command.Set, value, time), cancellationToken);
        }

        public Task StartLevelChange(bool increase, int startLevel, byte duration)
        {
            return StartLevelChange(increase, startLevel, duration, CancellationToken.None);
        }

        public async Task StartLevelChange(bool increase, int startLevel, byte duration, CancellationToken cancellationToken)
        {
            byte flags = 0x0;
            if (increase)
                flags = 0x40;
            if (startLevel < 0)
                flags |= 0x20;
            await Channel.Send(Node, new Command(Class, command.StartLevelChange, flags, (byte)Math.Max(0, startLevel),duration), cancellationToken);
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
