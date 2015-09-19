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

        protected abstract Enum[] Commands { get; }

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
            var command = Commands.Single(element => Convert.ToByte(element) == message.Command.CommandID);
            OnEvent(command, message.Command.Payload);
        }

        protected virtual void OnEvent(Enum command, byte[] payload)
        {
        }
    }
}
