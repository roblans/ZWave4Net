using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Commands;

namespace ZWave4Net.Communication
{
    class EventMessage
    {
        public readonly ReceiveStatus Status;
        public readonly byte NodeID;
        public readonly Command Command;

        private EventMessage(ReceiveStatus status, byte nodeID, Command command)
        {
            Status = status;
            NodeID = nodeID;
            Command = command;
        }

        public static EventMessage Parse(byte[] data)
        {
            var status = (ReceiveStatus)data[0];
            var nodeID = data[1];
            var command = Command.Parse(data.Skip(2).ToArray());

            return new EventMessage(status, nodeID, command);
        }
    }
}
