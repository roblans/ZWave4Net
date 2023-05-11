using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class WakeUp : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<WakeUpReport>> Changed;

        enum command
        {
            IntervalSet = 0x04,
            IntervalGet = 0x05,
            IntervalReport = 0x06,
            Notification = 0x07,
            NoMoreInformation = 0x08,
            CapabilitiesGet = 0x09,
            CapabilitiesReport = 0x0A
        }

        public WakeUp(Node node) : base(node, CommandClass.WakeUp)
        {
        }

        public Task<WakeUpIntervalReport> GetInterval()
        {
            return GetInterval(CancellationToken.None);
        }

        public async Task<WakeUpIntervalReport> GetInterval(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.IntervalGet), command.IntervalReport, cancellationToken);
            return new WakeUpIntervalReport(Node, response);
        }

        public Task<WakeUpCapabilitiesReport> GetIntervalCapabilities()
        {
            return GetIntervalCapabilities(CancellationToken.None);
        }

        public async Task<WakeUpCapabilitiesReport> GetIntervalCapabilities(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.CapabilitiesGet), command.CapabilitiesReport, cancellationToken);
            return new WakeUpCapabilitiesReport(Node, response);
        }

        public Task SetInterval(TimeSpan interval, byte targetNodeID)
        {
            return SetInterval(interval, targetNodeID, CancellationToken.None);
        }

        public async Task SetInterval(TimeSpan interval, byte targetNodeID, CancellationToken cancellationToken)
        {
            var seconds = PayloadConverter.GetBytes((uint)interval.TotalSeconds);
            await Channel.Send(Node, new Command(Class, command.IntervalSet, seconds[1], seconds[2], seconds[3], targetNodeID), cancellationToken);
        }

        public Task NoMoreInformation()
        {
            return NoMoreInformation(CancellationToken.None);
        }

        public async Task NoMoreInformation(CancellationToken cancellationToken)
        {
            await Channel.Send(Node, new Command(Class, command.NoMoreInformation), cancellationToken);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(WakeUp.command.Notification))
            {
                var report = new WakeUpReport(Node);
                OnChanged(new ReportEventArgs<WakeUpReport>(report));
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
    }
}
