using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class CommandClassBase : ICommandClass
    {
        public CommandClassBase(Node node, CommandClass @class)
        {
            Node  = node;
            Class = @class;
        }

        protected ZWaveChannel Channel => Node.Controller.Channel;
        public Node Node { get; }
        public CommandClass Class { get; }

        protected internal virtual void HandleEvent(Command command) { }
    }
}
