using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public abstract class CommandClass
    {
        public readonly byte ClassID;
        public readonly Node Node;
        internal readonly ICommandDispatcher Dispatcher;

        public CommandClass(byte classID, Node node)
        {
            ClassID = classID;
            Node = node;
            Dispatcher = new CommandDispatcher(this);
        }

        public override string ToString()
        {
            return ClassName;
        }

        public string ClassName
        {
            get { return GetType().Name; }
        }

        internal void HandleEvent(EventMessage message)
        {
            OnEvent(message.Command);
        }

        protected virtual void OnEvent(Command command)
        {
        }
    }
}
