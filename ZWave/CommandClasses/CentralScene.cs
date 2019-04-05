using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class CentralScene : CommandClassBase
    {
        enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Notification = 0x03,
        }

        public event EventHandler<ReportEventArgs<CentralSceneReport>> Changed;

        public CentralScene(Node node) : base(node, CommandClass.CentralScene)
        {
        }

        public Task<CentralSceneSupportedReport> GetSupportedScenes()
        {
            return GetSupportedScenes(CancellationToken.None);
        }

        public async Task<CentralSceneSupportedReport> GetSupportedScenes(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new CentralSceneSupportedReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(CentralScene.command.Notification))
            {
                var report = new CentralSceneReport(Node, command.Payload);
                OnChanged(new ReportEventArgs<CentralSceneReport>(report));
            }
        }

        protected virtual void OnChanged(ReportEventArgs<CentralSceneReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
