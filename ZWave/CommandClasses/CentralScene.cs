using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class CentralScene : CommandClassBase
    {
        public event EventHandler<ReportEventArgs<CentralSceneReport>> Changed;

        public CentralScene(Node node) : base(node, CommandClass.CentralScene)
        {
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new CentralSceneReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<CentralSceneReport>(report));
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
