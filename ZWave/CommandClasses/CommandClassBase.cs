using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class CommandClassBase : ICommandClass
    {
        public Node Node { get; private set; }
        public CommandClass Class { get; private set; }

        public CommandClassBase(Node node, CommandClass @class)
        {
            Node = node;
            Class = @class;
        }

        protected ZWaveChannel Channel
        {
            get { return Node.Controller.Channel; }
        }

        internal protected virtual void HandleEvent(Command command)
        {
        }
    }
}
