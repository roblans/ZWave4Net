using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public abstract class CommandClass
    {
        public readonly byte ClassID;
        public readonly Node Node;
        internal readonly CommandInvoker Invoker;

        protected abstract Enum[] Commands { get; }

        public CommandClass(byte classID, Node node)
        {
            ClassID = classID;
            Node = node;
            Invoker = new CommandInvoker(this);
        }

        public override string ToString()
        {
            return ClassName;
        }

        public string ClassName
        {
            get { return GetType().Name; }
        }

        internal bool IsCorrelated(Command request, Command response)
        {
            var r = Commands.Single(element => Convert.ToByte(element) == request.CommandID);
            var s = Commands.Single(element => Convert.ToByte(element) == response.CommandID);
            return IsCorrelated(r, s);
        }

        internal void HandleEvent(Command @event)
        {
            var command = Commands.Single(element => Convert.ToByte(element) == @event.CommandID);
            OnEvent(command, @event.Payload);
        }

        protected virtual bool IsCorrelated(Enum request, Enum response)
        {
            return false;
        }

        protected virtual void OnEvent(Enum command, byte[] payload)
        {
        }
    }
}
