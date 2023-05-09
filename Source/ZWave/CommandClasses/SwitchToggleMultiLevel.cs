using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SwitchToggleMultiLevel : EndpointSupportedCommandClassBase
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

        public SwitchToggleMultiLevel(Node node)
            : base(node, CommandClass.SwitchToggleMultiLevel)
        { }

        internal SwitchToggleMultiLevel(Node node, byte endpointId)
            : base(node, CommandClass.SwitchToggleMultiLevel, endpointId)
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

        /// <summary>
        /// Sets the multilevel toggle
        /// </summary>
        /// <param name="value">0x0 for off, 0xFF for On, 1-99 for percentages between</param>
        /// <returns></returns>
        public Task Set(byte value)
        {
            return Set(value, CancellationToken.None);
        }

        /// <summary>
        /// Sets the multilevel toggle
        /// </summary>
        /// <param name="value">0x0 for off, 0xFF for On, 1-99 for percentages between</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task Set(byte value, CancellationToken cancellationToken)
        {
            if (value > 99 && value != 0xFF)
                throw new ArgumentException(nameof(value) + " must be between 0 and 99 or 0xFF for 100%");
            await Channel.Send(Node, new Command(Class, command.Set, value), cancellationToken);
        }

        public Task StartLevelChange(bool rollover, bool ignoreStart, byte startLevel)
        {
            return StartLevelChange(rollover, ignoreStart, startLevel, CancellationToken.None);
        }

        public async Task StartLevelChange(bool rollover, bool ignoreStart, byte startLevel, CancellationToken cancellationToken)
        {
            byte cmd = 0x0;
            if (rollover)
                cmd = 0x80;
            if (ignoreStart)
                cmd |= 0x20;

            await Channel.Send(Node, new Command(Class, command.StartLevelChange, cmd, startLevel), cancellationToken);
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
