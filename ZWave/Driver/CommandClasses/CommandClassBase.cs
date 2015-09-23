using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
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
            get { return Node.Channel; }
        }

        internal protected virtual void HandleEvent(Command command)
        {
        }
    }
}
