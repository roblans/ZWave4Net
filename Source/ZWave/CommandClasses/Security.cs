using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Security : EndpointSupportedCommandClassBase
    {
        public event EventHandler<ReportEventArgs<NodeReport>> Verified;

        enum command
        {
            SupportedGet = 0x02,
            SupportedReport = 0x03,
            SchemeGet = 0x04,
            SchemeReport = 0x05,
            NetworkKeySet = 0x06,
            NetworkKeyVerify = 0x07,
            SchemeInherit = 0x08
        }

        public Security(Node node)
            : base(node, CommandClass.Security)
        { }

        internal Security(Node node, byte endpointId)
            : base(node, CommandClass.Security, endpointId)
        { }

        public async Task<SecuritySupportedReport> SupportedGet(CancellationToken cancellationToken = default)
        {
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new SecuritySupportedReport(Node, response);
        }

        /// <summary>
        /// Checks if Security0 is supported
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>True if Security0 is supported.  Otherwise False.</returns>
        public async Task<bool> SchemeGet(CancellationToken cancellationToken = default)
        {
            var response = await Send(new Command(Class, command.SchemeGet, new byte[] { 0x0}), command.SupportedReport, cancellationToken);
            return response[0] == 0x0;
        }

        public async Task SchemeInherit(CancellationToken cancellationToken = default)
        {
            await Send(new Command(Class, command.SchemeInherit, new byte[] { 0x0 }), true, cancellationToken);
        }

        public async Task NetworkKeySet(byte[] key, CancellationToken cancellationToken = default)
        {
            await Send(new Command(Class, command.NetworkKeySet, key), true, cancellationToken);
        }

        protected internal override void HandleEvent(Command cmd)
        {
            base.HandleEvent(cmd);
            if (cmd.CommandID == (byte)command.NetworkKeyVerify)
            {
                var report = new NodeReport(Node);
                OnVerified(new ReportEventArgs<NodeReport>(report));
            }
        }

        protected virtual void OnVerified(ReportEventArgs<NodeReport> e)
        {
            var handler = Verified;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
